using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Text
{
    /// <summary>
    /// Create the formatted text format used in Graphics
    /// </summary>
    public class FormattedTextBuilder
    {
        /// <summary>
        /// Create a formatter for specified text
        /// </summary>
        /// <param name="src"></param>
        public FormattedTextBuilder(string src)
        {
            this.src = src;
        }
        private string src;
        private enum FormatActions
        {
            Bold,Italic,Size,Color
        }

        private class FormatAction
        {
            public FormatActions format;
            public float value;
            public float nvalue;
            public string stringValue;
            public string nstringValue;
            public int begin,end;
        }

        private List<FormatAction> actions = new List<FormatAction>();
        /// <summary>
        /// Set bold on interval
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        public void Bold(int begin , int end)
        {
            actions.Add(new FormatAction() { format = FormatActions.Bold , begin = begin , end = end });
        }
        /// <summary>
        /// Set Italic in interval
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        public void Italic(int begin , int end)
        {
            actions.Add(new FormatAction() { format = FormatActions.Italic , begin = begin , end = end });
        }
        /// <summary>
        /// Set the size on interval
        /// </summary>
        /// <param name="size"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        public void Size(float size,int begin , int end)
        {
            actions.Add(new FormatAction() { format = FormatActions.Size , begin = begin , end = end, value=size });
        }
        /// <summary>
        /// Set the color on internal
        /// </summary>
        /// <param name="color"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        public void Color(Pixel color , int begin , int end)
        {
            actions.Add(new FormatAction() { format = FormatActions.Color , begin = begin , end = end , stringValue = color.A+","+color.R+","+color.G+","+color.B });
        }
        /// <summary>
        /// Build the formatted text for specified formatted info
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<int,List<FormatAction>> actionsTab = new Dictionary<int,List<FormatAction>>();
            Dictionary<int,List<FormatAction>> actionsTabEnd = new Dictionary<int,List<FormatAction>>();
            for ( var i = 0; i < actions.Count; i++ )
            {
                var begin = actions[i].begin;
                var end = actions[i].end;

                if ( !actionsTab.ContainsKey(begin) )
                    actionsTab[begin] = new List<FormatAction>();
                if ( !actionsTabEnd.ContainsKey(end) )
                    actionsTabEnd[end] = new List<FormatAction>();

                actionsTab[begin].Add(actions[i]);
                actionsTabEnd[end].Add(actions[i]);
            }
            float size=1;
            string color = "255,0,0,0";
            for ( var i = 0; i < src.Length; i++ )
            {
                if ( actionsTabEnd.ContainsKey(i - 1) )
                {
                    foreach ( var action in actionsTabEnd[i - 1] )
                    {
                        switch ( action.format )
                        {
                            case FormatActions.Bold:
                                sb.Append("\\!b");
                                break;
                            case FormatActions.Italic:
                                sb.Append("\\!i");
                                break;
                            case FormatActions.Size:
                                size = action.nvalue;
                                sb.Append("\\size{" + action.nvalue.ToString("0.0") + "}");
                                break;
                            case FormatActions.Color:
                                color = action.nstringValue;
                                sb.Append("\\color{" + action.nstringValue + "}");
                                break;
                        }
                    }
                }
                if ( actionsTab.ContainsKey(i) )
                {
                    foreach ( var action in actionsTab[i] )
                    {
                        switch ( action.format )
                        {
                            case FormatActions.Bold:
                                sb.Append("\\b");
                                break;
                            case FormatActions.Italic:
                                sb.Append("\\i");
                                break;
                            case FormatActions.Size:
                                action.nvalue = size;
                                size = action.value;
                                sb.Append("\\size{" + action.value.ToString("0.0") + "}");
                                break;
                            case FormatActions.Color:
                                action.nstringValue = color;
                                color = action.stringValue;
                                sb.Append("\\color{" + action.stringValue + "}");
                                break;
                        }
                    }
                }
                
                char c= src[i];
                if ( c == '\\' || c == '{' || c == '}' )
                    sb.Append('\\');
                sb.Append(src[i]);
            }
            return sb.ToString();
        }
    }
}
