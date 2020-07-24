using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace MoyskleyTech.Mathematics.Number
{
    public struct InpreciseNumber
    {
        public double BaseValue { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public InpreciseNumber WithAbsoluteMarginOf(double margin) => FromValue(BaseValue , BaseValue - margin , BaseValue + margin);
        public InpreciseNumber WithRelativeMarginOf(double margin) => FromValue(BaseValue , BaseValue - (margin*BaseValue) , BaseValue + ( margin * BaseValue ));
        public static InpreciseNumber FromValueAndMarginOfError(double value , double margin) => FromValue(value ,value - margin , value + margin);
        public static InpreciseNumber FromValue(double value , double min , double max) => new InpreciseNumber() { BaseValue = value , MinValue = System.Math.Min(min,max) , MaxValue = System.Math.Max(min , max) };
        public static explicit operator decimal(InpreciseNumber a) => (decimal)a.BaseValue;
        public static explicit operator double(InpreciseNumber a) => a.BaseValue;
        public static explicit operator float(InpreciseNumber a) => (float)a.BaseValue;
        public static explicit operator long(InpreciseNumber a) => (long)a.BaseValue;
        public static explicit operator int(InpreciseNumber a) => (int)a.BaseValue;
        public static explicit operator short(InpreciseNumber a) => (short)a.BaseValue;
        public static explicit operator sbyte(InpreciseNumber a) => (sbyte)a.BaseValue;
        public static explicit operator ulong(InpreciseNumber a) => (ulong)a.BaseValue;
        public static explicit operator uint(InpreciseNumber a) => (uint)a.BaseValue;
        public static explicit operator ushort(InpreciseNumber a) => (ushort)a.BaseValue;
        public static explicit operator byte(InpreciseNumber a) => (byte)a.BaseValue;
        public static explicit operator InpreciseNumber(decimal a) => FromValue((double)a , ( double ) a , ( double ) a);
        public static implicit operator InpreciseNumber(double a) => FromValue(a , a , a);
        public static implicit operator InpreciseNumber(float a) => FromValue(a , a , a);
        public static implicit operator InpreciseNumber(long a) => FromValue(a , a , a);
        public static implicit operator InpreciseNumber(int a) => FromValue(a , a , a);
        public static implicit operator InpreciseNumber(short a) => FromValue(a , a , a);
        public static implicit operator InpreciseNumber(sbyte a) => FromValue(a , a , a);
        public static implicit operator InpreciseNumber(ulong a) => FromValue(a , a , a);
        public static implicit operator InpreciseNumber(uint a) => FromValue(a , a , a);
        public static implicit operator InpreciseNumber(ushort a) => FromValue(a , a , a);
        public static implicit operator InpreciseNumber(byte a) => FromValue(a , a , a);
        public static InpreciseNumber operator +(InpreciseNumber a , InpreciseNumber b) => FromValue(a.BaseValue + b.BaseValue , a.MinValue + b.MinValue , a.MaxValue + b.MaxValue);
        public static InpreciseNumber operator -(InpreciseNumber a , InpreciseNumber b) => FromValue(a.BaseValue - b.BaseValue , a.MinValue - b.MaxValue , a.MaxValue - b.MinValue);
        public static InpreciseNumber operator *(InpreciseNumber a , InpreciseNumber b) => FromValue(a.BaseValue * b.BaseValue , a.MinValue * b.MinValue , a.MaxValue * b.MaxValue);
        public static InpreciseNumber operator /(InpreciseNumber a , InpreciseNumber b) => FromValue(a.BaseValue / b.BaseValue , a.MinValue / b.MaxValue , a.MaxValue / b.MinValue);
    }
    public struct InpreciseNumber<N>
        where N : struct
    {
        public N BaseValue { get; set; }
        public N MinValue { get; set; }
        public N MaxValue { get; set; }
        public InpreciseNumber<N> WithAbsoluteMarginOf(N margin)
        {
            return FromValue(BaseValue , ( dynamic ) BaseValue - margin , ( dynamic ) BaseValue + margin);
        }

        public InpreciseNumber<N> WithRelativeMarginOf(N margin)
        {
            return FromValue(BaseValue , BaseValue - ( ( dynamic ) margin * BaseValue ) , BaseValue + ( ( dynamic ) margin * BaseValue ));
        }

        public static InpreciseNumber<N> FromValueAndMarginOfError(N value , N margin)
        {
            return FromValue(value , ( dynamic ) value - margin , ( dynamic ) value + margin);
        }

        public static InpreciseNumber<N> FromValue(N value , N min , N max) => new InpreciseNumber<N>() { BaseValue = value , MinValue = System.Math.Min(( dynamic ) min , ( dynamic ) max) , MaxValue = System.Math.Max(( dynamic ) min , ( dynamic ) max) };
       
        public static explicit operator N(InpreciseNumber<N> a) => a.BaseValue;
        public static InpreciseNumber<N> operator +(InpreciseNumber<N> a , InpreciseNumber<N> b)
        {
            return FromValue(( dynamic ) a.BaseValue + b.BaseValue , ( dynamic ) a.MinValue + b.MinValue , ( dynamic ) a.MaxValue + b.MaxValue);
        }

        public static InpreciseNumber<N> operator -(InpreciseNumber<N> a , InpreciseNumber<N> b)
        {
            return FromValue(( dynamic ) a.BaseValue - b.BaseValue , ( dynamic ) a.MinValue - b.MaxValue , ( dynamic ) a.MaxValue - b.MinValue);
        }

        public static InpreciseNumber<N> operator *(InpreciseNumber<N> a , InpreciseNumber<N> b)
        {
            return FromValue(( dynamic ) a.BaseValue * b.BaseValue , ( dynamic ) a.MinValue * b.MinValue , ( dynamic ) a.MaxValue * b.MaxValue);
        }

        public static InpreciseNumber<N> operator /(InpreciseNumber<N> a , InpreciseNumber<N> b)
        {
            return FromValue(( dynamic ) a.BaseValue / b.BaseValue , ( dynamic ) a.MinValue / b.MaxValue , ( dynamic ) a.MaxValue / b.MinValue);
        }
    }
}
