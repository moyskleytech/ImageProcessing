﻿//using MoyskleyTech.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Constants for Pixels
    /// </summary>
    public static class Pixels
    {
#pragma warning disable CS1591
        public static readonly Pixel Transparent= Pixel.FromArgb(0,255,255,255);
        public static readonly Pixel AliceBlue= Pixel.FromArgb(255,240,248,255);
        public static readonly Pixel AntiqueWhite= Pixel.FromArgb(255,250,235,215);
        public static readonly Pixel Aqua= Pixel.FromArgb(255,0,255,255);
        public static readonly Pixel Aquamarine= Pixel.FromArgb(255,127,255,212);
        public static readonly Pixel Azure= Pixel.FromArgb(255,240,255,255);
        public static readonly Pixel Beige= Pixel.FromArgb(255,245,245,220);
        public static readonly Pixel Bisque= Pixel.FromArgb(255,255,228,196);
        public static readonly Pixel Black= Pixel.FromArgb(255,0,0,0);
        public static readonly Pixel BlanchedAlmond= Pixel.FromArgb(255,255,235,205);
        public static readonly Pixel Blue= Pixel.FromArgb(255,0,0,255);
        public static readonly Pixel BlueViolet= Pixel.FromArgb(255,138,43,226);
        public static readonly Pixel Brown= Pixel.FromArgb(255,165,42,42);
        public static readonly Pixel BurlyWood= Pixel.FromArgb(255,222,184,135);
        public static readonly Pixel CadetBlue= Pixel.FromArgb(255,95,158,160);
        public static readonly Pixel Chartreuse= Pixel.FromArgb(255,127,255,0);
        public static readonly Pixel Chocolate= Pixel.FromArgb(255,210,105,30);
        public static readonly Pixel Coral= Pixel.FromArgb(255,255,127,80);
        public static readonly Pixel CornflowerBlue= Pixel.FromArgb(255,100,149,237);
        public static readonly Pixel Cornsilk= Pixel.FromArgb(255,255,248,220);
        public static readonly Pixel Crimson= Pixel.FromArgb(255,220,20,60);
        public static readonly Pixel Cyan= Pixel.FromArgb(255,0,255,255);
        public static readonly Pixel DarkBlue= Pixel.FromArgb(255,0,0,139);
        public static readonly Pixel DarkCyan= Pixel.FromArgb(255,0,139,139);
        public static readonly Pixel DarkGoldenrod= Pixel.FromArgb(255,184,134,11);
        public static readonly Pixel DarkGray= Pixel.FromArgb(255,169,169,169);
        public static readonly Pixel DarkGreen= Pixel.FromArgb(255,0,100,0);
        public static readonly Pixel DarkKhaki= Pixel.FromArgb(255,189,183,107);
        public static readonly Pixel DarkMagenta= Pixel.FromArgb(255,139,0,139);
        public static readonly Pixel DarkOliveGreen= Pixel.FromArgb(255,85,107,47);
        public static readonly Pixel DarkOrange= Pixel.FromArgb(255,255,140,0);
        public static readonly Pixel DarkOrchid= Pixel.FromArgb(255,153,50,204);
        public static readonly Pixel DarkRed= Pixel.FromArgb(255,139,0,0);
        public static readonly Pixel DarkSalmon= Pixel.FromArgb(255,233,150,122);
        public static readonly Pixel DarkSeaGreen= Pixel.FromArgb(255,143,188,139);
        public static readonly Pixel DarkSlateBlue= Pixel.FromArgb(255,72,61,139);
        public static readonly Pixel DarkSlateGray= Pixel.FromArgb(255,47,79,79);
        public static readonly Pixel DarkTurquoise= Pixel.FromArgb(255,0,206,209);
        public static readonly Pixel DarkViolet= Pixel.FromArgb(255,148,0,211);
        public static readonly Pixel DeepPink= Pixel.FromArgb(255,255,20,147);
        public static readonly Pixel DeepSkyBlue= Pixel.FromArgb(255,0,191,255);
        public static readonly Pixel DimGray= Pixel.FromArgb(255,105,105,105);
        public static readonly Pixel DodgerBlue= Pixel.FromArgb(255,30,144,255);
        public static readonly Pixel Firebrick= Pixel.FromArgb(255,178,34,34);
        public static readonly Pixel FloralWhite= Pixel.FromArgb(255,255,250,240);
        public static readonly Pixel ForestGreen= Pixel.FromArgb(255,34,139,34);
        public static readonly Pixel Fuchsia= Pixel.FromArgb(255,255,0,255);
        public static readonly Pixel Gainsboro= Pixel.FromArgb(255,220,220,220);
        public static readonly Pixel GhostWhite= Pixel.FromArgb(255,248,248,255);
        public static readonly Pixel Gold= Pixel.FromArgb(255,255,215,0);
        public static readonly Pixel Goldenrod= Pixel.FromArgb(255,218,165,32);
        public static readonly Pixel Gray= Pixel.FromArgb(255,128,128,128);
        public static readonly Pixel Green= Pixel.FromArgb(255,0,128,0);
        public static readonly Pixel GreenYellow= Pixel.FromArgb(255,173,255,47);
        public static readonly Pixel Honeydew= Pixel.FromArgb(255,240,255,240);
        public static readonly Pixel HotPink= Pixel.FromArgb(255,255,105,180);
        public static readonly Pixel IndianRed= Pixel.FromArgb(255,205,92,92);
        public static readonly Pixel Indigo= Pixel.FromArgb(255,75,0,130);
        public static readonly Pixel Ivory= Pixel.FromArgb(255,255,255,240);
        public static readonly Pixel Khaki= Pixel.FromArgb(255,240,230,140);
        public static readonly Pixel Lavender= Pixel.FromArgb(255,230,230,250);
        public static readonly Pixel LavenderBlush= Pixel.FromArgb(255,255,240,245);
        public static readonly Pixel LawnGreen= Pixel.FromArgb(255,124,252,0);
        public static readonly Pixel LemonChiffon= Pixel.FromArgb(255,255,250,205);
        public static readonly Pixel LightBlue= Pixel.FromArgb(255,173,216,230);
        public static readonly Pixel LightCoral= Pixel.FromArgb(255,240,128,128);
        public static readonly Pixel LightCyan= Pixel.FromArgb(255,224,255,255);
        public static readonly Pixel LightGoldenrodYellow= Pixel.FromArgb(255,250,250,210);
        public static readonly Pixel LightGreen= Pixel.FromArgb(255,144,238,144);
        public static readonly Pixel LightGray= Pixel.FromArgb(255,211,211,211);
        public static readonly Pixel LightPink= Pixel.FromArgb(255,255,182,193);
        public static readonly Pixel LightSalmon= Pixel.FromArgb(255,255,160,122);
        public static readonly Pixel LightSeaGreen= Pixel.FromArgb(255,32,178,170);
        public static readonly Pixel LightSkyBlue= Pixel.FromArgb(255,135,206,250);
        public static readonly Pixel LightSlateGray= Pixel.FromArgb(255,119,136,153);
        public static readonly Pixel LightSteelBlue= Pixel.FromArgb(255,176,196,222);
        public static readonly Pixel LightYellow= Pixel.FromArgb(255,255,255,224);
        public static readonly Pixel Lime= Pixel.FromArgb(255,0,255,0);
        public static readonly Pixel LimeGreen= Pixel.FromArgb(255,50,205,50);
        public static readonly Pixel Linen= Pixel.FromArgb(255,250,240,230);
        public static readonly Pixel Magenta= Pixel.FromArgb(255,255,0,255);
        public static readonly Pixel Maroon= Pixel.FromArgb(255,128,0,0);
        public static readonly Pixel MediumAquamarine= Pixel.FromArgb(255,102,205,170);
        public static readonly Pixel MediumBlue= Pixel.FromArgb(255,0,0,205);
        public static readonly Pixel MediumOrchid= Pixel.FromArgb(255,186,85,211);
        public static readonly Pixel MediumPurple= Pixel.FromArgb(255,147,112,219);
        public static readonly Pixel MediumSeaGreen= Pixel.FromArgb(255,60,179,113);
        public static readonly Pixel MediumSlateBlue= Pixel.FromArgb(255,123,104,238);
        public static readonly Pixel MediumSpringGreen= Pixel.FromArgb(255,0,250,154);
        public static readonly Pixel MediumTurquoise= Pixel.FromArgb(255,72,209,204);
        public static readonly Pixel MediumVioletRed= Pixel.FromArgb(255,199,21,133);
        public static readonly Pixel MidnightBlue= Pixel.FromArgb(255,25,25,112);
        public static readonly Pixel MintCream= Pixel.FromArgb(255,245,255,250);
        public static readonly Pixel MistyRose= Pixel.FromArgb(255,255,228,225);
        public static readonly Pixel Moccasin= Pixel.FromArgb(255,255,228,181);
        public static readonly Pixel NavajoWhite= Pixel.FromArgb(255,255,222,173);
        public static readonly Pixel Navy= Pixel.FromArgb(255,0,0,128);
        public static readonly Pixel OldLace= Pixel.FromArgb(255,253,245,230);
        public static readonly Pixel Olive= Pixel.FromArgb(255,128,128,0);
        public static readonly Pixel OliveDrab= Pixel.FromArgb(255,107,142,35);
        public static readonly Pixel Orange= Pixel.FromArgb(255,255,165,0);
        public static readonly Pixel OrangeRed= Pixel.FromArgb(255,255,69,0);
        public static readonly Pixel Orchid= Pixel.FromArgb(255,218,112,214);
        public static readonly Pixel PaleGoldenrod= Pixel.FromArgb(255,238,232,170);
        public static readonly Pixel PaleGreen= Pixel.FromArgb(255,152,251,152);
        public static readonly Pixel PaleTurquoise= Pixel.FromArgb(255,175,238,238);
        public static readonly Pixel PaleVioletRed= Pixel.FromArgb(255,219,112,147);
        public static readonly Pixel PapayaWhip= Pixel.FromArgb(255,255,239,213);
        public static readonly Pixel PeachPuff= Pixel.FromArgb(255,255,218,185);
        public static readonly Pixel Peru= Pixel.FromArgb(255,205,133,63);
        public static readonly Pixel Pink= Pixel.FromArgb(255,255,192,203);
        public static readonly Pixel Plum= Pixel.FromArgb(255,221,160,221);
        public static readonly Pixel PowderBlue= Pixel.FromArgb(255,176,224,230);
        public static readonly Pixel Purple= Pixel.FromArgb(255,128,0,128);
        public static readonly Pixel Red= Pixel.FromArgb(255,255,0,0);
        public static readonly Pixel RosyBrown= Pixel.FromArgb(255,188,143,143);
        public static readonly Pixel RoyalBlue= Pixel.FromArgb(255,65,105,225);
        public static readonly Pixel SaddleBrown= Pixel.FromArgb(255,139,69,19);
        public static readonly Pixel Salmon= Pixel.FromArgb(255,250,128,114);
        public static readonly Pixel SandyBrown= Pixel.FromArgb(255,244,164,96);
        public static readonly Pixel SeaGreen= Pixel.FromArgb(255,46,139,87);
        public static readonly Pixel SeaShell= Pixel.FromArgb(255,255,245,238);
        public static readonly Pixel Sienna= Pixel.FromArgb(255,160,82,45);
        public static readonly Pixel Silver= Pixel.FromArgb(255,192,192,192);
        public static readonly Pixel SkyBlue= Pixel.FromArgb(255,135,206,235);
        public static readonly Pixel SlateBlue= Pixel.FromArgb(255,106,90,205);
        public static readonly Pixel SlateGray= Pixel.FromArgb(255,112,128,144);
        public static readonly Pixel Snow= Pixel.FromArgb(255,255,250,250);
        public static readonly Pixel SpringGreen= Pixel.FromArgb(255,0,255,127);
        public static readonly Pixel SteelBlue= Pixel.FromArgb(255,70,130,180);
        public static readonly Pixel Tan= Pixel.FromArgb(255,210,180,140);
        public static readonly Pixel Teal= Pixel.FromArgb(255,0,128,128);
        public static readonly Pixel Thistle= Pixel.FromArgb(255,216,191,216);
        public static readonly Pixel Tomato= Pixel.FromArgb(255,255,99,71);
        public static readonly Pixel Turquoise= Pixel.FromArgb(255,64,224,208);
        public static readonly Pixel Violet= Pixel.FromArgb(255,238,130,238);
        public static readonly Pixel Wheat= Pixel.FromArgb(255,245,222,179);
        public static readonly Pixel White= Pixel.FromArgb(255,255,255,255);
        public static readonly Pixel WhiteSmoke= Pixel.FromArgb(255,245,245,245);
        public static readonly Pixel Yellow= Pixel.FromArgb(255,255,255,0);
        public static readonly Pixel YellowGreen= Pixel.FromArgb(255,154,205,50);

        public static readonly Pixel[] NamedPixels = new Pixel[]{
       // public static readonly RArray<Pixel> NamedPixels = new RArray<Pixel>(new Pixel[ ]{
            Transparent,
            AliceBlue,
            AntiqueWhite,
            Aqua,
            Aquamarine,
            Azure,
            Beige,
            Bisque,
            Black,
            BlanchedAlmond,
            Blue,
            BlueViolet,
            Brown,
            BurlyWood,
            CadetBlue,
            Chartreuse,
            Chocolate,
            Coral,
            CornflowerBlue,
            Cornsilk,
            Crimson,
            Cyan,
            DarkBlue,
            DarkCyan,
            DarkGoldenrod,
            DarkGray,
            DarkGreen,
            DarkKhaki,
            DarkMagenta,
            DarkOliveGreen,
            DarkOrange,
            DarkOrchid,
            DarkRed,
            DarkSalmon,
            DarkSeaGreen,
            DarkSlateBlue,
            DarkSlateGray,
            DarkTurquoise,
            DarkViolet,
            DeepPink,
            DeepSkyBlue,
            DimGray,
            DodgerBlue,
            Firebrick,
            FloralWhite,
            ForestGreen,
            Fuchsia,
            Gainsboro,
            GhostWhite,
            Gold,
            Goldenrod,
            Gray,
            Green,
            GreenYellow,
            Honeydew,
            HotPink,
            IndianRed,
            Indigo,
            Ivory,
            Khaki,
            Lavender,
            LavenderBlush,
            LawnGreen,
            LemonChiffon,
            LightBlue,
            LightCoral,
            LightCyan,
            LightGoldenrodYellow,
            LightGreen,
            LightGray,
            LightPink,
            LightSalmon,
            LightSeaGreen,
            LightSkyBlue,
            LightSlateGray,
            LightSteelBlue,
            LightYellow,
            Lime,
            LimeGreen,
            Linen,
            Magenta,
            Maroon,
            MediumAquamarine,
            MediumBlue,
            MediumOrchid,
            MediumPurple,
            MediumSeaGreen,
            MediumSlateBlue,
            MediumSpringGreen,
            MediumTurquoise,
            MediumVioletRed,
            MidnightBlue,
            MintCream,
            MistyRose,
            Moccasin,
            NavajoWhite,
            Navy,
            OldLace,
            Olive,
            OliveDrab,
            Orange,
            OrangeRed,
            Orchid,
            PaleGoldenrod,
            PaleGreen,
            PaleTurquoise,
            PaleVioletRed,
            PapayaWhip,
            PeachPuff,
            Peru,
            Pink,
            Plum,
            PowderBlue,
            Purple,
            Red,
            RosyBrown,
            RoyalBlue,
            SaddleBrown,
            Salmon,
            SandyBrown,
            SeaGreen,
            SeaShell,
            Sienna,
            Silver,
            SkyBlue,
            SlateBlue,
            SlateGray,
            Snow,
            SpringGreen,
            SteelBlue,
            Tan,
            Teal,
            Thistle,
            Tomato,
            Turquoise,
            Violet,
            Wheat,
            White,
            WhiteSmoke,
            Yellow,
            YellowGreen
        };//);
        public static readonly string[] PixelNames = new string[]{
    //    public static readonly RArray<string> PixelNames = new RArray<string>(new string[ ]{
            "Transparent",
"AliceBlue",
"AntiqueWhite",
"Aqua",
"Aquamarine",
"Azure",
"Beige",
"Bisque",
"Black",
"BlanchedAlmond",
"Blue",
"BlueViolet",
"Brown",
"BurlyWood",
"CadetBlue",
"Chartreuse",
"Chocolate",
"Coral",
"CornflowerBlue",
"Cornsilk",
"Crimson",
"Cyan",
"DarkBlue",
"DarkCyan",
"DarkGoldenrod",
"DarkGray",
"DarkGreen",
"DarkKhaki",
"DarkMagenta",
"DarkOliveGreen",
"DarkOrange",
"DarkOrchid",
"DarkRed",
"DarkSalmon",
"DarkSeaGreen",
"DarkSlateBlue",
"DarkSlateGray",
"DarkTurquoise",
"DarkViolet",
"DeepPink",
"DeepSkyBlue",
"DimGray",
"DodgerBlue",
"Firebrick",
"FloralWhite",
"ForestGreen",
"Fuchsia",
"Gainsboro",
"GhostWhite",
"Gold",
"Goldenrod",
"Gray",
"Green",
"GreenYellow",
"Honeydew",
"HotPink",
"IndianRed",
"Indigo",
"Ivory",
"Khaki",
"Lavender",
"LavenderBlush",
"LawnGreen",
"LemonChiffon",
"LightBlue",
"LightCoral",
"LightCyan",
"LightGoldenrodYellow",
"LightGreen",
"LightGray",
"LightPink",
"LightSalmon",
"LightSeaGreen",
"LightSkyBlue",
"LightSlateGray",
"LightSteelBlue",
"LightYellow",
"Lime",
"LimeGreen",
"Linen",
"Magenta",
"Maroon",
"MediumAquamarine",
"MediumBlue",
"MediumOrchid",
"MediumPurple",
"MediumSeaGreen",
"MediumSlateBlue",
"MediumSpringGreen",
"MediumTurquoise",
"MediumVioletRed",
"MidnightBlue",
"MintCream",
"MistyRose",
"Moccasin",
"NavajoWhite",
"Navy",
"OldLace",
"Olive",
"OliveDrab",
"Orange",
"OrangeRed",
"Orchid",
"PaleGoldenrod",
"PaleGreen",
"PaleTurquoise",
"PaleVioletRed",
"PapayaWhip",
"PeachPuff",
"Peru",
"Pink",
"Plum",
"PowderBlue",
"Purple",
"Red",
"RosyBrown",
"RoyalBlue",
"SaddleBrown",
"Salmon",
"SandyBrown",
"SeaGreen",
"SeaShell",
"Sienna",
"Silver",
"SkyBlue",
"SlateBlue",
"SlateGray",
"Snow",
"SpringGreen",
"SteelBlue",
"Tan",
"Teal",
"Thistle",
"Tomato",
"Turquoise",
"Violet",
"Wheat",
"White",
"WhiteSmoke",
"Yellow",
"YellowGreen"
        };//);
        public static Pixel GetPixel(string name)
        {
           var idx= PixelNames.ToList().FindIndex((m)=>string.Equals(m,name,StringComparison.OrdinalIgnoreCase));
            if ( idx < 0 )
                return Pixels.Transparent;
            else
                return NamedPixels[idx];
        }
        public static IEnumerable<string> GetKnownPixelNames()
        {
            return PixelNames;
        }
        public static IEnumerable<Pixel> GetKnownPixels()
        {
            return NamedPixels;
        }

    }
}
