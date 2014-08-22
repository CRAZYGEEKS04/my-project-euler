using ProjectEuler.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// A game is played with three piles of stones and two players.
    /// At her turn, a player removes one or more stones from the piles. However, if
    /// she takes stones from more than one pile, she must remove the same number of
    /// stones from each of the selected piles.
    ///
    /// In other words, the player chooses some N>0 and removes:
    ///
    /// N stones from any single pile; or
    /// N stones from each of any two piles (2N total); or
    /// N stones from each of the three piles (3N total).
    /// The player taking the last stone(s) wins the game.
    /// A winning configuration is one where the first player can force a win.
    /// For example, (0,0,13), (0,11,11) and (5,5,5) are winning configurations because
    /// the first player can immediately remove all stones.
    ///
    /// A losing configuration is one where the second player can force a win, no
    /// matter what the first player does.
    /// For example, (0,1,2) and (1,3,3) are losing configurations: any legal move
    /// leaves a winning configuration for the second player.
    ///
    /// Consider all losing configurations (xi,yi,zi) where xi <= yi <= zi <= 100.
    /// We can verify that sum(xi+yi+zi) = 173895 for these.
    ///
    /// Find sum(xi+yi+zi) where (xi,yi,zi) ranges over the losing configurations with
    /// xi <= yi <= zi <= 1000.
    /// </summary>
    internal class Problem260 : Problem
    {
        private const int upper = 1000;

        public Problem260() : base(260) { }

        private int GetKey(int x, int y, int z)
        {
            return (x << 20) + (y << 10) + z;
        }

        private bool GetResult(BitVector wins, BitVector loses, int x, int y, int z)
        {
            int key = GetKey(x, y, z);
            bool ret = true;

            if (!wins[key] && !loses[key])
            {
                for (int n = 1; n <= z; n++)
                {
                    if (n <= x)
                    {
                        // Take n from x
                        ret = GetResult(wins, loses, x - n, y, z);
                        if (!ret)
                            break;
                        // Take n from x, y
                        ret = GetResult(wins, loses, x - n, y - n, z);
                        if (!ret)
                            break;
                        // Take n from x, z
                        if (z - n < y)
                            ret = GetResult(wins, loses, x - n, z - n, y);
                        else
                            ret = GetResult(wins, loses, x - n, y, z - n);
                        if (!ret)
                            break;
                        // Take n from x, y, z
                        ret = GetResult(wins, loses, x - n, y - n, z - n);
                        if (!ret)
                            break;
                    }

                    if (n <= y)
                    {
                        // Take n from y
                        if (y - n < x)
                            ret = GetResult(wins, loses, y - n, x, z);
                        else
                            ret = GetResult(wins, loses, x, y - n, z);
                        if (!ret)
                            break;
                        // Take n from y, z
                        if (z - n < x)
                            ret = GetResult(wins, loses, y - n, z - n, x);
                        else if (y - n < x)
                            ret = GetResult(wins, loses, y - n, x, z - n);
                        else
                            ret = GetResult(wins, loses, x, y - n, z - n);
                        if (!ret)
                            break;
                    }

                    // Take n from z
                    if (z - n < x)
                        ret = GetResult(wins, loses, z - n, x, y);
                    else if (z - n < y)
                        ret = GetResult(wins, loses, x, z - n, y);
                    else
                        ret = GetResult(wins, loses, x, y, z - n);
                    if (!ret)
                        break;
                }

                if (ret)
                    loses.Set(key);
                else
                    wins.Set(key);

                // if this is a losing configuration, add winning configuration
                if (ret)
                {
                    for (int n = 1; n <= upper - x; n++)
                    {
                        if (z + n <= upper)
                        {
                            // Add n for z
                            wins.Set(GetKey(x, y, z + n));
                            // Add n for y, z
                            wins.Set(GetKey(x, y + n, z + n));
                            // Add n for x, y, z
                            wins.Set(GetKey(x + n, y + n, z + n));
                        }
                        if (y + n <= upper)
                        {
                            // Add n for y
                            if (y + n > z)
                                wins.Set(GetKey(x, z, y + n));
                            else
                                wins.Set(GetKey(x, y + n, z));
                            // Add n for x, y
                            if (x + n > z)
                                wins.Set(GetKey(z, x + n, y + n));
                            else if (y + n > z)
                                wins.Set(GetKey(x + n, z, y + n));
                            else
                                wins.Set(GetKey(x + n, y + n, z));
                        }
                        // Add n for x
                        if (x + n > z)
                            wins.Set(GetKey(y, z, x + n));
                        else if (x + n > y)
                            wins.Set(GetKey(y, x + n, z));
                        else
                            wins.Set(GetKey(x + n, y, z));
                    }
                }
            }

            if (wins[key])
                return true;
            else
                return false;
        }

        protected override string Action()
        {
            BitVector wins = new BitVector(1 << 30), loses = new BitVector(1 << 30);
            long sum = 0;

            for (int z = 0; z <= upper; z++)
            {
                for (int y = 0; y <= z; y++)
                {
                    for (int x = 0; x <= y; x++)
                    {
                        if (!GetResult(wins, loses, x, y, z))
                            sum += (x + y + z);
                    }
                }
            }

            return sum.ToString();
        }
    }

    /// <summary>
    /// Let us call a positive integer k a square-pivot, if there is a pair of integers
    /// m > 0 and n >= k, such that the sum of the (m+1) consecutive squares up to k
    /// equals the sum of the m consecutive squares from (n+1) on:
    ///
    /// (k-m)^2 + ... + k^2 = (n+1)^2 + ... + (n+m)^2.
    /// Some small square-pivots are
    ///
    /// 4: 3^2 + 4^2 = 5^2
    /// 21: 20^2 + 21^2 = 29^2
    /// 24: 21^2 + 22^2 + 23^2 + 24^2 = 25^2 + 26^2 + 27^2
    /// 110: 108^2 + 109^2 + 110^2 = 133^2 + 134^2
    /// Find the sum of all distinct square-pivots <= 10^10.
    /// </summary>
    internal class Problem261 : Problem
    {
        private const long upper = 10000000000;

        public Problem261() : base(261) { }

        private bool Func(ref UInt64 sum, HashSet<UInt64> s, UInt64 m, UInt64 a, UInt64 b, UInt64 c)
        {
            UInt64 k = (4 * m + 2) * a - 2 * m * m - b, nextm = a + c - 1;

            if (k > upper)
                return a > m;
            if (s.Add(k))
                sum += k;
            c += k + a - m;
            if (a > m)
                Func(ref sum, s, nextm, c - 1, a, k + 1);

            return Func(ref sum, s, m, k, a, c);
        }

        protected override string Action()
        {
            /*
            Binary tree like structure from xsd's post
            + m=1 -    4,    5
            |         21,   29 => m= 8*-    (28,  22)
            |                               820, 862 => m=49#-   (861,    821)
            |                                                  165648, 167281
            |                                                      ......
            |                             27724,29398
            |                             ......
            |        120,  169 => m=49#-  (168,   121)
            |                            28441, 28681
            |                               ......
            |        697,  985 =>m=288%-  (984,    698)
            |                            969528, 970922
            |                               ......
            |         .....
            + m=2 -   12,   13
            |        110,  133 => m=24*-  (132,    111)
            |                             11772,  11991
            |       1080, 1321
            |         .....
            + m=3 -   .....
            */
            HashSet<UInt64> s = new HashSet<UInt64>();
            UInt64 sum = 0, m = 1;

            while (Func(ref sum, s, m, m, 0, 1))
                m++;

            return sum.ToString();
        }
    }

    internal class Problem262 : Problem
    {
        public Problem262() : base(262) { }
    }

    /// <summary>
    /// Consider the number 6. The divisors of 6 are: 1,2,3 and 6.
    /// Every number from 1 up to and including 6 can be written as a sum of distinct
    /// divisors of 6:
    /// 1=1, 2=2, 3=1+2, 4=1+3, 5=2+3, 6=6.
    /// A number n is called a practical number if every number from 1 up to and
    /// including n can be expressed as a sum of distinct divisors of n.
    ///
    /// A pair of consecutive prime numbers with a difference of six is called a sexy
    /// pair (since "sex" is the Latin word for "six"). The first sexy pair is (23,
    /// 29).
    ///
    /// We may occasionally find a triple-pair, which means three consecutive sexy
    /// prime pairs, such that the second member of each pair is the first member of
    /// the next pair.
    ///
    /// We shall call a number n such that :
    ///
    /// (n-9, n-3), (n-3,n+3), (n+3, n+9) form a triple-pair, and
    /// the numbers n-8, n-4, n, n+4 and n+8 are all practical,
    /// an engineers’ paradise.
    /// Find the sum of the first four engineers’ paradises.
    /// </summary>
    internal class Problem263 : Problem
    {
        private const int upper = 100000000;
        private const int required = 4;

        public Problem263() : base(263) { }

        private bool IsParadise(Prime p, long num)
        {
            return p.IsPrime(num - 9) && !p.IsPrime(num - 7) && !p.IsPrime(num - 5) && p.IsPrime(num - 3) && !p.IsPrime(num - 1)
                && !p.IsPrime(num + 1) && p.IsPrime(num + 3) && !p.IsPrime(num + 5) && !p.IsPrime(num + 7) && p.IsPrime(num + 9)
                && Factor.IsPracticalNumber(p, num - 8) && Factor.IsPracticalNumber(p, num - 4) && Factor.IsPracticalNumber(p, num)
                && Factor.IsPracticalNumber(p, num + 4) && Factor.IsPracticalNumber(p, num + 8);
        }

        protected override string Action()
        {
            var prime = new Prime(upper);
            long sum = 0;
            int ncounter = 0;

            /**
             * http://en.wikipedia.org/wiki/Practical_number
             * If n is a "paradise" number, n-4 and n+4 cannot be divisible by 5 or 7
             * (because n-9, n-3, n+3, and n+9 are prime).  Thus, to be practical,
             * n-4 and n+4 must be disible by 8.  n cannot be divisible by 3; it is
             * divisible by 4 (but not 8) and by 5.  So n = 20 + 40j
             *
             * n-8 and n+8 are also divisible by 4 but not 8; they must be divisible
             * either by 3 or by 7.  This leaves the following two possibilities:
             * the remainders when n is divided by
             * 3 and 7 are
             * 1 and 1
             * 2 and 6
             * respectively.
             * Combining that with n = 20 + 40j we get
             *
             * n = 20 + 1680k or n = -20 + 1680k
             */
            prime.GenerateAll();
            for (long num = 1680; ; num += 1680)
            {
                if (IsParadise(prime, num - 20))
                {
                    sum += num - 20;
                    if (++ncounter == required)
                        break;
                }
                if (IsParadise(prime, num + 20))
                {
                    sum += num + 20;
                    if (++ncounter == required)
                        break;
                }
            }

            return sum.ToString();
        }
    }

    internal class Problem264 : Problem
    {
        public Problem264() : base(264) { }
    }

    /// <summary>
    /// 2^N binary digits can be placed in a circle so that all the N-digit clockwise
    /// subsequences are distinct.
    ///
    /// For N=3, two such circular arrangements are possible, ignoring rotations:
    ///
    /// 00010111    00011101
    ///
    /// For the first arrangement, the 3-digit subsequences, in clockwise order, are:
    /// 000, 001, 010, 101, 011, 111, 110 and 100.
    ///
    /// Each circular arrangement can be encoded as a number by concatenating the
    /// binary digits starting with the subsequence of all zeros as the most
    /// significant bits and proceeding clockwise. The two arrangements for N=3 are
    /// thus represented as 23 and 29:
    ///
    /// 00010111 = 23
    /// 00011101 = 29
    /// Calling S(N) the sum of the unique numeric representations, we can see that
    /// S(3) = 23 + 29 = 52.
    ///
    /// Find S(5).
    /// </summary>
    internal class Problem265 : Problem
    {
        private const int nDigits = 5;
        private static int length = (int)Misc.Pow(2, nDigits);

        public Problem265() : base(265) { }

        private bool IsBinaryCircle(int[] numbers)
        {
            bool[] flags = new bool[length];
            int n = 0;

            for (int i = 0; i < nDigits; i++)
            {
                n <<= 1;
                n += numbers[i];
            }
            for (int i = nDigits; i < length; i++)
            {
                n <<= 1;
                n += numbers[i];
                n &= (length - 1);
                if (flags[n])
                    return false;
                flags[n] = true;
            }

            return true;
        }

        private int GetNumber(int[] numbers)
        {
            int ret = 0;

            foreach (var digit in numbers.Take(length))
            {
                ret <<= 1;
                ret += digit;
            }

            return ret;
        }

        protected override string Action()
        {
            int[] digits = new int[length + nDigits];
            int freeSlot = length - nDigits * 2;
            var numbers = new HashSet<int>();
            long sum = 0;

            /**
             * There must be a sequence of 1s and 0s of length 5, assuming there is
             * five leading 0s, loop through the position of five 1s.
             */
            foreach (var posArray in Itertools.Combinations(Itertools.Range(nDigits, nDigits + freeSlot - 1), freeSlot / 2))
            {
                for (int pos = nDigits; pos <= length - nDigits + 1; pos++)
                {
                    for (int i = nDigits; i < length; i++)
                        digits[i] = 1;
                    foreach (var pos1 in posArray)
                    {
                        if (pos1 < pos)
                            digits[pos1] = 0;
                        else
                            digits[pos1 + nDigits] = 0;
                    }
                    if (IsBinaryCircle(digits))
                        numbers.Add(GetNumber(digits));
                }
            }
            foreach (var num in numbers)
                sum += num;

            return sum.ToString();
        }
    }

    /// <summary>
    /// The divisors of 12 are: 1,2,3,4,6 and 12.
    /// The largest divisor of 12 that does not exceed the square root of 12 is 3.
    /// We shall call the largest divisor of an integer n that does not exceed the
    /// square root of n the pseudo square root (PSR) of n.
    /// It can be seen that PSR(3102)=47.
    ///
    /// Let p be the product of the primes below 190.
    /// Find PSR(p) mod 10^16.
    /// </summary>
    internal class Problem266 : Problem
    {
        private const int upper = 190;

        public Problem266() : base(266) { }

        private SortedSet<BigInteger> GeneratePossibleFactors(List<int> primes)
        {
            SortedSet<BigInteger> set = new SortedSet<BigInteger>() { 1 };

            foreach (var p in primes)
            {
                foreach (var f in set.ToList())
                    set.Add(f * p);
            }

            return set;
        }

        private BigInteger SearchPSR(List<BigInteger> set1, List<BigInteger> set2, BigInteger sqrt)
        {
            BigInteger n, max = 1;

            for (int p1 = 0, p2 = set2.Count - 1; p1 < set1.Count && p2 >= 0; )
            {
                n = set1[p1] * set2[p2];
                if (n > sqrt)
                {
                    p2--;
                }
                else
                {
                    p1++;
                    if (n > max)
                        max = n;
                }
            }

            return max;
        }

        protected override string Action()
        {
            var prime = new Prime(upper);
            BigInteger number = 1, sqrt, max = 1;
            SortedSet<BigInteger> set1, set2;

            prime.GenerateAll();
            foreach (var p in prime)
                number *= p;
            sqrt = Misc.Sqrt(number);
            set1 = GeneratePossibleFactors(prime.Nums.Take(prime.Nums.Count / 2).ToList());
            set2 = GeneratePossibleFactors(prime.Nums.Skip(prime.Nums.Count / 2).ToList());
            max = SearchPSR(set1.ToList(), set2.ToList(), sqrt);

            return (max % BigInteger.Pow(10, 16)).ToString();
        }
    }

    /// <summary>
    /// You are given a unique investment opportunity.
    ///
    /// Starting with £1 of capital, you can choose a fixed proportion, f, of your
    /// capital to bet on a fair coin toss repeatedly for 1000 tosses.
    ///
    /// Your return is double your bet for heads and you lose your bet for tails.
    ///
    /// For example, if f = 1/4, for the first toss you bet £0.25, and if heads comes
    /// up you win £0.5 and so then have £1.5. You then bet £0.375 and if the second
    /// toss is tails, you have £1.125.
    ///
    /// Choosing f to maximize your chances of having at least £1,000,000,000 after
    /// 1,000 flips, what is the chance that you become a billionaire?
    ///
    /// All computations are assumed to be exact (no rounding), but give your answer
    /// rounded to 12 digits behind the decimal point in the form 0.abcdefghijkl.
    /// </summary>
    internal class Problem267 : Problem
    {
        private const int steps = 1000;
        private const double fstep = 0.0001;

        public Problem267() : base(267) { }

        private int CalcualteN(double f)
        {
            double n = (9 - steps * Math.Log10(1 - f)) / (Math.Log10(1 + 2 * f) - Math.Log10(1 - f));

            return (int)Math.Ceiling(n);
        }

        private int CalculateMinimalN(double lower, double upper)
        {
            int n = steps, tmp;

            for (double f = lower + fstep; f < upper; f += fstep)
            {
                tmp = CalcualteN(f);
                if (tmp < n)
                    n = tmp;
            }

            return n;
        }

        protected override string Action()
        {
            /**
             * http://en.wikipedia.org/wiki/Binomial_options_pricing_model
             *
             * for n heads, the expected money will be 1 * (1+2f)^n * (1-f)^(1000-n)
             * to be a billionaire:
             * (1+2f)^n * (1-f)^(1000-n) >= 1000000000
             * n*lg(1+2f) + (1000-n)*lg(1-f) >= 9
             * n*(lg(1+2f) - lg(1-f)) >= 9 - 1000*lg(1-f)
             * n >= (9 - 1000*lg(1-f)) / (lg(1+2f) - lg(1-f))
             * We need minimal n for maximal probability to become a billionaire
             */
            BigInteger total, sum = 0;
            int n = CalculateMinimalN(0, 1);

            total = BigInteger.Pow(2, steps);
            for (BigInteger i = n; i <= steps; i++)
                sum += Probability.CountCombinations(steps, i);

            return Math.Round((double)sum / (double)total, 12).ToString();
        }
    }

    /// <summary>
    /// It can be verified that there are 23 positive integers less than 1000 that are
    /// divisible by at least four distinct primes less than 100.
    ///
    /// Find how many positive integers less than 10^16 are divisible by at least four
    /// distinct primes less than 100.
    /// </summary>
    internal class Problem268 : Problem
    {
        private static long upper = Misc.Pow(10, 16);

        public Problem268() : base(268) { }

        private void Calculate(List<int> primes, long[] counter, long factor, int nPrimeFactors, int id)
        {
            if (factor >= upper)
                return;

            counter[nPrimeFactors] += (upper - 1) / factor;
            for (int i = id; i < primes.Count; i++)
                Calculate(primes, counter, factor * primes[i], nPrimeFactors + 1, i + 1);
        }

        protected override string Action()
        {
            var prime = new Prime(100);
            long sum = 0;
            long[] multi, counter;

            prime.GenerateAll();
            multi = new long[prime.Nums.Count + 1];
            counter = new long[prime.Nums.Count + 1];
            /**
            * for any number divisible by a,b,c,d,e. it's counted by C(5,4) = 5 times for four prime factors,
            * so need to be subtracted C(5,4)-1 = 4 times.
            * for any number divisible by a,b,c,d,e,f. it's counted for C(6,4) - (C(5,4)-1)*C(6,5) = -9 times,
            * so need to be plused 10 times. Calculate multi time array for n factors
            */
            multi[4] = 1;
            for (int n = 5; n <= prime.Nums.Count; n++)
            {
                long tmp = 0;

                for (int i = 4; i < n; i++)
                    tmp += multi[i] * Probability.CountCombinations(n, i);
                multi[n] = 1 - tmp;
            }
            for (int i = 0; i < prime.Nums.Count; i++)
                Calculate(prime.Nums, counter, prime.Nums[i], 1, i + 1);
            for (int i = 0; i < multi.Length; i++)
                sum += multi[i] * counter[i];

            return sum.ToString();
        }
    }

    /// <summary>
    /// A root or zero of a polynomial P(x) is a solution to the equation P(x) = 0.
    /// Define Pn as the polynomial whose coefficients are the digits of n.
    /// For example, P5703(x) = 5*x^3 + 7*x^2 + 3.
    ///
    /// We can see that:
    /// Pn(0) is the last digit of n,
    /// Pn(1) is the sum of the digits of n,
    /// Pn(10) is n itself.
    ///
    /// Define Z(k) as the number of positive integers, n, not exceeding k for which
    /// the polynomial Pn has at least one integer root.
    ///
    /// It can be verified that Z(100000) is 14696.
    ///
    /// What is Z(10^16)?
    /// </summary>
    internal class Problem269 : Problem
    {
        private static int length = 16;

        /// <summary>
        /// class for remember equation value when x=-9,...,0
        /// </summary>
        private class Polynomial : IEquatable<Polynomial>
        {
            public Dictionary<int, int> Values;
            public string Key;

            public Polynomial(Dictionary<int, int> values)
            {
                Values = values;
                Key = string.Join("|", values.OrderBy(it => it.Key).Select(it => it.Key + "," + it.Value));
            }

            public Polynomial AppendDigit(int d)
            {
                var ret = new Dictionary<int, int>();
                int tmp;

                foreach (var pair in Values)
                {
                    /**
                     * It is sufficient to ignore x,p(x) pair where |p(x)|>=9 (except for digits 0,1)
                     * because appending digits can never get the polynomial to zero.
                     */
                    tmp = pair.Key * pair.Value + d;
                    if (pair.Key <= -2 && Math.Abs(pair.Value) >= 9)
                        continue;
                    ret.Add(pair.Key, tmp);
                }

                return new Polynomial(ret);
            }

            public bool HasRoot()
            {
                return Values.Values.Any(it => it == 0);
            }

            public override string ToString()
            {
                return Key;
            }

            public bool Equals(Polynomial other)
            {
                bool ret = (Key == other.Key);

                return ret;
            }

            public override int GetHashCode()
            {
                return Key.GetHashCode();
            }
        }

        public Problem269() : base(269) { }

        private Dictionary<Polynomial, long> AppendDigit(Dictionary<Polynomial, long> dict)
        {
            var ret = new Dictionary<Polynomial, long>();

            foreach (var pair in dict)
            {
                for (int d = 0; d < 10; d++)
                {
                    var poly = pair.Key.AppendDigit(d);

                    if (ret.ContainsKey(poly))
                        ret[poly] += pair.Value;
                    else
                        ret.Add(poly, pair.Value);
                }
            }

            return ret;
        }

        protected override string Action()
        {
            var dict = new Dictionary<Polynomial, long>();
            long counter = 0;

            /**
             * http://en.wikipedia.org/wiki/Rational_root_theorem
             * The integer root must be a minus divisor of the last digit.
             * Also number ending with 0 have integer root of 0.
             * So root must be from 0 to -9
             */
            // Create polynomial for const value 0
            var tmp = new Dictionary<int, int>();
            for (int i = -9; i <= 0; i++)
                tmp.Add(i, 0);
            dict.Add(new Polynomial(tmp), 1);
            for (int i = 0; i < length; i++)
                dict = AppendDigit(dict);

            foreach (var pair in dict)
            {
                if (pair.Key.HasRoot())
                    counter += pair.Value;
            }

            return counter.ToString();
        }
    }
}