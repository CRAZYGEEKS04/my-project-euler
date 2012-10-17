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

        private class Factor
        {
            public long p;
            public int e;

            public Factor(long p, int e)
            {
                this.p = p;
                this.e = e;
            }

            public Factor(Factor other)
            {
                this.p = other.p;
                this.e = other.e;
            }
        }

        private class FactorList : LinkedList<Factor>
        {
            // for all powers in the list with prime <= cutoff, increment exponent by incr
            public void IncrementPowers(int incr, int cutoff)
            {
                foreach (var factor in this)
                {
                    if (factor.p <= cutoff)
                        factor.e += incr;
                }
            }

            // see if the list of factors is a good answer
            public bool IsCorrect()
            {
                double tmp = 1;

                foreach (var f in this)
                {
                    tmp *= Misc.Pow(f.p, f.e + 1) - 1;
                    tmp /= Misc.Pow(f.p, f.e);
                    tmp /= (f.p - 1);
                }

                return Math.Abs(tmp - Math.Floor(tmp) - 0.5) <= 0.000001;
            }

            // from source, remove those factors in primes
            public void RemoveUsedPrimes(FactorList primes)
            {
                var node = First;

                while (node != null)
                {
                    var next = node.Next;

                    foreach (var p in primes)
                    {
                        if (p.p == node.Value.p)
                        {
                            Remove(node);
                            break;
                        }
                    }
                    node = next;
                }
            }

            // Add any factors from second into first to make first have largest of all exponents. Return the modified first. Both are sorted by prime
            public static FactorList operator +(FactorList left, FactorList right)
            {
                FactorList ret = new FactorList();
                var p1 = left.First;
                var p2 = right.First;

                while (p1 != null && p2 != null)
                {
                    if (p1.Value.p < p2.Value.p)
                    {
                        ret.AddLast(p1.Value);
                        p1 = p1.Next;
                    }
                    else if (p1.Value.p == p2.Value.p)
                    {
                        ret.AddLast(new Factor(p1.Value.p, p1.Value.e + p2.Value.e));
                        p1 = p1.Next;
                        p2 = p2.Next;
                    }
                    else
                    {
                        ret.AddLast(p2.Value);
                        p2 = p2.Next;
                    }
                }

                while (p1 != null)
                {
                    ret.AddLast(p1.Value);
                    p1 = p1.Next;
                }
                while (p2 != null)
                {
                    ret.AddLast(p2.Value);
                    p2 = p2.Next;
                }

                return ret;
            }

            private static Dictionary<long, List<Factor>> dict = new Dictionary<long, List<Factor>>();

            public static FactorList Factorize(Prime prime, long n)
            {
                var list = new FactorList();

                if (!dict.ContainsKey(n))
                {
                    List<Factor> factors = new List<Factor>();
                    long tmp = n;
                    int e = 0;

                    foreach (int p in prime)
                    {
                        if (p * p > tmp)
                            break;
                        if (tmp % p != 0)
                            continue;

                        e = 0;
                        while (tmp % p == 0)
                        {
                            tmp /= p;
                            e++;
                        }
                        factors.Add(new Factor(p, e));
                    }
                    if (tmp > 1)
                        factors.Add(new Factor(tmp, 1));
                    dict.Add(n, factors);
                }

                foreach (var factor in dict[n])
                    list.AddLast(new Factor(factor));

                return list;
            }
        }

        // recurse through the given position, adding solutions to global answers
        private void Recurse(HashSet<long> nums, Prime primes, long num, FactorList used, FactorList left, long twoPower, int incr, int cutoff, int depth)
        {
            // update num which is current value
            long pow = Misc.Pow(used.Last.Value.p, used.Last.Value.e);
            if (num > upper / pow)
                return;
            num *= pow;

            if (used.IsCorrect())
            {
                nums.Add(num);
                return;
            }
            if (left.Count == 0)
                return;
            Factor factor = left.First.Value;
            long p = factor.p;
            int e = factor.e;
            left.RemoveFirst();

            long s = 0;
            used.AddLast(new Factor(p, 0));
            for (int tmpe = 0; tmpe <= e; tmpe++) // for each possible power
            {
                s += Misc.Pow(p, tmpe);
                var f = FactorList.Factorize(primes, s);
                long twos = 0;
                if (f.Count > 0 && f.First.Value.p == 2)
                    twos = f.First.Value.e;
                if (twos <= twoPower)
                {
                    used.Last.Value.e = tmpe;
                    f.RemoveUsedPrimes(used);
                    f.IncrementPowers(incr, cutoff);
                    Recurse(nums, primes, num, used, f + left, twoPower - twos, incr, cutoff, ++depth);
                }
            }
            used.RemoveLast();
        }

        protected override string Action()
        {
            /**
             * Let n = q1^i1 * q2^i2 * ... * qn^in, s(n) = (q1^(i1+1)-1)/(q1-1) * (q2^(i2+1)-1)/(q2-1) ... * (qn^(in+1)-1)/(qn-1)
             * In the formula for s(n)/n, a prime power p^i gives a fraction (1+p+p^2+...+p^i)/(p^i).
             * So to get s(n)/n=k+1/2 means there is a power of 2 dividing n.
             *
             * The other primes dividing n require the same prime in a numerator somewhere.
             * Start with the power of 2, factor the numerator, and add these primes and their exponents to a possible prime list.
             * Recursively add these primes into n, checking for good answers.
             */
            var nums = new HashSet<long>();
            var p = new Prime(1024);
            FactorList val;

            p.GenerateAll();
            // Only need to check up to 2^20 by experiment
            for (int e = 1; e <= 20; e++)
            {
                /**
                 * Find all answers with 2^e in the prime decomposition,
                 * incr and cutoff are used to bump low exponents up:
                 * any prime appearing <= cutoff gets the max exponent incremented by incr
                 */
                var num = FactorList.Factorize(p, Misc.Pow(2, e + 1) - 1);
                int incr = 3, cutoff = 31;

                val = new FactorList();
                val.AddFirst(new Factor(2, e));
                num.IncrementPowers(incr, cutoff);
                Recurse(nums, p, 1, val, num, e - 1, incr, cutoff, 0);
            }

            return nums.Sum().ToString();
        }
    }
}