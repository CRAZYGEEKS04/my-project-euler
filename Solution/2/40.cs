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
}