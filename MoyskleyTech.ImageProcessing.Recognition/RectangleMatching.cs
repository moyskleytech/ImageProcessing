using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyskleyTech.ImageProcessing.Recognition
{
    public static class RectangleMatching
    {
        public static Rectangle MatchRectangle(this Bitmap bmp,int x,int y,Func<Pixel , bool> condition)
        {
            Rectangle zone = new Rectangle(x,y,1,1);

            bool foundThisRound=false;
            do {
                foundThisRound = false;

                for ( var i = zone.X; zone.Y > 0 && i <= zone.Right; i++ )
                    if ( condition(bmp[i , zone.Y - 1]) )
                    {
                        zone.Y--;
                        zone.Height++;
                        foundThisRound = true;
                        break;
                    }
                for ( var i = zone.X; zone.Bottom+1 > 0 && i <= zone.Right; i++ )
                    if ( condition(bmp[i , zone.Bottom + 1]) )
                    {
                        zone.Height++;
                        foundThisRound = true;
                        break;
                    }

                for (var j=zone.Y;zone.Right+1 < bmp.Width&& j<=zone.Bottom;j++ )
                    if ( condition(bmp[zone.Right + 1 , j]) )
                    {
                        zone.Width++;
                        foundThisRound = true;
                        break;
                    }
                for ( var j = zone.Y; zone.X >0 && j <= zone.Bottom; j++ )
                    if ( condition(bmp[zone.X - 1 , j]) )
                    {
                        zone.X--;
                        zone.Width++;
                        foundThisRound = true;
                        break;
                    }

            } while ( foundThisRound );
            return zone;
        }
    }
}
