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
    /// Consider the set S(r) of points (x,y) with integer coordinates satisfying
    /// |x| + |y| <= r.
    /// Let O be the point (0, 0) and C the point (r/4, r/4).
    /// Let N(r) be the number of points B in S(r), so that the triangle OBC has an
    /// obtuse angle, i.e. the largest angle α satisfies 90° < α < 180°.
    /// So, for example, N(4) = 24 and N(8) = 100.
    /// What is N(1,000,000,000)?
    /// </summary>
    internal class Problem210 : Problem
    {
        private const long radius = 1000000000;

        public Problem210() : base(210) { }

        protected override string Action()
        {
            long counter = 0;

            /**
             * OBC is obtuse, O(0, 0), B(x, y), C(r/4, r/4)
             * OB^2 = x^2 + y^2, OC^2 = r^2/8, BC^2 = x^2 + y^2 + r^2/8 - (x+y)r/2
             *
             * case 1: OB^2 > OC^2 + BC^2
             *   0 > r^2/4 - (x+y)r/2 => x+y > r/2
             *   All points in right side of x+y=r/2
             *
             * case 2: BC^2 > OB^2 + OC^2
             *   -(x+y)r/2 > 0 => 0 > x+y
             *   All points in left side of x+y = 0
             *
             * case 3: OC^2 > OB^2 + BC^2
             *   0 > 2x^2 - xr/2 + 2y^2 - yr/2 => (x+y)r/4 > x^2 + y^2
             *   All points inside the circle at (r/8, r/8), radius r/8*sqrt(2)
             */

            if (radius % 8 != 0)
                throw new ArgumentException("Can's use gaussian circle formula");
            // case 1, minus points where x = y
            counter += (radius + 1 + radius) * radius / 4;
            // case 2, minus points where x = y
            counter += (radius + 1 + radius) * radius / 2;
            // case 3, minus points where x = y, must not on the circle
            counter += GaussianCircle.Count(radius * radius / 32 - 1);
            // remove points at (n, n), (0, 0) and (r/4, r/4) is already excluded
            counter -= radius - 1;

            return counter.ToString();
        }
    }

    /// <summary>
    /// For a positive integer n, let σ2(n) be the sum of the squares of its divisors.
    /// For example,
    ///
    /// σ2(10) = 1 + 4 + 25 + 100 = 130.
    ///
    /// Find the sum of all n, 0 < n < 64,000,000 such that σ2(n) is a perfect square.
    /// </summary>
    internal class Problem211 : Problem
    {
        private const int upper = 64000000;

        public Problem211() : base(211) { }

        protected override string Action()
        {
            var sigma2 = new long[upper];
            long sum = 0;

            for (int i = 1; i < upper; i++)
            {
                var s = (long)i * i;

                for (int k = i; k < upper; k += i)
                    sigma2[k] += s;
            }

            for (int i = 1; i < upper; i++)
            {
                if (Misc.IsPerfectSquare(sigma2[i]))
                    sum += i;
            }

            return sum.ToString();
        }
    }

    /// <summary>
    /// An axis-aligned cuboid, specified by parameters {(x0,y0,z0), (dx,dy,dz)},
    /// consists of all points (X,Y,Z) such that x0 < X < x0+dx, y0 < Y < y0+dy and
    /// z0 < Z < z0+dz. The volume of the cuboid is the product, dx X dy X dz. The
    /// combined volume of a collection of cuboids is the volume of their union and
    /// will be less than the sum of the individual volumes if any cuboids overlap.
    ///
    /// Let C1, ..., C50000 be a collection of 50000 axis-aligned cuboids such that
    /// Cn has parameters
    ///
    /// x0 = S(6n-5) modulo 10000
    /// y0 = S(6n-4) modulo 10000
    /// z0 = S(6n-3) modulo 10000
    /// dx = 1 + (S(6n-2) modulo 399)
    /// dy = 1 + (S(6n-1) modulo 399)
    /// dz = 1 + (S(6n) modulo 399)
    ///
    /// where S1, ..., S300000 come from the "Lagged Fibonacci Generator":
    ///
    /// For 1 <= k <= 55, S(k) = [100003 - 200003k + 300007k^3] (modulo 1000000)
    /// For 56 <= k, Sk = [S(k-24) + S(k-55)] (modulo 1000000)
    ///
    /// Thus, C1 has parameters {(7, 53, 183), (94, 369, 56)}, C2 has parameters
    /// {(2383, 3563, 5079), (42, 212, 344)}, and so on.
    ///
    /// The combined volume of the first 100 cuboids, C1,...,C100, is 723581599.
    ///
    /// What is the combined volume of all 50000 cuboids, C1,...,C50000?
    /// </summary>
    internal class Problem212 : Problem
    {
        private const int upper = 50000;

        public Problem212() : base(212) { }

        private List<int[]> Generate()
        {
            var c = new List<int[]>();
            var xyz = new int[6];
            int counter = 0;

            foreach (var value in PseudoNumberGenerator.GenerateLaggedFibonacci())
            {
                if (counter % 6 < 3)
                    xyz[counter % 6] = value % 10000;
                else
                    xyz[counter % 6] = value % 399 + 1;
                if (counter % 6 == 5)
                {
                    c.Add(xyz);
                    xyz = new int[6];
                }
                if (++counter == upper * 6)
                    break;
            }

            return c;
        }

        private void GetIntersectionPoint(int start1, int end1, int start2, int end2, ref int start, ref int len)
        {
            if (start1 >= end2 || start2 >= end1)
                return;
            start = start1 > start2 ? start1 : start2;
            len = end1 < end2 ? end1 - start : end2 - start;
        }

        private int[] GetIntersection(int[] cube1, int[] cube2)
        {
            var ret = new int[6];

            // X axis
            GetIntersectionPoint(cube1[0], cube1[0] + cube1[3], cube2[0], cube2[0] + cube2[3], ref ret[0], ref ret[3]);
            if (ret[3] == 0)
                return null;
            // Y axis
            GetIntersectionPoint(cube1[1], cube1[1] + cube1[4], cube2[1], cube2[1] + cube2[4], ref ret[1], ref ret[4]);
            if (ret[4] == 0)
                return null;
            // Z axis
            GetIntersectionPoint(cube1[2], cube1[2] + cube1[5], cube2[2], cube2[2] + cube2[5], ref ret[2], ref ret[5]);
            if (ret[5] == 0)
                return null;

            return ret;
        }

        private long Calculate(List<int[]> cubes, int[] c, int id)
        {
            long volume = c[3] * c[4] * c[5];
            int[] inter;

            // using inclusive/exclusive principle
            for (int i = id + 1; i < upper; i++)
            {
                inter = GetIntersection(c, cubes[i]);
                if (inter == null)
                    continue;

                volume -= Calculate(cubes, inter, i);
            }

            return volume;
        }

        protected override string Action()
        {
            List<int[]> cubes = Generate();
            long volume = 0;

            for (int i = 0; i < upper; i++)
                volume += Calculate(cubes, cubes[i], i);

            return volume.ToString();
        }
    }

    /// <summary>
    /// A 30*30 grid of squares contains 900 fleas, initially one flea per square.
    /// When a bell is rung, each flea jumps to an adjacent square at random (usually 4
    /// possibilities, except for fleas on the edge of the grid or at the corners).
    ///
    /// What is the expected number of unoccupied squares after 50 rings of the bell?
    /// Give your answer rounded to six decimal places.
    /// </summary>
    internal class Problem213 : Problem
    {
        private const int size = 30;
        private const int times = 50;

        public Problem213() : base(213) { }

        private double[,] RingTheBell(double[,] array)
        {
            var ret = new double[size, size];

            // Four corners
            ret[0, 1] += array[0, 0] / 2;
            ret[1, 0] += array[0, 0] / 2;
            ret[size - 2, 0] += array[size - 1, 0] / 2;
            ret[size - 1, 1] += array[size - 1, 0] / 2;
            ret[0, size - 2] += array[0, size - 1] / 2;
            ret[1, size - 1] += array[0, size - 1] / 2;
            ret[size - 2, size - 1] += array[size - 1, size - 1] / 2;
            ret[size - 1, size - 2] += array[size - 1, size - 1] / 2;
            // Four sides
            for (int i = 1; i < size - 1; i++)
            {
                ret[0, i - 1] += array[0, i] / 3;
                ret[1, i] += array[0, i] / 3;
                ret[0, i + 1] += array[0, i] / 3;
                ret[size - 1, i - 1] += array[size - 1, i] / 3;
                ret[size - 2, i] += array[size - 1, i] / 3;
                ret[size - 1, i + 1] += array[size - 1, i] / 3;
                ret[i - 1, 0] += array[i, 0] / 3;
                ret[i, 1] += array[i, 0] / 3;
                ret[i + 1, 0] += array[i, 0] / 3;
                ret[i - 1, size - 1] += array[i, size - 1] / 3;
                ret[i, size - 2] += array[i, size - 1] / 3;
                ret[i + 1, size - 1] += array[i, size - 1] / 3;
            }
            // Inner points
            for (int x = 1; x < size - 1; x++)
                for (int y = 1; y < size - 1; y++)
                {
                    ret[x - 1, y] += array[x, y] / 4;
                    ret[x + 1, y] += array[x, y] / 4;
                    ret[x, y - 1] += array[x, y] / 4;
                    ret[x, y + 1] += array[x, y] / 4;
                }

            return ret;
        }

        protected override string Action()
        {
            var array = new List<List<double[,]>>();
            double sum = 0;

            for (int x = 0; x < size; x++)
            {
                array.Add(new List<double[,]>());
                for (int y = 0; y < size; y++)
                {
                    array[x].Add(new double[size, size]);
                    array[x][y][x, y] = 1;
                    for (int t = 0; t < times; t++)
                        array[x][y] = RingTheBell(array[x][y]);
                }
            }
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    double e = 1;

                    // calculate the possibility of none flea in the point
                    for (int i = 0; i < size * size; i++)
                        e *= (1 - array[i / size][i % size][x, y]);

                    sum += e;
                }
            }

            return sum.ToString("F6");
        }
    }

    /// <summary>
    /// Let φ be Euler's totient function, i.e. for a natural number n, φ(n) is the
    /// number of k, 1 <= k <= n, for which gcd(k,n) = 1.
    ///
    /// By iterating φ, each positive integer generates a decreasing chain of numbers
    /// ending in 1.
    /// E.g. if we start with 5 the sequence 5,4,2,1 is generated.
    /// Here is a listing of all chains with length 4:
    ///
    /// 5,4,2,1
    /// 7,6,2,1
    /// 8,4,2,1
    /// 9,6,2,1
    /// 10,4,2,1
    /// 12,4,2,1
    /// 14,6,2,1
    /// 18,6,2,1
    ///
    /// Only two of these chains start with a prime, their sum is 12.
    ///
    /// What is the sum of all primes less than 40000000 which generate a chain of
    /// length 25?
    /// </summary>
    internal class Problem214 : Problem
    {
        private const int upper = 40000000;

        public Problem214() : base(214) { }

        private int GetLength(Prime p, int[] next, int[] len, int n)
        {
            if (next[n] != 0)
                return len[n];

            next[n] = EulerPhi.GetPhi(p, n);
            len[n] = GetLength(p, next, len, next[n]) + 1;

            return len[n];
        }

        protected override string Action()
        {
            var p = new Prime(upper);
            int[] next = new int[upper];
            int[] len = new int[upper];
            long sum = 0;

            next[1] = 1;
            len[1] = 1;

            p.GenerateAll();
            foreach (var n in p)
            {
                if (GetLength(p, next, len, n) == 25)
                    sum += n;
            }

            return sum.ToString();
        }
    }

    /// <summary>
    /// Consider the problem of building a wall out of 2x1 and 3x1 bricks (horizontal
    /// x vertical dimensions) such that, for extra strength, the gaps between
    /// horizontally-adjacent bricks never line up in consecutive layers, i.e. never
    /// form a "running crack".
    ///
    /// For example, the following 9x3 wall is not acceptable due to the running crack
    /// shown in red:
    ///
    /// There are eight ways of forming a crack-free 9x3 wall, written W(9, 3) = 8.
    ///
    /// Calculate W(32, 10).
    /// </summary>
    internal class Problem215 : Problem
    {
        private const int length = 32;
        private const int height = 10;

        public Problem215() : base(215) { }

        private void GeneratePattern(List<string> list, List<int> elements, int left)
        {
            if (left == 0)
                list.Add(string.Join("", elements));
            if (left < 2)
                return;

            elements.Add(2);
            GeneratePattern(list, elements, left - 2);
            if (left > 2)
            {
                elements[elements.Count - 1] = 3;
                GeneratePattern(list, elements, left - 3);
            }

            elements.RemoveAt(elements.Count - 1);
        }

        private bool Check(string left, string right)
        {
            int p1 = 0, p2 = 0, l1 = left[0] - '0', l2 = right[0] - '0';

            while (l1 != l2)
            {
                if (l1 > l2)
                    l2 += right[++p2] - '0';
                else
                    l1 += left[++p1] - '0';
            }

            return l1 == length;
        }

        private long[] GetNextCounter(HashSet<int>[] next, long[] counter)
        {
            var ret = new long[counter.Length];

            for (int i = 0; i < next.Length; i++)
            {
                foreach (var j in next[i])
                    ret[i] += counter[j];
            }

            return ret;
        }

        protected override string Action()
        {
            var pattern = new List<string>();
            long sum = 0;

            GeneratePattern(pattern, new List<int>(), length);

            var next = new HashSet<int>[pattern.Count];
            var counter = new long[pattern.Count];

            for (int i = 0; i < pattern.Count; i++)
                next[i] = new HashSet<int>();
            for (int i = 0; i < pattern.Count; i++)
            {
                counter[i] = 1;
                for (int j = i + 1; j < pattern.Count; j++)
                {
                    if (Check(pattern[i], pattern[j]))
                    {
                        next[i].Add(j);
                        next[j].Add(i);
                    }
                }
            }

            for (int i = 1; i < height; i++)
                counter = GetNextCounter(next, counter);
            foreach (var c in counter)
                sum += c;

            return sum.ToString();
        }
    }

    /// <summary>
    /// Consider numbers t(n) of the form t(n) = 2n^2-1 with n > 1.
    /// The first such numbers are 7, 17, 31, 49, 71, 97, 127 and 161.
    /// It turns out that only 49 = 7*7 and 161 = 7*23 are not prime.
    /// For n <= 10000 there are 2202 numbers t(n) that are prime.
    ///
    /// How many numbers t(n) are prime for n <= 50,000,000 ?
    /// </summary>
    internal class Problem216 : Problem
    {
        private const long upper = 50000000;

        public Problem216() : base(216) { }

        private long TonelliShanks(long n, int p)
        {
            // http://en.wikipedia.org/wiki/Shanks-Tonelli_algorithm
            Modulo modulo = new Modulo(p);
            long s, q, z, t, c;

            // Step 1:
            if (p % 4 == 3)
                return modulo.Pow(n, (p + 1) / 4);
            s = 0;
            q = p - 1;
            while ((q & 1) == 0)
            {
                s++;
                q /= 2;
            }

            // Step 2:
            z = 2;
            while ((t = modulo.Pow(z, (p - 1) / 2)) == 1)
                z++;
            c = modulo.Pow(z, q);

            // Step 3:
            long r = modulo.Pow(n, (q + 1) / 2);

            // Step 4:
            while (true)
            {
                long u = (2 * r * r) % p, i = 0;

                while (u != 1)
                {
                    u = modulo.Mul(u, u);
                    i++;
                }
                if (i == 0)
                    return r;
                u = c;
                while (i < s - 1)
                {
                    u = modulo.Mul(u, u);
                    i++;
                }
                r = modulo.Mul(r, u);
            }
        }

        protected override string Action()
        {
            /**
             * For an odd prime p: 2*n^2-1 = 0 mod p, 2*n^2 = 1 mod p
             * n^2 = (p+1)/2 mod p
             *
             */
            var prime = new Prime((int)Misc.Sqrt(2 * upper * upper));
            var flags = new bool[upper + 1];

            prime.GenerateAll();
            foreach (var p in prime.Nums.Skip(1))
            {
                // Only prime of form 8n+-1 is valid
                if (p % 8 != 1 && p % 8 != 7)
                    continue;

                int n = (int)TonelliShanks((p + 1) / 2, p);

                if (n > upper)
                    continue;
                for (long i = n; i <= upper; i += p)
                    if (2 * i * i - 1 > p)
                        flags[i] = true;
                for (long i = p - n; i <= upper; i += p)
                    if (2 * i * i - 1 > p)
                        flags[i] = true;
            }

            return flags.Skip(2).Count(it => !it).ToString();
        }
    }

    /// <summary>
    /// A positive integer with k (decimal) digits is called balanced if its first
    /// [k/2] digits sum to the same value as its last [k/2] digits, where [x],
    /// pronounced ceiling of x, is the smallest integer >= x, thus [π] = 4 and
    /// [5] = 5.
    ///
    /// So, for example, all palindromes are balanced, as is 13722.
    ///
    /// Let T(n) be the sum of all balanced numbers less than 10^n.
    /// Thus: T(1) = 45, T(2) = 540 and T(5) = 334795890.
    ///
    /// Find T(47) mod 3^15
    /// </summary>
    internal class Problem217 : Problem
    {
        private static Modulo modulo = new Modulo((long)BigInteger.Pow(3, 15));
        private const int length = 47;

        private class BSet
        {
            public int Length;
            public long[] LeftSum;
            public long[] RightSum;
            public long[] LeftCounter;
            public long[] RightCounter;

            public BSet(int l)
            {
                Length = l / 2;
                LeftSum = new long[9 * Length + 1];
                RightSum = new long[9 * Length + 1];
                LeftCounter = new long[9 * Length + 1];
                RightCounter = new long[9 * Length + 1];
            }

            public long GetSumOfBalancedNumbers()
            {
                checked
                {
                    // Get sum of all balanced numbers lr where sum(l) = sum(r)
                    long ret = 0;

                    for (int i = 0; i <= 9 * Length; i++)
                    {
                        // Add sum of l0
                        ret += modulo.Mul(LeftSum[i], RightCounter[i]);
                        // Add sum of 0r
                        ret += modulo.Mul(RightSum[i], LeftCounter[i]);
                        ret = modulo.Mod(ret);
                    }

                    return ret;
                }
            }

            public long GetSumOfBalancedNumbersPlus()
            {
                checked
                {
                    // Get sum of all balanced numbers l*r where sum(l) = sum(r)
                    long ret = 0;
                    long pow = modulo.Pow(10, Length);

                    for (int i = 0; i <= 9 * Length; i++)
                    {
                        // Add sum of l00
                        ret += modulo.Mul(LeftSum[i] * 10, RightCounter[i] * 10);
                        // Add sum of 00r
                        ret += modulo.Mul(RightSum[i], LeftCounter[i] * 10);
                        // Add sum of 0*0
                        ret += modulo.Mul(45 * pow, modulo.Mul(LeftCounter[i], RightCounter[i]));
                        ret = modulo.Mod(ret);
                    }

                    return ret;
                }
            }
        }

        public Problem217() : base(217) { }

        private BSet GetNext(BSet old)
        {
            checked
            {
                BSet ret = new BSet(old.Length * 2 + 2);
                long pow = modulo.Pow(10, old.Length);

                for (int suml = 1; suml <= old.Length * 9; suml++)
                {
                    for (int l = 0; l < 10; l++)
                    {
                        ret.LeftSum[suml + l] = modulo.Add(ret.LeftSum[suml + l], old.LeftSum[suml] * 100
                            + modulo.Mul(pow * 10 * l, old.LeftCounter[suml]));
                        ret.LeftCounter[suml + l] = modulo.Add(ret.LeftCounter[suml + l], old.LeftCounter[suml]);
                    }
                }
                for (int sumr = 0; sumr <= old.Length * 9; sumr++)
                {
                    for (int r = 0; r < 10; r++)
                    {
                        ret.RightSum[sumr + r] = modulo.Add(ret.RightSum[sumr + r], old.RightSum[sumr]
                            + modulo.Mul(r * pow, old.RightCounter[sumr]));
                        ret.RightCounter[sumr + r] = modulo.Add(ret.RightCounter[sumr + r], old.RightCounter[sumr]);
                    }
                }

                return ret;
            }
        }

        protected override string Action()
        {
            BSet s = new BSet(2);
            long sum = 45;
            int l, r;

            for (l = 1; l < 10; l++)
            {
                s.LeftSum[l] = l * 10;
                s.LeftCounter[l] = 1;
            }
            for (r = 0; r < 10; r++)
            {
                s.RightSum[r] = r;
                s.RightCounter[r] = 1;
            }

            for (l = 2; l <= length; l++)
            {
                if (l % 2 == 0)
                {
                    sum += s.GetSumOfBalancedNumbers();
                }
                else
                {
                    sum += s.GetSumOfBalancedNumbersPlus();
                    s = GetNext(s);
                }
            }

            return modulo.Mod(sum).ToString();
        }
    }

    /// <summary>
    /// Consider the right angled triangle with sides a=7, b=24 and c=25. The area of
    /// this triangle is 84, which is divisible by the perfect numbers 6 and 28.
    /// Moreover it is a primitive right angled triangle as gcd(a,b)=1 and gcd(b,c)=1.
    /// Also c is a perfect square.
    ///
    /// We will call a right angled triangle perfect if
    /// -it is a primitive right angled triangle
    /// -its hypotenuse is a perfect square
    ///
    /// We will call a right angled triangle super-perfect if
    /// -it is a perfect right angled triangle and
    /// -its area is a multiple of the perfect numbers 6 and 28.
    ///
    /// How many perfect right-angled triangles with c <= 10^16 exist that are not
    /// super-perfect?
    /// </summary>
    internal class Problem218 : Problem
    {
        private static long upper = (long)BigInteger.Pow(10, 16);

        public Problem218() : base(218) { }

        protected override string Action()
        {
            /**
             * Proof from forum thread:
             *
             * Let S1=(a,b,c) be a perfect right triangle
             * Let, c=k^2. k>0 and is an integer
             *
             * Also, a=i^2-j^2, b=2ij, c=i^2+j^2 for some i,j with i>j, i,j>0, gcd(i,j)=1 and i+j odd.
             * This comes from the definition of a primitive pythagorean triple.
             *
             * Then, i^2+j^2=k^2. Hence, S2=(i,j,k) is a pythagorean triple.
             * It can be shown that S2 is a primitive triple as k^2 mod i^2 = (i^2+j^2) mod i^2 = j^2 mod i^2.
             * Hence, gcd(k,i)=gcd(i,j) which is 1. Similarly, gcd(k,j)=1.
             *
             * Area A1 of S1 is ab/2 = ij(i^2-j^2).
             * For S1 to be super perfect, A1 must be divisible by 3,4 and 7.
             *
             * As S2 is a pythagorean triple, i and j are of the form r^2-s^2, 2rs where r>s>0 and r,s are integers.
             * Also, one of r,s is even.
             *
             * Hence, 2rs mod 4 = 0 or one of i,j mod 4 =0. Therefore, A1 mod 4 = 0
             *
             * Let r1=r mod 3 and s1 = s mod 3
             * If r1=0 or s1=0, 2rs mod 3 = 0.
             * If r1=s1, r^2-s^2 mod 3 = 0.
             * if r1!=s1 and r1,s1!=0, r1^2-s1^2 is in (-3,3) or congruent to 0 mod 3.
             * Therefore, either 2rs or r^2-s^2 mod 3 = 0 or one of i,j mod 3 = 0 and hence A1 mod 3 = 0
             *
             * Let i1=i mod 7, j1= j mod 7, k1 = k mod 7
             * if i1=0 or j1=0, A1 mod 7 = 0
             * Otherwise, let us consider i^2+j^=k^2
             * As any square mod 7 is in (0,1,2,4), the only non zero values for i1 and j1 that result in a valid value of k1 are for i1=j1.
             * This can be seen by enumerating all combinations of i1,j1 for i1,j1 in (1,2,4) and k1 in (0,1,2,4)
             * If i1=j1, the i^2-j^2 mod 7 = 0. Hence, A1 mod 7 = 0
             *
             * So, A1 mod 4 = 0, A1 mod 3 = 0 and A1 mod 7 = 0. Hence, A1 is divided by 6 (2*3) and 28 (4*7)
             * So, all S1 are super perfect.
             */

            return "0";
        }
    }

    /// <summary>
    /// Let A and B be bit strings (sequences of 0's and 1's).
    /// If A is equal to the leftmost length(A) bits of B, then A is said to be a
    /// prefix of B.
    /// For example, 00110 is a prefix of 001101001, but not of 00111 or 100110.
    ///
    /// A prefix-free code of size n is a collection of n distinct bit strings such
    /// that no string is a prefix of any other. For example, this is a prefix-free
    /// code of size 6:
    ///
    /// 0000, 0001, 001, 01, 10, 11
    ///
    /// Now suppose that it costs one penny to transmit a '0' bit, but four pence to
    /// transmit a '1'.
    /// Then the total cost of the prefix-free code shown above is 35 pence, which
    /// happens to be the cheapest possible for the skewed pricing scheme in question.
    /// In short, we write Cost(6) = 35.
    ///
    /// What is Cost(10^9)?
    /// </summary>
    internal class Problem219 : Problem
    {
        private const int size = 1000000000;

        public Problem219() : base(219) { }

        protected override string Action()
        {
            /**
             * All prefix-free codes are leaf nodes of a binary-tree
             *
             * Every time, you took away a leaf node and replace it with two leaf nodes beneath it.
             * Assume the node you take costs x, where x is the cost of the cheapest leaf available
             * C(2)=5, x=1
             * C(3)=11, x=2
             * C(4)=18, x=3
             * ...
             * Using a sorted stack would work
             *
             * Also, since we always pick the cheapest node from the stack and add x+1 and x+4 into the stack,
             * every element in the stack must differ at most 4, no need to implement a stack
             */
            int[] counter = new int[5];
            int current = 0, left = size - 1;
            long sum = 0;

            // First element 0
            counter[0] = 1;
            while (left != 0)
            {
                if (counter[0] <= left)
                {
                    left -= counter[0];
                    counter[1] += counter[0];
                    counter[4] += counter[0];
                    counter[0] = counter[1];
                    counter[1] = counter[2];
                    counter[2] = counter[3];
                    counter[3] = counter[4];
                    counter[4] = 0;
                    current++;
                }
                else
                {
                    counter[0] -= left;
                    counter[1] += left;
                    counter[4] += left;
                    left = 0;
                }
            }

            left = size;
            for (int i = 0; i < 5; i++)
            {
                if (left > counter[i])
                {
                    sum += (long)counter[i] * (current + i);
                    left -= counter[i];
                }
                else
                {
                    sum += (long)left * (current + i);
                    break;
                }
            }

            return sum.ToString();
        }
    }
}