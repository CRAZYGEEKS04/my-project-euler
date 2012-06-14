using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using ProjectEuler.Common;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// For any N, let f(N) be the last five digits before the trailing zeroes in N!.
    /// For example,
    ///
    /// 9! = 362880 so f(9)=36288
    /// 10! = 3628800 so f(10)=36288
    /// 20! = 2432902008176640000 so f(20)=17664
    ///
    /// Find f(1,000,000,000,000)
    /// </summary>
    internal class Problem160 : Problem
    {
        private const long upper = 1000000000000;
        private const long range = 100000;

        public Problem160() : base(160) { }

        private long CountNFactors(long upper, long factor)
        {
            long ret = 0, tmp = factor;

            while (tmp <= upper)
            {
                ret += upper / tmp;
                tmp *= factor;
            }

            return ret;
        }

        protected override string Action()
        {
            long single = 1, ret = 1;
            var array = new long[range + 1];

            /**
             * calculate factorial product of non-2,5 numbers from 1~100000(1,3,7,9,11,...,99993,99997,99999)
             */
            array[0] = 1;
            for (long i = 1; i < range; i++)
            {
                if (i % 2 != 0 && i % 5 != 0)
                {
                    single *= i;
                    single %= range;
                }
                array[i] = single;
            }

            /**
             * calculate 1*(2^p*5^r)*(1,3,7,9,...,99999)
             * only part in bracket so no 0 will be produced
             */
            for (long two = 1; two < upper; two *= 2)
            {
                for (long five = 1; five * two < upper; five *= 5)
                {
                    var total = upper / two / five;

                    for (int i = 0; i < total / range; i++)
                    {
                        ret *= single;
                        ret %= range;
                    }
                    ret *= array[total % range];
                    ret %= range;
                }
            }
            /**
             * calculate all 2 and 5 factors
             */
            ret *= (long)BigInteger.ModPow(2, CountNFactors(upper, 2) - CountNFactors(upper, 5), range);
            ret %= range;

            return ret.ToString();
        }
    }

    /// <summary>
    /// A triomino is a shape consisting of three squares joined via the edges. There
    /// are two basic forms:
    ///
    /// ** ***
    /// *
    ///
    /// If all possible orientations are taken into account there are six:
    ///
    /// ** ** *   *  *
    /// *   * ** **  * ***
    ///              *
    ///
    /// Any n by m grid for which n*m is divisible by 3 can be tiled with triominoes.
    ///
    /// If we consider tilings that can be obtained by reflection or rotation from
    /// another tiling as different there are 41 ways a 2 by 9 grid can be tiled with
    /// triominoes.
    ///
    /// In how many ways can a 9 by 12 grid be tiled in this way by triominoes?
    /// </summary>
    internal class Problem161 : Problem
    {
        private const int width = 9;
        private const int height = 12;

        public Problem161() : base(161) { }

        private string GetIDX(int[] board)
        {
            return string.Join("|", board);
        }

        private bool IsEmpty(int[] board, int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return false;

            return (board[y] & (1 << x)) == 0;
        }

        private void FindNext(int[] board, ref int x, ref int y)
        {
            while (!IsEmpty(board, x, y))
            {
                x++;
                if (x == width)
                {
                    x = 0;
                    y++;
                }
                if (y == height)
                    break;
            }
        }

        private long Count(Dictionary<string, long> dict, int[] board, int x, int y)
        {
            var key = GetIDX(board);
            long ret = 0;

            if (dict.ContainsKey(key))
                return dict[key];
            FindNext(board, ref x, ref y);
            if (y == height)
                return 1;

            board[y] |= (1 << x);
            if (IsEmpty(board, x + 1, y))
            {
                board[y] |= (2 << x);
                // horizontal line
                if (IsEmpty(board, x + 2, y))
                {
                    board[y] |= (4 << x);
                    ret += Count(dict, board, x, y);
                    board[y] &= ~(4 << x);
                }
                // upperleft triangle
                if (IsEmpty(board, x, y + 1))
                {
                    board[y + 1] |= (1 << x);
                    ret += Count(dict, board, x, y);
                    board[y + 1] &= ~(1 << x);
                }
                // upperright triangle
                if (IsEmpty(board, x + 1, y + 1))
                {
                    board[y + 1] |= (2 << x);
                    ret += Count(dict, board, x, y);
                    board[y + 1] &= ~(2 << x);
                }
                board[y] &= ~(2 << x);
            }
            if (IsEmpty(board, x, y + 1))
            {
                board[y + 1] |= (1 << x);
                // vertical line
                if (IsEmpty(board, x, y + 2))
                {
                    board[y + 2] |= (1 << x);
                    ret += Count(dict, board, x, y);
                    board[y + 2] &= ~(1 << x);
                }
                // buttomleft triangle
                if (IsEmpty(board, x + 1, y + 1))
                {
                    board[y + 1] |= (2 << x);
                    ret += Count(dict, board, x, y);
                    board[y + 1] &= ~(2 << x);
                }
                // buttomright triangle
                if (IsEmpty(board, x - 1, y + 1))
                {
                    board[y + 1] |= (1 << (x - 1));
                    ret += Count(dict, board, x, y);
                    board[y + 1] &= ~(1 << (x - 1));
                }
                board[y + 1] &= ~(1 << x);
            }
            board[y] &= ~(1 << x);
            dict[key] = ret;

            return ret;
        }

        protected override string Action()
        {
            var dict = new Dictionary<string, long>();
            var board = new int[height];

            return Count(dict, board, 0, 0).ToString();
        }
    }

    /// <summary>
    /// In the hexadecimal number system numbers are represented using 16 different
    /// digits:
    ///
    /// 0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F
    ///
    /// The hexadecimal number AF when written in the decimal number system equals
    /// 10x16+15=175.
    ///
    /// In the 3-digit hexadecimal numbers 10A, 1A0, A10, and A01 the digits 0,1 and A
    /// are all present.
    /// Like numbers written in base ten we write hexadecimal numbers without leading
    /// zeroes.
    ///
    /// How many hexadecimal numbers containing at most sixteen hexadecimal digits
    /// exist with all of the digits 0,1, and A present at least once?
    /// Give your answer as a hexadecimal number.
    ///
    /// (A,B,C,D,E and F in upper case, without any leading or trailing code that marks
    /// the number as hexadecimal and without leading zeroes , e.g. 1A3F and not: 1a3f
    /// and not 0x1a3f and not $1A3F and not #1A3F and not 0000001A3F)
    /// </summary>
    internal class Problem162 : Problem
    {
        private const int length = 16;

        public Problem162() : base(162) { }

        protected override string Action()
        {
            var one16 = new BigInteger[length];
            var one15 = new BigInteger[length];
            var one14 = new BigInteger[length];
            var two16 = new BigInteger[length];
            var two15 = new BigInteger[length];
            var three = new BigInteger[length];
            BigInteger counter = 0;

            // one letter exists at least once
            for (int l = 1; l < length; l++)
            {
                one16[l] = BigInteger.Pow(16, l) - BigInteger.Pow(15, l);
                one15[l] = BigInteger.Pow(15, l) - BigInteger.Pow(14, l);
                one14[l] = BigInteger.Pow(14, l) - BigInteger.Pow(13, l);
            }
            // two letters exist at least once
            for (int l = 2; l < length; l++)
            {
                two16[l] = 0;
                two15[l] = 0;
                // letter 1 exist only l1 times
                for (int l1 = 1; l1 < l; l1++)
                {
                    two16[l] += Probability.CountCombinations(l, l1) * one15[l - l1];
                    two15[l] += Probability.CountCombinations(l, l1) * one14[l - l1];
                }
            }
            // three letters exist at least once
            for (int l = 3; l < length; l++)
            {
                three[l] = 0;
                // letter 1 exist only l1 times
                for (int l1 = 1; l1 < l - 1; l1++)
                    three[l] += Probability.CountCombinations(l, l1) * two15[l - l1];
            }

            for (int l = 3; l <= length; l++)
            {
                // first letter is 1
                counter += two16[l - 1];
                // first letter is A
                counter += two16[l - 1];
                // first letter is others
                counter += three[l - 1] * 13;
            }

            return counter.ToString("X");
        }
    }

    /// <summary>
    /// Consider an equilateral triangle in which straight lines are drawn from each
    /// vertex to the middle of the opposite side, such as in the size 1 triangle in
    /// the sketch below.
    ///
    /// Sixteen triangles of either different shape or size or orientation or location
    /// can now be observed in that triangle. Using size 1 triangles as building
    /// blocks, larger triangles can be formed, such as the size 2 triangle in the
    /// above sketch. One-hundred and four triangles of either different shape or size
    /// or orientation or location can now be observed in that size 2 triangle.
    ///
    /// It can be observed that the size 2 triangle contains 4 size 1 triangle building
    /// blocks. A size 3 triangle would contain 9 size 1 triangle building blocks and a
    /// size n triangle would thus contain n^2 size 1 triangle building blocks.
    ///
    /// If we denote T(n) as the number of triangles present in a triangle of size n,
    /// then
    ///
    /// T(1) = 16
    /// T(2) = 104
    ///
    /// Find T(36).
    /// </summary>
    internal class Problem163 : Problem
    {
        private const int size = 36;

        public Problem163() : base(163) { }

        private int CalculateABC()
        {
            return (2 * size * size * size + 5 * size * size + 2 * size - (size % 2)) / 8;
        }

        private int CalculateDEF()
        {
            return (3 * size * size * size - size - 2 * (size % 3)) / 3;
        }

        private int CalculateABF()
        {
            return (2 * size * size * size + 3 * size * size - 2 * size - 3 * (size % 2)) / 12;
        }

        private int CalculateBDF()
        {
            return (14 * size * size * size + 33 * size * size + 4 * size - 6 * (size % 4) + 3 * (size % 2)) / 48;
        }

        private int CalculateABD()
        {
            return (5 * size * size * size + 12 * size * size + 3 * size - 2 * (size % 3)) / 18;
        }

        private int CalculateADF()
        {
            return (18 * size * size * size + 37 * size * size - 2 * size - 5 * (size % 2)
                - 8 * ((size * size * size - size * size + size) % 5)) / 40;
        }

        protected override string Action()
        {
            BigInteger sum = 0;

            /**
             * http://www.mathpuzzle.com/bdalytriangles.html
             * Mark lines which has an angle of 0, 60, 120 between itself and the horizontal
             * line as A, B, C, lines which has an angle of 30, 90, 150 as D, E, F
             */
            sum += CalculateABC();
            sum += CalculateDEF();
            sum += CalculateABF() * 3;
            sum += CalculateBDF() * 3;
            sum += CalculateABD() * 6;
            sum += CalculateADF() * 6;

            return sum.ToString();
        }
    }

    /// <summary>
    /// How many 20 digit numbers n (without any leading zero) exist such that no three
    /// consecutive digits of n have a sum greater than 9?
    /// </summary>
    internal class Problem164 : Problem
    {
        private const int length = 20;

        public Problem164() : base(164) { }

        private string GetIDX(int sum, int b, int c, int l)
        {
            return string.Join("|", new int[] { sum, b, c, l });
        }

        private long Count(Dictionary<string, long> dict, int sum, int b, int c, int l)
        {
            var key = GetIDX(sum, b, c, l);
            long counter = 0;

            if (l == 0)
                return sum < 10 ? 1 : 0;
            if (dict.ContainsKey(key))
                return dict[key];

            for (int i = 0; i < 10 - b - c; i++)
                counter += Count(dict, b + c + i, c, i, l - 1);
            dict.Add(key, counter);

            return counter;
        }

        protected override string Action()
        {
            var dict = new Dictionary<string, long>();
            long counter = 0;

            for (int leading = 1; leading < 10; leading++)
                counter += Count(dict, leading, 0, leading, length - 1);

            return counter.ToString();
        }
    }

    /// <summary>
    /// A segment is uniquely defined by its two endpoints.
    /// By considering two line segments in plane geometry there are three
    /// possibilities:
    /// the segments have zero points, one point, or infinitely many points in common.
    ///
    /// Moreover when two segments have exactly one point in common it might be the
    /// case that that common point is an endpoint of either one of the segments or of
    /// both. If a common point of two segments is not an endpoint of either of the
    /// segments it is an interior point of both segments.
    /// We will call a common point T of two segments L1 and L2 a true intersection
    /// point of L1 and L2 if T is the only common point of L1 and L2 and T is an
    /// interior point of both segments.
    ///
    /// Consider the three segments L1, L2, and L3:
    /// L1: (27, 44) to (12, 32)
    /// L2: (46, 53) to (17, 62)
    /// L3: (46, 70) to (22, 40)
    ///
    /// It can be verified that line segments L2 and L3 have a true intersection point.
    /// We note that as the one of the end points of L3: (22,40) lies on L1 this is not
    /// considered to be a true point of intersection. L1 and L2 have no common point.
    /// So among the three line segments, we find one true intersection point.
    ///
    /// Now let us do the same for 5000 line segments. To this end, we generate 20000
    /// numbers using the so-called "Blum Blum Shub" pseudo-random number generator.
    ///
    /// s(0) = 290797
    /// s(n+1) = s(n)*s(n) (modulo 50515093)
    /// t(n) = s(n) (modulo 500)
    ///
    /// To create each line segment, we use four consecutive numbers t(n). That is, the
    /// first line segment is given by:
    ///
    /// (t1, t2) to (t3, t4)
    ///
    /// The first four numbers computed according to the above generator should be: 27,
    /// 144, 12 and 232. The first segment would thus be (27,144) to (12,232).
    ///
    /// How many distinct true intersection points are found among the 5000 line
    /// segments?
    /// </summary>
    internal class Problem165 : Problem
    {
        private const int upper = 5000;

        public Problem165() : base(165) { }

        private int[] GenerateNumber()
        {
            var ret = new int[upper * 4];
            long s = 290797;

            for (int i = 0; i < upper * 4; i++)
            {
                s = s * s % 50515093;
                ret[i] = (int)(s % 500);
            }

            return ret;
        }

        private Tuple<SmallFraction, SmallFraction> zero = new Tuple<SmallFraction, SmallFraction>(0, 0);

        private Tuple<SmallFraction, SmallFraction> GetIntersection(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
        {
            // http://paulbourke.net/geometry/lineline2d/
            SmallFraction ua, ub, denominator = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);

            if (denominator == 0)
                return zero;

            ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / denominator;
            ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / denominator;
            if (ua <= 0 || ua >= 1 || ub <= 0 || ub >= 1)
                return zero;

            return new Tuple<SmallFraction, SmallFraction>(x1 + ua * (x2 - x1), y1 + ua * (y2 - y1));
        }

        protected override string Action()
        {
            var points = new HashSet<Tuple<SmallFraction, SmallFraction>>();
            var ns = GenerateNumber();

            foreach (var pair in Itertools.Combinations(Itertools.Range(0, upper - 1), 2))
            {
                int a = pair[0] * 4, b = pair[1] * 4;

                points.Add(GetIntersection(ns[a], ns[a + 1], ns[a + 2], ns[a + 3], ns[b], ns[b + 1], ns[b + 2], ns[b + 3]));
            }

            return (points.Count - 1).ToString();
        }
    }

    /// <summary>
    /// A 4x4 grid is filled with digits d, 0 <= d <= 9.
    ///
    /// It can be seen that in the grid
    ///
    /// 6 3 3 0
    /// 5 0 4 3
    /// 0 7 1 4
    /// 1 2 4 5
    ///
    /// the sum of each row and each column has the value 12. Moreover the sum of each
    /// diagonal is also 12.
    ///
    /// In how many ways can you fill a 4x4 grid with the digits d, 0 <= d <= 9 so that
    /// each row, each column, and both diagonals have the same sum?
    /// </summary>
    internal class Problem166 : Problem
    {
        public Problem166() : base(166) { }

        private string GetIDX(int[] h, int[] v, int[] d, int y)
        {
            var array = new List<int>();

            array.Add(y);
            array.Add(h[0]);
            array.AddRange(v);
            array.AddRange(d);

            return string.Join("|", array);
        }

        private int Count(Dictionary<string, int> dict, int[] h, int[] v, int[] d, int x, int y)
        {
            string key = null;
            int ret = 0, lower = 0, upper = 9;

            // last line, numbers are fixed
            if (y == 3)
            {
                if (v[0] == d[1] && v[3] == d[0] && v.Sum() == h[0] * 3)
                    return 1;
                else
                    return 0;
            }
            // Dynamic record when a new line started
            if (x == 0)
            {
                key = GetIDX(h, v, d, y);

                if (dict.ContainsKey(key))
                    return dict[key];
            }

            lower = Math.Max(lower, h[0] - h[y] - 9 * (3 - x));
            lower = Math.Max(lower, h[0] - v[x] - 9 * (3 - y));
            if (x == y)
                lower = Math.Max(lower, h[0] - d[0] - 9 * (3 - x));
            if (x + y == 3)
                lower = Math.Max(lower, h[0] - d[1] - 9 * (3 - y));
            upper = Math.Min(h[0] - h[y], upper);
            upper = Math.Min(h[0] - v[x], upper);
            if (x == y)
                upper = Math.Min(h[0] - d[0], upper);
            if (x + y == 3)
                upper = Math.Min(h[0] - d[1], upper);

            for (int i = lower; i <= upper; i++)
            {
                v[x] += i;
                h[y] += i;
                if (x == y)
                    d[0] += i;
                if (x + y == 3)
                    d[1] += i;
                ret += Count(dict, h, v, d, (x + 1) % 4, x == 3 ? y + 1 : y);
                if (x + y == 3)
                    d[1] -= i;
                if (x == y)
                    d[0] -= i;
                h[y] -= i;
                v[x] -= i;
            }

            if (x == 0)
                dict.Add(key, ret);

            return ret;
        }

        protected override string Action()
        {
            var dict = new Dictionary<string, int>();
            var counter = 0;

            foreach (var head in Itertools.PermutationsWithReplacement(Itertools.Range(0, 9), 4))
                counter += Count(dict, new int[] { head.Sum(), 0, 0, 0 }, head, new int[] { head[0], head[3] }, 0, 1);

            return counter.ToString();
        }
    }

    /// <summary>
    /// For two positive integers a and b, the Ulam sequence U(a,b) is defined by
    /// U(a,b)(1) = a, U(a,b)(2) = b and for k > 2, U(a,b)(k) is the smallest integer
    /// greater than U(a,b)(k-1) which can be written in exactly one way as the sum of
    /// two distinct previous members of U(a,b).
    ///
    /// For example, the sequence U(1,2) begins with
    /// 1, 2, 3 = 1 + 2, 4 = 1 + 3, 6 = 2 + 4, 8 = 2 + 6, 11 = 3 + 8;
    /// 5 does not belong to it because 5 = 1 + 4 = 2 + 3 has two representations as
    /// the sum of two previous members, likewise 7 = 1 + 6 = 3 + 4.
    ///
    /// Find Σ(U(2,2n+1)(k)) for 2 <= n <= 10, where k = 10^11.
    /// </summary>
    internal class Problem167 : Problem
    {
        private const long upper = 100000000000;

        public Problem167() : base(167) { }

        private List<int> Generate(int a, int b, int size)
        {
            var ret = new List<int>();
            var eliminated = new HashSet<int>();
            var single = new SortedSet<int>();
            int even = 0, index = 0;

            ret.Add(a);
            ret.Add(b);
            single.Add(a + b);

            /**
             * http://mathworld.wolfram.com/UlamSequence.html
             *
             * Schmerl and Spiegel (1994) proved that Ulam sequences (2,v) for odd v >= 5 have exactly
             * two even terms.
             * Ulam sequences with only finitely many even terms eventually must have periodic successive
             * differences (Finch 1991, 1992abc).
             *
             * some magic tricks from google, using even/index to generate ulam sequences
             */
            while (even == 0 || ret[ret.Count - 1] < 2 * even)
            {
                foreach (var i in ret)
                {
                    if (eliminated.Contains(i + single.Min))
                        continue;
                    if (single.Contains(i + single.Min))
                    {
                        single.Remove(i + single.Min);
                        eliminated.Add(i + single.Min);
                    }
                    else
                    {
                        single.Add(i + single.Min);
                    }
                }
                ret.Add(single.Min);
                single.Remove(single.Min);
                if (ret[ret.Count - 1] % 2 == 0)
                    even = ret[ret.Count - 1];
            }
            while (even + ret[index] <= ret[ret.Count - 1])
                index++;
            while (ret.Count < size)
            {
                if (even + ret[index] > ret[ret.Count - 1] + 2)
                {
                    ret.Add(ret[ret.Count - 1] + 2);
                }
                else
                {
                    ret.Add(even + ret[index + 1]);
                    index += 2;
                }
            }

            return ret;
        }

        private int GetRepetition(List<int> seq)
        {
            var differences = new int[seq.Count - 1];

            for (int i = 1; i < seq.Count; i++)
                differences[i - 1] = seq[i] - seq[i - 1];
            for (int l = differences.Length / 2; l > 0; l--)
            {
                bool flag = true;

                for (int i = 1; i <= l; i++)
                {
                    if (differences[differences.Length - i] != differences[differences.Length - l - i])
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                    return l;
            }

            throw new ArgumentException("no cycle found");
        }

        protected override string Action()
        {
            long sum = 0;

            for (int n = 2; n <= 10; n++)
            {
                var seq = Generate(2, 2 * n + 1, 1024 * 1024 * 8);
                var l = GetRepetition(seq);
                var head = seq.Count - l;
                var start = seq[head - 1];
                var cycle = seq[seq.Count - 1] - start;
                var left = seq[head - 1 + (int)((upper - head) % l)] - start;

                sum += start + (upper - head) / l * cycle + left;
            }

            return sum.ToString();
        }
    }

    /// <summary>
    /// Consider the number 142857. We can right-rotate this number by moving the last
    /// digit (7) to the front of it, giving us 714285.
    /// It can be verified that 714285 = 5 * 142857.
    /// This demonstrates an unusual property of 142857: it is a divisor of its
    /// right-rotation.
    ///
    /// Find the last 5 digits of the sum of all integers n, 10 < n < 10^100, that have
    /// this property.
    /// </summary>
    internal class Problem168 : Problem
    {
        private const int length = 100;

        public Problem168() : base(168) { }

        protected override string Action()
        {
            BigInteger sum = 0;
            var a = new int[length];

            /**
             * for number an...a2a1a0 * k = a0an...a2a1
             */
            for (int k = 1; k < 10; k++)
            {
                for (a[0] = 1; a[0] < 10; a[0]++)
                {
                    var c = 0;

                    for (int n = 1; n < length; n++)
                    {
                        var tmp = a[n - 1] * k + c;

                        a[n] = tmp % 10;
                        c = tmp / 10;
                        if (a[n] != 0 && a[n] * k + c == a[0])
                        {
                            var num = 0;

                            for (int i = Math.Min(4, n); i >= 0; i--)
                                num = num * 10 + a[i];
                            sum += num;
                        }
                    }
                }
            }

            return (sum % 100000).ToString();
        }
    }

    /// <summary>
    /// Define f(0)=1 and f(n) to be the number of different ways n can be expressed as
    /// a sum of integer powers of 2 using each power no more than twice.
    ///
    /// For example, f(10)=5 since there are five different ways to express 10:
    /// 1 + 1 + 8
    /// 1 + 1 + 4 + 4
    /// 1 + 1 + 2 + 2 + 4
    /// 2 + 4 + 4
    /// 2 + 8
    ///
    /// What is f(10^25)?
    /// </summary>
    internal class Problem169 : Problem
    {
        private const int pow = 25;

        public Problem169() : base(169) { }

        private long Calculate(Dictionary<string, long> dict, List<int> binary, int[] counter, int idx)
        {
            var key = string.Join("", binary.Take(idx + 1)) + "|" + string.Join("", counter.Take(idx + 1));
            long ret = 0;

            if (dict.ContainsKey(key))
                return dict[key];
            while (idx > 0 && binary[idx] == 0)
                idx--;
            if (idx == 0)
                return counter[0] + binary[0] <= 2 ? 1 : 0;

            if (binary[idx] == 2)
            {
                // two 2^idx
                if (counter[idx] == 0)
                    ret += Calculate(dict, binary, counter, idx - 1);
                // one 2^idx, one 2^(idx-1)
                if (counter[idx] <= 1)
                {
                    binary[idx - 1]++;
                    counter[idx - 1]++;
                    ret += Calculate(dict, binary, counter, idx - 1);
                    binary[idx - 1]--;
                    counter[idx - 1]--;
                }
            }
            else
            {
                // one 2^idx
                if (counter[idx] <= 1)
                    ret += Calculate(dict, binary, counter, idx - 1);
                // one 2^(idx-1)
                binary[idx - 1]++;
                counter[idx - 1]++;
                ret += Calculate(dict, binary, counter, idx - 1);
                binary[idx - 1]--;
                counter[idx - 1]--;
            }

            dict.Add(key, ret);

            return ret;
        }

        protected override string Action()
        {
            var upper = BigInteger.Pow(10, 25);
            var dict = new Dictionary<string, long>();
            var binary = new List<int>();

            while (upper > 0)
            {
                binary.Add(upper.IsEven ? 0 : 1);
                upper >>= 1;
            }

            return Calculate(dict, binary, new int[binary.Count], binary.Count - 1).ToString();
        }
    }
}