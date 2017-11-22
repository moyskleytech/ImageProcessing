#if (PCL)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace ICSharpCode.SharpZipLib
{
    /// <summary>
    /// Simulate ArrayList
    /// </summary>
    class ArrayList : List<Object>
    {
        class PComparer : IComparer<Object>
        {
            IComparer _Cmp;
            public PComparer(IComparer cmp)
            {
                _Cmp = cmp;
            }
            public int Compare(object x, object y)
            {
                return _Cmp.Compare(x, y);
            }
        }
        public ArrayList()
        {
        }
        public ArrayList(int capacity)
            : base(capacity)
        {
        }
        public new int Add(Object item)
        {
            base.Add(item);
            return Count - 1;
        }
        public void Sort(IComparer comparer)
        {
            base.Sort(new PComparer(comparer));
        }
        public virtual Array ToArray(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            var items = this.ToArray();
            Array array = Array.CreateInstance(type, items.Length);
            Array.Copy(items, 0, array, 0, items.Length);
            return array;
        }

    }
    /// <summary>
    /// Simulate Hashtable
    /// </summary>
    class Hashtable : Dictionary<Object, Object>
    {

    }
    /// <summary>
    /// Simulate ICloneable
    /// </summary>
    interface ICloneable
    {
        Object Clone();
    }
    /// <summary>
    /// Simulate System.IO.PathTooLongException
    /// </summary>
    public class PathTooLongException : Exception
    {
        /// <summary>
        /// Create a new exception
        /// </summary>
        public PathTooLongException()
            : base("Path too long")
        {
        }
        /// <summary>
        /// Create a new exception
        /// </summary>
        public PathTooLongException(String message)
            : base(message)
        {
        }
        /// <summary>
        /// Create a new exception
        /// </summary>
        public PathTooLongException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
    /// <summary>
    /// Implements a simple ASCII encoding
    /// </summary>
    public class AsciiEncoding : Encoding
    {
        /// <summary>
        /// Default encoding
        /// </summary>
        public static readonly AsciiEncoding Default = new AsciiEncoding();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int GetByteCount(char[] chars, int index, int count)
        {
            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="charIndex"></param>
        /// <param name="charCount"></param>
        /// <param name="bytes"></param>
        /// <param name="byteIndex"></param>
        /// <returns></returns>
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            for (int i = 0; i < charCount; i++)
                bytes[byteIndex + i] = (byte)chars[charIndex + i];
            return charCount;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="byteIndex"></param>
        /// <param name="byteCount"></param>
        /// <param name="chars"></param>
        /// <param name="charIndex"></param>
        /// <returns></returns>
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            for (int i = 0; i < byteCount; i++)
                chars[charIndex + i] = (char)bytes[byteIndex + i];
            return byteCount;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="charCount"></param>
        /// <returns></returns>
        public override int GetMaxByteCount(int charCount)
        {
            return charCount;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount;
        }
    }
}
#endif