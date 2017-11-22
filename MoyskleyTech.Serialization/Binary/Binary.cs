using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace MoyskleyTech.Serialization.Binary
{
    public class BinaryUtility
    {
        private const byte BEGIN_FIELD_BYTE=0x81;
        private const byte END_CLASS_BYTE= 0x82;
        private const byte ALREADY_SERIALIZED=0x83;
        private const byte NEW_OBJECT=0x84;
        private const byte NULL=0;
        private static LinkedList<CustomBinarySerializer> registredSerializers = new LinkedList<CustomBinarySerializer>();
        public static Func<Type , object> ObjectCreator { get; set; }
        static BinaryUtility()
        {
            Type t = Type.GetType("System.Runtime.Serialization.FormatterServices",false);
            if ( t != null )
            {
                var method =t.GetRuntimeMethod("GetUninitializedObject" , new Type[ ] { typeof(Type) });
                if ( method != null )
                {
                    ObjectCreator = (type) =>
                    {
                        return method.Invoke(null , new[ ] { type });
                    };
                }
            }
            if ( ObjectCreator == null )
                ObjectCreator = (type) =>
                {
                    return Activator.CreateInstance(type);
                };
        }
        public static void Serialize(object o , Stream s)
        {
            Serialize(o , s , new Dictionary<object , int>());
        }
        public static void RegisterCustomSerializer(CustomBinarySerializer c)
        {
            registredSerializers.AddLast(c);
        }
        public static void UnregisterCustomSerializer(CustomBinarySerializer c)
        {
            registredSerializers.Remove(c);
        }
        public static void Serialize(object o , Stream s , Dictionary<object , int> context)
        {
            MemoryStream ms = new MemoryStream();

            BinaryWriter bw;

            bw = new BinaryWriter(ms , Encoding.UTF8);

            SerializeInternal(o , bw , context);
            bw.Flush();
            ms.Position = 0;

            { ms.WriteTo(s); }

            bw.Dispose();
        }
        private static void SerializeInternal(object o , BinaryWriter bw , Dictionary<object , int> set , Type innerType = null)
        {
            if ( o == null || o is IntPtr )//Handle pointer as null because a pointer is not serializable
            {
                //Not serializable handled as null 1 byte
                bw.Write(NULL);
                return;
            }
            Type t = o.GetType();
            if ( t.GetTypeInfo().CustomAttributes.Any((x) => x.AttributeType.Name == typeof(NotSerializedAttribute).Name) )
            {
                //Not serializable handled as null 1 byte
                bw.Write(NULL);
                return;
            }
            if ( set.ContainsKey(o) )//5 bytes for already serialized object
            {
                bw.Write(ALREADY_SERIALIZED);
                bw.Write(set[o]);
            }
            else
            {

                bw.Write(NEW_OBJECT);//Begin object flag

                if ( innerType == null )//Write class name
                    bw.Write(NameFor(t));

                if ( SerializeBasicType(bw , o , set) )//Try basic type
                    return;
                set[o] = set.Count;
                if ( SerializeCustomType(bw , o , set) )//Try custom serializer(ToStream/FromStream)
                    return;
                if ( SerializeArrayInternal(bw , o , set) )//Try to handle as Array
                    return;
                //Serialize fields
                while ( t != null )
                {
                    var fields = t.GetRuntimeFields();
                    foreach ( var field in fields )
                    {
                        //If not static not constant and does not contains "NotSerialized"
                        if ( !field.CustomAttributes.Any((x) => x.AttributeType.Name == typeof(NotSerializedAttribute).Name) &&
                            ( field.Attributes & FieldAttributes.Static ) != FieldAttributes.Static &&
                            ( field.Attributes & FieldAttributes.NotSerialized ) != FieldAttributes.NotSerialized &&
                            ( field.Attributes & FieldAttributes.Literal ) != FieldAttributes.Literal )
                        {
                            //If is not a pointer
                            if ( !field.FieldType.IsPointer )
                            {
                                bw.Write(BEGIN_FIELD_BYTE);
                                bw.Write(field.Name);
                                SerializeInternal(field.GetValue(o) , bw , set);
                            }
                        }
                    }
                    t = t.GetTypeInfo().BaseType;
                }
                bw.Write(END_CLASS_BYTE);
            }
        }
        private static string NameFor(Type t)
        {
            if ( Type.GetType(t.FullName) == null )
                return t.AssemblyQualifiedName;
            else
                return t.FullName;

        }
        private static bool SerializeArrayInternal(BinaryWriter bw , object o , Dictionary<object , int> set)
        {
            if ( o is Array )
            {
                Array a = (Array)o;

                Type inner =o.GetType().GetElementType();
                int[] Dimensions = new int[a.Rank];
                bw.Write(a.Rank);
                for ( var i = 0 ; i < a.Rank ; i++ )
                {
                    int len = a.GetLength(i);
                    bw.Write(len);
                    Dimensions[i] = ( len );
                }
                int[] counter = new int[a.Rank];

                var isPrimitive = (typeof(BinaryWriter).GetRuntimeMethod("Write" , new Type[ ] { inner }))!=null;
                for ( var i = 0 ; i < a.Length ; i++ )
                {
                    SerializeInternal(a.GetValue(counter) , bw , set , isPrimitive ? inner : null);
                    counter[counter.Length - 1]++;
                    for ( var j = counter.Length - 1 ; j >= 0 ; j-- )
                    {
                        if ( counter[j] == Dimensions[j] )
                        {
                            if ( j > 0 )
                                counter[j - 1] += 1;
                            counter[j] = 0;
                        }
                        else
                            break;
                    }

                }
                return true;
            }
            return false;
        }
        private static object DeserializeArrayType(BinaryReader br , Type t , Dictionary<int , object> set)
        {
            if ( t == null )
                return null;
            if ( t.IsArray )
            {
                int rank = br.ReadInt32();

                Type inner =t.GetElementType();
                int[] Dimensions = new int[rank];
                for ( var i = 0 ; i < rank ; i++ )
                {
                    Dimensions[i] = ( br.ReadInt32() );
                }
                int[] counter = new int[rank];

                Array a = Array.CreateInstance(inner , Dimensions);
                set[set.Count] = a;

                var isPrimitive = (typeof(BinaryWriter).GetRuntimeMethod("Write" , new Type[ ] { inner }))!=null;
                for ( var i = 0 ; i < a.Length ; i++ )
                {
                    object val = DeserializeObjectInternal(br , set, isPrimitive?inner:null);
                    a.SetValue(val , counter);
                    counter[counter.Length - 1]++;
                    for ( var j = counter.Length - 1 ; j >= 0 ; j-- )
                    {
                        if ( counter[j] == Dimensions[j] )
                        {
                            if ( j > 0 )
                                counter[j - 1] += 1;
                            counter[j] = 0;
                        }
                        else
                            break;
                    }
                }
                return a;
            }
            else
            {
                return DeserializeTupleType(br , t , set);
            }
        }
        private class TupleHelper
        {
            public object m_Item1=null,m_Item2=null,m_Item3=null,m_Item4=null,m_Item5=null,m_Item6=null,m_Item7=null,m_Item8=null;
        }
        private static object DeserializeTupleType(BinaryReader br , Type t , Dictionary<int , object> set)
        {
            TupleHelper th = new TupleHelper();
            Object o=th;
            if ( t.Name == typeof(Tuple<>).Name )
            {
                DeserializeFields(br , set , typeof(TupleHelper) , ref o);
                return Activator.CreateInstance(t , th.m_Item1);
            }
            else if ( t.Name == typeof(Tuple<,>).Name )
            {
                DeserializeFields(br , set , typeof(TupleHelper) , ref o);
                return Activator.CreateInstance(t , th.m_Item1 , th.m_Item2);
            }
            else if ( t.Name == typeof(Tuple<,,>).Name )
            {
                DeserializeFields(br , set , typeof(TupleHelper) , ref o);
                return Activator.CreateInstance(t , th.m_Item1 , th.m_Item2 , th.m_Item3);
            }
            else if ( t.Name == typeof(Tuple<,,,>).Name )
            {
                DeserializeFields(br , set , typeof(TupleHelper) , ref o);
                return Activator.CreateInstance(t , th.m_Item1 , th.m_Item2 , th.m_Item3 , th.m_Item4);
            }
            else if ( t.Name == typeof(Tuple<,,,,>).Name )
            {
                DeserializeFields(br , set , typeof(TupleHelper) , ref o);
                return Activator.CreateInstance(t , th.m_Item1 , th.m_Item2 , th.m_Item3 , th.m_Item4 , th.m_Item5);
            }
            else if ( t.Name == typeof(Tuple<,,,,,>).Name )
            {
                DeserializeFields(br , set , typeof(TupleHelper) , ref o);
                return Activator.CreateInstance(t , th.m_Item1 , th.m_Item2 , th.m_Item3 , th.m_Item4 , th.m_Item5 , th.m_Item6);
            }
            else if ( t.Name == typeof(Tuple<,,,,,,>).Name )
            {
                DeserializeFields(br , set , typeof(TupleHelper) , ref o);
                return Activator.CreateInstance(t , th.m_Item1 , th.m_Item2 , th.m_Item3 , th.m_Item4 , th.m_Item5 , th.m_Item6 , th.m_Item7);
            }
            else if ( t.Name == typeof(Tuple<,,,,,,,>).Name )
            {
                DeserializeFields(br , set , typeof(TupleHelper) , ref o);
                return Activator.CreateInstance(t , th.m_Item1 , th.m_Item2 , th.m_Item3 , th.m_Item4 , th.m_Item5 , th.m_Item6 , th.m_Item7 , th.m_Item8);
            }
            return null;
        }

        private static bool SerializeCustomType(BinaryWriter bw , object o , Dictionary<object , int> context)
        {
            foreach ( CustomBinarySerializer c in registredSerializers )
            {
                if ( c.IsMatch(o?.GetType()) )
                {
                    bw.Flush();
                    c.ToStream(bw.BaseStream , o , context);
                    return true;
                }
            }
            var method = o.GetType().GetRuntimeMethod("ToStream", new Type[] {typeof(Stream) });
            if ( method != null )
            {
                bw.Flush();
                method.Invoke(o , new object[ ] { bw.BaseStream });

                return true;
            }
            method = o.GetType().GetRuntimeMethod("ToStream" , new Type[ ] { typeof(Stream) , typeof(Dictionary<object , int>) });
            if ( method != null )
            {
                bw.Flush();
                method.Invoke(o , new object[ ] { bw.BaseStream , context });

                return true;
            }
            return false;
        }
        private static object DeserializeCustomType(BinaryReader br , Type t , Dictionary<int , object> context)
        {
            foreach ( CustomBinarySerializer c in registredSerializers )
            {
                if ( c.IsMatch(t.GetType()) )
                {
                    object o= c.FromStream(br.BaseStream , context);
                    context[context.Count] = o;
                    return o;
                }
            }
            var method = t.GetRuntimeMethod("FromStream", new Type[] {typeof(Stream) });
            if ( method != null )
            {
                object o = method.Invoke(null , new object[ ] { br.BaseStream });
                context[context.Count] = o;
                return o;
            }
            method = t.GetRuntimeMethod("FromStream" , new Type[ ] { typeof(Stream) , typeof(Dictionary<int , object>) });
            if ( method != null )
            {
                object o = method.Invoke(null , new object[ ] { br.BaseStream,context });
                context[context.Count] = o;
                return o;
            }
            return null;
        }
        private static bool SerializeBasicType(BinaryWriter bw , object o , Dictionary<object , int> context)
        {
            if ( o is string )
            {
                context[o] = context.Count;
                bw.Write(o as string);
                return true;
            }
            var method = (typeof(BinaryWriter).GetRuntimeMethod("Write" , new Type[ ] { o.GetType() }));
            if ( method != null )
            {
                method.Invoke(bw , new object[ ] { o });
                return true;
            }
            if ( o is DateTime )
            {
                bw.Write(( o as DateTime? )?.Ticks ?? 0L);
                return true;
            }
            return false;
        }
        private static object DeserializeBasicType(BinaryReader br , Type t , Dictionary<int , object> context)
        {
            if ( t == null )
                return null;
            if ( t == typeof(string) )
            {
                object o = br.ReadString();
                context[context.Count] = o;
                return o;
            }
            var method = (typeof(BinaryReader).GetRuntimeMethod("Read"+t.Name , new Type[ ] {  }));
            if ( method != null )
            {
                return method.Invoke(br , new object[ ] { });
            }
            if ( t == typeof(DateTime) )
            {
                return new DateTime(br.ReadInt64());
            }
            return null;
        }

        public static object Deserialize(Stream s)
        {
            return Deserialize(s , new Dictionary<int , object>());
        }
        public static object Deserialize(Stream s , Dictionary<int , object> context)
        {
            BinaryReader br = new BinaryReader(s, Encoding.UTF8);
            object o = DeserializeObjectInternal(br, context);
            return o;
        }

        private static object DeserializeObjectInternal(BinaryReader br , Dictionary<int , object> set , Type innerType = null)
        {
            byte code = 0;
            code = br.ReadByte();
            if ( code == NEW_OBJECT )
            {
                Type t;
                t = GetTypeToDeserialize(br , innerType);

                //Try base type(from mscorlib) return null if not
                object o = DeserializeBasicType(br,t,set);
                if ( o != null )
                    return o;
                //Try array return null if not
                o = DeserializeArrayType(br , t , set);
                if ( o != null )
                    return o;
                //Try custom (ToStream, FromStream) return null if not
                o = DeserializeCustomType(br , t , set);
                if ( o != null )
                    return o;
                //Create instance if normal object not handled
                o = ObjectCreator(t);
                set[set.Count] = o;
                DeserializeFields(br , set , t , ref o);
                //END_CLASS_BYTE

                return o;
            }
            else if ( code == ALREADY_SERIALIZED )
            {
                //Get it from dictionary
                int no = br.ReadInt32();
                return set[no];
            }
            else// if ( code == NULL )
                return null;
        }

        private static void DeserializeFields(BinaryReader br , Dictionary<int , object> set , Type t , ref object o)
        {
            byte code;
            //Each field begin with BEGIN_FIELD_BYTE, if it's the last it's followed by END_CLASS_BYTE
            while ( ( code = br.ReadByte() ) == BEGIN_FIELD_BYTE )
            {
                //LAYOUT : BEGIN_FIELD_BYTE fieldName fieldValue
                string fieldName = br.ReadString();
                var field = t.GetRuntimeFields().Where((x)=>x.Name==fieldName).FirstOrDefault();
                Type t2 = t;
                while ( field == null && t2.GetTypeInfo().BaseType != null )
                {
                    t2 = t2.GetTypeInfo().BaseType;
                    field = t2.GetRuntimeFields().Where((x) => x.Name == fieldName).FirstOrDefault();
                }
                if ( field != null )//If class contains this field(for removed field)
                {
                    if ( field.FieldType == typeof(IntPtr) )
                        field.SetValue(o , IntPtr.Zero);
                    else
                    if ( o is ValueType )//If struct
                    {
                        o = SetValueToStruct(br , set , o , field);
                    }
                    else
                        field.SetValue(o , DeserializeObjectInternal(br , set));
                }
                else //If not in class read it but forget it
                    DeserializeObjectInternal(br , set);
            }
        }

        private static Type GetTypeToDeserialize(BinaryReader br , Type innerType)
        {
            Type t;
            if ( innerType == null )
            {
                string typeName = br.ReadString();
                t = Type.GetType(typeName);

                if ( t == null )
                    throw new ClassNotFoundException(typeName);
            }
            else
                t = innerType;
            return t;
        }

        private static object SetValueToStruct(BinaryReader br , Dictionary<int , object> set , object o , FieldInfo field)
        {
            object boxed=o;
            field.SetValue(boxed , DeserializeObjectInternal(br , set));

            o = boxed;
            return o;
        }

        public static T Deserialize<T>(Stream s)
        {
            return ( T ) Deserialize(s);
        }
        public static T Deserialize<T>(Stream s , Dictionary<int , object> context)
        {
            return ( T ) Deserialize(s , context);
        }

    }
}
