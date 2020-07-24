using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Mathematics.Condition
{
    public class Where<T> : IEnumerable<T>
    {
        private Func<T , bool> cond;
        private IEnumerable<T> source;
        public Where(IEnumerable<T> source,Func<T , bool> cond)
        {
            this.source = source;
            this.cond = cond;
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
            foreach ( T t in source )
            {
                if ( cond(t) )
                    yield return t;
            }
        }
    }
}
