using MoyskleyTech.ImageProcessing.Image;
using System;

namespace MoyskleyTech.ImageProcessing.Filters
{
    public static class Squarify
    {
        /// <summary>
        /// Not deterministic approach to censor images
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="size"></param>
        public static void Apply(ImageProxy proxy,int size)
        {
            Random r = new Random();
            Pixel chooseColor(int x, int y)
            {
                int sx = r.Next(x,System.Math.Min(x+size,proxy.Width));
                int sy = r.Next(y,System.Math.Min(y+size,proxy.Height));
                return proxy[sx , sy];
            }
            for ( var x = 0; x < proxy.Width; x += size )
            {
                for ( var y = 0; y < proxy.Height; y += size )
                {
                    Pixel p = chooseColor(x,y);
                    for ( var dx = x; dx < x + size && dx < proxy.Width; dx++ )
                        for ( var dy = y; dy < y + size && dy < proxy.Height; dy++ )
                            proxy[dx , dy] = p;
                }
            }
        }
        public static void Apply<Representation>(ImageProxy<Representation> proxy , int size)
            where Representation:struct
        {
            Random r = new Random();
            Representation chooseColor(int x , int y)
            {
                int sx = r.Next(x,System.Math.Min(x+size,proxy.Width));
                int sy = r.Next(y,System.Math.Min(y+size,proxy.Height));
                return proxy[sx , sy];
            }
            for ( var x = 0; x < proxy.Width; x += size )
            {
                for ( var y = 0; y < proxy.Height; y += size )
                {
                    Representation p = chooseColor(x,y);
                    for ( var dx = x; dx < x + size && dx < proxy.Width; dx++ )
                        for ( var dy = y; dy < y + size && dy < proxy.Height; dy++ )
                            proxy[dx , dy] = p;
                }
            }
        }
    }
}
