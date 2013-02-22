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

    /// <summary>
    /// A triplet of positive integers (a,b,c) is called a Cardano Triplet if it
    /// satisfies the condition:
    /// sqrt3(a + b * sqrt(c)) + sqrt3(a - b * sqrt(c)) = 1
    ///
    /// For example, (2,1,5) is a Cardano Triplet.
    ///
    /// There exist 149 Cardano Triplets for which a + b + c <= 1000.
    ///
    /// Find how many Cardano Triplets exist such that a + b + c <= 110,000,000.
    /// </summary>
    internal class Problem251 : Problem
    {
        private const int upper = 110000000;

        public Problem251() : base(251) { }

        protected override string Action()
        {
            /**
             * a+b*sqrt(c) = 1-a+b*sqrt(c) - 3x + 3x^2 where x = sqrt3(a-b*sqrt(c))
             * 3x^2 - 3x - 2a + 1 = 0
             * x = (3 +- sqrt(9 + 24a - 12)) / 6
             * c=((2*a-1)^3+27*a^2)/(27*b^2)
             * so a must be 2 mod 3, assume a = 3k-1
             * b^2*c = k^2*(8*k-3)
             */
            int maxk = (upper + 1) / 3;
            var sfc = new int[maxk + 1];
            var p = new Prime(maxk);
            var counter = 0;
            long a, b, c, tmpb, tmpc;
            double optb, optc;

            p.GenerateAll();
            for (int f = 2; f <= Misc.Sqrt(maxk * 8 - 3); f++)
            {
                int square = f * f;

                for (int j = square; j <= maxk * 8 - 3; j += square)
                {
                    if (j % 8 == 5)
                        sfc[(j + 3) / 8] = f;
                }
            }

            for (int k = 1; k <= maxk; k++)
            {
                long maxc;

                a = 3 * k - 1;
                // Get optimal b and c by derivation
                optb = Math.Exp(Math.Log(2.0 * k * k * (8 * k - 3)) / 3);
                optc = optb / 2;

                if (a + optb + optc > upper)
                    break;

                b = k;
                c = 8 * k - 3;
                if (sfc[k] != 0)
                {
                    c = c / sfc[k] / sfc[k];
                    b *= sfc[k];
                }

                var factors = Factor.GetDivisors(p, b);
                factors.Sort();

                maxc = upper / c;
                foreach (long f in factors)
                {
                    if (f * f > maxc)
                        break;

                    tmpb = b / f;
                    tmpc = c * f * f;
                    if (a + tmpb + tmpc <= upper)
                        counter++;
                }
            }

            return counter.ToString();
        }
    }
}