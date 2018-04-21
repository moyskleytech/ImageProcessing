using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image.Helper
{
    /// <summary>
    /// Array that loops on itself
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularArray<T> : IEnumerable<T>
    {
        T[] array;
        /// <summary>
        /// Create Circular from specified array
        /// </summary>
        /// <param name="input">Array</param>
        public CircularArray(T[ ] input)
        {
            array = input;
        }
        /// <summary>
        /// Enumerate cyclic
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            while ( true )
                foreach ( T t in array )
                    yield return t;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
