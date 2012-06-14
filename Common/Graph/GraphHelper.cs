using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.Graph
{
    public enum TopologicalSortType
    {
        VertexValueMinFirst,
    }

    public class PathFoundEventArgs<TVertex, TEdge> : EventArgs
    {
        public LinkedList<TVertex> Path { get; set; }
        public TEdge Distance { get; set; }

        public PathFoundEventArgs(LinkedList<TVertex> p, TEdge d)
        {
            Path = p;
            Distance = d;
        }
    }

    public static class VertexHelper
    {
        private class VertexSimpleHelper<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(T obj)
            {
                return obj.GetHashCode();
            }
        }

        public static IEqualityComparer<int> CreateIntHelper()
        {
            return new VertexSimpleHelper<int>();
        }

        public static IEqualityComparer<char> CreateCharHelper()
        {
            return new VertexSimpleHelper<char>();
        }

        public static IEqualityComparer<string> CreateStringHelper()
        {
            return new VertexSimpleHelper<string>();
        }

        public static IEqualityComparer<T> CreateSimpleHelper<T>()
        {
            return new VertexSimpleHelper<T>();
        }
    }

    public interface IEdgeComparer<T> : IComparer<T>
    {
        T Add(T x, T y);

        T Subtract(T x, T y);
    }

    public static class EdgeHelper
    {
        private class EdgeIntHelper : IEdgeComparer<int>
        {
            public int Compare(int x, int y)
            {
                return x.CompareTo(y);
            }

            public int Add(int x, int y)
            {
                return x + y;
            }

            public int Subtract(int x, int y)
            {
                return x - y;
            }
        }

        private class EdgeDoubleHelper : IEdgeComparer<double>
        {
            public int Compare(double x, double y)
            {
                return x.CompareTo(y);
            }

            public double Add(double x, double y)
            {
                return x + y;
            }

            public double Subtract(double x, double y)
            {
                return x - y;
            }
        }

        public static IEdgeComparer<int> CreateIntHelper()
        {
            return new EdgeIntHelper();
        }

        public static IEdgeComparer<double> CreateDoubleHelper()
        {
            return new EdgeDoubleHelper();
        }
    }
}