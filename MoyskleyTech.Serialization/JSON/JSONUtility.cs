using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using MoyskleyTech.Serialization.Extentions;
using System.Dynamic;

namespace MoyskleyTech.Serialization.JSON
{
    public class JSONUtility
    {
        private static Dictionary<string , object> DictionaryFromType(object atype)
        {
            if ( atype == null )
                return new Dictionary<string , object>();
            if ( atype is ExpandoObject )
            {
                ExpandoObject eo = (ExpandoObject)atype;
                Dictionary<string , object> eoadict = new Dictionary<string , object>( );
                foreach ( var kv in eo )
                {
                    eoadict.Add(kv.Key , kv.Value);
                }
                return eoadict;
            }
            Type t = atype.GetType( );
            IEnumerable<PropertyInfo> props = t.GetRuntimeProperties();
            Dictionary<string , object> dict = new Dictionary<string , object>( );
            foreach ( PropertyInfo prp in props )
            {
                object value = prp.GetValue(atype , new object[] { });
                dict.Add(prp.Name , value);
            }
            return dict;
        }
        [Obsolete("Use Parse")]
        public static Dictionary<string , object> parseJSON(string json)
        {
            return Parse(json);
        }
        public static Dictionary<string , object> Parse(string json)
        {
            if ( string.IsNullOrWhiteSpace(json) )
                return null;
            int pos = 0;
            StringBuilder sb = new StringBuilder(json.Trim( ));
            var ret = getJsonObj(sb , ref pos);
            if ( ret is Dictionary<string , object> )
                return ret as Dictionary<string , object>;
            else
            {
                Dictionary<string , object> d = new Dictionary<string , object>( );
                var array = (List<object>)ret;
                for ( var i = 0; i < array.Count; i++ )
                {
                    d[i + ""] = array[i];
                }
                return d;
            }
        }
        public static dynamic ParseAsDynamic(string json)
        {
            if ( string.IsNullOrWhiteSpace(json) )
                return null;
            int pos = 0;
            StringBuilder sb = new StringBuilder(json.Trim( ));
            var ret = getJsonDyn(sb , ref pos);
            return ret;
        }
        public static T parseJSON<T>(string json) where T : new()
        {
            var instance = default(T);
            if ( string.IsNullOrWhiteSpace(json) )
                return instance;
            int pos = 0;
            StringBuilder sb = new StringBuilder(json.Trim( ));
            var ret = getJsonObj(sb , ref pos);

            return ( T ) ConvertToT(typeof(T) , ret);
        }

        private static object ConvertToT(Type type , object ret)
        {
            var instance  = Activator.CreateInstance(type);
            if ( instance is Array || instance is IList )
            {
                var list = ret as List<object>;
                if ( instance is Array )
                {
                    Type t = type.GetElementType();
                    Array a = Array.CreateInstance(t,list.Count);
                    for ( var i = 0; i < list.Count; i++ )
                        a.SetValue(ConvertToT(t , list[i]) , i);

                    return a;
                }
                if ( instance is IList )
                {
                    Type t = type.GetRuntimeProperty("Item").PropertyType;
                    IList a = (IList)instance;

                    for ( var i = 0; i < list.Count; i++ )
                        a.Add(ConvertToT(t , list[i]));

                    return a;
                }
            }

            object o;

            o = TryType(type , typeof(Int16) , ret); if ( o != null ) return o;
            o = TryType(type , typeof(Int32) , ret); if ( o != null ) return o;
            o = TryType(type , typeof(Int64) , ret); if ( o != null ) return o;
            o = TryType(type , typeof(UInt16) , ret); if ( o != null ) return o;
            o = TryType(type , typeof(UInt32) , ret); if ( o != null ) return o;
            o = TryType(type , typeof(UInt64) , ret); if ( o != null ) return o;

            o = TryType(type , typeof(Byte) , ret); if ( o != null ) return o;
            o = TryType(type , typeof(SByte) , ret); if ( o != null ) return o;

            o = TryType(type , typeof(String) , ret); if ( o != null ) return o;

            var dico  =ret as Dictionary<string , object>;

            object obj = Activator.CreateInstance(type);

