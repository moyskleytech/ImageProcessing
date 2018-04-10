using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MoyskleyTech.Core
{
    public class RArray<T>: IEnumerable<T>
    {
        T[] inner;

        public RArray(IEnumerable<T> arr)
        {
            inner = arr.ToArray();
        }
        public IEnumerator<T> GetEnumerator()
        {
            return ( ( IEnumerable<T> ) inner ).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IEnumerable<T> ) inner ).GetEnumerator();
        }
        public int Length=> inner.Length; 
        public T this[int idx]
        {
            get
            {
                return inner[idx];
            }
        }
    }
}
