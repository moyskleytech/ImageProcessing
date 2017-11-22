using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LMST.Sportif.DLL.Windows.Controls
{
    public class TextOutputEventArgs<T> : EventArgs
    {
        public string Output
        {
            get; set;
        }
        public T Input
        {
            get; set;
        }
    }
}
