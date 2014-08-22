using ProjectEuler.Common;
using ProjectEuler.Common.Miscellany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// Let Sm = (x1, x2, ... , xm) be the m-tuple of positive real numbers with
    /// x1 + x2 + ... + xm = m for which Pm = x1 * x2^2 * ... * xm^m is maximised.
    ///
    /// For example, it can be verified that [P10] = 4112 ([ ] is the integer part
    /// function).
    ///
    /// Find Σ[Pm] for 2 <= m <= 15.
    /// </summary>
    internal class Problem190 : Problem
    {
        public Problem190() : base(190) { }

        protected override string Action()
        {
            long sum = 0;

            /**
             * Lagrange multiplier:
             * http://en.wikipedia.org/wiki/Lagrange_multiplier
             *
             * g(x1,x2,...,xm) = x1 + x2 + ... + xm - m = 0
             * calculate maximized f(x1,x2,...,xm) = x1 * x2^2 * ... * xm^m
             * grad g = (1,1,...,1)
             * grad f = (f/x1, 2f/x2, ..., mf/xm)
             * 1 - a*f/x1 = 0
             * 1 - 2a*f/x2 = 0
             * ...
             * 1 - ma*f/xm = 0
             * so x1:x2:...:xm = 1:2:...:m, xn = 2n/(m+1)
             */
            for (int i = 2; i <= 15; i++)
            {
                double x1 = 2.0 / (i + 1), tmp = x1;

                for (int x = 2; x <= i; x++)
                    tmp *= Math.Pow(x1 * x, x);
                sum += (long)Math.Floor(tmp);
            }

            return sum.ToString();
        }
    }

    /// <summary>
    /// A particular school offers cash rewards to children with good attendance and
    /// punctuality. If they are absent for three consecutive days or late on more than
    /// one occasion then they forfeit their prize.
    ///
    /// During an n-day period a trinary string is formed for each child consisting of
    /// L's (late), O's (on time), and A's (absent).
    ///
    /// Although there are eighty-one trinary strings for a 4-day period that can be
    /// formed, exactly forty-three strings would lead to a prize:
    ///
    /// OOOO OOOA OOOL OOAO OOAA OOAL OOLO OOLA OAOO OAOA OAOL OAAO OAAL OALO OALA OLOO
    /// OLOA OLAO OLAA AOOO AOOA AOOL AOAO AOAA AOAL AOLO AOLA AAOO AAOA AAOL AALO AALA
    /// ALOO ALOA ALAO ALAA LOOO LOOA LOAO LOAA LAOO LAOA LAAO
    ///
    /// How many "prize" strings exist over a 30-day period?
    /// </summary>
    internal class Problem191 : Problem
    {
        private const int days = 30;

        public Problem191() : base(191) { }

        private string GetIDX(int leadingA, int hasO, int length)
        {
            return leadingA + "," + hasO + "," + length;
        }

        private long Count(Dictionary<string, long> dict, int leadingA, int hasO, int length)
        {
            var key = GetIDX(leadingA, hasO, length);
            long ret = 0;

            if (length == 0)
                return 1;
            if (dict.ContainsKey(key))
                return dict[key];

            if (leadingA < 2)
                ret += Count(dict, leadingA + 1, hasO, length - 1);
            if (hasO == 0)
                ret += Count(dict, 0, 1, length - 1);
            ret += Count(dict, 0, hasO, length - 1);

            dict.Add(key, ret);

            return ret;
        }

        protected override string Action()
        {
            var dict = new Dictionary<string, long>();

            return Count(dict, 0, 0, days).ToString();
        }
    }

    /// <summary>
    /// Let x be a real number.
    /// A best approximation to x for the denominator bound d is a rational number r/s
    /// in reduced form, with s <= d, such that any rational number which is closer to
    /// x than r/s has a denominator larger than d:
    ///
    /// |p/q-x| < |r/s-x| => q > d
    ///
    /// For example, the best approximation to sqrt(13) for the denominator bound 20 is
    /// 18/5 and the best approximation to sqrt(13) for the denominator bound 30 is
    /// 101/28.
    ///
    /// Find the sum of all denominators of the best approximations to sqrt(n) for the
    /// denominator bound 10^12, where n is not a perfect square and 1 < n <= 100000.
    /// </summary>
    internal class Problem192 : Problem
    {
        private const long denominator = 1000000000000;
        private const int upper = 100000;

        public Problem192() : base(192) { }

        private long GetDenominator(int number)
        {
            var cf = ContinuedFraction.CreateFromSquareRoot(number);
            List<Fraction> candidates = new List<Fraction>();
            Fraction current, prev, tmp;

            /**
             * http://en.wikipedia.org/wiki/Continued_fraction#Semiconvergents
             */
            prev = cf.GetFraction(1);
            current = cf.GetFraction(2);
            for (int l = 3; ; l++)
            {
                tmp = cf.GetFraction(l);

                if (tmp.Denominator > denominator)
                    break;
                prev = current;
                current = tmp;
            }

            tmp = current * current;
            if (tmp > number)
                candidates.Add(tmp - number);
            else
                candidates.Add(number - tmp);
            // compare the differences between n and square of the fraction number
            for (int n = 1; n <= (denominator - prev.Denominator) / current.Denominator; n++)
            {
                tmp = new Fraction(prev.Numerator + n * current.Numerator, prev.Denominator + n * current.Denominator);

                tmp *= tmp;
                if (tmp > number)
                    candidates.Add(tmp - number);
                else
                    candidates.Add(number - tmp);
            }

            int idx = 0;
            for (int i = 1; i < candidates.Count; i++)
            {
                if (candidates[i] < candidates[idx])
                    idx = i;
            }

            if (idx == 0)
                return (long)current.Denominator;
            else
                return (long)(prev.Denominator + idx * current.Denominator);
        }

        protected override string Action()
        {
            long sum = 0;

            for (int i = 2; i <= upper; i++)
            {
                if (Misc.IsPerfectSquare(i))
                    continue;
                sum += GetDenominator(i);
            }

            return sum.ToString();
        }
    }

    /// <summary>
    /// A positive integer n is called squarefree, if no square of a prime divides n,
    /// thus 1, 2, 3, 5, 6, 7, 10, 11 are squarefree, but not 4, 8, 9, 12.
    ///
    /// How many squarefree numbers are there below 2^50?
    /// </summary>
    internal class Problem193 : Problem
    {
        private const long supper = 1 << 25;
        private const long upper = supper * supper;

        public Problem193() : base(193) { }

        private long Count(List<long> factors, long mul, int fpos, int idx, int nFactors)
        {
            long counter = 0;

            if (idx == nFactors)
                return (upper - 1) / (mul * mul);

            for (int i = fpos; i < factors.Count; i++)
            {
                var tmp = mul * factors[i];
                long ret = 0;

                if (tmp >= supper)
                    break;
                ret = Count(factors, tmp, i + 1, idx + 1, nFactors);
                if (ret == 0)
                    break;
                counter += ret;
            }

            return counter;
        }

        protected override string Action()
        {
            var p = new Prime(1 << 25);
            long counter = upper - 1;
            List<long> factors;

            /**
             * remove all numbers contains square prime factor, using inclusion-exclusion principle:
             * http://en.wikipedia.org/wiki/Inclusion-exclusion_principle
             */
            p.GenerateAll();
            factors = p.Nums.Select(it => (long)it).ToList();

            for (int n = 1; ; n++)
            {
                var tmp = Count(factors, 1, 0, 0, n);

                if (tmp == 0)
                    break;
                if (n % 2 == 1)
                    counter -= tmp;
                else
                    counter += tmp;
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// Consider graphs built with the units A and B, where the units are glued along
    /// the vertical edges as in the graph.
    ///
    /// A: X---X   B: X---X
    ///    |\ /|      |\ /|
    ///    | X |      | X |
    ///    | | |      | | |
    ///    | X |      | X |
    ///    | | |      | | |
    ///    | X |      | X |
    ///    |/ \|      |/ \|
    ///    X---X      X   X
    ///
    /// A configuration of type (a,b,c) is a graph thus built of a units A and b units
    /// B, where the graph's vertices are coloured using up to c colours, so that no
    /// two adjacent vertices have the same colour.
    ///
    /// 1---2---3---4---1
    /// |\ /|\ /|\ /|\ /|
    /// | 4 | 1 | 1 | 2 |
    /// | | | | | | | | |
    /// | 1 | 4 | 3 | 1 |
    /// | | | | | | | | |
    /// | 3 | 2 | 1 | 1 |
    /// |/ \|/ \|/ \|/ \|
    /// 2---1   4   2---4
    ///
    /// The compound graph above is an example of a configuration of type (2,2,6), in
    /// fact of type (2,2,c) for all c >= 4.
    ///
    /// Let N(a,b,c) be the number of configurations of type (a,b,c).
    /// For example, N(1,0,3) = 24, N(0,2,4) = 92928 and N(2,2,3) = 20736.
    ///
    /// Find the last 8 digits of N(25,75,1984).
    /// </summary>
    internal class Problem194 : Problem
    {
        private const int divisor = 100000000;
        private const int nColors = 1984;
        private const int nA = 25;
        private const int nB = 75;

        public Problem194() : base(194) { }

        private void Count(long[] counter, HashSet<int>[] array, int[] colors, int totalColors, int idx)
        {
            if (idx == colors.Length)
            {
                counter[colors.Distinct().Count()]++;
                return;
            }

            for (int c = 0; c < totalColors; c++)
            {
                bool valid = true;

                foreach (var v in array[idx])
                {
                    if (v < idx && colors[v] == c)
                    {
                        valid = false;
                        break;
                    }
                }
                if (!valid)
                    continue;

                colors[idx] = c;
                Count(counter, array, colors, totalColors, idx + 1);
            }
        }

        private long GetPattern(HashSet<int>[] array)
        {
            var counter = new long[8];
            var totalColors = nColors > 7 ? 7 : nColors;
            BigInteger ret = 0;

            // the color of vertex 0 and 1 at left side is fixed
            Count(counter, array, new int[7] { 0, 1, 0, 0, 0, 0, 0 }, totalColors, 2);
            // Combinations of other vertices' colors
            for (int i = 2; i < counter.Length; i++)
            {
                if (nColors > totalColors)
                    ret += counter[i] / Probability.CountCombinations(new BigInteger(totalColors - 2), i - 2)
                        * Probability.CountCombinations(new BigInteger(nColors - 2), i - 2);
                else
                    ret += counter[i] / Probability.CountCombinations(new BigInteger(totalColors - 2), i - 2);
            }

            return (long)(ret % divisor);
        }

        private long GetDictForA()
        {
            var array = new HashSet<int>[7];

            array[0] = new HashSet<int>(new int[] { 1, 2, 5 });
            array[1] = new HashSet<int>(new int[] { 0, 4, 6 });
            array[2] = new HashSet<int>(new int[] { 0, 3, 5 });
            array[3] = new HashSet<int>(new int[] { 2, 4 });
            array[4] = new HashSet<int>(new int[] { 1, 3, 6 });
            array[5] = new HashSet<int>(new int[] { 0, 2, 6 });
            array[6] = new HashSet<int>(new int[] { 1, 4, 5 });

            return GetPattern(array);
        }

        private long GetDictForB()
        {
            var array = new HashSet<int>[7];

            array[0] = new HashSet<int>(new int[] { 1, 2, 5 });
            array[1] = new HashSet<int>(new int[] { 0, 4 });
            array[2] = new HashSet<int>(new int[] { 0, 3, 5 });
            array[3] = new HashSet<int>(new int[] { 2, 4 });
            array[4] = new HashSet<int>(new int[] { 1, 3, 6 });
            array[5] = new HashSet<int>(new int[] { 0, 2, 6 });
            array[6] = new HashSet<int>(new int[] { 4, 5 });

            return GetPattern(array);
        }

        protected override string Action()
        {
            BigInteger ret;
            var a = GetDictForA();
            var b = GetDictForB();

            // Combinations of Sequences of AABB....
            ret = Probability.CountCombinations(nA + nB, new BigInteger(nA));
            // Permutations of the first two vertices at left
            ret *= nColors * (nColors - 1);
            // Permutations of Pattern A pieces
            ret *= BigInteger.ModPow(a, nA, divisor);
            // Permutations of Pattern B pieces
            ret *= BigInteger.ModPow(b, nB, divisor);

            return (ret % divisor).ToString();
        }
    }

    /// <summary>
    /// Let's call an integer sided triangle with exactly one angle of 60 degrees a
    /// 60-degree triangle.
    /// Let r be the radius of the inscribed circle of such a 60-degree triangle.
    ///
    /// There are 1234 60-degree triangles for which r <= 100.
    /// Let T(n) be the number of 60-degree triangles for which r <= n, so
    /// T(100) = 1234, T(1000) = 22767, and T(10000) = 359912.
    ///
    /// Find T(1053779).
    /// </summary>
    internal class Problem195 : Problem
    {
        private const long radius = 1053779;

        public Problem195() : base(195) { }

        protected override string Action()
        {
            var counter = 0;
            double r;

            /**
             * http://pythag.net/node10.html
             *
             * when GCD(m, n) = 1 and (m - n) mod 3 != 0
             * case 1:
             *   u = 2mn+m^2, v = 2mn+n^2, w = m^2+n^2+mn
             *   sin60*uv = r*(u+v+w)
             *   sin60 * m(2n+m)n(2m+n) = r*(2m^2+2n^2+5mn) = r*(2m+n)(2n+m)
             *   r = mn*sin60, min(r) = m*sin60
             * case 2:
             *   u = m^2-n^2, v = 2mn+m^2, w = m^2+n^2+mn
             *   sin60*(m-n)(m+n)m(2n+m) = r*(3m^2+3mn) = 3r*m(m+n)
             *   r = sin60/3*(m-n)(m+2n), min(r) = sin60(m-2/3)
             */
            for (int m = 2; m <= Math.Floor(radius / Math.Sqrt(3) * 2) + 1; m++)
            {
                for (int n = 1; n < m; n++)
                {
                    if (Factor.GetCommonFactor(m, n) != 1 || (m - n) % 3 == 0)
                        continue;

                    r = Math.Sqrt(3) / 2 * m * n;
                    if (r >= radius)
                        break;
                    counter += (int)Math.Floor(radius / r);
                }
                for (int n = 1; n <= m / 4; n++)
                {
                    if (Factor.GetCommonFactor(m, n) != 1 || (m - n) % 3 == 0)
                        continue;

                    r = Math.Sqrt(3) / 6 * (m - n) * (m + 2 * n);
                    if (r >= radius)
                        break;
                    counter += (int)Math.Floor(radius / r);
                }
                for (int n = m - 1; n > m / 4; n--)
                {
                    if (Factor.GetCommonFactor(m, n) != 1 || (m - n) % 3 == 0)
                        continue;

                    r = Math.Sqrt(3) / 6 * (m - n) * (m + 2 * n);
                    if (r >= radius)
                        break;
                    counter += (int)Math.Floor(radius / r);
                }
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// Build a triangle from all positive integers in the following way:
    ///
    ///  1
    ///  2  3
    ///  4  5  6
    ///  7  8  9 10
    /// 11 12 13 14 15
    /// 16 17 18 19 20 21
    /// 22 23 24 25 26 27 28
    /// 29 30 31 32 33 34 35 36
    /// 37 38 39 40 41 42 43 44 45
    /// 46 47 48 49 50 51 52 53 54 55
    /// 56 57 58 59 60 61 62 63 64 65 66
    /// . . .
    ///
    /// Each positive integer has up to eight neighbours in the triangle.
    ///
    /// A set of three primes is called a prime triplet if one of the three primes has
    /// the other two as neighbours in the triangle.
    ///
    /// For example, in the second row, the prime numbers 2 and 3 are elements of some
    /// prime triplet.
    ///
    /// If row 8 is considered, it contains two primes which are elements of some prime
    /// triplet, i.e. 29 and 31.
    /// If row 9 is considered, it contains only one prime which is an element of some
    /// prime triplet: 37.
    ///
    /// Define S(n) as the sum of the primes in row n which are elements of any prime
    /// triplet.
    /// Then S(8)=60 and S(9)=37.
    ///
    /// You are given that S(10000)=950007619.
    ///
    /// Find S(5678027) + S(7208785).
    /// </summary>
    internal class Problem196 : Problem
    {
        private static long[] lines = new long[] { 5678027, 7208785 };

        public Problem196() : base(196) { }

        private void AddPrimeTriplet(BitVector vector, long offset, HashSet<long> pts, long l)
        {
            long start = l * (l - 1) / 2 + 1;
            long[] tmp = new long[4];

            /**
             * number at row x, column y, N(1, 1) = 1
             * N(x, y) = x * (x - 1) / 2 + y
             * number among N(x, y) is n+1, n-1, n+x, n+x+1, n+x-1, n-x+2, n-x+1, n-x
             * n must be a prime
             * when x is odd, n+x+1, n+x-1, n-x+1 must contains 2 primes
             * when x is even, n+x, n-x+2, n-x must contains 2 primes
             */
            for (long n = start % 2 == 0 ? start + 1 : start; n < start + l; n += 2)
            {
                int pcounter = 0;

                if (vector[(int)(n - offset)])
                    continue;
                tmp[0] = n;
                if (l % 2 == 0)
                {
                    if (!vector[(int)(n + l - offset)])
                        tmp[++pcounter] = n + l;
                    if (n - start < l - 1 && !vector[(int)(n - l + 2 - offset)])
                        tmp[++pcounter] = n - l + 2;
                    if (n - start > 0 && !vector[(int)(n - l - offset)])
                        tmp[++pcounter] = n - l;
                }
                else
                {
                    if (!vector[(int)(n + l + 1 - offset)])
                        tmp[++pcounter] = n + l + 1;
                    if (n - start > 0 && !vector[(int)(n + l - 1 - offset)])
                        tmp[++pcounter] = n + l - 1;
                    if (n - start < l - 1 && !vector[(int)(n - l + 1 - offset)])
                        tmp[++pcounter] = n - l + 1;
                }

                if (pcounter >= 2)
                {
                    for (int i = 0; i <= pcounter; i++)
                        pts.Add(tmp[i]);
                }
            }
        }

        private long Calculate(Prime prime, long l)
        {
            var pts = new HashSet<long>();
            var vector = new BitVector((int)l * 5);
            long begin = (l - 2) * (l - 3) / 2 + 1, end = begin + l * 5;
            long start = l * (l - 1) / 2 + 1;

            foreach (var p in prime)
            {
                for (long i = begin % p == 0 ? begin / p * p : (begin / p + 1) * p; i < end; i += p)
                    vector.Set((int)(i - begin));
            }

            AddPrimeTriplet(vector, begin, pts, l - 1);
            AddPrimeTriplet(vector, begin, pts, l);
            AddPrimeTriplet(vector, begin, pts, l + 1);

            return pts.Where(it => it >= start && it < start + l).Sum();
        }

        protected override string Action()
        {
            var p = new Prime((int)lines.Max());
            long sum = 0;

            p.GenerateAll();
            foreach (var line in lines)
                sum += Calculate(p, line);

            return sum.ToString();
        }
    }

    /// <summary>
    /// Given is the function f(x) = floor(2^(30.403243784 - x^2)) * 10^-9,
    /// the sequence un is defined by u(0) = -1 and u(n+1) = f(u(n)).
    ///
    /// Find u(n) + u(n+1) for n = 10^12.
    /// Give your answer with 9 digits after the decimal point.
    /// </summary>
    internal class Problem197 : Problem
    {
        private static double precision = Math.Pow(10, -11);

        public Problem197() : base(197) { }

        private double f(double x)
        {
            return Math.Floor(Math.Pow(2, (30.403243784 - x * x))) * 0.000000001;
        }

        protected override string Action()
        {
            double prevx1, prevx2, x1 = 0, x2 = -1;

            // by observation u(n) converge into two value bi-number
            while (true)
            {
                prevx1 = x1;
                prevx2 = x2;
                x1 = f(x2);
                x2 = f(x1);

                if (Math.Abs(prevx1 - x1) < precision && Math.Abs(prevx2 - x2) < precision)
                    break;
            }

            return (x1 + x2).ToString().Substring(0, 11);
        }
    }

    /// <summary>
    /// A best approximation to a real number x for the denominator bound d is a
    /// rational number r/s (in reduced form) with s <= d, so that any rational number
    /// p/q which is closer to x than r/s has q > d.
    ///
    /// Usually the best approximation to a real number is uniquely determined for all
    /// denominator bounds. However, there are some exceptions, e.g. 9/40 has the two
    /// best approximations 1/4 and 1/5 for the denominator bound 6. We shall call a
    /// real number x ambiguous, if there is at least one denominator bound for which x
    /// possesses two best approximations. Clearly, an ambiguous number is necessarily
    /// rational.
    ///
    /// How many ambiguous numbers x = p/q, 0 < x < 1/100, are there whose denominator
    /// q does not exceed 10^8?
    /// </summary>
    internal class Problem198 : Problem
    {
        private const int upper = 100000000;

        public Problem198() : base(198) { }

        protected override string Action()
        {
            /**
             * http://www.cut-the-knot.org/blue/Stern.shtml
             */
            var stack = new Stack<long[]>();
            long counter = 0, midnum, midden, mednum, medden;

            /**
             * only search between 0 and 1/50
             * recursion will cause stack overflow, use a stack instead
             */
            stack.Push(new long[] { 0, 1, 1, 50 });
            while (stack.Count != 0)
            {
                var pair = stack.Pop();

                // Get the middle of the two number
                midnum = pair[0] * pair[3] + pair[1] * pair[2];
                midden = 2 * pair[1] * pair[3];
                if (midden > upper)
                    continue;
                if (100 * midnum < midden)
                    counter++;

                // Get the mediant number in Stern-Brocot tree
                mednum = pair[0] + pair[2];
                medden = pair[1] + pair[3];
                stack.Push(new long[] { pair[0], pair[1], mednum, medden });
                stack.Push(new long[] { mednum, medden, pair[2], pair[3] });
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// Three circles of equal radius are placed inside a larger circle such that each
    /// pair of circles is tangent to one another and the inner circles do not overlap.
    /// There are four uncovered "gaps" which are to be filled iteratively with more
    /// tangent circles.
    ///
    /// At each iteration, a maximally sized circle is placed in each gap, which
    /// creates more gaps for the next iteration. After 3 iterations (pictured), there
    /// are 108 gaps and the fraction of the area which is not covered by circles is
    /// 0.06790342, rounded to eight decimal places.
    ///
    /// What fraction of the area is not covered by circles after 10 iterations?
    /// Give your answer rounded to eight decimal places using the format x.xxxxxxxx .
    /// </summary>
    internal class Problem199 : Problem
    {
        private const int nIterations = 10;

        public Problem199() : base(199) { }

        private double CalculateCircle(double r1, double r2, double r3, int iter)
        {
            double area = 0;

            if (iter > nIterations)
                return area;

            /**
             * http://en.wikipedia.org/wiki/Descartes'_theorem
             */
            double k1 = r1 == 1 ? -1 / r1 : 1 / r1, k2 = 1 / r2, k3 = 1 / r3;
            double k = k1 + k2 + k3 + 2 * Math.Sqrt(k1 * k2 + k1 * k3 + k2 * k3), r = 1 / Math.Abs(k);

            area = r * r;
            area += CalculateCircle(r1, r2, r, iter + 1);
            area += CalculateCircle(r1, r3, r, iter + 1);
            area += CalculateCircle(r2, r3, r, iter + 1);

            return area;
        }

        protected override string Action()
        {
            /**
             * Assume radius of outer circle is 1
             * 1 = 3k-2sqrt(3k^2) = k(3-2sqrt(3), k = 1/(3-2sqrt(3))
             */
            double k0 = Math.Abs(1.0 / (3.0 - Math.Sqrt(3) * 2)), r0 = 1 / k0;
            double ret = 1 - 3 * r0 * r0;

            ret -= CalculateCircle(1, r0, r0, 1) * 3;
            ret -= CalculateCircle(r0, r0, r0, 1);

            return Math.Round(ret, 8).ToString();
        }
    }
}