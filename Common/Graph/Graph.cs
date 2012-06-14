using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.Graph
{
    public class Graph<TVertex, TEdge>
    {
        private IEqualityComparer<TVertex> vHelper;
        private IEdgeComparer<TEdge> eHelper;

        public Dictionary<TVertex, Dictionary<TVertex, TEdge>> Data { get; private set; }

        public event EventHandler<PathFoundEventArgs<TVertex, TEdge>> PathFound = delegate { };

        public Graph(IEqualityComparer<TVertex> vh, IEdgeComparer<TEdge> eh)
        {
            Data = new Dictionary<TVertex, Dictionary<TVertex, TEdge>>(vh);
            vHelper = vh;
            eHelper = eh;
        }

        public void AddVertex(TVertex v)
        {
            if (!Data.ContainsKey(v))
                Data.Add(v, new Dictionary<TVertex, TEdge>(vHelper));
        }

        public void AddEdge(TVertex vstart, TVertex vend, TEdge edge)
        {
            if (!Data.ContainsKey(vstart) || !Data.ContainsKey(vend))
                throw new ArgumentException("invalid vertex");

            Data[vstart][vend] = edge;
        }

        public void AddTwoWayEdge(TVertex v1, TVertex v2, TEdge edge)
        {
            if (!Data.ContainsKey(v1) || !Data.ContainsKey(v2))
                throw new ArgumentException("invalid vertex");

            Data[v1][v2] = edge;
            Data[v2][v1] = edge;
        }

        private void VertexValueFirstTopologicalSort(IComparer<TVertex> vc)
        {
            var degree = new Dictionary<TVertex, int>(vHelper);
            var visited = new HashSet<TVertex>(vHelper);
            var left = new SortedSet<TVertex>(vc);
            var ret = new LinkedList<TVertex>();

            foreach (var v in Data.Keys)
                degree.Add(v, 0);

            foreach (var edges in Data.Values)
            {
                foreach (var vertex in edges.Keys)
                {
                    degree[vertex]++;
                }
            }

            foreach (var d in degree)
            {
                if (d.Value == 0)
                    left.Add(d.Key);
            }

            while (left.Count != 0)
            {
                var tmp = left.Min;

                left.Remove(tmp);
                visited.Add(tmp);
                ret.AddLast(tmp);

                foreach (var v in Data[tmp].Keys)
                {
                    if (!visited.Contains(v))
                    {
                        degree[v]--;
                        if (degree[v] == 0)
                            left.Add(v);
                    }
                }
            }

            PathFound(this, new PathFoundEventArgs<TVertex, TEdge>(ret, default(TEdge)));
        }

        public void TopologicalSort(TopologicalSortType type, IComparer<TVertex> vc = null)
        {
            switch (type)
            {
                case TopologicalSortType.VertexValueMinFirst:
                    if (vc == null)
                        throw new ArgumentNullException("must specify icomparer");
                    VertexValueFirstTopologicalSort(vc);
                    break;
                default:
                    throw new ArgumentException("invalid type");
            }
        }

        private class VertexInfo
        {
            public TVertex prev;
            public TEdge distance;

            public VertexInfo(TVertex p, TEdge d)
            {
                prev = p;
                distance = d;
            }
        }

        private void Relax(PriorityQueue<TVertex, TEdge> q, HashSet<TVertex> s, Dictionary<TVertex, VertexInfo> p)
        {
            KeyValuePair<TVertex, TEdge> min = q.ExtractMin();

            s.Add(min.Key);
            foreach (var pair in Data[min.Key])
            {
                TEdge tmp = eHelper.Add(min.Value, pair.Value);

                if (s.Contains(pair.Key))
                    continue;
                if (!p.ContainsKey(pair.Key))
                {
                    p.Add(pair.Key, new VertexInfo(min.Key, tmp));
                    q.Add(pair.Key, tmp);
                }
                else if (eHelper.Compare(tmp, p[pair.Key].distance) < 0)
                {
                    p[pair.Key].prev = min.Key;
                    p[pair.Key].distance = tmp;
                    q.DecreaseKey(pair.Key, tmp);
                }
            }
        }

        public void FindMinPath(TVertex start, TVertex end, TEdge init = default(TEdge))
        {
            var q = new PriorityQueue<TVertex, TEdge>(vHelper, eHelper);
            var s = new HashSet<TVertex>(vHelper);
            var p = new Dictionary<TVertex, VertexInfo>(vHelper);

            q.Add(start, init);
            while (q.Count > 0 && !s.Contains(end))
                Relax(q, s, p);

            if (!s.Contains(end))
                return;

            LinkedList<TVertex> path = new LinkedList<TVertex>();
            TVertex tmp = end;

            while (p.ContainsKey(tmp))
            {
                path.AddFirst(tmp);
                tmp = p[tmp].prev;
            }
            path.AddFirst(tmp);

            PathFound(this, new PathFoundEventArgs<TVertex, TEdge>(path, p[end].distance));
        }

        private void GetLightEdge(PriorityQueue<TVertex, TEdge> q, HashSet<TVertex> s, Dictionary<TVertex, VertexInfo> p)
        {
            KeyValuePair<TVertex, TEdge> min = q.ExtractMin();

            s.Add(min.Key);
            foreach (var pair in Data[min.Key])
            {
                if (s.Contains(pair.Key))
                    continue;
                if (!p.ContainsKey(pair.Key))
                {
                    p.Add(pair.Key, new VertexInfo(min.Key, pair.Value));
                    q.Add(pair.Key, pair.Value);
                }
                else if (eHelper.Compare(pair.Value, p[pair.Key].distance) < 0)
                {
                    p[pair.Key].prev = min.Key;
                    p[pair.Key].distance = pair.Value;
                    q.DecreaseKey(pair.Key, pair.Value);
                }
            }
        }

        public Graph<TVertex, TEdge> FindMinSpinningTree()
        {
            var g = new Graph<TVertex, TEdge>(vHelper, eHelper);
            var q = new PriorityQueue<TVertex, TEdge>(vHelper, eHelper);
            var p = new Dictionary<TVertex, VertexInfo>(vHelper);
            var s = new HashSet<TVertex>(vHelper);

            foreach (var vertex in Data.Keys)
                g.AddVertex(vertex);

            TVertex tmp = Data.Keys.ToList()[0];
            foreach (var pair in Data[tmp])
            {
                p.Add(pair.Key, new VertexInfo(tmp, pair.Value));
                q.Add(pair.Key, pair.Value);
            }
            s.Add(tmp);

            while (q.Count > 0)
                GetLightEdge(q, s, p);

            foreach (var pair in p)
                g.AddTwoWayEdge(pair.Key, pair.Value.prev, pair.Value.distance);

            return g;
        }
    }
}