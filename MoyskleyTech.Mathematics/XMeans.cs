using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Mathematics
{
    public class XMeansDistanceBased<T>
    {
        private List<Cluster> clusters;
        public XMeansDistanceBased(int clusterCount , Func<T[ ]> getStartingMeans , Func<T , T , double> getDistance , Func<IEnumerable<T> , T> getMean , IEnumerable<T> values , int maxIteration,double maxDistance)
        {
            clusters = new List<Cluster>(clusterCount);
            T[] sm = getStartingMeans();
            for ( var i = 0; i < clusterCount; i++ )
                clusters.Add(new Cluster() { Mean = sm[i] });

            var arrValues = values.ToArray();

            Cluster getMin(T value)
            {
                double distance = double.MaxValue;
                Cluster retour = clusters[0];
                foreach ( var c in clusters )
                {
                    var d = System.Math.Abs(getDistance(c.Mean,value));
                    if ( d < distance )
                    {
                        retour = c;
                        distance = d;
                    }
                    if ( d == 0 )
                        break;
                }
                if(retour.Count > 3*arrValues.Length / clusters.Count || retour.Count/(double)arrValues.Length > 0.25)
                    retour = new Cluster() { Mean = value , Count = 0 };
                if ( distance > maxDistance )
                    retour = new Cluster() { Mean = value,Count=0 };
                return retour;
            }

            bool changed=true;
            do
            {
                int[] countDebut = (from x in clusters select x.Count).ToArray();

                foreach ( var c in clusters )
                {
                    c.Count = 0;
                    c.Items.Clear();
                }

                foreach ( T val in arrValues )
                {
                    var cluster=getMin(val);
                    cluster.Count++;
                    cluster.Items.Add(val);
                }

                foreach ( var c in clusters )
                {
                    c.Mean = getMean(c.Items);
                }
                for ( var i = 0; i < clusters.Count; i++ )
                    if ( clusters[i].Count == 0 )
                    {
                        clusters[i].chance--;
                        if ( clusters[i].chance == 0 )
                        {
                            clusters.RemoveAt(i);
                            i--;
                        }
                    }
                    else
                        clusters[i].chance = 3;
                changed = countDebut.SequenceEqual(( from x in clusters select x.Count ));

            } while ( changed && maxIteration-- > 0 );
            foreach ( var c in clusters )
            {
                c.Items.Clear();
            }
        }
        public IEnumerable<T> Means
        {
            get
            {
                return from x in clusters select x.Mean;
            }
        }
        public IEnumerable<Cluster> Clusters => from x in clusters select x;
        public class Cluster
        {
            public T Mean { get; internal set; }
            public int Count { get; internal set; }
            internal List<T> Items { get; set; } = new List<T>();
            internal int chance=3;
        }
    }
}