            IEnumerable<FieldInfo> members = type.GetRuntimeFields();
            foreach ( FieldInfo member in members  )
            {
                if ( dico.ContainsKey(member.Name) )
                    member.SetValue(obj , ConvertToT(member.DeclaringType , dico[member.Name]));
            }
            IEnumerable<PropertyInfo> props = type.GetRuntimeProperties();
            foreach ( PropertyInfo prp in  props  )
            {
                if ( dico.ContainsKey(prp.Name) )
                    prp.SetValue(obj , ConvertToT(prp.PropertyType , dico[prp.Name]) , new object[0]);
            }


            return obj;
        }

        private static object TryType(Type type1 , Type type2 , object ret)
        {
            if ( type1 == type2 )
                return ret;
            return null;
        }

        internal static dynamic getJsonDyn(StringBuilder sb , ref int pos)
        {
            ExpandoObject obj = new ExpandoObject( );
            IDictionary<string,object> dico = obj;
            pos = skipWhiteChar(sb , pos);
            if ( sb[pos] == '[' )
            {
                List<object> o = new List<object>();
                pos++;
                while ( true )
                {
                    pos = skipWhiteChar(sb , pos);
                    if ( sb[pos] == ']' )
                    {
                        pos++;
                        return o;
                    }
                    else
                    {
                        object val = null;
                        getValueDyn(sb , ref pos , ref val);
                        o.Add(val);
                        pos = skipWhiteChar(sb , pos);
                        if ( sb[pos] == ',' )
                            pos++;
                    }
                }
            }
            else if ( sb[pos] == '{' )
            {
                var isNext = true;
                pos += 1;
                while ( isNext )
                {
                    string key = getString(sb , ref pos);

                    if ( key != null )
                    {
                        //pos = skipWhiteChar(sb , pos) + key.Length + 2;
                        pos = skipWhiteChar(sb , pos);
                        object value = null;
                        if ( sb[pos] == ':' )
                        {
                            pos++;
                            getValueDyn(sb , ref pos , ref value);
                            dico[key] = value;
                        }
                    }
                    pos = skipWhiteChar(sb , pos);
                    if ( sb[pos] == '}' )
                    {
                        pos++;
                        return obj;
                    }
                    isNext = sb[pos] == ',';
                    if ( isNext )
                        pos++;

                }
            }
            return ( ExpandoObject ) dico;
        }
        internal static object getJsonObj(StringBuilder sb , ref int pos)
        {
            Dictionary<string , object> obj = new Dictionary<string , object>( );
            pos = skipWhiteChar(sb , pos);
            if ( sb[pos] == '[' )
            {
                pos++;
                List<object> o = new List<object>( );
                while ( true )
                {
                    pos = skipWhiteChar(sb , pos);
                    if ( sb[pos] == ']' )
                    {
                        pos++;

                        return o;
                    }
                    else
                    {
                        object val = null;
                        getValue(sb , ref pos , ref val);
                        o.Add(val);
                        pos = skipWhiteChar(sb , pos);
                        if ( sb[pos] == ',' )
                            pos++;
                    }
                }
            }
            else if ( sb[pos] == '{' )
            {
                var isNext = true;
                pos += 1;
                while ( isNext )
                {
                    string key = getString(sb , ref pos);

                    if ( key != null )
                    {
                        //pos = skipWhiteChar(sb , pos) + key.Length + 2;
                        pos = skipWhiteChar(sb , pos);
                        object value = null;
                        if ( sb[pos] == ':' )
                        {
                            pos++;
                            getValue(sb , ref pos , ref value);
                            obj[key] = value;
                        }
                    }
                    pos = skipWhiteChar(sb , pos);
                    if ( sb[pos] == '}' )
                    {
                        pos++;
                        return obj;
                    }
                    isNext = sb[pos] == ',';
                    if ( isNext )
                        pos++;

                }
            }
            return obj;
        }
        internal static void getValueDyn(StringBuilder sb , ref int pos , ref object value)
        {
            getValue(sb , ref pos , ref value , true);
        }
        internal static void getValue(StringBuilder sb , ref int pos , ref object value , bool dyn = false)
        {
            pos = skipWhiteChar(sb , pos);
            if ( sb[pos] == '\'' )
            {
                string val = getString(sb , ref pos , '\'');
                value = val;
            }
            else
                if ( sb[pos] == '"' )
            {
                string val = getString(sb , ref pos);
                value = val;
            }
            else if ( sb[pos] == '-' || char.IsDigit(sb[pos]) )
            {
                var val = getNumber(sb , pos);
                pos = val.Item2;
                value = val.Item1;
            }
            else if ( sb[pos] == 'n' && sb[pos + 1] == 'u' && sb[pos + 2] == 'l' && sb[pos + 3] == 'l' )
            {
                value = null;
                pos += 4;
            }
            else if ( sb[pos] == 't' && sb[pos + 1] == 'r' && sb[pos + 2] == 'u' && sb[pos + 3] == 'e' )
            {
                value = true;
                pos += 4;
            }
            else if ( sb[pos] == 'f' && sb[pos + 1] == 'a' && sb[pos + 2] == 'l' && sb[pos + 3] == 's' && sb[pos + 4] == 'e' )
            {
                value = false;
                pos += 5;
            }
            else if ( sb[pos] == '[' || sb[pos] == '{' )
            {
                if ( dyn )
                    value = getJsonDyn(sb , ref pos);
                else
                    value = getJsonObj(sb , ref pos);
            }
        }
        internal static Tuple<object , int> getNumber(StringBuilder json , int pos)
        {
            CultureInfo ci = new CultureInfo("en-ca");

            StringBuilder sb = new StringBuilder( );
            while ( !( json[pos] == ' ' || json[pos] == '\r' || json[pos] == '\n' || json[pos] == '\t' || json[pos] == ',' || json[pos] == '}' || json[pos] == ']' ) )
            {
                sb.Append(json[pos]);
                pos++;
            }
            string txt = sb.ToString( );
            if ( txt.Contains('.') )
                return new Tuple<object , int>(double.Parse(sb.ToString() , System.Globalization.NumberStyles.AllowDecimalPoint , System.Globalization.NumberFormatInfo.InvariantInfo) , pos);
            else
                return new Tuple<object , int>(int.Parse(sb.ToString() , ci.NumberFormat) , pos);


        }
        internal static int skipWhiteChar(StringBuilder json , int pos)
        {

            while ( pos < json.Length && ( json[pos] == ' ' || json[pos] == '\r' || json[pos] == '\n' || json[pos] == '\t' ) )
            {
                pos++;
            }
            return pos;
        }
        internal static string getString(StringBuilder json , ref int pos , char delimiter = '"')
        {
            StringBuilder sb = new StringBuilder( );
            pos = skipWhiteChar(json , pos);
            if ( json[pos] == delimiter )
            {
                pos += 1;
                while ( pos < json.Length )
                {
                    if ( json[pos] == '\\' )
                    {
                        if ( json[pos + 1] == 'b' )
                            sb.Append('\b');
                        else if ( json[pos + 1] == 'f' )
                            sb.Append('\f');
                        else if ( json[pos + 1] == 'n' )
                            sb.Append('\n');
                        else if ( json[pos + 1] == 'r' )
                            sb.Append('\r');
                        else if ( json[pos + 1] == 't' )
                            sb.Append('\t');
                        else if ( json[pos + 1] == '\\' )
                            sb.Append('\\');
                        else if ( json[pos + 1] == 'u' )
                        {
                            var unicodeCode = json.ToString(pos + 2 , 4);
                            var code = Convert.ToInt32(unicodeCode , 16);
                            sb.Append(( char ) code);
                            pos += 4;
                        }
                        if ( delimiter == '\'' )
                            if ( json[pos + 1] == '\'' )
                                sb.Append('\'');
                        if ( delimiter == '"' )
                            if ( json[pos + 1] == '"' )
                                sb.Append('"');
                        pos++;
                    }
                    else if ( json[pos] != delimiter )
                        sb.Append(json[pos]);
                    else
                    {
                        pos++;
                        break;
                    }
                    pos++;
                }
                return sb.ToString();
            }
            return null;
        }

