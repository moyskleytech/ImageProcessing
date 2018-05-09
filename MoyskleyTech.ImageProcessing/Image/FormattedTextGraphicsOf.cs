using MoyskleyTech.ImageProcessing.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public unsafe partial class Graphics<Representation>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="currentState"></param>
        /// <returns></returns>
        protected virtual int DrawFormattedCharInternal(bool[ , ] character , int x , int y , Graphics.FormattedTextState currentState)
        {
            var size = currentState.size;

            int h = character.GetLength(0);
            int w = character.GetLength(1);
            int ox=x,oy=y;
            int boldLevel=1;
            var brh = ColorConvert.Convert<Pixel,Representation>(currentState.color);
            if ( currentState.bold )
                boldLevel = 3;
            for ( var i = 0; i < h; i++ )
            {
                for ( var b = 0; b < boldLevel; b++ )
                {
                    x = ox + size * b;
                    if ( currentState.italic )
                        x += ( h - i ) / 4;
                    for ( var j = 0; j < w; j++ )
                    {
                        x += size;
                        if ( character[i , j] )
                            for ( var k = 0; k < size; k++ )
                            {
                                for ( var l = 0; l < size; l++ )
                                {
                                    SetPixel(brh , x + k , y + l);
                                }
                            }
                    }
                }
                y += size;
            }
            return character.GetLength(1) * size + size;
        }
        /// <summary>
        /// Draw string using formatted text
        /// </summary>
        /// <param name="str"></param>
        /// <param name="p"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="f"></param>
        /// <param name="size"></param>
        public virtual void DrawFormattedString(string str , Representation p , float x , float y , Font f , float size)
        {
            DrawFormattedString(str , new SolidBrush<Representation>(p) , x , y , f , size);
        }
        /// <summary>
        /// Draw string using formatted text
        /// </summary>
        /// <param name="str"></param>
        /// <param name="p"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="f"></param>
        /// <param name="size"></param>
        public virtual void DrawFormattedString(string str , Brush<Representation> p , float x , float y , Font f , float size)
        {
            //bmp.instructions.AddLast(new Instruction("text" , str , x , y , p , size));
            float ox=x,oy=y;
            float mh = 0;

            FormattedTextAnalyser analyser = new FormattedTextAnalyser(str,ColorConvert.Convert<Representation,Pixel>(p) ,size);

            foreach ( var fc in analyser )
            {
                bool[,] character = f.GetChar(fc.Char);
                mh = System.Math.Max(mh , character.GetLength(0) * fc.Size);
                if ( fc.Char == 10 )
                {
                    x = ox;
                    y += mh + 1;
                    mh = 0;
                }
                else
                    x += DrawFormattedCharInternalF(character , x , y , fc);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="currentState"></param>
        /// <returns></returns>
        protected virtual float DrawFormattedCharInternalF(bool[ , ] character , float x , float y , FormattedChar currentState)
        {
            var intState= new Graphics.FormattedTextState() { bold = currentState.Bold , color = (currentState.Color) , italic = currentState.Italic , size = ( int ) currentState.Size };
            if ( ( int ) currentState.Size == currentState.Size )
                return DrawFormattedCharInternal(character , ( int ) x , ( int ) y , intState);
            var size = currentState.Size;
            int h = character.GetLength(0);
            int w = character.GetLength(1);
            if ( h != 0 && w != 0 )
            {
                Bitmap src = new Bitmap((w*(int)(size+1))*2,(h*(int)(size+1)));
                Bitmap bmp = new Bitmap((int)(w*size)*2,(int)(h*size));
                Graphics gSrc = Graphics.FromImage(src);
                intState.size += 1;
                gSrc.Clear(Pixels.Transparent);
                gSrc.DrawFormattedCharInternal(character , 0 , 0 , intState);
                bmp = src.Rescale(bmp.Width , bmp.Height , ScalingMode.AverageInterpolate);
                var tmp = (Image<Pixel>)bmp;
                var tmp2=tmp.ConvertTo<Representation>();
                DrawImage(tmp2 , ( int ) x , ( int ) y);
                tmp2.Dispose();
                tmp.Dispose();
            }
            return w * size + size;
        }

    }
}
