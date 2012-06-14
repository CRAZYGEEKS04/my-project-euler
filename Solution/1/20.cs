using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using ProjectEuler.Common;
using ProjectEuler.Common.Miscellany;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// Let r be the remainder when (a - 1)^n + (a + 1)^n is divided by a^2.
    ///
    /// For example, if a = 7 and n = 3, then r = 42: 6^3 + 8^3 = 728 = 42 mod 49. And
    /// as n varies, so too will r, but for a = 7 it turns out that r(max) = 42.
    ///
    /// For 3 <= a <= 1000, find Σ(r(max)).
    /// </summary>
    internal class Problem120 : Problem
    {
        private const int upper = 1000;

        public Problem120() : base(120) { }

        protected override string Action()
        {
            var counter = 0;

            /**
             * (a-1)^n + (a+1)^n = 2*(a^n + C(n-2,n)a^(n-2) + C(n-4,n)a^(n-4) + ... )
             * n is even: 2*(a^n + C(n-2,n)a^(n-2) + ... + C(2,n)a^2 + 1) mod a^2 = 2 mod a^2 = 2
             * n is odd, n-1 is even:  2*a*(a^(n-1) + C(n-2,n)a^(n-3) + .. + C(3,n)a^2 + C(1,n)*1) mod a^2
             *   = 2*a*n mod a^2 = (2*n mod a) * a
             */
            for (int a = 3; a <= upper; a++)
            {
                if (a % 2 == 0)
                    counter += (a - 2) * a;
                else
                    counter += (a - 1) * a;
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// A bag contains one red disc and one blue disc. In a game of chance a player
    /// takes a disc at random and its colour is noted. After each turn the disc is
    /// returned to the bag, an extra red disc is added, and another disc is taken at
    /// random.
    ///
    /// The player pays £1 to play and wins if they have taken more blue discs than red
    /// discs at the end of the game.
    ///
    /// If the game is played for four turns, the probability of a player winning is
    /// exactly 11/120, and so the maximum prize fund the banker should allocate for
    /// winning in this game would be £10 before they would expect to incur a loss.
    /// Note that any payout will be a whole number of pounds and also includes the
    /// original £1 paid to play the game, so in the example given the player actually
    /// wins £9.
    ///
    /// Find the maximum prize fund that should be allocated to a single game in which
    /// fifteen turns are played.
    /// </summary>
    internal class Problem121 : Problem
    {
        private const int turns = 15;

        public Problem121() : base(121) { }

        protected override string Action()
        {
            /**
             * Change to get a blue disc is 1/2, 1/3, ... 1/(n+1)
             */
            Fraction chanceToWin = 0;
            var r = new HashSet<int>(Itertools.Range(2, turns + 1));

            for (int nb = turns / 2 + 1; nb <= turns; nb++)
            {
                foreach (var b in Itertools.Combinations(Itertools.Range(2, turns + 1), nb))
                {
                    Fraction tmp = 1;

                    r.ExceptWith(b);
                    foreach (var denominator in b)
                        tmp *= new Fraction(1, denominator);
                    foreach (var denominator in r)
                        tmp *= new Fraction(denominator - 1, denominator);
                    r.UnionWith(b);

                    chanceToWin += tmp;
                }
            }

            return (chanceToWin.Denominator / chanceToWin.Numerator).ToString();
        }
    }

    /// <summary>
    /// The most naive way of computing n^15 requires fourteen multiplications:
    ///
    /// n * n * ... * n = n^15
    ///
    /// But using a "binary" method you can compute it in six multiplications:
    ///
    /// n * n = n^2
    /// n^2 * n^2 = n^4
    /// n^4 * n^4 = n^8
    /// n^8 * n^4 = n^12
    /// n^12 * n^2 = n^14
    /// n^14 * n = n^15
    ///
    /// However it is yet possible to compute it in only five multiplications:
    ///
    /// n * n = n^2
    /// n^2 * n = n^3
    /// n^3 * n^3 = n^6
    /// n^6 * n^6 = n^12
    /// n^12 * n^3 = n^15
    ///
    /// We shall define m(k) to be the minimum number of multiplications to compute
    /// n^k; for example m(15) = 5.
    ///
    /// For 1 <= k <= 200, find Σ(m(k)).
    /// </summary>
    internal class Problem122 : Problem
    {
        private const int upper = 200;

        public Problem122() : base(122) { }

        private void Calculate(List<int> numbers, ref int min, int k)
        {
            if (numbers.Count >= min)
                return;
            if (numbers[numbers.Count - 1] * (1 << (min - numbers.Count)) < k)
                return;

            foreach (var pair in Itertools.CombinationsWithReplacement(numbers, 2))
            {
                int tmp = pair[0] + pair[1];

                if (tmp == k)
                {
                    min = numbers.Count;
                    return;
                }
                if (numbers[numbers.Count - 1] >= tmp)
                    continue;
                numbers.Add(tmp);
                Calculate(numbers, ref min, k);
                numbers.RemoveAt(numbers.Count - 1);
            }
        }

        private void Calculate(int[] m, int k)
        {
            // http://en.wikipedia.org/wiki/Addition-chain_exponentiation
            int min = m[k];

            Calculate(new List<int>(new int[] { 1 }), ref min, k);
            m[k] = min;
        }

        protected override string Action()
        {
            var m = new int[upper + 1];

            m[0] = 0;
            m[1] = 0;

            // http://en.wikipedia.org/wiki/Exponentiation_by_squaring
            for (int k = 2; k <= upper; k++)
            {
                int tmp = k, counter = 0;

                while (tmp > 1)
                {
                    if (tmp % 2 == 1)
                        counter++;
                    counter++;
                    tmp /= 2;
                }
                m[k] = counter;
            }

            for (int k = 2; k <= upper; k++)
                Calculate(m, k);

            return m.Sum().ToString();
        }
    }

    /// <summary>
    /// Let pn be the nth prime: 2, 3, 5, 7, 11, ..., and let r be the remainder when
    /// (pn - 1)^n + (pn + 1)^n is divided by pn^2.
    ///
    /// For example, when n = 3, p3 = 5, and 4^3 + 6^3 = 280 = 5 mod 25.
    ///
    /// The least value of n for which the remainder first exceeds 10^9 is 7037.
    ///
    /// Find the least value of n for which the remainder first exceeds 10^10.
    /// </summary>
    internal class Problem123 : Problem
    {
        private long upper = 10000000000L;

        public Problem123() : base(123) { }

        protected override string Action()
        {
            var p = new Prime((int)Misc.Sqrt(upper) * 50);

            p.GenerateAll();
            /**
             * (a-1)^n + (a+1)^n = 2*(a^n + C(n-2,n)a^(n-2) + C(n-4,n)a^(n-4) + ... )
             * n is even: 2*(a^n + C(n-2,n)a^(n-2) + ... + C(2,n)a^2 + 1) mod a^2 = 2 mod a^2 = 2, should be skipped
             * n is odd, n-1 is even:  2*a*(a^(n-1) + C(n-2,n)a^(n-3) + .. + C(3,n)a^2 + C(1,n)*1) mod a^2
             *   = 2*a*n mod a^2 = (2*n mod a) * a
             */
            int start = BinarySearch.SearchRight(p.Nums, (int)Misc.Sqrt(upper)) + 1;

            for (int n = start % 2 == 1 ? start : start + 1; n < p.Nums.Count; n += 2)
            {
                long tmp = 2 * n;

                tmp %= p.Nums[n - 1];
                tmp *= p.Nums[n - 1];

                if (tmp > upper)
                    return n.ToString();
            }

            return null;
        }
    }

    /// <summary>
    /// The radical of n, rad(n), is the product of distinct prime factors of n. For
    /// example, 504 = 2^3 * 3^2 * 7, so rad(504) = 2 * 3 * 7 = 42.
    ///
    /// If we calculate rad(n) for 1 <= n <= 10, then sort them on rad(n), and sorting
    /// on n if the radical values are equal, we get:
    ///
    /// Unsorted        Sorted
    /// n  rad(n)       n  rad(n)  k
    /// 1      1        1      1   1
    /// 2      2        2      2   2
    /// 3      3        4      2   3
    /// 4      2        8      2   4
    /// 5      5        3      3   5
    /// 6      6        9      3   6
    /// 7      7        5      5   7
    /// 8      2        6      6   8
    /// 9      3        7      7   9
    ///10     10       10     10  10
    ///
    /// Let E(k) be the kth element in the sorted n column; for example, E(4) = 8 and
    /// E(6) = 9.
    ///
    /// If rad(n) is sorted for 1 <= n <= 100000, find E(10000).
    /// </summary>
    internal class Problem124 : Problem
    {
        private const int upper = 100000;
        private const int index = 10000;

        public Problem124() : base(124) { }

        protected override string Action()
        {
            var l = new List<Tuple<int, int>>();
            var p = new Prime(upper);

            p.GenerateAll();
            for (int i = 1; i <= upper; i++)
                l.Add(new Tuple<int, int>(i, Factor.GetRadical(p, i)));

            l = l.OrderBy(it => it.Item2).ThenBy(it => it.Item1).ToList();

            return l[index - 1].Item1.ToString();
        }
    }

    /// <summary>
    /// The palindromic number 595 is interesting because it can be written as the sum
    /// of consecutive squares: 6^2 + 7^2 + 8^2 + 9^2 + 10^2 + 11^2 + 12^2.
    ///
    /// There are exactly eleven palindromes below one-thousand that can be written as
    /// consecutive square sums, and the sum of these palindromes is 4164. Note that
    /// 1 = 0^2 + 1^2 has not been included as this problem is concerned with the
    /// squares of positive integers.
    ///
    /// Find the sum of all the numbers less than 10^8 that are both palindromic and
    /// can be written as the sum of consecutive squares.
    /// </summary>
    internal class Problem125 : Problem
    {
        private const int upper = 100000000;

        public Problem125() : base(125) { }

        protected override string Action()
        {
            BigInteger ret = 0;
            var numbers = new HashSet<int>();
            var squares = new List<int>();

            for (int i = 1; i <= (int)Misc.Sqrt(upper); i++)
                squares.Add(i * i);

            for (int l = 2; l < squares.Count; l++)
            {
                int sum = squares.Take(l).Sum();

                if (sum >= upper)
                    break;
                if (Misc.IsPalindromic(sum.ToString()))
                    numbers.Add(sum);
                for (int start = 0; start < squares.Count; start++)
                {
                    sum -= squares[start];
                    sum += squares[start + l];

                    if (sum >= upper)
                        break;
                    if (Misc.IsPalindromic(sum.ToString()))
                        numbers.Add(sum);
                }
            }

            foreach (var n in numbers)
                ret += n;

            return ret.ToString();
        }
    }

    /// <summary>
    /// The minimum number of cubes to cover every visible face on a cuboid measuring
    /// 3 x 2 x 1 is twenty-two.
    ///
    /// If we then add a second layer to this solid it would require forty-six cubes to
    /// cover every visible face, the third layer would require seventy-eight cubes,
    /// and the fourth layer would require one-hundred and eighteen cubes to cover
    /// every visible face.
    ///
    /// However, the first layer on a cuboid measuring 5 x 1 x 1 also requires
    /// twenty-two cubes; similarly the first layer on cuboids measuring 5 x 3 x 1,
    /// 7 x 2 x 1, and 11 x 1 x 1 all contain forty-six cubes.
    ///
    /// We shall define C(n) to represent the number of cuboids that contain n cubes in
    /// one of its layers. So C(22) = 2, C(46) = 4, C(78) = 5, and C(118) = 8.
    ///
    /// It turns out that 154 is the least value of n for which C(n) = 10.
    ///
    /// Find the least value of n for which C(n) = 1000.
    /// </summary>
    internal class Problem126 : Problem
    {
        private const int maxcubes = 20000;
        private const int value = 1000;

        public Problem126() : base(126) { }

        private void Cover(Dictionary<int, int> C, int a, int b, int c)
        {
            /**
             * First will add a*b+b*c+a*c
             * Second will add a*b+b*c+a*c+4*(a+b+c)
             * Third will add a*b+b*c+a*c + 8*(a+b+c) + 8 corners
             * ..
             * Nth will add a*b+b*c+a*c+(n-1)(a+b+c)*4 + (8+16+..+4n) corners
             */
            int tmp = (a * b + b * c + c * a) * 2;

            for (int n = 1; tmp <= maxcubes; n++)
            {
                if (C.ContainsKey(tmp))
                    C[tmp]++;
                else
                    C[tmp] = 1;
                tmp += 4 * (a + b + c) + 8 * (n - 1);
            }
        }

        protected override string Action()
        {
            var dict = new Dictionary<int, int>();
            int ret = int.MaxValue;

            for (int a = 1; a <= (int)Misc.Sqrt(maxcubes / 6); a++)
            {
                Cover(dict, a, a, a);

                for (int b = a + 1; ; b++)
                {
                    int tmp = 2 * (a * a + a * b + b * a);

                    if (tmp >= maxcubes)
                        break;
                    Cover(dict, a, a, b);
                    Cover(dict, a, b, b);

                    tmp = 2 * (a * b + b * b + b * a);

                    for (int c = b + 1; ; c++)
                    {
                        tmp += (a + b) * 2;

                        if (tmp >= maxcubes)
                            break;
                        Cover(dict, a, b, c);
                    }
                }
            }

            foreach (var p in dict)
            {
                if (p.Value == value && p.Key < ret)
                    ret = p.Key;
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// The radical of n, rad(n), is the product of distinct prime factors of n. For
    /// example, 504 = 2^3 * 3^2 * 7, so rad(504) = 2 * 3 * 7 = 42.
    ///
    /// We shall define the triplet of positive integers (a, b, c) to be an abc-hit if:
    /// 1.GCD(a, b) = GCD(a, c) = GCD(b, c) = 1
    /// 2.a < b
    /// 3.a + b = c
    /// 4.rad(abc) < c
    ///
    /// For example, (5, 27, 32) is an abc-hit, because:
    /// 1.GCD(5, 27) = GCD(5, 32) = GCD(27, 32) = 1
    /// 2.5 < 27
    /// 3.5 + 27 = 32
    /// 4.rad(4320) = 30 < 32
    ///
    /// It turns out that abc-hits are quite rare and there are only thirty-one
    /// abc-hits for c < 1000, with Σ(c) = 12523.
    ///
    /// Find Σ(c) for c < 120000.
    /// </summary>
    internal class Problem127 : Problem
    {
        private const int upper = 120000;

        public Problem127() : base(127) { }

        protected override string Action()
        {
            var list = new List<KeyValuePair<int, int>>();
            var dict = new Dictionary<int, int>();
            var p = new Prime(upper);
            long counter = 0;

            p.GenerateAll();

            for (int i = 1; i < upper; i++)
            {
                var rad = Factor.GetRadical(p, i);

                list.Add(new KeyValuePair<int, int>(i, rad));
                dict.Add(i, rad);
            }
            // without this optimization, program will run for days
            list = list.OrderBy(it => it.Value).ToList();

            for (int c = 4; c < upper; c++)
            {
                var tmp = new HashSet<int>();
                int maxradab = (c - 1) / dict[c];

                foreach (var pair in list)
                {
                    int a = pair.Key, rada = pair.Value;

                    if (tmp.Contains(a) || tmp.Contains(c - a) || a > c - 2)
                        continue;

                    tmp.Add(a);
                    tmp.Add(c - a);
                    if (Factor.GetCommonFactor(c, a) != 1 || Factor.GetCommonFactor(c, c - a) != 1
                        || Factor.GetCommonFactor(c - a, a) != 1)
                        continue;
                    // assume rad(a) is less than rad(b)
                    if (dict[a] * dict[a] > maxradab)
                        break;
                    if (dict[a] * dict[c - a] <= maxradab)
                        counter += c;
                }
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// A hexagonal tile with number 1 is surrounded by a ring of six hexagonal tiles,
    /// starting at "12 o'clock" and numbering the tiles 2 to 7 in an anti-clockwise
    /// direction.
    ///
    /// New rings are added in the same fashion, with the next rings being numbered 8
    /// to 19, 20 to 37, 38 to 61, and so on. The diagram below shows the first three
    /// rings.
    ///
    /// By finding the difference between tile n and each its six neighbours we shall
    /// define PD(n) to be the number of those differences which are prime.
    ///
    /// For example, working clockwise around tile 8 the differences are 12, 29, 11, 6,
    /// 1, and 13. So PD(8) = 3.
    ///
    /// In the same way, the differences around tile 17 are 1, 17, 16, 1, 11, and 10,
    /// hence PD(17) = 2.
    ///
    /// It can be shown that the maximum value of PD(n) is 3.
    ///
    /// If all of the tiles for which PD(n) = 3 are listed in ascending order to form
    /// a sequence, the 10th tile would be 271.
    ///
    /// Find the 2000th tile in this sequence.
    /// </summary>
    internal class Problem128 : Problem
    {
        private const int index = 2000;

        public Problem128() : base(128) { }

        private bool Check(Prime p, int l, int n)
        {
            /**
             * first  : 6l, 6l+1, 1, 6(l-1), 6l-1, 6l+6(l+1)-1
             * check  : 6l+1, 6l-1, 12l+5
             */
            if (n == 0)
                return p.Contains(6 * l + 1) && p.Contains(12 * l + 5);
            /**
             * last   : 6l+5, 6l+5+1, 6l-1, 6l-1+6(l-1), 6l, 1
             * check  : 6l+5, 6l-1, 12l-7
             */
            if (n == 6 * l - 1)
                return p.Contains(6 * l + 5) && p.Contains(12 * l - 7);
            /**
             * corner : 6l+n/l-1, 6l+n/l, 6l+n/l+1, 1, 6(l-1)+n/l, 1
             * check  : 6l+c-1, 6l+c, 6l+c+1, 6(l-1)+c
             * two even number and two odd number, impossible
             */
            /**
             * side   : 6l+n/l, 6l+n/l+1, 1, 6(l-1)+n/l, 6(l-1)+n/l+1, 1
             * check  : 6l+c, 6l+c+1, 6l+c-6, 6l+c-5
             * two even number and two odd number, impossible
             */
            return false;
        }

        protected override string Action()
        {
            var p = new Prime(index * index);
            var ret = new List<long>(new long[] { 1, 2 });
            long start = 8;

            p.GenerateAll();

            // skip tier 0 and 1
            for (int l = 2; l <= p.Upper / 12; start += l * 6, l++)
            {
                // Check 6l-1
                if (!p.Contains(6 * l - 1))
                    continue;

                if (Check(p, l, 0))
                    ret.Add(start);
                if (Check(p, l, 6 * l - 1))
                    ret.Add(start + 6 * l - 1);

                if (ret.Count >= index)
                    return ret[index - 1].ToString();
            }

            return null;
        }
    }

    /// <summary>
    /// A number consisting entirely of ones is called a repunit. We shall define R(k)
    /// to be a repunit of length k; for example, R(6) = 111111.
    ///
    /// Given that n is a positive integer and GCD(n, 10) = 1, it can be shown that
    /// there always exists a value, k, for which R(k) is divisible by n, and let A(n)
    /// be the least such value of k; for example, A(7) = 6 and A(41) = 5.
    ///
    /// The least value of n for which A(n) first exceeds ten is 17.
    ///
    /// Find the least value of n for which A(n) first exceeds one-million.
    /// </summary>
    internal class Problem129 : Problem
    {
        private const int upper = 1000000;

        public Problem129() : base(129) { }

        protected override string Action()
        {
            /**
             * A(n) <= n because if A(n) > n, 1,11,111,...,1(n) must contains same mod n value.
             * Assume R(a) mod n = R(b) mod n, and a>b, R(a)-R(b) mod n = 0, and 1111....100..00 mod n = 0.
             * Since GCD(n,10)=1, the leading 1111..1 is divisable by n. and A(n) < n
             */
            for (int n = upper % 2 == 0 ? upper + 1 : upper; n < int.MaxValue; n += 2)
            {
                if (n % 5 == 0)
                    continue;
                if (RepUnit.GetA(n) > upper)
                    return n.ToString();
            }

            return null;
        }
    }
}