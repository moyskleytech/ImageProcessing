using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Serialization.XML
{
    public class XMLNode
    {
        public XMLNode() : this(XMLNodeType.Node)
        {

        }
        public XMLNode(XMLNodeType type)
        {
            this.Type = type;
        }

        public Dictionary<string , object> Attributes { get; set; } = new Dictionary<string , object>();
        public string Name { get; set; }
        public List<XMLNode> Children { get; set; } = new List<XMLNode>();
        public XMLNode Parent { get; private set; }
        public XMLNodeType Type { get; private set; }
        public static XMLNode Parse(string s)
        {
            int u;
            return ParseInternal(s , out u);
        }
        private static XMLNode ParseInternal(string s , out int len)
        {
            len = 0;
            XMLNode tmp = new XMLNode();
            StringBuilder sb = new StringBuilder(s);
            StringBuilder sb2 = new StringBuilder();
            string currentAttributeName="";

            const int WAITING_FOR_LT=0;
            const int WAITING_FOR_SPACE=1;
            const int WAITING_FOR_ATTRIBUTE_OR_GT=2;
            const int WAITING_FOR_EQUALS=3;
            const int WAITING_FOR_ATTRIBUTE=10;
            const int WAITING_FOR_STRING=4;
            const int WAITING_FOR_STRING_END=5;
            const int WAITING_FOR_TEXTNODE_LT=6;
            const int WAITING_FOR_COMMENT_BEGIN=7;
            const int WAITING_FOR_COMMENT_END=8;
            const int WAITING_FOR_DEFINITION_END=9;

            /*
             
             TRANSITION
             
             WAITING_FOR_LT
                '<'     -> WAITING_FOR_SPACE
                !< !_   -> WAITING_FOR_TEXTNODE_LT
             WAITING_FOR_SPACE
                '!'     -> WAITING_FOR_COMMENT_END
                '?'     -> WAITING_FOR_DEFINITION_END
                 _      -> WAITING_FOR_ATTRIBUTE_OR_GT
                '>' /   -> RETURN
                '>'!/   -> FIND CHILDREN
             WAITING_FOR_ATTRIBUTE_OR_GT
                'a'     -> WAITING_FOR_ATTRIBUTE
                '>' /   -> RETURN
                '>'!/   -> FIND CHILDREN
             WAITING_FOR_EQUALS
                '='     -> WAITING_FOR_STRING
                'a'     -> WAITING_FOR_EQUALS(Add Attribute)
             WAITING_FOR_STRING
                '"'     -> WAITING_FOR_STRING_END
                '''     -> WAITING_FOR_STRING_END
             WAITING_FOR_STRING_END
                '''     -> WAITING_FOR_ATTRIBUTE_OR_GT
                '"'     -> WAITING_FOR_ATTRIBUTE_OR_GT
             WAITING_FOR_TEXTNODE_LT
                '<'     -> RETURN TEXTNODE
             WAITING_FOR_COMMENT_BEGIN
                '--'    -> WAITING_FOR_COMMENT_END
             WAITING_FOR_COMMENT_END
                '-->'   -> RETURN COMMENT NODE
             WAITING_FOR_DEFINITION_END
                '?>'    -> WAITING_FOR_LT
             FIND CHILDREN
                ENDNODE ->RETURN NODE
             */


            char currentStringChar='"';
            int mode=WAITING_FOR_LT;
            int tcount=0;
            int i=0;
            for ( i = 0 ; i < sb.Length ; i++ )
            {
                if ( mode == WAITING_FOR_STRING_END && sb[i] == currentStringChar )
                {
                    mode = WAITING_FOR_ATTRIBUTE_OR_GT;
                    var val = sb2.ToString();
                    val = RemoveEntities(val);
                    tmp.Attributes[currentAttributeName] = val;
                }
                else if ( mode == WAITING_FOR_STRING_END && sb[i] != currentStringChar )
                {
                    sb2.Append(sb[i]);
                }
                else if ( mode == WAITING_FOR_STRING && sb[i] == '"' || sb[i] == '\'' )
                {
                    currentStringChar = sb[i];
                    mode = WAITING_FOR_STRING_END;
                    sb2.Clear();
                }
                else if ( mode == WAITING_FOR_EQUALS && char.IsLetter(sb[i]) )
                {
                    mode = WAITING_FOR_ATTRIBUTE;
                    currentAttributeName = sb2.ToString().Trim();
                    sb.Clear();
                    tmp.Attributes.Add(currentAttributeName , "");
                }
                else if ( mode == WAITING_FOR_ATTRIBUTE && sb[i] != '=' )
                {
                    sb2.Append(sb[i]);
                }
                else if ( mode == WAITING_FOR_ATTRIBUTE && char.IsWhiteSpace(sb[i]) )
                {
                    mode = WAITING_FOR_EQUALS;
                }
                else if ( ( mode == WAITING_FOR_EQUALS || mode == WAITING_FOR_ATTRIBUTE ) && sb[i] == '=' )
                {
                    mode = WAITING_FOR_STRING;
                    currentAttributeName = sb2.ToString().Trim();
                }
                else if ( mode == WAITING_FOR_ATTRIBUTE_OR_GT && sb[i] == '>' || mode == WAITING_FOR_SPACE && sb[i] == '>' )
                {
                    if ( mode == WAITING_FOR_SPACE )
                    {
                        tmp.Name = sb2.ToString();

                    }
                    if ( tmp.Name[0] == '/' )
                    {
                        i++;
                        break;
                    }
                    //Find childrens
                    bool foundEnd = false;
                    i++;
                    while ( !foundEnd )
                    {
                        int u=0;
                        XMLNode xmlNode = ParseInternal(s.Substring(i) , out u);
                        if ( xmlNode.Name.ToUpper() == "/" + tmp.Name.ToUpper() )
                            foundEnd = true;
                        else
                        {
                            xmlNode.Parent = tmp;
                            tmp.Children.Add(xmlNode);
                        }
                        i += u;
                    }
                    break;
                }
                else if ( mode == WAITING_FOR_ATTRIBUTE_OR_GT && sb[i] == '/' )
                {
                    len = i;
                    i++;
                    i++;
                    break;
                }
                else if ( mode == WAITING_FOR_ATTRIBUTE_OR_GT && Char.IsLetter(sb[i]) )
                {
                    mode = WAITING_FOR_ATTRIBUTE;

                    sb2.Clear();
                    sb2.Append(sb[i]);
                }
                else if ( mode == WAITING_FOR_SPACE && sb2.Length > 0 && sb[i] == '/' )
                {
                    tmp.Name = sb2.ToString();
                    i++;
                    i++;
                    break;
                }
                else if ( mode == WAITING_FOR_SPACE && char.IsWhiteSpace(sb[i]) )
                {
                    tmp.Name = sb2.ToString();
                    mode = WAITING_FOR_ATTRIBUTE_OR_GT;
                }
                else if ( mode == WAITING_FOR_SPACE && sb[i] == '?' )
                {
                    mode = WAITING_FOR_DEFINITION_END;
                }
                else if ( mode == WAITING_FOR_SPACE && sb[i] == '!' )
                {
                    mode = WAITING_FOR_COMMENT_BEGIN;
                    tcount = 0;
                }
                else if ( mode == WAITING_FOR_SPACE && !char.IsWhiteSpace(sb[i]) )
                {
                    sb2.Append(sb[i]);
                }
                else if ( mode == WAITING_FOR_DEFINITION_END )
                {
                    if ( sb[i] == '?' )
                    {
                        if ( sb[i + 1] == '>' )
                        {
                            i++;
                            mode = WAITING_FOR_LT;
                        }
                    }
                }
                else if ( mode == WAITING_FOR_COMMENT_END )
                {
                    sb2.Append(sb[i]);
                    if ( sb[i] == '-' )
                        tcount++;
                    else
                        tcount = 0;
                    if ( tcount >= 2 )
                    {
                        if ( sb[i + 1] == '>' )
                        {
                            i++;
                            i++;
                            sb2.Remove(sb2.Length - 2 , 2);
                            tmp.Name = sb2.ToString();
                            tmp.Type = XMLNodeType.Comment;
                            break;
                        }
                    }
                }
                else if ( mode == WAITING_FOR_COMMENT_BEGIN && sb[i] == '-' )
                {
                    tcount++;
                    if ( tcount == 2 )
                    {
                        mode = WAITING_FOR_COMMENT_END;
                        tcount = 0;
                    }
                }

                else if ( mode == WAITING_FOR_TEXTNODE_LT && sb[i] == '<' )
                {
                    tmp.Name = RemoveEntities(sb2.ToString());
                    break;
                }
                else if ( mode == WAITING_FOR_TEXTNODE_LT && sb[i] != '<' )
                {
                    sb2.Append(sb[i]);
                }
                else if ( mode == WAITING_FOR_LT && sb[i] != '<' && !char.IsWhiteSpace(sb[i]) )
                {
                    tmp.Type = XMLNodeType.Text;
                    mode = WAITING_FOR_TEXTNODE_LT;
                    sb2.Clear();
                    sb2.Append(sb[i]);
                }
                else if ( mode == WAITING_FOR_LT && sb[i] == '<' )
                {
                    mode = WAITING_FOR_SPACE;
                    sb2.Clear();
                }

            }
            len = i;
            return tmp;
        }

        private static string RemoveEntities(string val)
        {
            if ( val == "NULL" )
                return null;
            return val.Replace("&amp;" , "&").Replace("&gt;" , ">").Replace("&apos;" , "'").Replace("&lt;" , "<").Replace("&quot;" , "\"");
        }
        private static string AddEntities(string val)
        {
            if ( val == null )
                return "NULL";
            return val.Replace("&" , "&amp;").Replace(">" , "&gt;").Replace("'" , "&apos;").Replace("<" , "&lt;").Replace("\"" , "&quot;");
        }
        public IEnumerable<XMLNode> Decendants
        {
            get
            {
                yield return this;
                foreach ( XMLNode child in Children )
                    yield return child;
                foreach ( XMLNode child in Children )
                    foreach ( XMLNode childOfChild in child.Decendants )
                        if ( child != childOfChild )
                            yield return childOfChild;
            }
        }
        public void RemoveChild(XMLNode node)
        {
            Children.Remove(node);
        }
        public IEnumerable<XMLNode> QuerySelectorAll(string selector)
        {
            selector = selector.Trim();
            var className = "";
            var attribute ="";
            var attributeValue="";
            className = selector;
            var selectorChild = "";
            if ( selector.Contains(">") )
            {
                var splitted = selector.Split('>');
                selector = splitted[0].TrimEnd();
                className = selector;
                selectorChild = string.Join(">" , splitted.Skip(1)).TrimStart();
            }
            if ( selector.Contains("[") )
            {
                className = className.Remove(className.IndexOf('['));
                attribute = selector.Substring(selector.IndexOf('[') + 1);
                attribute = attribute.Remove(attribute.IndexOf(']'));

                if ( attribute.Contains("=") )
                {
                    attributeValue = attribute.Substring(attribute.IndexOf('=') + 1);
                    attribute = attribute.Remove(attribute.IndexOf('='));
                }
            }
            className = className.ToUpper();
            attribute = attribute.ToUpper();
            attributeValue = attributeValue.ToUpper();
            if ( selector.Trim().ToUpper() == ":TEXT" )
            {
                foreach ( XMLNode that in Decendants.Where((x) => x.Type == XMLNodeType.Text) )
                    yield return that;
                yield break;
            }
            if ( selector.Trim().ToUpper() == ":COMMENT" )
            {
                foreach ( XMLNode that in Decendants.Where((x) => x.Type == XMLNodeType.Comment) )
                    yield return that;
                yield break;
            }

            IEnumerable<XMLNode> thisResult = Decendants.Where((x) =>
            {
                if ( attribute == "" )
                {
                    return x.Type == XMLNodeType.Node && x.Name.ToUpper() == className;
                }
                if ( className == "" )
                {
                    return x.Type == XMLNodeType.Node && AttributeCondition(x , attribute , attributeValue);
                }
                return x.Type == XMLNodeType.Node && x.Name.ToUpper() == className && AttributeCondition(x , attribute , attributeValue);
            });
            if ( selectorChild != "" )
            {
                foreach ( XMLNode that in thisResult )
                {
                    foreach ( XMLNode that2 in that.QuerySelectorAll(selectorChild) )
                        yield return that2;
                }
            }
            else
            {
                foreach ( XMLNode that in thisResult )
                    yield return that;
            }
        }

        private static bool AttributeCondition(XMLNode x , string attribute , string attributeValue)
        {
            if ( attributeValue == "" )
                return x.Attributes.Any((a) =>
                a.Key.ToUpper() == attribute);
            else
                return x.Attributes.Any((a) =>
                a.Key.ToUpper() == attribute && a.Value?.ToString().ToUpper() == attributeValue);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if ( Type == XMLNodeType.Node )
            {
                sb.Append("<");
                sb.Append(Name);

                foreach ( var kv in Attributes )
                {
                    sb.Append(' ');
                    sb.Append(kv.Key);
                    sb.Append('=');
                    sb.Append('"');
                    sb.Append(AddEntities(kv.Value?.ToString()));
                    sb.Append('"');
                }
                if ( Children.Count == 0 )
                    sb.Append("/>");
                else
                {
                    sb.Append('>');
                    foreach ( var child in Children )
                    {
                        sb.Append(child.ToString());
                    }
                    sb.Append("</");
                    sb.Append(Name);
                    sb.Append('>');
                }
            }
            else if ( Type == XMLNodeType.Text )
            {
                sb.Append(AddEntities(Name));
            }
            else if ( Type == XMLNodeType.Comment )
            {
                sb.Append("<!--");
                sb.Append(Name);
                sb.Append("-->");
            }
            return sb.ToString();
        }

        public enum XMLNodeType
        {
            Node, Text,
            Comment
        }
    }
}
