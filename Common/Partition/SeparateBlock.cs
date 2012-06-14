using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ProjectEuler.Common.Partition
{
    public class SeparateBlock
    {
        private List<BigInteger> results;
        private int m;

        public BigInteger this[int i] { get { return results[i]; } }

        public SeparateBlock(int m)
        {
            this.m = m;
            results = new List<BigInteger>();

            for (int i = 0; i < m; i++)
                results.Add(1);
            results.Add(2);
        }

        public void Generate(int length)
        {
            for (int i = results.Count; i <= length; i++)
            {
                BigInteger tmp = 0;

                // m*b [...]
                tmp += this[i - m];

                for (int lb = 1; lb < m; lb++)
                {
                    // i*b r b [...]
                    for (int lr = m; lr <= i - lb - 1; lr++)
                        tmp += results[i - lb - lr - 1];
                    // i*b r
                    if (lb + m <= i)
                        tmp++;
                }
                // r b [...]
                for (int lr = m; lr <= i - 1; lr++)
                    tmp += this[i - lr - 1];
                // r
                tmp++;

                results.Add(tmp);
            }
        }
    }
}