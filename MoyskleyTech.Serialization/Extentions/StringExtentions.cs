using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Serialization.Extentions
{
    public static class StringExtentions
    {
        public static bool Contains(this string d , char c)
        {
            bool resp=false;
            int len=d.Length;
            unsafe
            {
                fixed ( char* str = d )
                {
                    char* ptr=str;
                    for ( var i = 0 ; i < len&&!resp ; i++ )
                        resp = ( *ptr++ == c );
                }
            }
            return resp;
        }
    }
}