        public static string Serialize(object obj , bool linefeed = false , int deep = 0 , bool isArray = false)
        {
            if ( obj == null )
                return "null";
            if ( obj.GetType().Name.Contains("DBNull") )
                return "null";
            else if ( obj is IJSONString )
            {
                return obj.ToString();
            }
            else if ( obj is DateTime )
            {
                return '"' + ( ( DateTime ) obj ).ToString("yyyy-MM-ddThh:mm:ss") + '"' + String.Empty;
            }
            else if ( obj is TimeSpan )
            {
                return '"' + ( ( TimeSpan ) obj ).ToString("hh:mm:ss") + '"' + String.Empty;
            }
            else if ( obj is string )
            {
                return '"' +
                    string.Join("" ,
                    ( ( obj as string ).Replace("\\" , "\\\\").Replace("\"" , "\\\"").Replace("\n" , "\\n").Replace("\r" , "\\r") )
                    .OfType<char>()
                    .Select((x) => x < 128 ? x.ToString() : "\\u" + ( ( int ) x ).ToString("X").PadLeft(4 , '0')))
                        + '"';
            }
            else if ( obj is int || obj is byte || obj is sbyte || obj is short || obj is long || obj is ushort || obj is ulong )
            {
                return obj.ToString();
            }
            else if ( obj is double )
            {
                return ( ( double ) obj ).ToString(CultureInfo.InvariantCulture);
            }
            else if ( obj is float )
            {
                return ( ( float ) obj ).ToString(CultureInfo.InvariantCulture);
            }
            else if ( obj is decimal )
            {
                return ( ( decimal ) obj ).ToString(CultureInfo.InvariantCulture);
            }
            else if ( obj is Array )
            {
                Array a = obj as Array;
                StringBuilder sb = new StringBuilder( );
                sb.Append('[');
                for ( var i = 0; i < a.GetLength(0); i++ )
                {
                    sb.Append(Serialize(a.GetValue(i) , linefeed , deep + 1 , true));
                    if ( i != a.GetLength(0) - 1 )
                        sb.Append(',');
                }
                sb.Append(']');
                return sb.ToString();
            }
            else if ( obj is System.Collections.IList )
            {
                IList a = obj as IList;
                StringBuilder sb = new StringBuilder( );
                sb.Append('[');
                for ( var i = 0; i < a.Count; i++ )
                {
                    sb.Append(Serialize(a[i] , linefeed , deep + 1 , true));
                    if ( i != a.Count - 1 )
                        sb.Append(',');
                }
                sb.Append(']');
                return sb.ToString();
            }
            else if ( obj is System.Collections.IDictionary )
            {
                StringBuilder sb = new StringBuilder( );
                var dico = obj as System.Collections.IDictionary;
                if ( linefeed && isArray )
                {
                    sb.Append(Environment.NewLine);
                    sb.Append(" ".PadLeft(deep * 4));
                }
                sb.Append('{');
                if ( linefeed )
                    sb.Append(Environment.NewLine);
                var ks = dico.Keys;
                for ( var i = 0; i < ks.Count; i++ )
                {
                    var key = ks.OfType<object>( ).ElementAt(i);
                    var val = dico[ key ];
                    if ( linefeed )
                        sb.Append(" ".PadLeft(deep * 4));
                    sb.Append('"');
                    sb.Append(( key + "" ).Replace("\"" , "\\\""));
                    sb.Append('"');
                    sb.Append(':');
                    sb.Append(Serialize(val , linefeed , deep + 1 , false));

                    if ( i != ks.Count - 1 )
                        sb.Append(',');
                    if ( linefeed )
                        sb.Append(Environment.NewLine);
                }
                if ( linefeed )
                    sb.Append(" ".PadLeft(deep * 4));
                sb.Append('}');
                return sb.ToString();
            }
            else if ( obj is bool )
            {
                return obj.ToString().ToLower();
            }
            else
            {
                var dico = DictionaryFromType(obj);
                StringBuilder sb = new StringBuilder( );
                sb.Append('{');
                if ( linefeed )
                    sb.Append(Environment.NewLine);
                for ( var i = 0; i < dico.Count; i++ )
                {
                    var kv = dico.ElementAt(i);
                    if ( linefeed )
                        sb.Append(" ".PadLeft(deep * 4));
                    sb.Append('"');
                    sb.Append(kv.Key.Replace("\"" , "\\\""));
                    sb.Append('"');
                    if ( linefeed )
                        sb.Append(' ');
                    sb.Append(':');
                    if ( linefeed )
                        sb.Append(' ');
                    sb.Append(Serialize(kv.Value , linefeed , deep + 1 , false));


                    if ( i != dico.Count - 1 )
                        sb.Append(',');
                    if ( linefeed )
                        sb.Append(Environment.NewLine);
                }
                sb.Append(" ".PadLeft(deep * 4));
                if ( linefeed )
                    sb.Append(Environment.NewLine);
                sb.Append('}');
                return sb.ToString();
            }
        }
    }
}
