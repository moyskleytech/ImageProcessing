using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Windows.Forms
{
    public static class BitmapHelper
    {
        public static System.Drawing.Bitmap ToWinFormBitmap(this ImageProxy<Pixel> bmp)
        {
            System.Drawing.Bitmap dest = new System.Drawing.Bitmap(bmp.Width,bmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var img=bmp.ToImage();
            dest.CopyFrom(img);
            img.Dispose();
            return dest;
        }
        public static System.Drawing.Bitmap ToWinFormBitmap(this Image<Pixel> bmp)
        {
            System.Drawing.Bitmap dest = new System.Drawing.Bitmap(bmp.Width,bmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            dest.CopyFrom(bmp);
            return dest;
        }

        public static System.Drawing.Bitmap ToWinFormBitmap(this ImageProxy<BGR> bmp)
        {
            System.Drawing.Bitmap dest = new System.Drawing.Bitmap(bmp.Width,bmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var img=bmp.ToImage<Pixel>();
            dest.CopyFrom(img);
            img.Dispose();
            return dest;
        }
        public static System.Drawing.Bitmap ToWinFormBitmap(this Image<BGR> bmp)
        {
            System.Drawing.Bitmap dest = new System.Drawing.Bitmap(bmp.Width,bmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var img = bmp.ConvertTo<Pixel>();
            dest.CopyFrom(img);
            img.Dispose();
            return dest;
        }

        public static Image<Pixel> ToBitmap(this System.Drawing.Bitmap src)
        {
            Image<Pixel> bmp= Image<Pixel>.Create(src.Width,src.Height);
            bmp.CopyFrom(src);
            return bmp;
        }
        public static Image<BGR> ToBGR(this System.Drawing.Bitmap src)
        {
            return src.ToBitmap().ConvertBufferTo<BGR>();
        }
        public static void CopyFrom(this System.Drawing.Bitmap dest , Image<Pixel> bmp)
        {
            var loc = dest.LockBits(new System.Drawing.Rectangle(0 , 0 , bmp.Width , bmp.Height) , System.Drawing.Imaging.ImageLockMode.WriteOnly , System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            bmp.CopyToBGRA(loc.Scan0);
            dest.UnlockBits(loc);
        }
        public static void CopyFrom(this Image<Pixel> dest ,System.Drawing.Bitmap bmp)
        {
            var loc = bmp.LockBits(new System.Drawing.Rectangle(0 , 0 , bmp.Width , bmp.Height) , System.Drawing.Imaging.ImageLockMode.WriteOnly , System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            dest.CopyFromBGRA(loc.Scan0);
            bmp.UnlockBits(loc);
        }
        public static void CopyFrom(this System.Drawing.Bitmap dest , Image<BGR> bmp)
        {
            var loc = dest.LockBits(new System.Drawing.Rectangle(0 , 0 , bmp.Width , bmp.Height) , System.Drawing.Imaging.ImageLockMode.WriteOnly , System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            bmp.CopyToBGRA(loc.Scan0);
            dest.UnlockBits(loc);
        }
        public static void CopyFrom(this Image<BGR> dest , System.Drawing.Bitmap bmp)
        {
            var loc = bmp.LockBits(new System.Drawing.Rectangle(0 , 0 , bmp.Width , bmp.Height) , System.Drawing.Imaging.ImageLockMode.WriteOnly , System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            dest.CopyFromBGRA(loc.Scan0);
            bmp.UnlockBits(loc);
        }
    }
}
