using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.Miscellany
{
    /// <summary>
    /// Let S(A) represent the sum of elements in set A of size n. We shall call it a
    /// special sum set if for any two non-empty disjoint subsets, B and C, the
    /// following properties are true:
    ///
    /// * S(B) != S(C); that is, sums of subsets cannot be equal.
    /// * If B contains more elements than C then S(B) > S(C).
    /// </summary>
    public static class OptimumSpecialSumSet
    {
        // Subsets have different sum
        private static bool CheckRule1(int[] set)
        {
            var tmp = new HashSet<int>();

            foreach (var n in Itertools.Range(1, set.Length - 1))
            {
                foreach (var subset in Itertools.Combinations(set, n))
                {
                    int sum = subset.Sum();

                    if (tmp.Contains(sum))
                        return false;
                    tmp.Add(sum);
                }
            }

            return true;
        }

        // more elements, bigger sum
        private static bool CheckRule2(int[] set)
        {
            int lsum = set[0];
            int rsum = 0;

            for (int l = 1; l < set.Length - 1; l++)
            {
                lsum += set[l];
                rsum += set[set.Length - l];

                if (lsum <= rsum)
                    return false;
            }

            return true;
        }

        private static void Find(int[] minimalSet, int[] currentSet, int pos, ref int minimalSum)
        {
            if (pos == minimalSet.Length)
            {
                if (CheckRule1(currentSet) && currentSet.Sum() <= minimalSum)
                {
                    minimalSum = currentSet.Sum();
                    currentSet.CopyTo(minimalSet, 0);
                }
                return;
            }

            // guarantee rule 2
            int lower = pos == 0 ? 1 : currentSet[pos - 1] + 1;
            int upper = (minimalSum - currentSet.Take(pos).Sum()) / (minimalSet.Length - pos);
            int lsum = currentSet[0];
            int rsum = 0;

            for (int i = 1; i < pos; i++)
            {
                lsum += currentSet[i];
                if (upper > lsum - rsum)
                    upper = lsum - rsum;
                rsum += currentSet[currentSet.Length - i - 1];
            }

            for (currentSet[pos] = lower; currentSet[pos] <= upper; currentSet[pos]++)
                Find(minimalSet, currentSet, pos + 1, ref minimalSum);
        }

        public static int[] GetNext(int[] prev)
        {
            /**
             * It seems that for a given optimum set, A = {a1, a2, ... , an}, the next approximate optimum
             * set is of the form B = {b, a1+b, a2+b, ... ,an+b}, where b is the "middle" element on the previous row.
             */
            var ret = new int[prev.Length + 1];
            var sum = prev.Sum() + prev[prev.Length / 2] * ret.Length;

            Find(ret, new int[ret.Length], 0, ref sum);

            return ret;
        }

        public static bool IsOptimumSpecialSumSet(int[] set)
        {
            var sortedset = set.OrderBy(it => it).ToArray();

            return CheckRule2(sortedset) && CheckRule1(sortedset);
        }
    }
}