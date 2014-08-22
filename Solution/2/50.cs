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
    /// Find the number of non-empty subsets of {1^1, 2^2, 3^3, ..., 250250^250250},
    /// the sum of whose elements is divisible by 250. Enter the rightmost 16 digits
    /// as your answer.
    /// </summary>
    internal class Problem250 : Problem
    {
        private const int upper = 250250;
        private const int divisor = 250;
        private static long modulo = Misc.Pow(10, 16);

        public Problem250() : base(250) { }

        private long Count(int[] values)
        {
            /**
             * n^n > (n-1)^(n-1) + ... + 2^2 + 1^1, using DP
             */
            long[] counter = new long[divisor];

            for (int n = 1; n <= upper; n++)
            {
                long[] tmp = (long[])counter.Clone();

                for (int i = 0; i < divisor; i++)
                {
                    int nv = (i + values[n]) % divisor;

                    tmp[nv] += counter[i];
                    tmp[nv] %= modulo;
                }
                tmp[values[n]]++;
                counter = tmp;
            }

            return counter[0];
        }

        protected override string Action()
        {
            var mod = new Modulo(divisor);
            int[] values = new int[upper + 1];

            for (int i = 1; i <= upper; i++)
                values[i] = (int)mod.Pow(i, i);

            return Count(values).ToString();
        }
    }

    /// <summary>
    /// A triplet of positive integers (a,b,c) is called a Cardano Triplet if it
    /// satisfies the condition:
    /// sqrt3(a + b * sqrt(c)) + sqrt3(a - b * sqrt(c)) = 1
    ///
    /// For example, (2,1,5) is a Cardano Triplet.
    ///
    /// There exist 149 Cardano Triplets for which a + b + c <= 1000.
    ///
    /// Find how many Cardano Triplets exist such that a + b + c <= 110,000,000.
    /// </summary>
    internal class Problem251 : Problem
    {
        private const int upper = 110000000;

        public Problem251() : base(251) { }

        protected override string Action()
        {
            /**
             * a+b*sqrt(c) = 1-a+b*sqrt(c) - 3x + 3x^2 where x = sqrt3(a-b*sqrt(c))
             * 3x^2 - 3x - 2a + 1 = 0
             * x = (3 +- sqrt(9 + 24a - 12)) / 6
             * c=((2*a-1)^3+27*a^2)/(27*b^2)
             * so a must be 2 mod 3, assume a = 3k-1
             * b^2*c = k^2*(8*k-3)
             */
            int maxk = (upper + 1) / 3;
            var sfc = new int[maxk + 1];
            var p = new Prime(maxk);
            var counter = 0;
            long a, b, c, tmpb, tmpc;
            double optb, optc;

            p.GenerateAll();
            for (int f = 2; f <= Misc.Sqrt(maxk * 8 - 3); f++)
            {
                int square = f * f;

                for (int j = square; j <= maxk * 8 - 3; j += square)
                {
                    if (j % 8 == 5)
                        sfc[(j + 3) / 8] = f;
                }
            }

            for (int k = 1; k <= maxk; k++)
            {
                long maxc;

                a = 3 * k - 1;
                // Get optimal b and c by derivation
                optb = Math.Exp(Math.Log(2.0 * k * k * (8 * k - 3)) / 3);
                optc = optb / 2;

                if (a + optb + optc > upper)
                    break;

                b = k;
                c = 8 * k - 3;
                if (sfc[k] != 0)
                {
                    c = c / sfc[k] / sfc[k];
                    b *= sfc[k];
                }

                var factors = Factor.GetDivisors(p, b);
                factors.Sort();

                maxc = upper / c;
                foreach (long f in factors)
                {
                    if (f * f > maxc)
                        break;

                    tmpb = b / f;
                    tmpc = c * f * f;
                    if (a + tmpb + tmpc <= upper)
                        counter++;
                }
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// Given a set of points on a plane, we define a convex hole to be a convex
    /// polygon having as vertices any of the given points and not containing any of
    /// the given points in its interior (in addition to the vertices, other given
    /// points may lie on the perimeter of the polygon).
    ///
    /// As an example, the image below shows a set of twenty points and a few such
    /// convex holes. The convex hole shown as a red heptagon has an area equal to
    /// 1049694.5 square units, which is the highest possible area for a convex hole on
    /// the given set of points.
    ///
    /// For our example, we used the first 20 points (T(2k-1), T(2k)), for k = 1,2,…,20,
    /// produced with the pseudo-random number generator:
    ///
    /// S0 = 290797
    /// S(n+1) = S(n)^2 mod 50515093
    /// Tn = (Sn mod 2000) - 1000
    /// i.e. (527, 144), (-488, 732), (-454, -947), …
    ///
    /// What is the maximum area for a convex hole on the set containing the first 500
    /// points in the pseudo-random sequence?
    /// Specify your answer including one digit after the decimal point.
    /// </summary>
    internal class Problem252 : Problem
    {
        private const int NPoints = 500;

        public Problem252() : base(252) { }

        private IEnumerable<int> GetNumbers()
        {
            long s = 290797;

            while (true)
            {
                s = s * s % 50515093;
                yield return (int)(s % 2000 - 1000);
            }
        }

        protected override string Action()
        {
            var list = GetNumbers().Take(NPoints * 2).ToList();
            var ch = new ConvexHoles();

            return string.Format("{0:F1}", ch.Solve(list) / 2.0);
        }
    }

    /// <summary>
    /// A small child has a "number caterpillar" consisting of forty jigsaw pieces,
    /// each with one number on it, which, when connected together in a line, reveal
    /// the numbers 1 to 40 in order.
    ///
    /// Every night, the child's father has to pick up the pieces of the caterpillar
    /// that have been scattered across the play room. He picks up the pieces at random
    /// and places them in the correct order.
    /// As the caterpillar is built up in this way, it forms distinct segments that
    /// gradually merge together.
    /// The number of segments starts at zero (no pieces placed), generally increases
    /// up to about eleven or twelve, then tends to drop again before finishing at a
    /// single segment (all pieces placed).
    ///
    /// For example:
    ///
    /// Piece Placed	Segments So Far
    /// 12					1
    /// 4					2
    /// 29					3
    /// 6					4
    /// 34					5
    /// 5					4
    /// 35					4
    /// …					…
    ///
    /// Let M be the maximum number of segments encountered during a random tidy-up of
    /// the caterpillar.
    /// For a caterpillar of ten pieces, the number of possibilities for each M is
    ///
    /// M	Possibilities
    /// 1	512
    /// 2	250912
    /// 3	1815264
    /// 4	1418112
    /// 5	144000
    /// so the most likely value of M is 3 and the average value is
    /// 385643/113400 = 3.400732, rounded to six decimal places.
    ///
    /// The most likely value of M for a forty-piece caterpillar is 11; but what is the
    /// average value of M?
    ///
    /// Give your answer rounded to six decimal places.
    /// </summary>
    internal class Problem253 : Problem
    {
        private const int length = 40;

        public Problem253() : base(253) { }

        private BigInteger Get(Dictionary<string, BigInteger> dict, int a, int b, int c)
        {
            BigInteger n;

            if (a <= 0 || b <= 0 || b > c)
                return 0;
            if (a == 1 && b == 1)
                return 1;

            string key = string.Join(",", a, b, c);

            if (dict.ContainsKey(key))
                return dict[key];

            n = b * (Get(dict, a - 1, b - 1, c) + Get(dict, a - 1, b + 1, c) + 2 * Get(dict, a - 1, b, c));
            dict[key] = n;

            return n;
        }

        protected override string Action()
        {
            var dict = new Dictionary<string, BigInteger>();
            BigInteger num = 0, dem = 1;

            for (int i = 1; i <= length; i++)
            {
                num += i * (Get(dict, length, 1, i) - Get(dict, length, 1, i - 1));
                dem *= i;
            }

            return string.Format("{0:F6}", (double)num / (double)dem);
        }
    }

    /// <summary>
    /// Define f(n) as the sum of the factorials of the digits of n. For example,
    /// f(342) = 3! + 4! + 2! = 32.
    ///
    /// Define sf(n) as the sum of the digits of f(n). So sf(342) = 3 + 2 = 5.
    ///
    /// Define g(i) to be the smallest positive integer n such that sf(n) = i. Though
    /// sf(342) is 5, sf(25) is also 5, and it can be verified that g(5) is 25.
    ///
    /// Define sg(i) as the sum of the digits of g(i). So sg(5) = 2 + 5 = 7.
    ///
    /// Further, it can be verified that g(20) is 267 and sum(sg(i)) for 1 <= i <= 20
    /// is 156.
    ///
    /// What is sum( sg(i)) for 1 <= i <= 150?
    /// </summary>
    internal class Problem254 : Problem
    {
        public Problem254() : base(254) { }

        private int sf(long x)
        {
            int s = 0;

            while (x > 0)
            {
                s += (int)(x % 10);
                x /= 10;
            }

            return s;
        }

        protected override string Action()
        {
            long[,] A = new long[151, 10];
            long n, s, s1, s2, diff;
            long[] p10 = new long[20], v = new long[10], w = new long[10];
            int[] fact = new int[10];

            /**
             * It is true for all optimal solutions that the digit d appears at most d times in it if d!=9.
             * Otherwise delete (d+1) of them and use one (d+1) digit, this works because d!*(d+1)=(d+1)!
             * and we get a smaller number. It is not a proof for d=0, a similar proof works.
             * Every positive integer is representable in exactly one way as n=sum(j=1,9,v[j]*j!),
             * where v[j]<=j if j<9.
             * It is more important that it is also true that n=d*10^(L-1)-i, where i<1000000, 0<d<10,
             * and L<=18 for the problem.
             */
            fact[0] = 1;
            p10[0] = 1;
            for (int i = 1; i < 10; i++)
                fact[i] = fact[i - 1] * i;
            for (int i = 1; i < 20; i++)
                p10[i] = 10 * p10[i - 1];
            for (int i = 1; i <= 150; i++)
            {
                for (int j = 1; j < 10; j++)
                    A[i, j] = (j == 9) ? p10[18] : 0;
            }

            for (int L = 1; L <= 18; L++)
            {
                for (int d = 1; d < 10; d++)
                {
                    for (int i = 0; i < 1000000; i++)
                    {
                        n = d * p10[L - 1] - i;
                        if (n > 0)
                        {
                            s = sf(n);
                            if (s <= 150)
                            {
                                for (int j = 9; j >= 1; j--)
                                {
                                    v[j] = n / fact[j];
                                    n %= fact[j];
                                }
                                for (int j = 1; j < 10; j++) w[j] = A[s, j];

                                s1 = 0;
                                s2 = 0;
                                for (int j = 1; j < 10; j++)
                                {
                                    s1 += v[j];
                                    s2 += w[j];
                                }
                                diff = 0;
                                if (s1 != s2) diff = s2 - s1;
                                else
                                {
                                    for (int j = 1; j < 10; j++)
                                        if (v[j] != w[j])
                                        {
                                            diff = v[j] - w[j];
                                            break;
                                        }
                                }
                                if (diff > 0)
                                {
                                    for (int j = 1; j < 10; j++)
                                        A[s, j] = v[j];
                                }
                            }
                        }
                    }
                }
            }
            s = 0;
            for (int i = 1; i <= 150; i++)
                for (int j = 1; j < 10; j++)
                    s += j * A[i, j];

            return s.ToString();
        }
    }

    /// <summary>
    /// We define the rounded-square-root of a positive integer n as the square root of
    /// n rounded to the nearest integer.
    ///
    /// The following procedure (essentially Heron's method adapted to integer
    /// arithmetic) finds the rounded-square-root of n:
    ///
    /// Let d be the number of digits of the number n.
    /// If d is odd, set x(0) = 2*10^((d-1)/2).
    /// If d is even, set x(0) = 7*10^((d-2)/2).
    /// Repeat:
    ///
    /// x(k+1) = f((x(k) + c(n/x(k)))/2)
    ///
    /// until x(k+1) = x(k).
    ///
    /// As an example, let us find the rounded-square-root of n = 4321.
    /// n has 4 digits, so x(0) = 7*10^((4-2)/2) = 70.
    /// x(1) = f((70 + c(4321/70)) / 2) = 66.
    /// x(2) = f((66 + c(4321/66)) / 2) = 66.
    /// Since x(2) = x(1), we stop here.
    /// So, after just two iterations, we have found that the rounded-square-root of
    /// 4321 is 66 (the actual square root is 65.7343137…).
    ///
    /// The number of iterations required when using this method is surprisingly low.
    /// For example, we can find the rounded-square-root of a 5-digit integer
    /// (10,000 <= n <= 99,999) with an average of 3.2102888889 iterations (the average
    /// value was rounded to 10 decimal places).
    ///
    /// Using the procedure described above, what is the average number of iterations
    /// required to find the rounded-square-root of a 14-digit number
    /// (10^13 <= n <= 10^14)?
    /// Give your answer rounded to 10 decimal places.
    ///
    /// Note: The symbols f(x) and c(x) represent the floor function and ceiling
    /// function respectively.
    /// </summary>
    internal class Problem255 : Problem
    {
        public Problem255() : base(255) { }
    }

    /// <summary>
    /// Tatami are rectangular mats, used to completely cover the floor of a room,
    /// without overlap.
    ///
    /// Assuming that the only type of available tatami has dimensions 1*2, there are
    /// obviously some limitations for the shape and size of the rooms that can be
    /// covered.
    ///
    /// For this problem, we consider only rectangular rooms with integer dimensions
    /// a, b and even size s = a * b.
    /// We use the term 'size' to denote the floor surface area of the room, and —
    /// without loss of generality — we add the condition a <= b.
    ///
    /// There is one rule to follow when laying out tatami: there must be no points
    /// where corners of four different mats meet.
    /// For example, consider the two arrangements below for a 4*4 room:
    ///
    /// AABB    AACD
    /// CDDF    BBCD
    /// CEEF    EFGG
    /// GGHH    EFHH
    ///
    /// The arrangement on the left is acceptable, whereas the one on the right is not:
    /// a red "X" in the middle, marks the point where four tatami meet.
    ///
    /// Because of this rule, certain even-sized rooms cannot be covered with tatami:
    /// we call them tatami-free rooms.
    /// Further, we define T(s) as the number of tatami-free rooms of size s.
    ///
    /// The smallest tatami-free room has size s = 70 and dimensions 7*10.
    /// All the other rooms of size s = 70 can be covered with tatami; they are: 1*70,
    /// 2*35 and 5*14.
    /// Hence, T(70) = 1.
    ///
    /// Similarly, we can verify that T(1320) = 5 because there are exactly 5
    /// tatami-free rooms of size s = 1320:
    /// 20*66, 22*60, 24*55, 30*44 and 33*40.
    /// In fact, s = 1320 is the smallest room-size s for which T(s) = 5.
    ///
    /// Find the smallest room-size s for which T(s) = 200.
    /// </summary>
    internal class Problem256 : Problem
    {
        private const int upper = 100000000;
        private const int t = 200;

        public Problem256() : base(256) { }

        protected override string Action()
        {
            /**
             * http://webhome.cs.uvic.ca/~ruskey/Publications/Tatami/TatamiDraftSubmitted.pdf
             *
             * When a is even, room is tatami-acceptable if there is an integar k such that: b/(a+1) <= k <= b/(a-1).
             * When odd, (b-1)/(a+1) <= k <= (b+1)/(a-1).
             */
            var counter = new int[upper];

            for (int a = 3; a < Misc.Sqrt(upper); a++)
            {
                if (a % 2 == 0)
                {
                    for (int r = 3; r < a - 3; r++)
                    {
                        for (int k = 1, b = a + r, uk = Math.Min(r - 1, a - r - 2); k < uk && a * b < upper; k++)
                        {
                            counter[a * b]++;
                            b += a;
                        }
                    }
                }
                else
                {
                    for (int r = 2; r < a / 2; r++)
                    {
                        for (int k = 1, b = a - 1 + 2 * r; k < r && a * b < upper; k++)
                        {
                            counter[a * b]++;
                            b += a - 1;
                        }
                    }
                }
            }

            for (int s = 2; s < upper; s += 2)
            {
                if (counter[s] == t)
                    return s.ToString();
            }

            return null;
        }
    }

    internal class Problem257 : Problem
    {
        public Problem257() : base(257) { }
    }

    /// <summary>
    /// A sequence is defined as:
    ///
    /// g(k) = 1, for 0 <= k <= 1999
    /// g(k) = g(k-2000) + g(k-1999), for k >= 2000.
    ///
    /// Find g(k) mod 20092010 for k = 10^18.
    /// </summary>
    internal class Problem258 : Problem
    {
        private static long upper = Misc.Pow(10, 18);
        private const int modulo = 20092010;
        private const int size = 2000;

        public Problem258() : base(258) { }

        protected override string Action()
        {
            /**
             * http://math.stackexchange.com/questions/91173/recurrence-for-a-lagged-fibonacci-sequence
             *
             * Use matrix:
             *   [g(1), ..., g(2000)] = M * [g(0), ..., g(1999)]
             *   [g(n), ..., g(n+1999)] = M^n * [g(0), ..., g(1999)]
             *   where M is [0, 1, ..., 0, 0,
             *               0, 0, 1, ..., 0,
             *               ...
             *               1, 1, 0, ..., 0]
             */
            long[] start = new long[size];
            long[] matrix = new long[size * size];
            SmallMatrix s, m, r;

            for (int i = 0; i < size; i++)
            {
                start[i] = 1;
                if (i != size - 1)
                    matrix[i * size + i + 1] = 1;
                else
                    matrix[i * size] = matrix[i * size + 1] = 1;
            }
            s = new SmallMatrix(start, size, 1);
            m = new SmallMatrix(matrix, size, size);

            m = SmallMatrix.ModPow(m, upper, modulo);
            r = m * s;

            return r[0, 0].ToString();
        }
    }

    /// <summary>
    /// A positive integer will be called reachable if it can result from an arithmetic
    /// expression obeying the following rules:
    ///
    /// Uses the digits 1 through 9, in that order and exactly once each.
    /// Any successive digits can be concatenated (for example, using the digits 2, 3
    /// and 4 we obtain the number 234).
    /// Only the four usual binary arithmetic operations (addition, subtraction,
    /// multiplication and division) are allowed.
    /// Each operation can be used any number of times, or not at all.
    /// Unary minus is not allowed.
    /// Any number of (possibly nested) parentheses may be used to define the order of
    /// operations.
    /// For example, 42 is reachable, since (1/23) * ((4*5)-6) * (78-9) = 42.
    ///
    /// What is the sum of all positive reachable integers?
    /// </summary>
    internal class Problem259 : Problem
    {
        private const string numbers = "123456789";

        public Problem259() : base(259) { }

        private void Intersect(HashSet<SmallFraction> target, HashSet<SmallFraction> left, HashSet<SmallFraction> right)
        {
            foreach (var l in left)
            {
                foreach (var r in right)
                {
                    target.Add(l + r);
                    target.Add(l - r);
                    target.Add(l * r);
                    if (r != 0)
                        target.Add(l / r);
                }
            }
        }

        private HashSet<SmallFraction> Calculate(Dictionary<string, HashSet<SmallFraction>> dict, string digits)
        {
            HashSet<SmallFraction> ret = new HashSet<SmallFraction>();

            if (dict.ContainsKey(digits))
                return dict[digits];

            ret.Add(int.Parse(digits));
            if (digits.Length > 1)
            {
                for (int i = 1; i < digits.Length; i++)
                    Intersect(ret, Calculate(dict, digits.Substring(0, i)), Calculate(dict, digits.Substring(i)));
            }
            dict.Add(digits, ret);

            return ret;
        }

        protected override string Action()
        {
            var dict = new Dictionary<string, HashSet<SmallFraction>>();
            long sum = 0;

            Calculate(dict, numbers);
            foreach (var value in dict[numbers])
            {
                if (value.Denominator == 1 && value.Numerator > 0)
                    sum += value.Numerator;
            }

            return sum.ToString();
        }
    }
}