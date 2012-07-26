using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProjectEuler.Common;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// Consider the infinite polynomial series AG(x) = xG1 + x^2G2 + x^3G3 + ...,
    /// where Gk is the kth term of the second order recurrence relation
    /// Gk = Gk1 + Gk2, G1 = 1 and G2 = 4; that is, 1, 4, 5, 9, 14, 23, ... .
    ///
    /// For this problem we shall be concerned with values of x for which AG(x) is a
    /// positive integer.
    ///
    /// The corresponding values of x for the first five natural numbers are shown
    /// below.
    ///
    ///                x      AG(x)
    ///    (sqrt(5)-1)/4         1
    ///              2/5         2
    ///   (sqrt(22)-2)/6         3
    /// (sqrt(137)-5)/14         4
    ///              1/2         5
    ///
    /// We shall call AG(x) a golden nugget if x is rational, because they become
    /// increasingly rarer; for example, the 20th golden nugget is 211345365.
    ///
    /// Find the sum of the first thirty golden nuggets.
    /// </summary>
    internal class Problem140 : Problem
    {
        private const int index = 30;

        public Problem140() : base(140) { }

        protected override string Action()
        {
            var ret = new List<BigInteger>();
            BigInteger sum = 0;

            /**
             * AG(x)*x^2 + AG(x)*x - AG(x)
             * =     0 +     0 + x^3G1 + x^4G2 + ...
             * +     0 + x^2G1 + x^3G2 + x^4G3 + ...
             * - x^1G1 - x^2G2 - x^3G3 - x^4G4 + ...
             * =    -x - 3x^2
             *
             * AG(x) = (3x^2+x)/(1-x-x^2) = n
             * (n+3)x^2 + (n+1)x - n = 0, x is rational, so (n+1)^2 + 4n(n+3) is perfect square.
             * 5n^2+14n+1 = 5(n+7/5)^2 - 44/5 = y^2
             * 5y^2 = (5n+7)^2 - 44 => x^2 - 5a^2 = 44, n = (x-7)/5
             */
            var rsList = new List<Tuple<BigInteger, BigInteger>>();
            var pqList = new List<Tuple<BigInteger, BigInteger>>();
            const int D = 5;

            /**
             * Get Fundamental solution by google/brute force
             */
            pqList.Add(new Tuple<BigInteger, BigInteger>(7, 1));
            pqList.Add(new Tuple<BigInteger, BigInteger>(8, 2));
            pqList.Add(new Tuple<BigInteger, BigInteger>(13, 5));

            /**
             * http://mathworld.wolfram.com/PellEquation.html
             * Pell-like equations
             */
            foreach (var ans in PellEquation.GetSolutions(D))
            {
                rsList.Add(ans);
                if (rsList.Count == index)
                    break;
            }
            foreach (var pq in pqList)
            {
                if (pq.Item1 > 7 && pq.Item1 % 5 == 2)
                    ret.Add((pq.Item1 - 7) / 5);
                foreach (var rs in rsList)
                {
                    var x = pq.Item1 * rs.Item1 + D * pq.Item2 * rs.Item2;
                    var y = pq.Item1 * rs.Item2 + pq.Item2 * rs.Item1;

                    if (x > 7 && x % 5 == 2)
                        ret.Add((x - 7) / 5);

                    x = pq.Item1 * rs.Item1 - D * pq.Item2 * rs.Item2;
                    y = pq.Item1 * rs.Item2 - pq.Item2 * rs.Item1;

                    if (x > 7 && x % 5 == 2)
                        ret.Add((x - 7) / 5);
                }
            }

            ret = ret.Distinct().ToList();
            ret.Sort();
            for (int i = 0; i < index; i++)
                sum += ret[i];

            return sum.ToString();
        }
    }

    /// <summary>
    /// A positive integer, n, is divided by d and the quotient and remainder are q and
    /// r respectively. In addition d, q, and r are consecutive positive integer terms
    /// in a geometric sequence, but not necessarily in that order.
    ///
    /// For example, 58 divided by 6 has quotient 9 and remainder 4. It can also be
    /// seen that 4, 6, 9 are consecutive terms in a geometric sequence (common ratio
    /// 3/2).
    ///
    /// We will call such numbers, n, progressive.
    ///
    /// Some progressive numbers, such as 9 and 10404 = 102^2, happen to also be
    /// perfect squares.
    ///
    /// The sum of all progressive perfect squares below one hundred thousand is
    /// 124657.
    ///
    /// Find the sum of all progressive perfect squares below one trillion (10^12).
    /// </summary>
    internal class Problem141 : Problem
    {
        private const long upper = 1000000000000L;

        public Problem141() : base(141) { }

        protected override string Action()
        {
            var ret = new HashSet<long>();

            /**
             * assume a, b, c is gemoetric sequence, a*c = b^2
             * r, d, q: n^2 = dq + r
             * r, q, d: n^2 = dq + r
             * q, r, d: n^2 = dq + r = r(1+r), impossible
             *
             * only check r, d, q where rq = d^2
             * dq+r = r+r^2(n/d)^3, and r = a*d^2
             * dq+r = ad^2+a^2n^3d is perfect number, n>d, GCD(n,d)=1
             */
            long maxn = (long)Math.Pow(upper, (double)1 / 3) - 1;

            while (maxn * maxn * maxn < upper)
                maxn++;
            maxn--;

            // very important use n as upper bound, or it will cost much more time
            Parallel.For(1, maxn, (n) =>
            {
                long n3 = (long)n * n * n, d2 = 0;

                for (long d = 1; d <= n; d++)
                {
                    d2 += 2 * d - 1;

                    if (d2 + n3 >= upper)
                        break;
                    if (Factor.GetCommonFactor((long)n, d) != 1)
                        continue;
                    for (long a = 1; ; a++)
                    {
                        long num = a * d2 + a * a * d * n3;

                        if (num >= upper)
                            break;
                        if (Misc.IsPerfectSquare(num))
                            lock (ret)
                                ret.Add(num);
                    }
                }
            });

            return ret.Sum().ToString();
        }
    }

    /// <summary>
    /// Find the smallest x + y + z with integers x > y > z > 0 such that x + y, x - y,
    /// x + z, x - z, y + z, y - z are all perfect squares.
    /// </summary>
    internal class Problem142 : Problem
    {
        private const int upper = 1000000;

        public Problem142() : base(142) { }

        protected override string Action()
        {
            var squares = new long[upper];
            long min = long.MaxValue;

            /**
             * assume:
             *   x + y = a^2,
             *   y + z = b^2,
             *   y - z = c^2,
             * then:
             *   x + z = a^2 - c^2
             *   x - y = a^2 - b^2 - c^2
             *   x - z = a^2 - b^2
             *
             * find a>b>c, where a^2-c^2, a^2-b^2, a^2-b^2-c^2 is perfect square.
             * x + y + z = (a^2 + b^2 + a^2 - c^2) / 2
             */
            for (int i = 0; i < upper; i++)
                squares[i] = (long)i * i;

            for (int a = 1; a < upper; a++)
            {
                if (squares[a] * 2 > min)
                    break;
                for (int b = 1; b < a; b++)
                {
                    if (!Misc.IsPerfectSquare(squares[a] - squares[b]))
                        continue;

                    if (squares[a] * 2 + squares[b] > min)
                        break;
                    for (int c = 1; c < b; c++)
                    {
                        var tmp = squares[a] - squares[c];
                        if (!Misc.IsPerfectSquare(tmp))
                            continue;

                        tmp -= squares[b];
                        if (tmp > 0 && Misc.IsPerfectSquare(tmp))
                        {
                            var sum = 2 * squares[a] + squares[b] - squares[c];
                            if (sum % 2 == 0 && sum / 2 < min)
                                min = sum / 2;
                        }
                    }
                }
            }

            return min.ToString();
        }
    }

    /// <summary>
    /// Let ABC be a triangle with all interior angles being less than 120 degrees. Let
    /// X be any point inside the triangle and let XA = p, XB = q, and XC = r.
    ///
    /// Fermat challenged Torricelli to find the position of X such that p + q + r was
    /// minimised.
    ///
    /// Torricelli was able to prove that if equilateral triangles AOB, BNC and AMC are
    /// constructed on each side of triangle ABC, the circumscribed circles of AOB,
    /// BNC, and AMC will intersect at a single point, T, inside the triangle. Moreover
    /// he proved that T, called the Torricelli/Fermat point, minimises p + q + r. Even
    /// more remarkable, it can be shown that when the sum is minimised,
    /// AN = BM = CO = p + q + r and that AN, BM and CO also intersect at T.
    ///
    /// If the sum is minimised and a, b, c, p, q and r are all positive integers we
    /// shall call triangle ABC a Torricelli triangle. For example, a = 399, b = 455,
    /// c = 511 is an example of a Torricelli triangle, with p + q + r = 784.
    ///
    /// Find the sum of all distinct values of p + q + r <= 120000 for Torricelli
    /// triangles.
    /// </summary>
    internal class Problem143 : Problem
    {
        private const int upper = 120000;

        public Problem143() : base(143) { }

        protected override string Action()
        {
            var dict = new Dictionary<int, HashSet<int>>();
            BigInteger ret = 0;

            /**
             * http://d.hatena.ne.jp/Rion778/20110619/1308480254
             * three angle of T is 120 degree.
             * a^2 = q^2 + r^2 + qr,
             * b^2 = p^2 + r^2 + pr,
             * c^2 = p^2 + q^2 + pq,
             *
             * Find pair of a,b where a^2+b^2+ab is perfect square
             * http://d.hatena.ne.jp/Rion778/20110619
             * Generate m,n where GCD(m,n)=1 and (m-n)%3 != 0
             * a = 2mn+n^2, b = m^2-n^2
             */
            //for (int m = 1; m < (int)Misc.Sqrt(upper) * 2; m++)
            //{
            //    for (int n = 1; n < m; n++)
            for (int n = 1; n <= (int)Misc.Sqrt(upper); n++)
            {
                for (int m = n + 1; ; m++)
                {
                    if (Factor.GetCommonFactor(m, n) != 1)// || (m - n) % 3 == 0)
                        continue;

                    int a = 2 * m * n + n * n, b = m * m - n * n;
                    int k = a, v = b;

                    if (a + b > upper)
                        break;

                    while (k + v <= upper)
                    {
                        if (dict.ContainsKey(k))
                            dict[k].Add(v);
                        else
                            dict.Add(k, new HashSet<int>(new int[] { v }));
                        if (dict.ContainsKey(v))
                            dict[v].Add(k);
                        else
                            dict.Add(v, new HashSet<int>(new int[] { k }));

                        k += a;
                        v += b;
                    }
                }
            }

            var peri = new HashSet<int>();

            foreach (var item in dict)
            {
                if (item.Value.Count < 2)
                    continue;
                foreach (var keys in Itertools.Combinations(item.Value, 2))
                {
                    if (!dict.ContainsKey(keys[0]) || !dict.ContainsKey(keys[1]))
                        continue;

                    if (dict[keys[0]].Contains(keys[1]) || dict[keys[1]].Contains(keys[0]))
                    {
                        if (item.Key + keys.Sum() <= upper)
                            peri.Add(item.Key + keys.Sum());
                    }
                }
            }

            return peri.Sum().ToString();
        }
    }

    /// <summary>
    /// In laser physics, a "white cell" is a mirror system that acts as a delay line
    /// for the laser beam. The beam enters the cell, bounces around on the mirrors,
    /// and eventually works its way back out.
    ///
    /// The specific white cell we will be considering is an ellipse with the equation
    /// 4x^2 + y^2 = 100
    ///
    /// The section corresponding to -0.01 <= x <= +0.01 at the top is missing,
    /// allowing the light to enter and exit through the hole.
    ///
    /// The light beam in this problem starts at the point (0.0, 10.1) just outside the
    /// white cell, and the beam first impacts the mirror at (1.4, -9.6).
    ///
    /// Each time the laser beam hits the surface of the ellipse, it follows the usual
    /// law of reflection "angle of incidence equals angle of reflection." That is,
    /// both the incident and reflected beams make the same angle with the normal line
    /// at the point of incidence.
    ///
    /// In the figure on the left, the red line shows the first two points of contact
    /// between the laser beam and the wall of the white cell; the blue line shows the
    /// line tangent to the ellipse at the point of incidence of the first bounce.
    ///
    /// The slope m of the tangent line at any point (x,y) of the given ellipse is:
    /// m = -4x/y
    ///
    /// The normal line is perpendicular to this tangent line at the point of
    /// incidence.
    ///
    /// The animation on the right shows the first 10 reflections of the beam.
    ///
    /// How many times does the beam hit the internal surface of the white cell before
    /// exiting?
    /// </summary>
    internal class Problem144 : Problem
    {
        public Problem144() : base(144) { }

        private Tuple<double, double> Reflect(double x1, double y1, double x2, double y2)
        {
            double a, b;

            a = Math.Tan(2 * Math.Atan(-4 * x2 / y2) + Math.PI - Math.Atan2(y2 - y1, x2 - x1));
            b = y2 - a * x2;

            return new Tuple<double, double>(a, b);
        }

        private Tuple<double, double> Intersect(double a, double b, double x, double y)
        {
            double x2, y2;

            x2 = (-(a * a + 4) * x - 2 * a * b) / (4 + a * a);
            y2 = a * x2 + b;

            return new Tuple<double, double>(x2, y2);
        }

        protected override string Action()
        {
            double x1 = 0, y1 = 10.1, x2 = 1.4, y2 = -9.6;
            double a = 0, b = 0;
            var counter = 0;

            // http://d.hatena.ne.jp/Rion778/20110626/1309015162
            while (Math.Abs(x2) > 0.01 || y2 < 0)
            {
                var tmp = Reflect(x1, y1, x2, y2);

                a = tmp.Item1;
                b = tmp.Item2;
                x1 = x2;
                y1 = y2;

                tmp = Intersect(a, b, x2, y2);
                x2 = tmp.Item1;
                y2 = tmp.Item2;

                counter++;
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// Some positive integers n have the property that the sum [ n + reverse(n) ]
    /// consists entirely of odd (decimal) digits. For instance, 36 + 63 = 99 and 409 +
    /// 904 = 1313. We will call such numbers reversible; so 36, 63, 409, and 904 are
    /// reversible. Leading zeroes are not allowed in either n or reverse(n).
    ///
    /// There are 120 reversible numbers below one-thousand.
    ///
    /// How many reversible numbers are there below one-billion (10^9)?
    /// </summary>
    internal class Problem145 : Problem
    {
        private const int upper = 1000000000;

        public Problem145() : base(145) { }

        protected override string Action()
        {
            var counter = 0;

            /**
             * for 1-digit number, impossible
             */
            counter += 0;
            /**
             * for number ab:
             *   a+b must be odd, less than 10 and nonzero:
             *   12, 14, 16, 18, 23, 25, 27, 34, 36, 45, totally 20 solutions
             */
            counter += (4 + 3 + 2 + 1) * 2;
            /**
             * for number abc:
             *   a+c must be odd, greater than 10:
             *   98, 96, 94, 92, 87, 85, 83, 76, 74, 65, totally 20 solutions
             *   b must be less than 5: 5 solutions
             */
            counter += 20 * 5;
            /**
             * for number abcd, bc must follow law of ab:
             *   b+c must be odd, less than 10:
             *   01, 03, 05, 07, 09 plus nonzero, totally 30 solutions
             *   a+d must be odd, less than 10 and nonzero: 20 solutions
             */
            counter += 30 * 20;
            /**
             * for numebr abcde, bcd must follow law of abc:
             *   b+d must be odd, greater than 10: 20 solutions
             *   c is less than 5: 5 solutions
             *   a+e must be even, impossible
             */
            counter += 0;
            /**
             * for number abcdef, bcde must follow law of abcd:
             *   c+d must be odd, less than 10: 30 solutions
             *   b+e must be odd, less than 10: 30 solutions
             *   a+f must be odd, less than 10 and nonzero: 20 solutions
             */
            counter += 20 * 30 * 30;
            /**
             * for number abcdefg, cde must follow law of abc:
             *   c+e must be odd, greater than 10: 20 solutions
             *   d must be less than 5: 5 solutions
             *   b+f must be even, less than 10:
             *   02, 04, 06, 08, 13, 15, 17, 24, 26, 35 * 2 plus 00, 11, 22, 33, 44, totally 25 solutions
             *   a+g must be odd, greater than 10: 20 solutions
             */
            counter += 5 * 20 * 25 * 20;
            /**
             * for number abcdefgh, bcdefg must follow law of abcdef:
             *   d+e must be odd, less than 10: 30 solutions
             *   c+f must be odd, less than 10: 30 solutions
             *   b+g must be odd, less than 10: 30 solutions
             *   a+h must be odd, less than 10 and nonzero: 20 solutions
             */
            counter += 20 * 30 * 30 * 30;
            /**
             * for number abcdefghi, bcdefgh must follow law of abcdefg:
             * e must be less than 5: 5 solutions
             * d+f must be odd, greater than 10: 20 solutions
             * c+g must be even, less than 10: 25 solutions
             * b+h must be odd, greater than 10: 20 solutions
             * a+i must be even, impossible
             */
            counter += 0;

            return counter.ToString();
        }
    }

    /// <summary>
    /// The smallest positive integer n for which the numbers n^2+1, n^2+3, n^2+7,
    /// n^2+9, n^2+13, and n^2+27 are consecutive primes is 10. The sum of all such
    /// integers n below one-million is 1242490.
    ///
    /// What is the sum of all such integers n below 150 million?
    /// </summary>
    internal class Problem146 : Problem
    {
        private const int upper = 150000000;
        private const int precise = 5;
        private readonly int[] offsets = new int[] { 1, 3, 7, 9, 13, 27 };

        public Problem146() : base(146) { }

        private bool Check(Prime prime, long n)
        {
            BigInteger a;
            var x = n * n;

            if (x % 3 != 1)
                return false;
            if (x % 7 != 2)
                return false;
            if (x % 13 != 1 && x % 13 != 3 && x % 13 != 9)
                return false;
            if (x % 11 != 0 && x % 11 != 1 && x % 11 != 3 && x % 11 != 5)
                return false;

            foreach (var p in prime)
            {
                foreach (var off in offsets)
                {
                    if ((x + off) % p == 0)
                        return false;
                }
            }

            foreach (var off in offsets)
            {
                for (int i = 0; i < precise; i++)
                {
                    a = Misc.Random(x + off - 2) + 1;
                    if (BigInteger.ModPow(a, x + off - 1, x + off) != 1)
                        return false;
                }
            }

            // Check n^2 + 21 in enough
            for (int i = 0; i < precise; i++)
            {
                a = Misc.Random(x + 21 - 2) + 1;
                if (BigInteger.ModPow(a, x + 21 - 1, x + 21) != 1)
                    return true;
            }

            return false;
        }

        protected override string Action()
        {
            var p = new Prime(100);
            long counter = 0;

            p.GenerateAll();
            /**
             * taking mod 2, 3, 5, 7, we must have:
             * n = 0 mod 2.
             * n = 1,2 mod 3.
             * n = 0 mod 5.
             * n = 3,4 mod 7.
             */
            Parallel.For(0, upper / 210, (i) =>
            {
                if (Check(p, i * 210 + 10))
                    Interlocked.Add(ref counter, i * 210 + 10);
                if (Check(p, i * 210 + 80))
                    Interlocked.Add(ref counter, i * 210 + 80);
                if (Check(p, i * 210 + 130))
                    Interlocked.Add(ref counter, i * 210 + 130);
                if (Check(p, i * 210 + 200))
                    Interlocked.Add(ref counter, i * 210 + 200);
            });

            return counter.ToString();
        }
    }

    /// <summary>
    /// In a 3x2 cross-hatched grid, a total of 37 different rectangles could be
    /// situated within that grid as indicated in the sketch.
    ///
    /// There are 5 grids smaller than 3x2, vertical and horizontal dimensions being
    /// important, i.e. 1x1, 2x1, 3x1, 1x2 and 2x2. If each of them is cross-hatched,
    /// the following number of different rectangles could be situated within those
    /// smaller grids:
    ///
    /// 1x1: 1
    /// 2x1: 4
    /// 3x1: 8
    /// 1x2: 4
    /// 2x2: 18
    ///
    /// Adding those to the 37 of the 3x2 grid, a total of 72 different rectangles
    /// could be situated within 3x2 and smaller grids.
    ///
    /// How many different rectangles could be situated within 47x43 and smaller grids?
    /// </summary>
    internal class Problem147 : Problem
    {
        private const int maxWidth = 47;
        private const int maxHeight = 43;

        public Problem147() : base(147) { }

        private int GetValidWidth(int width, int height, int crosspos, int crossheight)
        {
            int x1, y1, x2, y2, ret;

            if (crosspos <= 1 || crosspos >= width + height)
                return 0;

            x1 = crosspos;
            y1 = 0;
            x2 = 0;
            y2 = crosspos;
            if (x1 > width)
            {
                y1 = x1 - width;
                x1 = width;
            }
            if (y2 > height)
            {
                x2 = y2 - height;
                y2 = height;
            }

            ret = (y2 - y1) * 2;
            if (y1 == 0)
                ret -= crossheight;
            else if (crossheight > 2 * y1)
                ret -= crossheight - 2 * y1;
            if (x2 == 0)
                ret -= crossheight;
            else if (crossheight > 2 * x2)
                ret -= crossheight - 2 * x2;

            return ret;
        }

        private void CalculateCrossGrids(BigInteger[,] grids, int width, int height)
        {
            /**
             * For a m*n board:
             * total cross-hatched 1*1 square number is (m*n - n/4*2 - m/4*2)*2 = 2*m*n - n - m
             * length of cross-hatched line is 1, 2, ... min(m,n), ..., min(m,n), ..., 2, 1, totally (m+n-1) lines
             */
            for (int m = 1; m <= width; m++)
            {
                for (int n = 1; n <= height; n++)
                {
                    for (int crosspos = 2; crosspos < m + n; crosspos++)
                    {
                        for (int crossheight = 1; ; crossheight++)
                        {
                            int tmp = GetValidWidth(m, n, crosspos, crossheight);

                            if (tmp == 0)
                                break;
                            grids[m, n] += tmp * (tmp + 1) / 2;
                        }
                    }
                }
            }
        }

        private void CalculateNormalGrids(BigInteger[,] grids, int width, int height)
        {
            /**
             * For a m*n board:
             * 1*1-1*n squares: (1+2+...+n)*m = m*n*(n+1)/2
             * 2*1-2*n squares = (m-1)*n*(n+1)/2
             * m*1-m*n squares = 1*n*(n+1)/2
             * number of rectangles is m*(m+1)*n*(n+1)/4
             */
            for (int m = 0; m <= width; m++)
            {
                for (int n = 0; n <= height; n++)
                    grids[m, n] += m * (m + 1) * n * (n + 1) / 4;
            }
        }

        protected override string Action()
        {
            var grids = new BigInteger[maxWidth + 1, maxHeight + 1];
            BigInteger ret = 0;

            CalculateCrossGrids(grids, maxWidth, maxHeight);
            CalculateNormalGrids(grids, maxWidth, maxHeight);

            foreach (var grid in grids)
                ret += grid;

            return ret.ToString();
        }
    }

    /// <summary>
    /// We can easily verify that none of the entries in the first seven rows of
    /// Pascal's triangle are divisible by 7:
    ///       1
    ///      1 1
    ///     1 2 1
    ///    1 3 3 1
    ///   1 4 6 4 1
    ///  1 51010 5 1
    /// 1 6152015 6 1
    ///
    /// However, if we check the first one hundred rows, we will find that only 2361
    /// of the 5050 entries are not divisible by 7.
    ///
    /// Find the number of entries which are not divisible by 7 in the first one
    /// billion (10^9) rows of Pascal's triangle.
    /// </summary>
    internal class Problem148 : Problem
    {
        private const int rows = 1000000000;

        public Problem148() : base(148) { }

        protected override string Action()
        {
            BigInteger counter = 0;
            /**
             * using triangle mod 7
             * 1
             * 1 1
             * 1 2 1
             * 1 3 3 1
             * 1 4 6 4 1
             * 1 5 3 3 5 1
             * 1 6 1 6 1 6 1
             * 1 0 0 0 0 0 0 1
             * 1 1 0 0 0 0 0 1 1
             * 1 2 1 0 0 0 0 1 2 1
             * 1 3 3 1 0 0 0 1 3 3 1
             * 1 4 6 4 1 0 0 1 4 6 4 1
             * 1 5 3 3 5 1 0 1 5 3 3 5 1
             * 1 6 1 6 1 6 1 1 6 1 6 1 6 1
             * 1 0 0 0 0 0 0 2 0 0 0 0 0 0 1
             * 1 1 0 0 0 0 0 2 2 0 0 0 0 0 1 1
             * 1 2 1 0 0 0 0 2 4 2 0 0 0 0 1 2 1
             * 1 3 3 1 0 0 0 2 6 6 2 0 0 0 1 3 3 1
             * 1 4 6 4 1 0 0 2 1 5 1 2 0 0 1 4 6 4 1
             * 1 5 3 3 5 1 0 2 3 6 6 3 2 0 1 5 3 3 5 1
             * 1 6 1 6 1 6 1 2 5 2 5 2 5 2 1 6 1 6 1 6 1
             * 1 0 0 0 0 0 0 3 0 0 0 0 0 0 3 0 0 0 0 0 0 1
             * .......
             * in every seven lines
             * 1
             * 1~1
             * 1~2~1
             * 1~3~3~1
             * 1~4~4~4~1
             * 1~5~5~5~5~1
             * 1~6~6~6~6~6~1
             * 1~0~0~0~0~0~0~1
             * 1~1~0~0~0~0~0~1~1
             * 1~2~1~0~0~0~0~1~2~1
             * 1~3~3~1~0~0~0~1~3~3~1
             * 1~4~6~4~1~0~0~1~4~6~4~1
             * 1~5~3~3~5~1~0~1~5~3~3~5~1
             * 1~6~1~6~1~6~1~1~6~1~6~1~6~1
             *
             * .....
             * Any in bigger scales
             *
             * any single number surrounded by zero will be divisible by 7 in 7 steps
             */
            var scales = new List<BigInteger>();
            var rowrange = new List<BigInteger>();
            BigInteger factor = 1;
            BigInteger left = rows;

            scales.Add(1);
            rowrange.Add(1);
            while (rowrange[rowrange.Count - 1] * 7 <= rows)
            {
                scales.Add(scales[scales.Count - 1] * 28);
                rowrange.Add(rowrange[rowrange.Count - 1] * 7);
            }

            for (int i = rowrange.Count - 1; i > 0; i--)
            {
                var tmp = left / rowrange[i];

                counter += factor * scales[i] * tmp * (tmp + 1) / 2;
                left %= rowrange[i];

                factor *= tmp + 1;
            }
            counter += factor * left * (left + 1) / 2;

            return counter.ToString();
        }
    }

    /// <summary>
    /// Looking at the table below, it is easy to verify that the maximum possible sum
    /// of adjacent numbers in any direction (horizontal, vertical, diagonal
    /// or anti-diagonal) is 16 (= 8 + 7 + 1).
    ///
    /// -2  5  3  2
    ///  9 -6  5  1
    ///  3  2  7  3
    /// -1  8 -4  8
    ///
    /// Now, let us repeat the search, but on a much larger scale:
    ///
    /// First, generate four million pseudo-random numbers using a specific form of
    /// what is known as a "Lagged Fibonacci Generator":
    ///
    /// For 1 <= k <= 55, sk = [100003 - 200003k + 300007k^3] (modulo 1000000)
    /// - 500000.
    /// For 56 <= k <= 4000000, sk = [s(k-24) + s(k-55) + 1000000] (modulo 1000000)
    /// - 500000.
    ///
    /// Thus, s10 = -393027 and s100 = 86613.
    ///
    /// The terms of s are then arranged in a 2000 * 2000 table, using the first 2000
    /// numbers to fill the first row (sequentially), the next 2000 numbers to fill the
    /// second row, and so on.
    ///
    /// Finally, find the greatest sum of (any number of) adjacent entries in any
    /// direction (horizontal, vertical, diagonal or anti-diagonal).
    /// </summary>
    internal class Problem149 : Problem
    {
        private const int size = 2000;

        public Problem149() : base(149) { }

        private void Generate(int[] array)
        {
            Modulo modulo = new Modulo(1000000);

            for (int i = 0; i < 55; i++)
                array[i] = (int)modulo.Mod(100003 - 200003 * (i + 1) + (long)300007 * (i + 1) * (i + 1) * (i + 1)) - 500000;
            for (int i = 55; i < size * size; i++)
                array[i] = (int)modulo.Mod(array[i - 24] + array[i - 55] + 1000000) - 500000;
        }

        protected override string Action()
        {
            var array = new int[size * size];
            var prev = new int[4][] { new int[size], new int[size], new int[size], new int[size] };
            var current = new int[4][] { new int[size], new int[size], new int[size], new int[size] };
            int max = 0;

            Generate(array);

            max = array.Take(size).Max();
            for (int r = 1; r < size; r++)
            {
                int offset = r * size;

                for (int c = 0; c < size; c++)
                {
                    // from Up
                    current[0][c] = prev[0][c] + array[offset + c];
                    if (current[0][c] < 0)
                        current[0][c] = 0;
                    // from left
                    if (c == 0)
                        current[1][c] = array[offset + c];
                    else
                        current[1][c] = current[1][c - 1] + array[offset + c];
                    if (current[1][c] < 0)
                        current[1][c] = 0;
                    // from UpLeft
                    if (c == 0)
                        current[2][c] = array[offset + c];
                    else
                        current[2][c] = prev[2][c - 1] + array[offset + c];
                    if (current[2][c] < 0)
                        current[2][c] = 0;
                    // from UpRight
                    if (c == size - 1)
                        current[3][c] = array[offset + c];
                    else
                        current[3][c] = prev[3][c + 1] + array[offset + c];
                    if (current[3][c] < 0)
                        current[3][c] = 0;
                }

                var tmpm = Math.Max(current[0].Max(), Math.Max(current[1].Max(), Math.Max(current[2].Max(), current[3].Max())));

                if (tmpm > max)
                    max = tmpm;

                var tmp = current;
                current = prev;
                prev = tmp;
            }

            return max.ToString();
        }
    }
}