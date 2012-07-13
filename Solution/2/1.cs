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

            foreach (var value in LaggedFibonacci.Generate())
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
}