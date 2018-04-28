using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public static class ColorConvert
    {
        #region HSB
        public static HSB ToHSB(Pixel src)
        {
            return src.ToHSB();
        }
        public static HSB ToHSB(this HSBA src)
        {
            return HSB.FromHSB(src.H,src.S,src.B);
        }
        public static HSB ToHSB(this CYMK src)
        {
            return src.ToRGB().ToHSB();
        }
        public static HSB ToHSB(this HSB_Float src)
        {
            return HSB.FromHSB(src.H,src.S,src.B);
        }
        public static HSB ToHSB(this HSL src)
        {
            var light = src.L;
            var sat = src.S;
            var hue = src.H;
            sat *= light < .5 ? light : 1 - light;
            
            return HSB.FromHSB(src.H , 2 * sat / ( light + sat ) , light+sat);
        }
        public static HSB ToHSB(this _565 src)
        {
            return src.ToRGB().ToHSB();
        }
        public static HSB ToHSB(this _555 src)
        {
            return src.ToRGB().ToHSB();
        }
        public static HSB ToHSB(this _1555 src)
        {
            return src.ToRGB().ToHSB();
        }
        #endregion
        #region HSBA
        public static HSBA ToHSBA(this Pixel src)
        {
            var tmp=src.ToHSB();
            return HSBA.FromHSB(tmp.H , tmp.S , tmp.B , src.A);
        }
        public static HSBA ToHSBA(this HSB src)
        {
            return HSBA.FromHSB(src.H , src.S , src.B , 255);
        }
        public static HSBA ToHSBA(this CYMK src)
        {
            return src.ToRGB().ToHSBA();
        }
        public static HSBA ToHSBA(this HSB_Float src)
        {
            return src.ToHSB().ToHSBA();
        }
        public static HSBA ToHSBA(this HSL src)
        {
            return src.ToHSB().ToHSBA();
        }
        public static HSBA ToHSBA(this _565 src)
        {
            return src.ToRGB().ToHSBA();
        }
        public static HSBA ToHSBA(this _555 src)
        {
            return src.ToRGB().ToHSBA();
        }
        public static HSBA ToHSBA(this _1555 src)
        {
            return src.ToRGB().ToHSBA();
        }
        #endregion
        #region CYMK
        public static CYMK ToCYMK(this Pixel src)
        {
            float R=src.R/255f,G=src.G/255f,B=src.B/255f;
            CYMK dst;
            dst.K = ( 1 - Math.Max(R , Math.Max(G , B)) );
            dst.C = ( 1 - R - dst.K ) / ( 1 - dst.K );
            dst.M = ( 1 - G - dst.K ) / ( 1 - dst.K );
            dst.Y = ( 1 - B - dst.K ) / ( 1 - dst.K );
            return dst;
        }
        public static CYMK ToCYMK(this HSB src)
        {
            return src.ToRGB().ToCYMK();
        }
        public static CYMK ToCYMK(this HSBA src)
        {
            return src.ToRGB().ToCYMK();
        }
        public static CYMK ToCYMK(this HSB_Float src)
        {
            return src.ToHSB().ToCYMK();
        }
        public static CYMK ToCYMK(this HSL src)
        {
            return src.ToHSB().ToCYMK();
        }
        public static CYMK ToCYMK(this _565 src)
        {
            return src.ToRGB().ToCYMK();
        }
        public static CYMK ToCYMK(this _555 src)
        {
            return src.ToRGB().ToCYMK();
        }
        public static CYMK ToCYMK(this _1555 src)
        {
            return src.ToRGB().ToCYMK();
        }
        #endregion
        #region Pixel
        public static Pixel ToRGB(this CYMK src)
        {
            var R = 255 * ( 1 - src.C ) * ( 1 - src.K );
            var G = 255 * ( 1 - src.M ) * ( 1 - src.K );
            var B = 255 * ( 1 - src.Y ) * ( 1 - src.K );

            return Pixel.FromArgb(255 , ( byte ) R , ( byte ) G , ( byte ) B);
        }
        public static Pixel ToRGB(HSB src)
        {
            return src.ToRGB();
        }
        public static Pixel ToRGB(this HSBA src)
        {
            return src.ToRGB();
        }
        public static Pixel ToRGB(this HSB_Float src)
        {
            return src.ToHSB().ToRGB();
        }
        public static Pixel ToRGB(this HSL src)
        {
            return src.ToHSB().ToRGB();
        }
        public static Pixel ToRGB(this _555 src)
        {
            return Pixel.FromRGB555(src._555_);
        }
        public static Pixel ToRGB(this _565 src)
        {
            return Pixel.FromRGB565(src._565_);
        }
        public static Pixel ToRGB(this _1555 src)
        {
            return Pixel.FromArgb(Pixel.FromRGB555(src._1555_), ( byte ) (src.A?255:0));
        }
        #endregion
        #region HSL
        public static HSL ToHSL(this CYMK src)
        {
            return src.ToHSB().ToHSL();
        }
        public static HSL ToHSL(this HSB src)
        {
            return src.ToHSB_Float().ToHSL();
        }
        public static HSL ToHSL(this HSBA src)
        {
            return src.ToHSB_Float().ToHSL();
        }
        public static HSL ToHSL(this HSB_Float src)
        {
            var hue = src.H;
            var sat = src.S;
            var val = src.B;
            HSL dst = new HSL();
            dst.H = hue;
            dst.S = sat * val / ( ( hue = ( 2 - sat ) * val ) < 1 ? hue : 2 - hue );
            dst.L = hue / 2;//Lightness is (2-sat)*val/2

            return dst;
        }
        public static HSL ToHSL(this Pixel src)
        {
            return src.ToHSB().ToHSL();
        }
        public static HSL ToHSL(this _565 src)
        {
            return src.ToRGB().ToHSL();
        }
        public static HSL ToHSL(this _555 src)
        {
            return src.ToRGB().ToHSL();
        }
        public static HSL ToHSL(this _1555 src)
        {
            return src.ToRGB().ToHSL();
        }
        #endregion
        #region HSB_Float
        public static HSB_Float ToHSB_Float(this CYMK src)
        {
            return src.ToHSB().ToHSB_Float();
        }
        public static HSB_Float ToHSB_Float(this HSB src)
        {
            return new HSB_Float {H= ( src.H * 360f / 255f ),S = src.S/255f,B=src.B/255f };
        }
        public static HSB_Float ToHSB_Float(this HSBA src)
        {
            return src.ToHSB().ToHSB_Float();
        }
        public static HSB_Float ToHSB_Float(this HSL src)
        {
            var light = src.L;
            var sat = src.S;
            var hue = src.H;
            sat *= light < .5 ? light : 1 - light;

            return new HSB_Float {H = src.H, S = 2 * sat / ( light + sat ), B = light + sat};
        }
        public static HSB_Float ToHSB_Float(this Pixel src)
        {
            return src.ToHSB().ToHSB_Float();
        }
        public static HSB_Float ToHSB_Float(this _565 src)
        {
            return src.ToRGB().ToHSB_Float();
        }
        public static HSB_Float ToHSB_Float(this _555 src)
        {
            return src.ToRGB().ToHSB_Float();
        }
        public static HSB_Float ToHSB_Float(this _1555 src)
        {
            return src.ToRGB().ToHSB_Float();
        }
        #endregion
        #region 565
        public static _565 To_565(this CYMK src)
        {
            return new _565() { _565_ = src.ToRGB().ToRGB565() };
        }
        public static _565 To_565(this HSB src)
        {
            return new _565() { _565_ = src.ToRGB().ToRGB565() };
        }
        public static _565 To_565(this HSBA src)
        {
            return new _565() { _565_ = src.ToRGB().ToRGB565() };
        }
        public static _565 To_565(this Pixel src)
        {
            return new _565() { _565_ = src.ToRGB565() };
        }
        public static _565 To_565(this HSB_Float src)
        {
            return new _565() { _565_ = src.ToRGB().ToRGB565() };
        }
        public static _565 To_565(this HSL src)
        {
            return new _565() { _565_ = src.ToRGB().ToRGB565() };
        }
        public static _565 To_565(this _555 src)
        {
            return new _565() { _565_ = src.ToRGB().ToRGB565() };
        }
        public static _565 To_565(this _1555 src)
        {
            return new _565() { _565_ = src.ToRGB().ToRGB565() };
        }
        #endregion
        #region 555
        public static _555 To_555(this CYMK src)
        {
            return new _555() { _555_ = src.ToRGB().ToRGB555() };
        }
        public static _555 To_555(this HSB src)
        {
            return new _555() { _555_ = src.ToRGB().ToRGB555() };
        }
        public static _555 To_555(this HSBA src)
        {
            return new _555() { _555_ = src.ToRGB().ToRGB555() };
        }
        public static _555 To_555(this Pixel src)
        {
            return new _555() { _555_ = src.ToRGB555() };
        }
        public static _555 To_555(this HSB_Float src)
        {
            return new _555() { _555_ = src.ToRGB().ToRGB555() };
        }
        public static _555 To_555(this HSL src)
        {
            return new _555() { _555_ = src.ToRGB().ToRGB555() };
        }
        public static _555 To_555(this _565 src)
        {
            return new _555() { _555_ = src.ToRGB().ToRGB555() };
        }
        public static _555 To_555(this _1555 src)
        {
            return new _555() { _555_ = src._1555_ };
        }
        #endregion
        #region 1555
        public static _1555 To_1555(this CYMK src)
        {
            return src.ToRGB().To_1555();
        }
        public static _1555 To_1555(this HSB src)
        {
            return src.ToRGB().To_1555();
        }
        public static _1555 To_1555(this HSBA src)
        {
            return src.ToRGB().To_1555();
        }
        public static _1555 To_1555(this Pixel src)
        {
            return new _1555() { _1555_ = src.ToRGB555(), A= src.A > 128 };
        }
        public static _1555 To_1555(this HSB_Float src)
        {
            return src.ToRGB().To_1555();
        }
        public static _1555 To_1555(this HSL src)
        {
            return src.ToRGB().To_1555();
        }
        public static _1555 To_1555(this _565 src)
        {
            return src.ToRGB().To_1555();
        }
        public static _1555 To_1555(this _1555 src)
        {
            return src.ToRGB().To_1555();
        }
        #endregion
    }
}
