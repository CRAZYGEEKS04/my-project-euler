using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectEuler.Common;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// Find the number of non-empty subsets of {1^1, 2^2, 3^3, ..., 250250^250250},
    /// the sum of whose elements is divisible by 250. Enter the rightmost 16 digits
    /// as your answer.
    /// </summary>
    internal class Problem250 : Problem
    {
        private const int upper = 250250;
        private const int divisor = 250;
        private static long modulo = Misc.Pow(10, 16);

        public Problem250() : base(250) { }

        private long Count(int[] values)
        {
            /**
             * n^n > (n-1)^(n-1) + ... + 2^2 + 1^1, using DP
             */
            long[] counter = new long[divisor];

            for (int n = 1; n <= upper; n++)
            {
                long[] tmp = (long[])counter.Clone();

                for (int i = 0; i < divisor; i++)
                {
                    int nv = (i + values[n]) % divisor;

                    tmp[nv] += counter[i];
                    tmp[nv] %= modulo;
                }
                tmp[values[n]]++;
                counter = tmp;
            }

            return counter[0];
        }

        protected override string Action()
        {
            var mod = new Modulo(divisor);
            int[] values = new int[upper + 1];

            for (int i = 1; i <= upper; i++)
                values[i] = (int)mod.Pow(i, i);

            return Count(values).ToString();
        }
    }
}