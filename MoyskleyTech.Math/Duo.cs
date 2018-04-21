using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Mathematics
{
    public struct Duo<T, V>
    {
        public T ValueA;
        public V ValueB;
        public Duo(T t , V v)
        {
            this.ValueA = t;
            this.ValueB = v;
        }
        public static implicit operator T(Duo<T , V> duo)
        {
            return duo.ValueA;
        }
        public static implicit operator V(Duo<T , V> duo)
        {
            return duo.ValueB;
        }

        public static explicit operator Duo<T , V>(T val)
        {
            return new Duo<T , V>(val , default(V));
        }
        public static explicit operator Duo<T , V>(V val)
        {
            return new Duo<T , V>(default(T) , val);
        }
        public override string ToString()
        {
            return "{" + typeof(T).Name + ":" + ValueA + ";" + typeof(V).Name + ":" + ValueB + "}";
        }

    }
}
