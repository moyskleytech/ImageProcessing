using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MoyskleyTech.ImageProcessing.Image;
using G = Android.Graphics;
namespace MoyskleyTech.ImageProcessing.Android
{
    public static class ColorConvertAndroid
    {
        public static Pixel ToPixel(this G.Color src)
        {
            return Pixel.FromArgb(src.ToArgb());
        }

        public static void RegisterIfNot()
        {
            var convert =ColorConvert.GetConversionFrom<G.Color , Pixel>();
            if ( convert == null )
            {
                ColorConvert.RegisterTransition<G.Color , Pixel>(ToPixel,1);
                ColorConvert.RegisterTransition<Pixel, G.Color>(ToAndroidColor,1);
                ColorConvert.CompleteTransitions();
            }
        }

        public static G.Color ToAndroidColor(this Pixel src)
        {
            return new G.Color(src.ToArgb());
        }
    }
}