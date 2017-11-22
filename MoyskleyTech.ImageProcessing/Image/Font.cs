using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Represent a FOnt
    /// </summary>
    public class Font
    {
        private bool[][,] chars = new bool[256][,];
        private string name;
        /// <summary>
        /// Create Font without name
        /// </summary>
        public Font():this("[unnamed]") {

        }
        /// <summary>
        /// Return the name
        /// </summary>
        public string Name { get { return name; } }
        /// <summary>
        /// Create Font with name
        /// </summary>
        /// <param name="name"></param>
        public Font(string name)
        {
            this.name = name;
            for ( var i = 0 ; i < 256 ; i++ )
                chars[i] = new bool[0 , 0];
        }
        /// <summary>
        /// Get Char of Font
        /// </summary>
        /// <param name="c">char</param>
        /// <returns>Char as bool[,]</returns>
        public bool[ , ] GetChar(char c)
        {
            return chars[( byte ) c];
        }
        /// <summary>
        /// Set font char
        /// </summary>
        /// <param name="c">char</param>
        /// <param name="character">char as bool[,]</param>
        public void SetChar(char c , bool[ , ] character)
        {
            System.Diagnostics.Debug.WriteLine("Settings char " + c + " in Font " + name);
            SetChar(( int ) c , character);
        }
        /// <summary>
        /// Set font char
        /// </summary>
        /// <param name="c">char</param>
        /// <param name="character">char as bool[,]</param>
        public void SetChar(int c , bool[ , ] character)
        {
            chars[( byte ) c] = character;
        }

        /// <summary>
        /// Open Font from file
        /// </summary>
        /// <param name="fs">Stream of file</param>
        /// <returns>Font</returns>
        public static Font FromFileStream(Stream fs)
        {
            byte[] length = new byte[4];
            fs.Read(length , 0 , 4);
            int sizeOf=BitConverter.ToInt32(length,0);

            var str = new string(' ' , sizeOf);
            unsafe
            {
                fixed ( char* fptr = str )
                {
                    char* ptr = fptr;
                    for ( var i = 0 ; i < sizeOf ; i++ )
                        *ptr++ = ( char ) fs.ReadByte();
                }
            }
            Font f = FromStream(fs);
            f.name = str;
            return f;
        }
        /// <summary>
        /// Open Font from stream
        /// </summary>
        /// <param name="ms">Stream</param>
        /// <returns>Font</returns>
        public static Font FromStream(Stream ms)
        {
            Font f = new Font("[Stream]");
            for ( var c = 0 ; c < 256 ; c++ )
            {
                byte h = (byte)ms.ReadByte();
                byte w = (byte)ms.ReadByte();
                bool [,] character = new bool[h,w];
                for ( var i = 0 ; i < h ; i++ )
                {
                    //Read line
                    {
                        byte b=(byte)ms.ReadByte(); ;
                        for ( var j = 0 ; j < w ; j++ )
                        {
                            if ( j % 8 == 0 && j != 0 )
                                b = ( byte ) ms.ReadByte();

                            int mod = 7 - (j%8);

                            character[i , j] = ( ( ( b >> mod ) & 0x01 ) == 1 );
                        }
                    }
                }
                f.SetChar(( char ) c , character);
            }
            return f;
        }
        /// <summary>
        /// Write Font to file
        /// </summary>
        /// <param name="fs">Stream of file</param>
        public void ToFileStream(Stream fs)
        {
            fs.Write(BitConverter.GetBytes(name.Length) , 0 , 4);
            unsafe
            {
                fixed ( char* fptr = name )
                {
                    char* ptr = fptr;
                    for ( var i = 0 ; i < name.Length ; i++ )
                        fs.WriteByte(( byte ) *ptr++);
                }
            }
            ToStream(fs);
        }
        /// <summary>
        /// Write Font to Stream
        /// </summary>
        /// <param name="ms">Stream</param>
        public void ToStream(Stream ms)
        {
            for ( var c = 0 ; c < 256 ; c++ )
            {
                bool[,] character = chars[c];

                byte h = (byte)character.GetLength(0);
                byte w = ( byte ) character.GetLength(1);
                ms.WriteByte(h);
                ms.WriteByte(w);

                for ( var i = 0 ; i < h ; i++ )
                {
                    //Write line
                    {
                        byte b=0;
                        for ( var j = 0 ; j < w ; j++ )
                        {
                            int mod = j%8;

                            b |= ( byte ) ( ( ( character[i , j] ) ? 1 : 0 ) << ( 7 - mod ) );
                            if ( j % 8 == 7 )
                            {
                                ms.WriteByte(b);
                                b = 0;
                            }
                        }
                        if ( w % 8 != 0 )
                            ms.WriteByte(b);
                    }
                }
            }
        }
    }
}
