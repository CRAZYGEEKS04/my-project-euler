using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectEuler.Common;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// There are 1111 ways in which five 6-sided dice (sides numbered 1 to 6) can be
    /// rolled so that the top three sum to 15. Some examples are:
    ///
    /// D1,D2,D3,D4,D5 = 4,3,6,3,5
    /// D1,D2,D3,D4,D5 = 4,3,3,5,6
    /// D1,D2,D3,D4,D5 = 3,3,3,6,6
    /// D1,D2,D3,D4,D5 = 6,6,3,3,3
    ///
    /// In how many ways can twenty 12-sided dice (sides numbered 1 to 12) be rolled so
    /// that the top ten sum to 70?
    /// </summary>
    internal class Problem240 : Problem
    {
        private const int nDice = 20;
        private const int nSelectedDice = 10;
        private const int nSides = 12;
        private const int targetScore = 70;

        public Problem240() : base(240) { }

        private long Count(int[] counter, int max)
        {
            List<int> list = new List<int>();
            int left = nDice, min = 0;
            long sum = 0, mul = 1;

            for (int i = nSides; i > 0; i--)
            {
                if (counter[i] != 0)
                {
                    list.Add(counter[i]);
                    min = i;
                }
            }

            // dice bigger than the minimal die of the top dice
            for (int i = 0; i < list.Count - 1; i++)
            {
                mul *= Probability.CountCombinations(left, list[i]);
                left -= list[i];
            }
            for (int nMin = list[list.Count - 1]; nMin <= left; nMin++)
                sum += Probability.CountCombinations(left, nMin) * Misc.Pow(min - 1, left - nMin);

            return sum * mul;
        }

        private long Calculate(int[] counter, int id, int max, int left)
        {
            long sum = 0;

            if (id == nSelectedDice - 1)
            {
                counter[left]++;
                sum = Count(counter, max);
                counter[left]--;
            }
            else
            {
                if (left - max < nSelectedDice - id - 1)
                    max = left - (nSelectedDice - id - 1);

                for (int value = max; ; value--)
                {
                    if (value * (nSelectedDice - id) < left)
                        break;
                    counter[value]++;
                    sum += Calculate(counter, id + 1, value, left - value);
                    counter[value]--;
                }
            }

            return sum;
        }

        protected override string Action()
        {
            var counter = new int[nSides + 1];

            return Calculate(counter, 0, nSides, targetScore).ToString();
        }
    }

    /// <summary>
    /// For a positive integer n, let σ(n) be the sum of all divisors of n, so e.g.
    /// σ(6) = 1 + 2 + 3 + 6 = 12.
    ///
    /// A perfect number, as you probably know, is a number with σ(n) = 2n.
    ///
    /// Let us define the perfection quotient of a positive integer as p(n) = σ(n) / n.
    ///
    /// Find the sum of all positive integers n <= 10^18 for which p(n) has the form
    /// k + 1⁄2, where k is an integer.
    /// </summary>
    internal class Problem241 : Problem
    {
        private static long upper = Misc.Pow(10, 18);

        public Problem241() : base(241) { }

        protected override string Action()
        {
            // http://www.numericana.com/answer/numbers.htm#hemiperfect
            long[] nums = new long[] {
                2, 24, 91963648, 10200236032,4320, 4680, 26208, 20427264, 197064960, 21857648640, 57575890944, 88898072401645056, 301183421949935616,
                8910720, 17428320, 8583644160, 57629644800, 206166804480, 1416963251404800, 15338300494970880, 6275163455171297280,
                17116004505600, 75462255348480000, 6219051710415667200,
            };
            long sum = 0;

            foreach (var num in nums)
            {
                if (num <= upper)
                    sum += num;
            }

            return sum.ToString();
        }
    }
}