using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Mathematics.Number
{
    public static class Range
    {
        public static IEnumerable<System.Int32> Int32()
        {
            for ( var i = System.Int32.MinValue; i < System.Int32.MaxValue - 1; i++ )
                yield return i;
            yield return System.Int32.MaxValue;
        }
        public static IEnumerable<System.Int64> Int64()
        {
            for ( var i = System.Int64.MinValue; i < System.Int64.MaxValue - 1; i++ )
                yield return i;
            yield return System.Int64.MaxValue;
        }
        public static IEnumerable<System.Int16> Int16()
        {
            for ( var i = System.Int16.MinValue; i < System.Int16.MaxValue - 1; i++ )
                yield return i;
            yield return System.Int16.MaxValue;
        }
        public static IEnumerable<System.Int32> PositiveInt32()
        {
            for ( int i = 0; i < System.Int32.MaxValue - 1; i++ )
                yield return i;
            yield return System.Int32.MaxValue;
        }
        public static IEnumerable<System.Int64> PositiveInt64()
        {
            for ( long i = 0; i < System.Int64.MaxValue - 1; i++ )
                yield return i;
            yield return System.Int64.MaxValue;
        }
        public static IEnumerable<System.Int16> PositiveInt16()
        {
            for ( short i = 0; i < System.Int16.MaxValue - 1; i++ )
                yield return i;
            yield return System.Int16.MaxValue;
        }
        public static IEnumerable<System.Int32> NegativeInt32()
        {
            for ( var i = System.Int32.MinValue; i < 0; i++ )
                yield return i;
        }
        public static IEnumerable<System.Int64> NegativeInt64()
        {
            for ( var i = System.Int64.MinValue; i < 0; i++ )
                yield return i;
        }
        public static IEnumerable<System.Int16> NegativeInt16()
        {
            for ( var i = System.Int16.MinValue; i < 0; i++ )
                yield return i;
        }
        public static IEnumerable<System.UInt32> UInt32()
        {
            for ( var i = System.UInt32.MinValue; i < System.UInt32.MaxValue - 1; i++ )
                yield return i;
            yield return System.UInt32.MaxValue;
        }
        public static IEnumerable<System.UInt64> UInt64()
        {
            for ( var i = System.UInt64.MinValue; i < System.UInt64.MaxValue - 1; i++ )
                yield return i;
            yield return System.UInt64.MaxValue;
        }
        public static IEnumerable<System.UInt16> UInt16()
        {
            for ( var i = System.UInt16.MinValue; i < System.UInt16.MaxValue - 1; i++ )
                yield return i;
            yield return System.UInt16.MaxValue;
        }

        public static IEnumerable<System.Byte> Byte()
        {
            for ( var i = System.Byte.MinValue; i < System.Byte.MaxValue - 1; i++ )
                yield return i;
            yield return System.Byte.MaxValue;
        }
        public static IEnumerable<System.SByte> SByte()
        {
            for ( var i = System.SByte.MinValue; i < System.SByte.MaxValue - 1; i++ )
                yield return i;
            yield return System.SByte.MaxValue;
        }
    }
}
