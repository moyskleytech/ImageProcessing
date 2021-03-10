using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FontLibrary
{
    public class WindowsFonts
    {
        static Dictionary<string,Font> Cache= new Dictionary<string, Font>();
        public static Font Get(string name)
        {
            var f_name = FontHelper(name);
            
            if ( f_name == null )
                throw new System.IO.FileNotFoundException(name);
            if ( Cache.ContainsKey(f_name) )
                return Cache[f_name];
            Font f = Font.FromFileStream(typeof(WindowsFonts).GetTypeInfo().Assembly.GetManifestResourceStream((f_name)));
            Cache[f_name] = f;
            return f;
        }

        private static string FontHelper(string name)
        {
            string lowered = name.ToLower();
            var fonts = GetFonts();
            return fonts.Where((x) => x.ToLower() == lowered).FirstOrDefault() ??
               fonts.Where((x) => x.ToLower() == "moyskleytech.imageprocessing.fontlibrary.fonts." + lowered).FirstOrDefault() ??
                fonts.Where((x) => x.ToLower() == "moyskleytech.imageprocessing.fontlibrary.fonts." + lowered + ".bin").FirstOrDefault();
        }
        public static IEnumerable<string> GetFonts()
        {
            return typeof(WindowsFonts).GetTypeInfo().Assembly.GetManifestResourceNames();
        }
    }
}
