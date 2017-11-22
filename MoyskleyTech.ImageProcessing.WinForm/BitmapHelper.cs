using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.WinForm
{
    public static class BitmapHelper
    {
        public static System.Drawing.Bitmap ToWinFormBitmap(this Bitmap bmp)
        {
            System.Drawing.Bitmap dest = new System.Drawing.Bitmap(bmp.Width,bmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            dest.CopyFrom(bmp);
            return dest;
        }
        public static Bitmap ToBitmap(this System.Drawing.Bitmap src)
        {
            Bitmap bmp= new Bitmap(src.Width,src.Height);
            bmp.CopyFrom(src);
            return bmp;
        }
        public static void CopyFrom(this System.Drawing.Bitmap dest , Bitmap bmp)
        {
            var loc = dest.LockBits(new System.Drawing.Rectangle(0 , 0 , bmp.Width , bmp.Height) , System.Drawing.Imaging.ImageLockMode.WriteOnly , System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            bmp.CopyToBGRA(loc.Scan0);
            dest.UnlockBits(loc);
        }
        public static void CopyFrom(this Bitmap dest,System.Drawing.Bitmap bmp)
        {
            var loc = bmp.LockBits(new System.Drawing.Rectangle(0 , 0 , bmp.Width , bmp.Height) , System.Drawing.Imaging.ImageLockMode.WriteOnly , System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            dest.CopyFromBGRA(loc.Scan0);
            bmp.UnlockBits(loc);
        }
    }
}
