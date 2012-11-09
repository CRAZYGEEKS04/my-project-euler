﻿using System;
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

    /// <summary>
    /// Given the set {1,2,...,n}, we define f(n,k) as the number of its k-element
    /// subsets with an odd sum of elements. For example, f(5,3) = 4, since the set
    /// {1,2,3,4,5} has four 3-element subsets having an odd sum of elements, i.e.:
    /// {1,2,4}, {1,3,5}, {2,3,4} and {2,4,5}.
    ///
    /// When all three values n, k and f(n,k) are odd, we say that they make an
    /// odd-triplet [n,k,f(n,k)].
    ///
    /// There are exactly five odd-triplets with n <= 10, namely:
    /// [1,1,f(1,1) = 1], [5,1,f(5,1) = 3], [5,5,f(5,5) = 1], [9,1,f(9,1) = 5] and
    /// [9,9,f(9,9) = 1].
    ///
    /// How many odd-triplets are there with n <= 10^12 ?
    /// </summary>
    internal class Problem242 : Problem
    {
        private const long upper = 1000000000000;

        public Problem242() : base(242) { }

        private long GetSum(List<long> array, int row, long id)
        {
            long counter = 0, pow = Misc.Pow(2, row);

            if (id <= 0)
                throw new InvalidOperationException();

            if (id > pow / 2)
            {
                counter = GetSum(array, row - 1, id - pow / 2) * 2;
                counter += array[row - 1];
            }
            else if (id == pow / 2)
            {
                counter = array[row - 1];
            }
            else
            {
                counter = GetSum(array, row - 1, id);
            }
            counter += Misc.Pow(2, row - 1);

            return counter;
        }

        protected override string Action()
        {
            /**
             * Only when n = 4k+1 is valid:
             * from 1, 5, ... 4k+1:
             * calculate the number of odd-triplet when n = 1, 5, 9, ...
             * 1, 2, 2, 4, 2, 4, 4, ...
             * write the numbers in like:
             * 1
             * 2, 2
             * 4, 2, 4, 4,
             * 8, 2, 4, 4, 8, 4, 8, 8,
             * 16,2, 4, 4, 8, 4, 8, 8, 16,4, 8, 8, 16,8,16,16,
             */
            var array = new List<long>() { 1 };
            long counter = 0, left = (upper + 3) / 4;

            for (long l = 1; l <= upper; l *= 2)
                array.Add(array[array.Count - 1] * 3 + l);
            for (int r = 0; left != 0; r++)
            {
                if (left >= Misc.Pow(2, r))
                {
                    counter += array[r];
                    left -= Misc.Pow(2, r);
                }
                else
                {
                    counter += GetSum(array, r, left);
                    break;
                }
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// A positive fraction whose numerator is less than its denominator is called a
    /// proper fraction.
    /// For any denominator, d, there will be d-1 proper fractions; for example, with
    /// d = 12:
    /// 1/12, 2/12, 3/12, 4/12, 5/12, 6/12, 7/12, 8/12, 9/12, 10/12, 11/12.
    ///
    /// We shall call a fraction that cannot be cancelled down a resilient fraction.
    /// Furthermore we shall define the resilience of a denominator, R(d), to be the
    /// ratio of its proper fractions that are resilient; for example, R(12) = 4/11.
    /// In fact, d = 12 is the smallest denominator having a resilience R(d) < 4/10.
    ///
    /// Find the smallest denominator d, having a resilience R(d) < 15499/94744.
    /// </summary>
    internal class Problem243 : Problem
    {
        private const int numerator = 15499;// 4;
        private const int denominator = 94744;// 10;

        public Problem243() : base(243) { }

        private void Calculate(List<int> p, ref long min, long n, long d, int id)
        {
            if (id == p.Count)
                return;

            n *= p[id] - 1;
            d *= p[id];
            if (d >= min)
                return;
            if (n * denominator < (d - 1) * numerator)
            {
                min = d;
                return;
            }
            Calculate(p, ref min, n, d, id + 1);

            while (d * p[id] < min)
            {
                n *= p[id];
                d *= p[id];
                if (n * denominator < (d - 1) * numerator)
                {
                    min = d;
                    return;
                }
                Calculate(p, ref min, n, d, id + 1);
            }
        }

        protected override string Action()
        {
            /**
             * R(d) = phi(d)/(d-1) = phi(d)/d * d/(d-1) = (1-1/p1) * (1-1/p2) * ... * (1-1/pn) * d / (d-1)
             */
            var prime = new Prime(100);
            long n = 1, d = 1;

            // Calculate temporary minimal value of d by p1*p2*...*pn
            prime.GenerateAll();
            foreach (var p in prime)
            {
                n *= p - 1;
                d *= p;
                if (n * denominator < (d - 1) * numerator)
                    break;
            }
            if (n * denominator >= (d - 1) * numerator)
                throw new ArgumentException();

            Calculate(prime.Nums, ref d, 1, 1, 0);

            return d.ToString();
        }
    }

    /// <summary>
    /// You probably know the game Fifteen Puzzle. Here, instead of numbered tiles, we
    /// have seven red tiles and eight blue tiles.
    ///
    /// A move is denoted by the uppercase initial of the direction (Left, Right, Up,
    /// Down) in which the tile is slid, e.g. starting from configuration (S), by the
    /// sequence LULUR we reach the configuration (E):
    ///
    ///  (S)     (E)
    ///  RBB    RRBB
    /// RRBB    RBBB
    /// RRBB    R RB
    /// RRBB    RRBB
    ///
    /// For each path, its checksum is calculated by (pseudocode):
    ///
    /// checksum = 0
    /// checksum = (checksum * 243 + m1) mod 100000007
    /// checksum = (checksum * 243 + m2) mod 100000007
    ///  …
    /// checksum = (checksum * 243 + mn) mod 100000007
    /// where mk is the ASCII value of the kth letter in the move sequence and the
    /// ASCII values for the moves are:
    ///
    /// L  76
    /// R  82
    /// U  85
    /// D  68
    ///
    /// For the sequence LULUR given above, the checksum would be 19761398.
    ///
    /// Now, starting from configuration (S), find all shortest ways to reach
    /// configuration (T).
    ///
    ///  (S)     (T)
    ///  RBB     BRB
    /// RRBB    BRBR
    /// RRBB    RBRB
    /// RRBB    BRBR
    ///
    /// What is the sum of all checksums for the paths having the minimal length?
    /// </summary>
    internal class Problem244 : Problem
    {
        private static string source = " RBBRRBBRRBBRRBB";
        private static string target = "RRBBRBBBR RBRRBB";

        public Problem244() : base(244) { }

        private struct State
        {
            public List<int> checksums;
            public int state;
            public int blank;

            public State(int s, int b, int c)
            {
                checksums = new List<int>() { c };
                state = s;
                blank = b;
            }

            public State(int s, int b, IEnumerable<int> c)
            {
                checksums = new List<int>(c);
                state = s;
                blank = b;
            }

            public void AddChecksum(IEnumerable<int> c)
            {
                checksums.AddRange(c);
            }
        }

        private State Parse(string text)
        {
            int ret = 0;
            int blank = 0;

            for (int i = 0; i < 16; i++)
            {
                switch (text[i])
                {
                    case 'R':
                        ret |= (1 << (i * 2));
                        break;
                    case 'B':
                        ret |= (2 << (i * 2));
                        break;
                    default:
                        blank = i;
                        break;
                }
            }

            return new State(ret, blank, 0);
        }

        private void AddNextState(HashSet<int> visited, Dictionary<int, State> next, State current, char dir)
        {
            int nextblank = 0, newstate = current.state, value;

            switch (dir)
            {
                case 'U': nextblank = current.blank + 4; break;
                case 'D': nextblank = current.blank - 4; break;
                case 'L': nextblank = current.blank + 1; break;
                case 'R': nextblank = current.blank - 1; break;
                default: throw new ArgumentException();
            }

            value = (newstate & (3 << nextblank)) >> nextblank;
            newstate &= ~(3 << nextblank);
            newstate |= value << current.blank;

            if (visited.Contains(newstate))
                return;

            if (next.ContainsKey(newstate))
                next[newstate].AddChecksum(current.checksums.Select(it => (it * 243 + dir) % 100000007));
            else
                next.Add(newstate, new State(newstate, nextblank, current.checksums.Select(it => (it * 243 + dir) % 100000007)));
        }

        private void AddNextStates(HashSet<int> visited, Dictionary<int, State> next, State current)
        {
            if (current.blank % 4 > 0)
                AddNextState(visited, next, current, 'R');
            if (current.blank % 4 < 3)
                AddNextState(visited, next, current, 'L');
            if (current.blank / 4 > 0)
                AddNextState(visited, next, current, 'D');
            if (current.blank / 4 < 3)
                AddNextState(visited, next, current, 'U');
        }

        protected override string Action()
        {
            var visited = new HashSet<int>();
            Dictionary<int, State> current, next;
            int finishState = 0;

            current = new Dictionary<int, State>();
            current.Add(Parse(source).state, Parse(source));
            visited.Add(Parse(source).state);
            finishState = Parse(target).state;

            while (true)
            {
                next = new Dictionary<int, State>();
                foreach (var state in current.Values)
                    AddNextStates(visited, next, state);
                current = next;
                foreach (var state in current.Values)
                    visited.Add(state.state);
                if (visited.Contains(finishState))
                    break;
            }

            return current[finishState].checksums.Sum().ToString();
        }
    }
}