using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Mathematics.LINQ
{
    public static class Linq
    {
        public static bool HasMoreThan<T>(this IEnumerable<T> src , int maxCount)
        {
            int count=0;
            foreach ( T t in src )
            {
                count++;
                if ( count > maxCount )
                    return true;
            }
            return false;
        }
        public static T OnlyValue<T>(this IEnumerable<T> src)
        {
            int count=0;
            T value=default(T);
            foreach ( T t in src )
            {
                value = t;
                count++;
                if ( count > 1 )
                    throw new InvalidOperationException("More than one value");
            }
            return value;
        }
        public static T OnlyValueOrDefault<T>(this IEnumerable<T> src)
        {
            int count=0;
            T value=default(T);
            foreach ( T t in src )
            {
                value = t;
                count++;
                if ( count > 1 )
                    return default(T);
            }
            return value;
        }
        public static T OnlyValueOrFallback<T>(this IEnumerable<T> src,T fallback)
        {
            int count=0;
            T value=fallback;
            foreach ( T t in src )
            {
                value = t;
                count++;
                if ( count > 1 )
                    return fallback;
            }
            return value;
        }
    }
}
