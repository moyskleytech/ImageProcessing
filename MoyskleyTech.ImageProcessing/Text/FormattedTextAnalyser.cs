using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace MoyskleyTech.ImageProcessing.Text
{
    public class FormattedTextAnalyser : IEnumerable<FormattedChar>
    {
        protected struct FormattedTextStateF
        {
            public Brush color;
            public float size;
            public bool bold;
            public bool italic;
        }
        private string raw;
        private string processed;
        private List<FormattedChar> chars=new List<FormattedChar>();
        public FormattedTextAnalyser(string formatted , Brush defaultBrush = null , float defaultSize = 1)
        {
            raw = formatted;
            if ( defaultBrush == null )
                defaultBrush = new SolidBrush(Pixels.Black);

            Stack<FormattedTextStateF> stateStack = new Stack<FormattedTextStateF>();
            FormattedTextStateF currentState = new FormattedTextStateF() { bold=false,italic=false, color=(Brush)defaultBrush,size=defaultSize };
            StringBuilder sbToken = new StringBuilder();

            Dictionary<string,Action<string>> methods = new Dictionary<string, Action<string>>(StringComparer.CurrentCultureIgnoreCase)
            {
                {"color",(s)=> {
                    var splitted = s.Split(',');
                    byte a=255,r,g,b;
                    byte index=0;
                    if(splitted.Length==4)
                        a=Convert.ToByte(splitted[index++]);
                    r=Convert.ToByte(splitted[index++]);
                    g=Convert.ToByte(splitted[index++]);
                    b=Convert.ToByte(splitted[index++]);
                    currentState.color = (Brush)Pixel.FromArgb(a,r,g,b);
                } },
                {"i",(s)=> { currentState.italic=true; }},
                {"emph",(s)=> { currentState.italic=true; }},
                {"b",(s)=> { currentState.bold=true; }},
                {"!i",(s)=> { currentState.italic=false; }},
                {"!emph",(s)=> { currentState.italic=false; }},
                {"!b",(s)=> { currentState.bold=false; }},
                {"size",(s)=> { currentState.size=Convert.ToSingle(s);}}
            };

            int readState=0;
            bool render=true;
            string methodName=null;
            foreach ( char c in raw )
            {
                redoChar: render = true;
                if ( c == '\\' && readState == 0 )
                {
                    readState = 1;
                    sbToken.Clear();
                    render = false;
                }
                else if ( c == '\\' && readState == 1 )
                {
                    readState = 0;
                    render = true;
                }
                else if ( c == '{' && readState == 0 )
                {
                    stateStack.Push(currentState);
                    render = false;
                }
                else if ( c == '}' && readState == 0 )
                {
                    currentState = stateStack.Pop();
                    render = false;
                }
                else if ( readState == 1 )
                {
                    sbToken.Append(c);
                    render = false;
                    if ( methods.ContainsKey(sbToken.ToString()) )
                    {
                        methodName = sbToken.ToString();
                        readState = 2;
                    }
                }
                else if ( c != '{' && readState == 2 )
                {
                    methods[methodName](null);
                    readState = 0;
                    goto redoChar;
                }
                else if ( c == '{' && readState == 2 )
                {
                    readState = 3;
                    sbToken.Clear();
                    render = false;
                }
                else if ( c != '}' && readState == 3 )
                {
                    sbToken.Append(c);
                    render = false;
                }
                else if ( c == '}' && readState == 3 )
                {
                    methods[methodName](sbToken.ToString());
                    readState = 0;
                    render = false;
                }

                if ( render )
                {
                    PushChar(c , currentState);
                }
            }
            processed = new string(chars.Select((x) => x.Char).ToArray());
        }

        private void PushChar(char c , FormattedTextStateF currentState)
        {
            FormattedChar fc = new FormattedChar()
            {
                Bold = currentState.bold,
                Char=c,
                Color = currentState.color,
                Italic=currentState.italic,
                Size =currentState.size
            };
            chars.Add(fc);
        }

        public IEnumerator<FormattedChar> GetEnumerator()
        {
            return ( ( IEnumerable<FormattedChar> ) chars ).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IEnumerable<FormattedChar> ) chars ).GetEnumerator();
        }
        public int Length { get { return chars.Count; } }
        public string Text { get { return processed; } }
        public FormattedChar this[int p]
        { get { return chars[p]; } }
    }
    public struct FormattedChar
    {
        public char Char { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public Brush Color { get; set; }
        public float Size { get; set; }
    }
}
