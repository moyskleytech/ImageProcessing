using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// All operation relative to convertion of colors
    /// </summary>
    public static partial class ColorConvert
    {
#pragma warning disable CS1591
        private class Transition
        {
            public Func<object , object> action;
            public double quality;
        }
        private static Dictionary<Type,Dictionary<Type,Transition>> transitions = new Dictionary<Type, Dictionary<Type, Transition>>();
        public static Func<T , V> GetConversionFrom<T, V>()
        {
            return (x)=> (V)GetConversionFrom(typeof(T) , typeof(V))(x);
        }
        public static Func<object , object> GetConversionFrom(Type t , Type v)
        {
            if ( t == v )
                return (x) => x;
            var from = transitions;
            if ( from.ContainsKey(t) )
            {
                var to = transitions[t];
                if ( to.ContainsKey(v) )
                {
                    return to[v].action;
                }
            }
            return null;
        }
        public static double GetConversionQualityFrom<T, V>()
        {
            return GetConversionQualityFrom(typeof(T) , typeof(V));
        }
        public static double GetConversionQualityFrom(Type t , Type v)
        {
            if ( t == v )
                return 1;
            var from = transitions;
            if ( from.ContainsKey(t) )
            {
                var to = transitions[t];
                if ( to.ContainsKey(v) )
                {
                    return to[v].quality;
                }
            }
            return 0;
        }
        public static void RegisterTransition<T, V>(Func<T , V> action , double quality)
        {
            Transition t = new Transition() {  action=(x)=>action((T)x), quality=quality};
            var from = transitions;
            if ( !from.ContainsKey(typeof(T)) )
                from[typeof(T)] = new Dictionary<Type , Transition>();
            var to = from[typeof(T)];
            to[typeof(V)] = t;
        }
        public static void CompleteTransitions()
        {
            //transitions.Keys should always contain everything since transitions should be both way
            var types = transitions.Keys.Union(transitions.SelectMany((x)=>x.Value.Keys)).ToList();
            var from = transitions;
            bool foundOne=true;
            foreach ( Type t in types )
            {
                if ( !from.ContainsKey(t) )
                    from[t] = new Dictionary<Type , Transition>();
                double[] weight = (from x in Enumerable.Range(0, types.Count) select (double)-1).ToArray();// new double[types.Count];
                Func<object,object>[] links = new Func<object, object>[types.Count];
                foreach ( var kv in from[t] )
                {
                    var idx=types.IndexOf(kv.Key);
                    weight[idx] = kv.Value.quality;
                    links[idx] = kv.Value.action;
                }
                foundOne = true;
                while ( foundOne )
                {
                    foundOne = false;
                    for ( var i = 0; i < types.Count; i++ )//for all non direct or loss
                    {
                        var dest = types[i];
                        if ( dest != t )
                        {
                            var possibleSources = from.Where((x) => x.Value.ContainsKey(dest)).OrderByDescending((x)=>x.Value[dest].quality);
                            foreach ( var link in possibleSources )
                            {
                                var idx = types.IndexOf(link.Key);
                                var w = weight[idx] * link.Value[dest].quality;
                                if ( w > weight[i] )
                                {
                                    weight[i] = w;
                                    links[i] = Merge(links[idx] , link.Value[dest].action);
                                    from[t][dest] = new Transition() { action = links[i] , quality = weight[i] };
                                    foundOne = true;
                                }
                            }
                        }
                    }
                }
            }

            //Ensure
            foreach (var t in types)
            {
                try
                {
                    var a = Pixels.DeepPink;
                    var b = GetConversionFrom(typeof(Pixel), t)(a);
                    var c = GetConversionFrom(t, typeof(Pixel))(b);
                }
                catch(Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Cannot convert Pixel to " + t.Name + " and back:"+e.Message+e.StackTrace);
                }
            }
        }
        private static Func<object , object> Merge<T, V, W>(Func<T , V> a , Func<V , W> b)
        {
            return (x) => b(a(( T ) x));
        }
        public static Brush<V> Convert<T, V>(Brush<T> b)
            where T: unmanaged
            where V: unmanaged
        {
            if ( b is SolidBrush<T> a )
                return a.As<V>();
            return new ConvertedBrush<T,V>() { Brush = b , Converter = GetConversionFrom<T , V>() };
        }
        public static Image<V> Convert<T, V>(Image<T> b)
           where T : unmanaged
           where V : unmanaged
        {
            return b.ConvertTo<V>();
        }
        public static V Convert<T, V>(T b)
        {
            return GetConversionFrom<T , V>()(b);
        }
        static ColorConvert()
        {
            transitions = new Dictionary<Type , Dictionary<Type , Transition>>();

            RegisterTransition<HSBA , HSB>(ToHSB , 0.95);
            RegisterTransition<HSB_Float , HSB>(ToHSB , 0.8);
            RegisterTransition<Pixel , HSB>(ToHSB , 0.75);
            RegisterTransition<HSL , HSB>(ToHSB , 0.8);

            RegisterTransition<Pixel , HSBA>(ToHSBA , 0.8);
            RegisterTransition<HSB , HSBA>(ToHSBA , 1);

            RegisterTransition<Pixel , CYMK>(ToCYMK , 1);
            RegisterTransition<ARGB_Float , CYMK>(ToCYMK , 1);
            RegisterTransition<ARGB_16bit , CYMK>(ToCYMK , 1);

            RegisterTransition<CYMK , Pixel>(ToPixel , 0.5);
            RegisterTransition<HSBA , Pixel>(ToPixel , 1);
            RegisterTransition<HSB , Pixel>(ToPixel , 1);
            RegisterTransition<_555 , Pixel>(ToPixel , 1);
            RegisterTransition<_565 , Pixel>(ToPixel , 1);
            RegisterTransition<_1555 , Pixel>(ToPixel , 1);
            RegisterTransition<RGB , Pixel>(ToPixel , 1);
            RegisterTransition<BGR , Pixel>(ToPixel , 1);
            RegisterTransition<_332 , Pixel>(ToPixel , 1);
            RegisterTransition<ARGB_16bit , Pixel>(ToPixel, 0.5);
            RegisterTransition<ARGB_Float , Pixel>(ToPixel, 0.5);
            RegisterTransition<RGBA , Pixel>(ToPixel , 1);
            RegisterTransition<BGRA , Pixel>(ToPixel , 1);
            RegisterTransition<ARGB , Pixel>(ToPixel , 1);
            RegisterTransition<ABGR , Pixel>(ToPixel , 1);

            RegisterTransition<HSB_Float , HSL>(ToHSL , 1);

            RegisterTransition<HSB , HSB_Float>(ToHSB_Float , 1);
            RegisterTransition<HSL , HSB_Float>(ToHSB_Float , 1);
            RegisterTransition<ARGB_16bit , HSB_Float>(ToHSB_Float , 1);
            RegisterTransition<ARGB_Float , HSB_Float>(ToHSB_Float , 1);

            RegisterTransition<Pixel , _565>(To_565 , 0.5);
            RegisterTransition<Pixel , _555>(To_555 , 0.4);
            RegisterTransition<_1555 , _555>(To_555 , 0.9);
            RegisterTransition<Pixel , _1555>(To_1555 , 0.47);
            RegisterTransition<_555 , _1555>(To_1555 , 1);

            RegisterTransition<byte , bool>(ToBool , 1/255d);
            RegisterTransition<bool , byte>(ToByte , 1);
            RegisterTransition<ushort , byte>(ToByte , 1/255d);
            RegisterTransition<byte , ushort>(ToUShort , 1);
            RegisterTransition<ARGB_16bit , ushort>(ToUShort , 0);
            RegisterTransition<uint , ushort>(ToUShort , 1/255d);
            RegisterTransition<ushort , uint>(ToUInt , 1);
            RegisterTransition<ulong , uint>(ToUInt , 1/32000d);
            RegisterTransition<uint , ulong>(ToULong , 1);
            RegisterTransition<ARGB_Float , float>(ToFloat , 0);
            RegisterTransition<Pixel , byte>(ToByte , 0);

            RegisterTransition<BGR , RGB>(ToRGB , 1);
            RegisterTransition<Pixel , RGB>(ToRGB , 0.75);

            RegisterTransition<RGB , BGR>(ToBGR , 1);
            RegisterTransition<Pixel , BGR>(ToBGR , 0.75);
            RegisterTransition<bool , BGR>(ToBGR , 1);
            RegisterTransition<byte , BGR>(ToBGR , 1);

            RegisterTransition<Pixel , RGBA>(ToRGBA , 1);

            RegisterTransition<Pixel , ARGB>(ToARGB , 1);

            RegisterTransition<Pixel , BGRA>(ToBGRA , 1);

            RegisterTransition<Pixel , ABGR>(ToABGR , 1);

            RegisterTransition<Pixel , _332>(To_332 , 0.1);

            RegisterTransition<Pixel , ARGB_16bit>(ToARGB_16bit , 1);
            RegisterTransition<ARGB_Float , ARGB_16bit>(ToARGB_16bit , 0.75);
            RegisterTransition<CYMK , ARGB_16bit>(ToARGB_16bit , 0.75);
            RegisterTransition<ushort , ARGB_16bit>(ToARGB_16bit , 1);

            RegisterTransition<Pixel , ARGB_Float>(ToARGB_Float , 1);
            RegisterTransition<float , ARGB_Float>(ToARGB_Float , 1);
            RegisterTransition<ARGB_16bit , ARGB_Float>(ToARGB_Float , 1);
            RegisterTransition<CYMK , ARGB_Float>(ToARGB_Float , 1);
            RegisterTransition<HSB_Float , ARGB_Float>(ToARGB_Float , 1);

            CompleteTransitions();
        }
        #region HSB
        public static HSB ToHSB(Pixel src)
        {
            return src.ToHSB();
        }
       
        public static HSB ToHSB(this HSBA src)
        {
            return HSB.FromHSB(src.H , src.S , src.B);
        }
        public static HSB ToHSB(this CYMK src)
        {
            return src.ToPixel().ToHSB();
        }
        public static HSB ToHSB(this HSB_Float src)
        {
            return HSB.FromHSB(src.H , src.S , src.B);
        }
        public static HSB ToHSB(this HSL src)
        {
            var light = src.L;
            var sat = src.S;
            var hue = src.H;
            sat *= light < .5 ? light : 1 - light;

            return HSB.FromHSB(src.H , 2 * sat / ( light + sat ) , light + sat);
        }
        public static HSB ToHSB(this _565 src)
        {
            return src.ToPixel().ToHSB();
        }
        public static HSB ToHSB(this _555 src)
        {
            return src.ToPixel().ToHSB();
        }
        public static HSB ToHSB(this _1555 src)
        {
            return src.ToPixel().ToHSB();
        }
        public static HSB ToHSB(this BGR src)
        {
            return src.ToPixel().ToHSB();
        }
        public static HSB ToHSB(this RGB src)
        {
            return src.ToPixel().ToHSB();
        }
        #endregion
        #region HSBA
        public static HSBA ToHSBA(this Pixel src)
        {
            var tmp=src.ToHSB();
            return HSBA.FromHSBA(tmp.H , tmp.S , tmp.B , src.A);
        }
        public static HSBA ToHSBA(this HSB src)
        {
            return HSBA.FromHSBA(src.H , src.S , src.B , 255);
        }
        public static HSBA ToHSBA(this CYMK src)
        {
            return src.ToPixel().ToHSBA();
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
            return src.ToPixel().ToHSBA();
        }
        public static HSBA ToHSBA(this _555 src)
        {
            return src.ToPixel().ToHSBA();
        }
        public static HSBA ToHSBA(this _1555 src)
        {
            return src.ToPixel().ToHSBA();
        }
        public static HSBA ToHSBA(this BGR src)
        {
            return src.ToPixel().ToHSBA();
        }
        public static HSBA ToHSBA(this RGB src)
        {
            return src.ToPixel().ToHSBA();
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
        public static CYMK ToCYMK(this ARGB_16bit src)
        {
            float R=src.R/(float)ushort.MaxValue,G=src.G/(float)ushort.MaxValue,B=src.B/(float)ushort.MaxValue;
            CYMK dst;
            dst.K = ( 1 - Math.Max(R , Math.Max(G , B)) );
            dst.C = ( 1 - R - dst.K ) / ( 1 - dst.K );
            dst.M = ( 1 - G - dst.K ) / ( 1 - dst.K );
            dst.Y = ( 1 - B - dst.K ) / ( 1 - dst.K );
            return dst;
        }
        public static CYMK ToCYMK(this ARGB_Float src)
        {
            float R=src.R,G=src.G,B=src.B;
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
            return src.ToPixel().ToCYMK();
        }
        public static CYMK ToCYMK(this _555 src)
        {
            return src.ToPixel().ToCYMK();
        }
        public static CYMK ToCYMK(this _1555 src)
        {
            return src.ToPixel().ToCYMK();
        }
        public static CYMK ToCYMK(this BGR src)
        {
            return src.ToPixel().ToCYMK();
        }
        public static CYMK ToCYMK(this RGB src)
        {
            return src.ToPixel().ToCYMK();
        }
        #endregion
        #region Pixel
        public static Pixel ToPixel(this CYMK src)
        {
            var R = 255 * ( 1 - src.C ) * ( 1 - src.K );
            var G = 255 * ( 1 - src.M ) * ( 1 - src.K );
            var B = 255 * ( 1 - src.Y ) * ( 1 - src.K );

            return Pixel.FromArgb(255 , ( byte ) R , ( byte ) G , ( byte ) B);
        }
        
        public static Pixel ToPixel(this HSB src)
        {
            return src.ToRGB();
        }
        public static Pixel ToPixel(this HSBA src)
        {
            return src.ToRGB();
        }
        public static Pixel ToPixel(this HSB_Float src)
        {
            return src.ToHSB().ToRGB();
        }
        public static Pixel ToPixel(this HSL src)
        {
            return src.ToHSB().ToRGB();
        }
        public static Pixel ToPixel(this _555 src)
        {
            return Pixel.FromRGB555(src._555_);
        }
        public static Pixel ToPixel(this _565 src)
        {
            return Pixel.FromRGB565(src._565_);
        }
        public static Pixel ToPixel(this _1555 src)
        {
            return Pixel.FromArgb(Pixel.FromRGB555(src._1555_) , ( byte ) ( src.A ? 255 : 0 ));
        }
        public static Pixel ToPixel(this BGR src)
        {
            return Pixel.FromArgb(255 , src.R , src.G , src.B);
        }
        public static Pixel ToPixel(this RGB src)
        {
            return Pixel.FromArgb(255 , src.R , src.G , src.B);
        }
        public static Pixel ToPixel(this RGBA src)
        {
            return Pixel.FromArgb(src.A , src.R , src.G , src.B);
        }
        public static Pixel ToPixel(this ARGB src)
        {
            return Pixel.FromArgb(src.A , src.R , src.G , src.B);
        }
        public static Pixel ToPixel(this BGRA src)
        {
            return Pixel.FromArgb(src.A , src.R , src.G , src.B);
        }
        public static Pixel ToPixel(this ABGR src)
        {
            return Pixel.FromArgb(src.A , src.R , src.G , src.B);
        }
        public static Pixel ToPixel(this _332 p)
        {
            return Pixel.FromArgb(255 , ( byte ) ( p.R << 5 ) , ( byte ) ( p.G << 5 ) , ( byte ) ( p.B << 6 ));
        }
        public static Pixel ToPixel(this ARGB_Float p)
        {
            return new Pixel() { A = ( byte ) ( p.A * byte.MaxValue ) , R = ( byte ) ( p.R * byte.MaxValue ) , G = ( byte ) ( p.G * byte.MaxValue ) , B = ( byte ) ( p.B * byte.MaxValue ) };
        }
        public static Pixel ToPixel(this ARGB_16bit p)
        {
            return new Pixel() { A = ( byte ) ( p.A >>8 ) , R = ( byte ) ( p.R >>8 ) , G = ( byte ) ( p.G >>8 ) , B = ( byte ) ( p.B >>8 ) };
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
            HSL dst = new HSL
            {
                H = hue ,
                S = sat * val / ( ( hue = ( 2 - sat ) * val ) < 1 ? hue : 2 - hue ) ,
                L = hue / 2//Lightness is (2-sat)*val/2
            };

            return dst;
        }
        public static HSL ToHSL(this Pixel src)
        {
            return src.ToHSB().ToHSL();
        }
        public static HSL ToHSL(this _565 src)
        {
            return src.ToPixel().ToHSL();
        }
        public static HSL ToHSL(this _555 src)
        {
            return src.ToPixel().ToHSL();
        }
        public static HSL ToHSL(this _1555 src)
        {
            return src.ToPixel().ToHSL();
        }
        public static HSL ToHSL(this BGR src)
        {
            return src.ToPixel().ToHSL();
        }
        public static HSL ToHSL(this RGB src)
        {
            return src.ToPixel().ToHSL();
        }
        #endregion
        #region HSB_Float
        public static HSB_Float ToHSB_Float(this ARGB_16bit src)
        {
            var R = src.R /(float)ushort.MaxValue;
            var G = src.G /(float)ushort.MaxValue;
            var B = src.B /(float)ushort.MaxValue;
            return new HSB_Float() { B = GetBrightness(R,G,B) , H = GetHue(R,G,B) , S = GetSaturation(R,G,B) };
        }
        public static HSB_Float ToHSB_Float(this ARGB_Float src)
        {
            return new HSB_Float() { B = GetBrightness(src.R , src.G , src.B) , H = GetHue(src.R , src.G , src.B) , S = GetSaturation(src.R , src.G , src.B) };
        }
        public static HSB_Float ToHSB_Float(this CYMK src)
        {
            return src.ToHSB().ToHSB_Float();
        }
        public static HSB_Float ToHSB_Float(this HSB src)
        {
            return new HSB_Float { H = ( src.H * 360f / 255f ) , S = src.S / 255f , B = src.B / 255f };
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

            return new HSB_Float { H = src.H , S = 2 * sat / ( light + sat ) , B = light + sat };
        }
        public static HSB_Float ToHSB_Float(this Pixel src)
        {
            return src.ToHSB().ToHSB_Float();
        }
        public static HSB_Float ToHSB_Float(this _565 src)
        {
            return src.ToPixel().ToHSB_Float();
        }
        public static HSB_Float ToHSB_Float(this _555 src)
        {
            return src.ToPixel().ToHSB_Float();
        }
        public static HSB_Float ToHSB_Float(this _1555 src)
        {
            return src.ToPixel().ToHSB_Float();
        }
        public static HSB_Float ToHSB_Float(this BGR src)
        {
            return src.ToPixel().ToHSB_Float();
        }
        public static HSB_Float ToHSB_Float(this RGB src)
        {
            return src.ToPixel().ToHSB_Float();
        }
        #endregion
        #region 565
        public static _565 To_565(this CYMK src)
        {
            return new _565() { _565_ = src.ToPixel().ToRGB565() };
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
            return new _565() { _565_ = src.ToPixel().ToRGB565() };
        }
        public static _565 To_565(this HSL src)
        {
            return new _565() { _565_ = src.ToPixel().ToRGB565() };
        }
        public static _565 To_565(this _555 src)
        {
            return new _565() { _565_ = src.ToPixel().ToRGB565() };
        }
        public static _565 To_565(this _1555 src)
        {
            return new _565() { _565_ = src.ToPixel().ToRGB565() };
        }
        public static _565 To_565(this BGR src)
        {
            return src.ToPixel().To_565();
        }
        public static _565 To_565(this RGB src)
        {
            return src.ToPixel().To_565();
        }
        #endregion
        #region 555
        public static _555 To_555(this CYMK src)
        {
            return new _555() { _555_ = src.ToPixel().ToRGB555() };
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
            return new _555() { _555_ = src.ToPixel().ToRGB555() };
        }
        public static _555 To_555(this HSL src)
        {
            return new _555() { _555_ = src.ToPixel().ToRGB555() };
        }
        public static _555 To_555(this _565 src)
        {
            return new _555() { _555_ = src.ToPixel().ToRGB555() };
        }
        public static _555 To_555(this _1555 src)
        {
            return new _555() { _555_ = src._1555_ };
        }
        public static _555 To_555(this BGR src)
        {
            return src.ToPixel().To_555();
        }
        public static _555 To_555(this RGB src)
        {
            return src.ToPixel().To_555();
        }
        #endregion
        #region 1555
        public static _1555 To_1555(this CYMK src)
        {
            return src.ToPixel().To_1555();
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
            return new _1555() { _1555_ = src.ToRGB555() , A = src.A > 128 };
        }
        public static _1555 To_1555(this HSB_Float src)
        {
            return src.ToPixel().To_1555();
        }
        public static _1555 To_1555(this HSL src)
        {
            return src.ToPixel().To_1555();
        }
        public static _1555 To_1555(this _565 src)
        {
            return src.ToPixel().To_1555();
        }
        public static _1555 To_1555(this _555 src)
        {
            return new _1555 { _1555_ = src._555_ , A = true };
        }
        public static _1555 To_1555(this BGR src)
        {
            return src.ToPixel().To_1555();
        }
        public static _1555 To_1555(this RGB src)
        {
            return src.ToPixel().To_1555();
        }
        #endregion
        #region BGR
        public static BGR ToBGR(this CYMK src)
        {
            return src.ToPixel().ToBGR();
        }
        public static BGR ToBGR(this HSB src)
        {
            return src.ToPixel().ToBGR();
        }
        public static BGR ToBGR(this HSBA src)
        {
            return src.ToPixel().ToBGR();
        }
        public static BGR ToBGR(this Pixel src)
        {
            return new BGR() { R = src.R , G = src.G , B = src.B };
        }
        public static BGR ToBGR(this HSB_Float src)
        {
            return src.ToPixel().ToBGR();
        }
        public static BGR ToBGR(this HSL src)
        {
            return src.ToPixel().ToBGR();
        }
        public static BGR ToBGR(this _565 src)
        {
            return src.ToPixel().ToBGR();
        }
        public static BGR ToBGR(this _555 src)
        {
            return src.ToPixel().ToBGR();
        }
        public static BGR ToBGR(this _1555 src)
        {
            return src.ToPixel().ToBGR();
        }
        public static BGR ToBGR(this RGB src)
        {
            return src.ToPixel().ToBGR();
        }
        public static BGR ToBGR(this bool src)
        {
            var v=(byte)(src?255:0);
            return new BGR() { R = v , G = v , B = v };
        }
        public static BGR ToBGR(this byte src)
        {
            var v=(byte)src;
            return new BGR() { R = v , G = v , B = v };
        }
        #endregion
        #region RGB
        public static RGB ToRGB(this CYMK src)
        {
            return src.ToPixel().ToRGB();
        }
        public static RGB ToRGB(this HSB src)
        {
            return src.ToPixel().ToRGB();
        }
        public static RGB ToRGB(this HSBA src)
        {
            return src.ToPixel().ToRGB();
        }
        public static RGB ToRGB(this Pixel src)
        {
            return new RGB() { R = src.R , G = src.G , B = src.B };
        }
        public static RGB ToRGB(this HSB_Float src)
        {
            return src.ToPixel().ToRGB();
        }
        public static RGB ToRGB(this HSL src)
        {
            return src.ToPixel().ToRGB();
        }
        public static RGB ToRGB(this _565 src)
        {
            return src.ToPixel().ToRGB();
        }
        public static RGB ToRGB(this _555 src)
        {
            return src.ToPixel().ToRGB();
        }
        public static RGB ToRGB(this _1555 src)
        {
            return src.ToPixel().ToRGB();
        }
        public static RGB ToRGB(this BGR src)
        {
            return src.ToPixel().ToRGB();
        }
        #endregion
        #region 332
        public static _332 To_332(this Pixel p)
        {
            return new _332 { R = ( byte ) ( p.R >> 5 ) , G = ( byte ) ( p.G >> 5 ) , B = ( byte ) ( p.B >> 6 ) };
        }
        #endregion
        #region ARGB_16bit
        public static ARGB_16bit ToARGB_16bit(this CYMK src)
        {
            var R = ushort.MaxValue * ( 1 - src.C ) * ( 1 - src.K );
            var G = ushort.MaxValue * ( 1 - src.M ) * ( 1 - src.K );
            var B = ushort.MaxValue * ( 1 - src.Y ) * ( 1 - src.K );

            return new ARGB_16bit { A = 255 , R = ( ushort ) R , G = ( ushort ) G , B = ( ushort ) B };
        }
        public static ARGB_16bit ToARGB_16bit(this Pixel p)
        {
            return new ARGB_16bit() { A = ( ushort ) ( p.A/255f * ushort.MaxValue ) , R = ( ushort ) ( p.R / 255f * ushort.MaxValue ) , G = ( ushort ) ( p.G / 255f * ushort.MaxValue ) , B = ( ushort ) ( p.B / 255f * ushort.MaxValue ) };
        }
        public static ARGB_16bit ToARGB_16bit(this ARGB_Float p)
        {
            return new ARGB_16bit() { A = ( ushort ) ( p.A*ushort.MaxValue ) , R = ( ushort ) ( p.R * ushort.MaxValue ) , G = ( ushort ) ( p.G * ushort.MaxValue ) , B = ( ushort ) ( p.B * ushort.MaxValue ) };
        }
        public static ARGB_16bit ToARGB_16bit(this ushort src)
        {
            return new ARGB_16bit { A = 255 , R = ( ushort ) src , G = ( ushort ) src , B = ( ushort ) src };
        }
        #endregion
        #region ARGB_Float
        public static ARGB_Float ToARGB_Float(this HSB_Float src)
        {
            float hue=src.H , saturation=src.S , brightness=src.B;

            float r = 0, g = 0, b = 0;
            if ( saturation == 0 )
            {
                r = g = b = ( brightness );
            }
            else
            {
                var q = brightness < 0.5 ? brightness * (1 + saturation) : brightness + saturation - brightness * saturation;
                var p = 2 * brightness - q;
                r = ( float ) ( HueToRgb(p , q , hue + ( 1d / 3 )) );
                g = ( float ) ( HueToRgb(p , q , hue) );
                b = ( float ) ( HueToRgb(p , q , hue - ( 1d / 3 )) );
            }
            return new ARGB_Float() { A = 1 , R = r , G = g , B = b };
        }
        public static ARGB_Float ToARGB_Float(this CYMK src)
        {
            var R = ( 1 - src.C ) * ( 1 - src.K );
            var G = ( 1 - src.M ) * ( 1 - src.K );
            var B = ( 1 - src.Y ) * ( 1 - src.K );

            return new ARGB_Float { A = 1 , R = ( float ) R , G = ( float ) G , B = ( float ) B };
        }
        public static ARGB_Float ToARGB_Float(this Pixel p)
        {
            return new ARGB_Float() { A = p.A/255f , R = p.R / 255f , G = p.G / 255f , B = p.B / 255f };
        }
        public static ARGB_Float ToARGB_Float(this ARGB_16bit p)
        {
            return new ARGB_Float() { A = p.A / (float)ushort.MaxValue , R = p.R / ( float ) ushort.MaxValue , G = p.G / ( float ) ushort.MaxValue , B = p.B / ( float ) ushort.MaxValue };
        }
        public static ARGB_Float ToARGB_Float(this float src)
        {
            return new ARGB_Float { A = 255 , R = src , G = src , B = src };
        }
        #endregion
        #region helper
        public static ARGB ToARGB(this Pixel src)
        {
            return new ARGB() {A = src.A, R = src.R , G = src.G , B = src.B };
        }
        public static RGBA ToRGBA(this Pixel src)
        {
            return new RGBA() { A = src.A , R = src.R , G = src.G , B = src.B };
        }
        public static ABGR ToABGR(this Pixel src)
        {
            return new ABGR() { A = src.A , R = src.R , G = src.G , B = src.B };
        }
        public static BGRA ToBGRA(this Pixel src)
        {
            return new BGRA() { A = src.A , R = src.R , G = src.G , B = src.B };
        }
        public static float ToFloat(this ARGB_Float src)
        {
            return src.GetGrayTone();
        }
        public static bool ToBool(this byte src)
        {
            return src>128;
        }
        public static byte ToByte(this bool src)
        {
           return (byte)(src?255:0);
        }
        public static byte ToByte(this ushort src)
        {
            return ( byte ) ( src>>8 );
        }
        public static byte ToByte(this Pixel src)
        {
            return src.GetGrayTone() ;
        }
        public static ushort ToUShort(this byte src)
        {
            return (ushort)(src<<8);
        }
        public static ushort ToUShort(this uint src)
        {
            return ( ushort ) ( src >>16 );
        }
        public static ushort ToUShort(this ARGB_16bit src)
        {
            return src.GetGrayTone();
        }
        public static uint ToUInt(this ushort src)
        {
            return ( uint ) ( src << 16 );
        }
        public static uint ToUInt(this ulong src)
        {
            return ( uint ) ( src >>32 );
        }
        public static ulong ToULong(this uint src)
        {
            return ( ulong ) ( src << 32 );
        }
        private static double HueToRgb(double p , double q , double t)
        {
            if ( t < 0 )
                t += 1;
            if ( t > 1 )
                t -= 1;
            if ( t < ( 1d / 6 ) )
                return ( p + ( q - p ) * 6 * t );
            if ( t < ( 1d / 2 ) )
                return ( q );
            if ( t < ( 2d / 3 ) )
                return ( p + ( q - p ) * ( 2d / 3 - t ) * 6 );
            return ( p );
        }
        /// <summary>Gets the hue-saturation-brightness (HSB) brightness value for this <see cref="T:System.Drawing.Color" /> structure.</summary>
        /// <returns>The brightness of this <see cref="T:System.Drawing.Color" />. The brightness ranges from 0.0 through 1.0, where 0.0 represents black and 1.0 represents white.</returns>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public static float GetBrightness(float R , float G , float B)
        {
            float r = R;
            float g = G;
            float b = B;
            float single = r;
            float single1 = r;
            if ( g > single )
            {
                single = g;
            }
            if ( b > single )
            {
                single = b;
            }
            if ( g < single1 )
            {
                single1 = g;
            }
            if ( b < single1 )
            {
                single1 = b;
            }
            return ( single + single1 ) / 2f;
        }

        /// <summary>Gets the hue-saturation-brightness (HSB) hue value, in degrees, for this <see cref="T:System.Drawing.Color" /> structure.</summary>
        /// <returns>The hue, in degrees, of this <see cref="T:System.Drawing.Color" />. The hue is measured in degrees, ranging from 0.0 through 360.0, in HSB color space.</returns>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public static float GetHue(float R , float G , float B)
        {
            if ( R == G && G == B )
                return 0f;
            float r = R;
            float g = G;
            float b = B;
            float single = 0f;
            float single1 = r;
            float single2 = r;
            if ( g > single1 )
            {
                single1 = g;
            }
            if ( b > single1 )
            {
                single1 = b;
            }
            if ( g < single2 )
            {
                single2 = g;
            }
            if ( b < single2 )
            {
                single2 = b;
            }
            float single3 = single1 - single2;
            if ( r == single1 )
            {
                single = ( g - b ) / single3;
            }
            else if ( g == single1 )
            {
                single = 2f + ( b - r ) / single3;
            }
            else if ( b == single1 )
            {
                single = 4f + ( r - g ) / single3;
            }
            single = single * 60f;
            if ( single < 0f )
            {
                single = single + 360f;
            }
            return single;
        }

        /// <summary>Gets the hue-saturation-brightness (HSB) saturation value for this <see cref="T:System.Drawing.Color" /> structure.</summary>
        /// <returns>The saturation of this <see cref="T:System.Drawing.Color" />. The saturation ranges from 0.0 through 1.0, where 0.0 is grayscale and 1.0 is the most saturated.</returns>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        private static float GetSaturation(float R,float G, float B)
        {
            float r = R;
            float g = G;
            float b = B;
            float single = 0f;
            float single1 = r;
            float single2 = r;
            if ( g > single1 )
            {
                single1 = g;
            }
            if ( b > single1 )
            {
                single1 = b;
            }
            if ( g < single2 )
            {
                single2 = g;
            }
            if ( b < single2 )
            {
                single2 = b;
            }
            if ( single1 != single2 )
            {
                single = ( ( double ) ( ( single1 + single2 ) / 2f ) > 0.5 ? ( single1 - single2 ) / ( 2f - single1 - single2 ) : ( single1 - single2 ) / ( single1 + single2 ) );
            }
            return single;
        }
        #endregion
    }
#pragma warning restore CS1591
}
