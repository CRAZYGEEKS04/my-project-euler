using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ProjectEuler.Common.Partition
{
    public class DivideBlock
    {
        private List<BigInteger> results;
        private List<int> lengths;

        public BigInteger this[int i] { get { return results[i]; } }

        public DivideBlock(IEnumerable<int> lengths)
        {
            this.results = new List<BigInteger>();
            this.lengths = lengths.ToList();
            this.lengths.Sort();

            this.results.Add(1);
        }

        public void Generate(int length)
        {
            for (int i = results.Count; i <= length; i++)
            {
                BigInteger tmp = 0;

                // leading l block
                foreach (var l in lengths)
                {
                    if (i - l < 0)
                        break;
                    tmp += results[i - l];
                }
                results.Add(tmp);
            }
        }
    }
}