using ProjectEuler.Common;
using ProjectEuler.Common.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private static string target = " BRBBRBRRBRBBRBR";

        public Problem244() : base(244) { }

        private class State
        {
            public static State Parse(string text)
            {
                State ret = new State(0);

                for (int i = 0; i < 16; i++)
                {
                    switch (text[i])
                    {
                        case 'R': ret.state |= (1 << i); break;
                        case ' ': ret.state |= (i << 16); break;
                        case 'B': break;
                    }
                }

                return ret;
            }

            public State Left;
            public State Right;
            public State Up;
            public State Down;
            public int state;

            public State(int state)
            {
                this.state = state;
            }

            private State NextMove(HashSet<int> visited, Dictionary<int, State> next, int blank, char dir)
            {
                int nextblank = 0, nextstate = state, value = 0;

                switch (dir)
                {
                    case 'U': nextblank = blank + 4; break;
                    case 'D': nextblank = blank - 4; break;
                    case 'L': nextblank = blank + 1; break;
                    case 'R': nextblank = blank - 1; break;
                }
                value = (state & (1 << nextblank));
                nextstate = (state & 0xFFFF);
                nextstate &= ~(1 << blank);
                nextstate &= ~(1 << nextblank);
                if (value != 0)
                    nextstate |= (1 << blank);
                nextstate |= (nextblank << 16);

                if (visited.Contains(nextstate))
                    return null;
                if (!next.ContainsKey(nextstate))
                    next.Add(nextstate, new State(nextstate));

                return next[nextstate];
            }

            public void NextMoves(HashSet<int> visited, Dictionary<int, State> next)
            {
                int blank = (state >> 16);

                if (blank % 4 > 0)
                    Right = NextMove(visited, next, blank, 'R');
                if (blank % 4 < 3)
                    Left = NextMove(visited, next, blank, 'L');
                if (blank / 4 > 0)
                    Down = NextMove(visited, next, blank, 'D');
                if (blank / 4 < 3)
                    Up = NextMove(visited, next, blank, 'U');
            }
        }

        private long GetCheckSum(State current, State end, long checksum)
        {
            long sum = 0;

            if (current.state == end.state)
                return checksum;

            if (current.Left != null)
                sum += GetCheckSum(current.Left, end, (checksum * 243 + 'L') % 100000007);
            if (current.Right != null)
                sum += GetCheckSum(current.Right, end, (checksum * 243 + 'R') % 100000007);
            if (current.Up != null)
                sum += GetCheckSum(current.Up, end, (checksum * 243 + 'U') % 100000007);
            if (current.Down != null)
                sum += GetCheckSum(current.Down, end, (checksum * 243 + 'D') % 100000007);

            return sum;
        }

        protected override string Action()
        {
            var visited = new HashSet<int>();
            Dictionary<int, State> current, next;
            State start, end;

            current = new Dictionary<int, State>();
            start = State.Parse(source);
            end = State.Parse(target);
            current.Add(start.state, start);
            visited.Add(start.state);

            while (true)
            {
                next = new Dictionary<int, State>();
                foreach (var state in current.Values)
                    state.NextMoves(visited, next);
                current = next;
                foreach (var state in current.Values)
                    visited.Add(state.state);
                if (visited.Contains(end.state))
                    break;
            }

            return GetCheckSum(start, end, 0).ToString();
        }
    }

    /// <summary>
    /// We shall call a fraction that cannot be cancelled down a resilient fraction.
    /// Furthermore we shall define the resilience of a denominator, R(d), to be the
    /// ratio of its proper fractions that are resilient; for example, R(12) = 4⁄11.
    ///
    /// The resilience of a number d > 1 is then φ(d) / (d - 1), where φ is Euler's
    /// totient function.
    ///
    /// We further define the coresilience of a number n > 1 as C(n) = (n - φ(n))
    /// / (n - 1).
    ///
    /// The coresilience of a prime p is C(p) = 1 / (p - 1).
    ///
    /// Find the sum of all composite integers 1 < n < 2*10^11, for which C(n) is a
    /// unit fraction.
    /// </summary>
    internal class Problem245 : Problem
    {
        private static long upper = 2 * Misc.Pow(10, 11);

        public Problem245() : base(245) { }

        private List<long> GetDivisors(List<long> ps)
        {
            List<long> ds = new List<long>() { 1 }, tmp = new List<long>();
            long q = 1;

            foreach (var p in ps)
            {
                if (p != q)
                {
                    tmp.Clear();
                    tmp.AddRange(ds);
                }
                for (int i = 0; i < tmp.Count; i++)
                    tmp[i] *= p;
                ds.AddRange(tmp);
                q = p;
            }
            ds.Sort();

            return ds;
        }

        private long MultiPrime(Prime prime, long m, long phi, long q_min, long q_max)
        {
            long sum = 0, p_max = Math.Min(upper / m, m * m);
            long k_min = (m * q_max - 1) / ((m - phi) * q_max + phi) + 1;
            long k_max = Math.Min((m * p_max - 1) / ((m - phi) * p_max + phi), q_min - 1);

            if (k_max < k_min)
                return 0;

            for (long k = k_min; k <= k_max; k++)
            {
                if ((k * phi + 1) % (m - k * (m - phi)) != 0)
                    continue;

                long q = (phi * k + 1) / (m - (m - phi) * k);

                if (prime.IsPrime(q))
                    sum += m * q;
            }

            int id = BinarySearch.SearchRight(prime.Nums, (int)q_max);
            p_max = Misc.Sqrt(upper / m);
            while (true)
            {
                int p = prime.Nums[id++];

                if (p > p_max)
                    break;
                sum += MultiPrime(prime, m * p, phi * (p - 1), q_min, p);
            }

            return sum;
        }

        protected override string Action()
        {
            /**
             * Proof n has to be squarefree when C(n) is a unit fraction:
             * Let p be a prime dividing n with p^k the largest power dividing n.
             * C(n) a unit fraction gives (n-1)/(n-phi) = m, an integer.
             * p^(k-1) divides n and phi, so divides the denominator (and is the highest power of p to do so).
             * Thus m integral implies p^(k-1) divides the numerator also.
             * Since p^(k-1) divides n, it has to divide 1, so p^(k-1) = 1 and k = 1.
             * n must be made up from square-free prime factors.
             *
             * n is square-free and odd, c(n)=1/k, k must be even
             */
            long v = (long)Math.Pow(upper, 2.0 / 3), w = Misc.Sqrt(upper);
            long[] values = new long[w + 1];
            List<long>[] factors = new List<long>[w + 1];
            var prime = new Prime((int)v);
            long sum = 0;

            prime.GenerateAll();
            for (long n = 0; n <= w; n++)
            {
                values[n] = n * (n - 1) + 1;
                factors[n] = new List<long>();
            }
            for (int n = 2; n <= w; n += 3) // Sieve 3
            {
                for (; values[n] % 3 == 0; values[n] /= 3)
                    factors[n].Add(3);
            }
            for (int n = 3; n <= w; n++)
            {
                long p = values[n];

                if (p == 1)
                    continue;
                for (long m = n; m <= w; m += p)
                {
                    for (; values[m] % p == 0; values[m] /= p)
                        factors[m].Add(p);
                }
                for (long m = p + 1 - n; m <= w; m += p)
                {
                    for (; values[m] % p == 0; values[m] /= p)
                        factors[m].Add(p);
                }
            }

            // Semi Primes
            foreach (var p in prime.Nums.Skip(1))
            {
                if (p > w)
                    break;
                foreach (var d in GetDivisors(factors[p]))
                {
                    if (d > upper / p + p - 1)
                        break;
                    if (d <= 2 * p - 1)
                        continue;
                    if (prime.IsPrime(d - p + 1))
                        sum += (d - p + 1) * p;
                }
            }

            // Multi Primes
            long q_min_max = (long)Math.Pow(upper, 1.0 / 3);

            for (int mid = 1; prime.Nums[mid] < q_min_max; mid++)
            {
                int q_min = prime.Nums[mid];
                long q_max = Misc.Sqrt(upper / q_min);

                for (int qid = mid + 1; prime.Nums[qid] < q_max; qid++)
                {
                    int q = prime.Nums[qid];

                    sum += MultiPrime(prime, q_min * q, (q_min - 1) * (q - 1), q_min, q);
                }
            }

            return sum.ToString();
        }
    }

    /// <summary>
    /// A definition for an ellipse is:
    ///
    /// Given a circle c with centre M and radius r and a point G such that d(G,M) < r,
    /// the locus of the points that are equidistant from c and G form an ellipse.
    /// The construction of the points of the ellipse is shown below.
    ///
    /// Given are the points M(-2000,1500) and G(8000,1500).
    /// Given is also the circle c with centre M and radius 15000.
    /// The locus of the points that are equidistant from G and c form an ellipse e.
    /// From a point P outside e the two tangents t1 and t2 to the ellipse are drawn.
    /// Let the points where t1 and t2 touch the ellipse be R and S.
    ///
    /// For how many lattice points P is angle RPS greater than 45 degrees?
    /// </summary>
    internal class Problem246 : Problem
    {
        public Problem246() : base(246) { }

        private static double A = 7500, B = Math.Sqrt(5) * 2500;
        private static double A45 = Math.PI / 4;

        // compute angle of tangents given point x0, y0 outside
        private static double Angle(double x0, double y0)
        {
            // testing Angle(10000, 8000);
            // should be points {{7329.87, -1183.93}, {-2102.17, 5366.09}}
            checked
            {
                double d2 = ((B * x0 / A) * (B * x0 / A) + y0 * y0);
                double h = Math.Sqrt(d2 - B * B);
                double x1 = (B * B * x0 + A * h * y0) / d2;
                double x2 = (B * B * x0 - A * h * y0) / d2;
                double y1 = (B * B * y0 - h * B * B * x0 / A) / d2;
                double y2 = (B * B * y0 + h * B * B * x0 / A) / d2;

                double dx1 = x1 - x0, dy1 = y1 - y0;
                double dx2 = x2 - x0, dy2 = y2 - y0;

                // compute two angles
                double a1 = Math.Atan2(dy1, dx1);
                double a2 = Math.Atan2(dy2, dx2);

                // compute difference - rotate a2 to the x axis
                // and normalize to [-180,180)
                double ans = a1 - a2;
                while (ans < Math.PI)
                    ans += Math.PI * 2;
                while (ans >= Math.PI)
                    ans -= Math.PI * 2;

                ans += Math.PI; //[0,360)

                if (ans >= Math.PI)
                    ans = 2 * Math.PI - ans; // [0,180)

                // check cross product to see if we need ans or 180-ans
                if (dx1 * dy2 < dx2 * dy1)
                    ans = Math.PI - ans; // [0,90)

                return ans;
            }
        }

        // starting at x0, y0 a valid point, walk along x0 += dx until
        // good = false, then back in one then from x0 > 0, y0, walk up
        // and to left while inside = true until (and including) x0 = 0,
        // then total lattice pointsinside region and return count.
        // Assumes quadrant symmetric
        private static long Walk(long x0, long y0, long dx, Func<long, long, bool> good)
        {
            long total = 0;

            do
            {
                x0 += dx;
            } while (good(x0, y0));
            x0 -= dx; // start here
            // x0,y0 good, now walk counterclockwise on boundary

            long xstart = x0;
            while (x0 >= 0)
            {
                while (good(x0, y0))
                    y0++;
                y0--; // back to good point
                total += y0; // count part above x axis
                //Console.WriteLine("{0},{1}", x0, y0);
                x0--;
            }
            total -= y0; // remove y axis
            total *= 4; // 4 quadrants
            total += xstart * 2; // x axis minus 0,0
            total += y0 * 2; // y axis minus 0,0
            total += 1; // 0,0
            return total;
        }

        private static long Count()
        {
            // start where angle is already good
            long outside = Walk(7501, 0, 1,
               (x, y) => Angle(x, y) > A45	// 45 degree bound
               );

            // start inside ellipse, count points in ellipse
            long inside = Walk(5000, 0, 1,
               (x, y) => B * B * x * x + A * A * y * y <= A * A * B * B // inside or on ellipse
               );

            return outside - inside;
        }

        protected override string Action()
        {
            /**
             * 1) Find the equation for the ellipse. This is straight forward when we realize a point
             * on the ellipse is the mid distance between G and the circle.
             * (x-3000)^2 / 7500^2  +  (y-1500)^2 / (2500*sqrt(5))^2 = 1
             *
             * 2) Derivate and find the slope. This is not so straight forward and results in a kinda
             * long equation for y given the point P. From the equation we get four solutions, only two
             * of which are the tangent points. The correct solutions depend on the quadrant, where P
             * lies relative to the ellipse.
             *
             * 3) The set of lattice points P that result in valid angles form a shape
             * (bounded by -17450 < y < 20450 and -12440 < x < 18440). We count the number of points in this shape
             * (we also include the points in the ellipse) by walking the edge. A small increase in x
             * results in a small change in y and vice versa.
             *
             * 4) The final step is to count the number of points inside the ellipse and subtract the
             * result from the result above.
             *
             * It is important to take care of the edge cases, that is the points (-4500,1500) and (10500,1500).
             */

            return Count().ToString();
        }
    }

    /// <summary>
    /// Consider the region constrained by 1 <= x and 0 <= y <= 1/x.
    ///
    /// Let S1 be the largest square that can fit under the curve.
    /// Let S2 be the largest square that fits in the remaining area, and so on.
    /// Let the index of Sn be the pair (left, below) indicating the number of squares
    /// to the left of Sn and the number of squares below Sn.
    ///
    /// The diagram shows some such squares labelled by number.
    /// S2 has one square to its left and none below, so the index of S2 is (1,0).
    /// It can be seen that the index of S32 is (1,1) as is the index of S50.
    /// 50 is the largest n for which the index of Sn is (1,1).
    ///
    /// What is the largest n for which the index of Sn is (3,3)?
    /// </summary>
    internal class Problem247 : Problem
    {
        private const int left = 3;
        private const int below = 3;

        public Problem247() : base(247) { }

        private class HyperbolaZone
        {
            public double xOffset;
            public double yOffset;
            public double x;
            public double y;
            public double size;
            public int left;
            public int below;

            public HyperbolaZone(double xoff, double yoff, int left, int below)
            {
                var B = xoff - yoff;

                this.xOffset = xoff;
                this.yOffset = yoff;
                this.left = left;
                this.below = below;

                this.x = (B + Math.Sqrt(B * B + 4)) / 2;
                this.y = 1 / this.x;
                this.size = this.x - this.xOffset;
            }

            public IEnumerable<HyperbolaZone> Split()
            {
                yield return new HyperbolaZone(xOffset, y, left, below + 1);
                yield return new HyperbolaZone(x, yOffset, left + 1, below);
            }
        }

        private class ReverseDoubleComparer : IComparer<double>
        {
            public int Compare(double x, double y)
            {
                return x.CompareTo(y) * -1;
            }
        }

        protected override string Action()
        {
            /**
             * for a hyperbola y=1/x where x>=a and y>=b, calculate the point of square upper-right point(x,y)
             * the square split the area into two remaining zones where a1 = a, b1 = y and a2 = x, b2 = b
             *
             * x-a = y-b and y = 1/x
             * x^2 - ax = 1 - bx => x^2 - (a-b)x - 1 = 0
             * x = (a - b + sqrt((a-b)^2+4)) / 2
             *
             * also, there is C(n,2n) ways to get to index(n,n)
             */
            var queue = new PriorityQueue<HyperbolaZone, double>(VertexHelper.CreateSimpleHelper<HyperbolaZone>(), new ReverseDoubleComparer());
            var start = new HyperbolaZone(1, 0, 0, 0);
            int counter = 0, nTarget = 0, totalTarget = (int)Probability.CountCombinations(left + below, left);

            queue.Add(start, start.size);
            while (nTarget != totalTarget)
            {
                var zone = queue.ExtractMin().Key;

                counter++;
                if (zone.left == left && zone.below == below)
                    nTarget++;
                foreach (var tmp in zone.Split())
                    queue.Add(tmp, tmp.size);
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// The first number n for which φ(n)=13! is 6227180929.
    ///
    /// Find the 150,000th such number.
    /// </summary>
    internal class Problem248 : Problem
    {
        private const int index = 150000;
        private static long phi = Misc.Factorial(13);

        public Problem248() : base(248) { }

        private void Search(List<long> nums, List<long> primes, long n, long x, int id)
        {
            if (n == 1)
            {
                nums.Add(x);
                return;
            }
            if (id == primes.Count || n < primes[id] - 1)
                return;

            long p = primes[id];

            Search(nums, primes, n, x, id + 1);
            if (n % (p - 1) == 0)
            {
                n /= (p - 1);
                x *= p;
                Search(nums, primes, n, x, id + 1);
                while (n % p == 0)
                {
                    n /= p;
                    x *= p;
                    Search(nums, primes, n, x, id + 1);
                }
            }
        }

        protected override string Action()
        {
            /**
             * if n = p1^a1 * p2^a2 * ... * pn^an
             * phi(n) = p1^(a1-1)*(p1-1) * ... * pn^(an-1)*(pn-1) = 13!
             *
             * Find all p^a which p^(a-1)*(p-1) divides phi(n)
             * http://www.numbertheory.org/php/carmichael.html
             */
            var primes = new Prime((int)Misc.Sqrt(phi) + 1);
            var ret = new List<long>();
            var validp = new List<long>();

            primes.GenerateAll();
            foreach (var divisor in Factor.GetDivisors(primes, phi))
            {
                if (primes.IsPrime(divisor + 1))
                    validp.Add(divisor + 1);
            }
            validp = validp.Distinct().ToList();
            validp.Sort();
            Search(ret, validp, phi, 1, 0);
            ret.Sort();

            return ret[index - 1].ToString();
        }
    }

    /// <summary>
    /// Let S = {2, 3, 5, ..., 4999} be the set of prime numbers less than 5000.
    ///
    /// Find the number of subsets of S, the sum of whose elements is a prime number.
    /// Enter the rightmost 16 digits as your answer.
    /// </summary>
    internal class Problem249 : Problem
    {
        private static long modulo = Misc.Pow(10, 16);
        private const int upper = 5000;

        public Problem249() : base(249) { }

        protected override string Action()
        {
            var primes = new Prime(upper * upper);
            long[] current = new long[upper * upper];
            long counter = 0, sum = 0;

            primes.GenerateAll();
            foreach (var p in primes)
            {
                if (p >= 5000)
                    break;

                for (long tmp = sum; tmp >= 2; tmp--)
                {
                    current[tmp + p] += current[tmp];
                    current[tmp + p] %= modulo;
                }
                current[p]++;
                sum += p;
            }
            for (int i = 2; i <= sum; i++)
            {
                if (primes.IsPrime(i))
                {
                    counter += current[i];
                    counter %= modulo;
                }
            }

            return counter.ToString();
        }
    }
}