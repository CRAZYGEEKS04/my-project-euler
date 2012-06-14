using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.Graph
{
    public class DistjointSetNode
    {
        public DistjointSetNode Parent;
        public int Rank;
        public int Size;

        public DistjointSetNode()
        {
            Parent = this;
            Rank = 0;
            Size = 1;
        }
    }
}