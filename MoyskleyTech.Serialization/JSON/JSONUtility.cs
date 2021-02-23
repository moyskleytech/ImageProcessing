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
        [Obsolete("Use Parse")]
        public static Dictionary<string , object> parseJSON(string json)
        {
            return Parse(json);
        }
        public static object Deserialize(string json)
        {
            StringBuilder sb = new StringBuilder(json.Trim( ));
            int pos=0;
            object output=null;
            getValue(sb , ref pos , ref output);
            return output;
        }
        public static List<object> ParseList(string json)
        {
            StringBuilder sb = new StringBuilder(json.Trim( ));
            int pos=0;
            return getJsonArray(sb , ref pos);
        }
        public static Dictionary<string , object> ParseObject(string json)
        {
            StringBuilder sb = new StringBuilder(json.Trim( ));
            int pos=0;
            return getJsonObj(sb , ref pos);
        }
        public static Dictionary<string , object> Parse(string json)
        {
            if ( string.IsNullOrWhiteSpace(json) )
                return null;
            var ret = Deserialize(json);
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
        internal static List<object> getJsonArray(StringBuilder sb , ref int pos)
        {
            List<object> obj = null;
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
            else
                throw new InvalidProgramException("JSON string is in an incorrect format");
        }
        internal static Dictionary<string , object> getJsonObj(StringBuilder sb , ref int pos)
        {
            Dictionary<string , object> obj =null;
            pos = skipWhiteChar(sb , pos);
            if ( sb[pos] == '{' )
            {
                obj = new Dictionary<string , object>();
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
            else
                throw new InvalidProgramException("JSON string is in an incorrect format");
            return obj;
        }
        internal static void getValue(StringBuilder sb , ref int pos , ref object value)
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
            else if ( sb[pos] == '[' )
                value = getJsonArray(sb , ref pos);
            else
                value = getJsonObj(sb , ref pos);
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
    }
}
