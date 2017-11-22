using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Math.Condition
{
    public class ModuloWhere<T> : IEnumerable<T>
    {
        int max,count,initial;
        private IEnumerable<T> source;
        public ModuloWhere(IEnumerable<T> source,int max=2,int count=1,int initial=0)
        {
            this.source = source;
            this.max = max;
            this.count = count;
            this.initial = initial;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return Apply().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Apply().GetEnumerator();
        }

        private IEnumerable<T> Apply()
        {
            long pos=initial;
            foreach ( T t in source )
            {
                if ( pos < count )
                    yield return t;
                pos++;
                if ( pos == max )
                    pos = 0;
            }
        }
    }
}
