using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.Graph
{
    public static class DisjointSet
    {
        public static DistjointSetNode MakeSet()
        {
            return new DistjointSetNode();
        }

        public static DistjointSetNode FindSet(DistjointSetNode x)
        {
            if (x != x.Parent)
                x.Parent = FindSet(x.Parent);

            return x.Parent;
        }

        public static void Union(DistjointSetNode x, DistjointSetNode y)
        {
            x = FindSet(x);
            y = FindSet(y);

            if (x == y)
                return;
            if (x.Rank > y.Rank)
            {
                y.Parent = x;
                x.Size += y.Size;
            }
            else
            {
                x.Parent = y;
                y.Size += x.Size;
                if (y.Rank == x.Rank)
                    y.Rank++;
            }
        }
    }
}