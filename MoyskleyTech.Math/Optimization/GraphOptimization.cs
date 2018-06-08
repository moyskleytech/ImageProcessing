using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Mathematics.Optimization
{
    public class GraphOptimization<T>
    {
        public double CostIfSame { get; set; } = 0;
        public double CostIfImpossible { get; set; } = double.NaN;
        private class Transition
        {
            public List<T> action;
            public double cost;
        }
        public double GetConversionCostFrom(T a, T b)
        {
            if ( a.Equals( b) )
                return CostIfSame;
            var from = transitions;
            if ( from.ContainsKey(a) )
            {
                var to = transitions[a];
                if ( to.ContainsKey(b) )
                {
                    return to[b].cost;
                }
            }
            return CostIfImpossible;
        }
        public List<T> GetConversionFrom(T a , T b)
        {
            if ( a.Equals(b) )
                return new List<T>() { a };
            var from = transitions;
            if ( from.ContainsKey(a) )
            {
                var to = transitions[a];
                if ( to.ContainsKey(b) )
                {
                    return to[b].action.ToList();
                }
            }
            return null;
        }
        public void RegisterTransition(T a, T b , double cost)
        {
            Transition t = new Transition() {  action=new List<T>(){a,b} , cost=cost};
            var from = transitions;
            if ( !from.ContainsKey(a) )
                from[a] = new Dictionary<T , Transition>();
            var to = from[a];
            to[b] = t;
        }
        private Dictionary<T,Dictionary<T,Transition>> transitions = new Dictionary<T, Dictionary<T, Transition>>();
        public void CompleteTransitions()
        {
            //transitions.Keys should always contain everything since transitions should be both way
            var types = transitions.Keys.Union(transitions.SelectMany((x)=>x.Value.Keys)).ToList();
            var from = transitions;
            bool foundOne=true;
            for(var tn=0;tn<types.Count;tn++ )
            {
                T t = types[tn];
                if ( !from.ContainsKey(t) )
                    from[t] = new Dictionary<T , Transition>();
                double[] weight = new double[types.Count];
                List<T>[] links = new List<T>[types.Count];
                foreach ( var kv in from[t] )
                {
                    var idx=types.IndexOf(kv.Key);
                    weight[idx] = kv.Value.cost;
                    links[idx] = kv.Value.action;
                }
                foundOne = true;
                while ( foundOne )
                {
                    foundOne = false;
                    for ( var i = 0; i < types.Count; i++ )//for all non direct or loss
                    {
                        var dest = types[i];
                        if ( tn!=i )
                        {
                            var possibleSources = from.Where((x) => x.Value.ContainsKey(dest)).OrderBy((x)=>x.Value[dest].cost);
                            foreach ( var link in possibleSources )
                            {
                                var idx = types.IndexOf(link.Key);
                                var w = weight[idx] + link.Value[dest].cost;
                                if ( w < weight[i] )
                                {
                                    weight[i] = w;
                                    links[i] = Merge(links[idx] , link.Value[dest].action);
                                    from[t][dest] = new Transition() { action = links[i] , cost = weight[i] };
                                    foundOne = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        private List<T> Merge(List<T> a,List<T> b)
        {
            return a.Concat(b.Skip(1)).ToList();
        }
    }
}
