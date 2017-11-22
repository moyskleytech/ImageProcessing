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
            name = FontHelper(name);
            
            if ( name == null )
                throw new System.IO.FileNotFoundException(name);
            if ( Cache.ContainsKey(name) )
                return Cache[name];
            Font f = Font.FromFileStream(typeof(WindowsFonts).GetTypeInfo().Assembly.GetManifestResourceStream((name)));
            Cache[name] = f;
            return f;
        }

        private static string FontHelper(string name)
        {
            string lowered = name.ToLower();
            return GetFonts().Where((x) => x.ToLower() == lowered).FirstOrDefault() ??
                GetFonts().Where((x) => x.ToLower() == "fontlibrary.fonts." + lowered).FirstOrDefault() ??
                GetFonts().Where((x) => x.ToLower() == "fontlibrary.fonts." + lowered + ".bin").FirstOrDefault();
        }
        public static IEnumerable<string> GetFonts()
        {
            return typeof(WindowsFonts).GetTypeInfo().Assembly.GetManifestResourceNames();
        }
    }
}
