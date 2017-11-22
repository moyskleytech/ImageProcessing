using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public class NotSerializedAttribute : Attribute
    {
    }
}
