using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Serialization.Binary
{
    public interface CustomBinarySerializer
    {
        bool IsMatch(Type serializedType);
        void ToStream(Stream s , object o , Dictionary<object , int> context);
        object FromStream(Stream s , Dictionary<int , object> context);
    }
}
