using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public class ImageProxy
    {
        private Bitmap img;
        private Rectangle rct;
        public ImageProxy(Bitmap bitmap , Rectangle rectangle)
        {
            this.img = bitmap;
            this.rct = rectangle;
        }

        public Pixel this[int x , int y]
        {
            get
            {
                if ( x <= rct.Width && x >= 0 && y <= rct.Height && y >= 0 )
                    return img[x + rct.X , y + rct.Y];
                return Pixels.Transparent;
            }
            set
            {
                if ( x <= rct.Width && x >= 0 && y <= rct.Height && y >= 0 )
                    img[x + rct.X , y + rct.Y] = value;
            }
        }

    }
}
