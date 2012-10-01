using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ProjectEuler.Common.Miscellany
{
    /// <summary>
    /// http://en.wikipedia.org/wiki/Derangement
    /// </summary>
    public class Derangement
    {
        public int N { get; private set; }

        private List<BigInteger> subfactorial;

        public Derangement(int n)
        {
            subfactorial = new List<BigInteger>() { 1 };
            N = n;
        }

        /// <summary>
        /// Exactly index items are not in their original positions.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public BigInteger this[int index]
        {
            get
            {
                /**
                 * http://mathworld.wolfram.com/PartialDerangement.html
                 */
                int k = N - index;

                if (k > N || k < 0)
                    throw new ArgumentException("invalid index");

                while (N - k >= subfactorial.Count)
                {
                    int n = subfactorial.Count;

                    subfactorial.Add(subfactorial[n - 1] * n);
                    if (n % 2 == 0)
                        subfactorial[n]++;
                    else
                        subfactorial[n]--;
                }

                return Probability.CountCombinations(new BigInteger(N), k) * subfactorial[N - k];
            }
        }
    }
}