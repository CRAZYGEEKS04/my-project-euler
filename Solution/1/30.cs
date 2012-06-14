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
    /// A number consisting entirely of ones is called a repunit. We shall define R(k)
    /// to be a repunit of length k; for example, R(6) = 111111.
    ///
    /// Given that n is a positive integer and GCD(n, 10) = 1, it can be shown that
    /// there always exists a value, k, for which R(k) is divisible by n, and let A(n)
    /// be the least such value of k; for example, A(7) = 6 and A(41) = 5.
    ///
    /// You are given that for all primes, p > 5, that p - 1 is divisible by A(p). For
    /// example, when p = 41, A(41) = 5, and 40 is divisible by 5.
    ///
    /// However, there are rare composite values for which this is also true; the first
    /// five examples being 91, 259, 451, 481, and 703.
    ///
    /// Find the sum of the first twenty-five composite values of n for which
    /// GCD(n, 10) = 1 and n - 1 is divisible by A(n).
    /// </summary>
    internal class Problem130 : Problem
    {
        private const int upper = 1000000;
        private const int counter = 25;

        public Problem130() : base(130) { }

        protected override string Action()
        {
            var p = new Prime(upper);
            var nums = new List<int>();

            p.GenerateAll();

            for (int n = 7; n < upper; n += 2)
            {
                if (n % 5 == 0)
                    continue;
                if (p.Contains(n))
                    continue;

                if ((n - 1) % RepUnit.GetA(n) == 0)
                {
                    nums.Add(n);
                    if (nums.Count == counter)
                        break;
                }
            }

            return nums.Sum().ToString();
        }
    }

    /// <summary>
    /// There are some prime values, p, for which there exists a positive integer, n,
    /// such that the expression n^3 + n^2p is a perfect cube.
    ///
    /// For example, when p = 19, 8^3 + 8^2 * 19 = 12^3.
    ///
    /// What is perhaps most surprising is that for each prime with this property the
    /// value of n is unique, and there are only four such primes below one-hundred.
    ///
    /// How many primes below one million have this remarkable property?
    /// </summary>
    internal class Problem131 : Problem
    {
        private const int upper = 1000000;

        public Problem131() : base(131) { }

        protected override string Action()
        {
            var p = new Prime(upper);
            var counter = 0;

            p.GenerateAll();
            /**
             * n^2(n+p) = a^3, p is prime, any factor of n can't divide n+p, so GCD(n^2, n+p) == 1,
             * both n^2 and n+p must be cubes, p = a^3/n^2 - n^2 = x^3 - y^3 = (x-y)(x^2+xy+y^2).
             * because p is prime, x-y is 1. Check the difference of every consecutive pair of cubes
             * p = x^2+x(x+1)+(x+1)^2 = 3x^2+3x+1.
             */
            for (int n = 1; n <= (int)Misc.Sqrt(upper / 3); n++)
            {
                var tmp = 3 * n * n + 3 * n + 1;

                if (p.Contains(tmp))
                    counter++;
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// A number consisting entirely of ones is called a repunit. We shall define R(k)
    /// to be a repunit of length k.
    ///
    /// For example, R(10) = 1111111111 = 11*41*271*9091, and the sum of these prime
    /// factors is 9414.
    ///
    /// Find the sum of the first forty prime factors of R(10^9).
    /// </summary>
    internal class Problem132 : Problem
    {
        private const int length = 1000000000;
        private const int counter = 40;
        private const int upper = 1000000;

        public Problem132() : base(132) { }

        protected override string Action()
        {
            var p = new Prime(upper);
            var ret = new List<int>();

            p.GenerateAll();
            /**
             * clearly not divisible by 2,3,5
             * if length is divisible by A(n), length = cA(n)
             * then R(n) = R(A(n))*(10^c + 1) is also divisible by n
             */
            foreach (var n in p.Nums.Skip(3))
            {
                if (length % RepUnit.GetA(n) == 0)
                    ret.Add(n);
                if (ret.Count == counter)
                    break;
            }

            return ret.Sum().ToString();
        }
    }

    /// <summary>
    /// A number consisting entirely of ones is called a repunit. We shall define R(k)
    /// to be a repunit of length k; for example, R(6) = 111111.
    ///
    /// Let us consider repunits of the form R(10^n).
    ///
    /// Although R(10), R(100), or R(1000) are not divisible by 17, R(10000) is
    /// divisible by 17. Yet there is no value of n for which R(10^n) will divide by
    /// 19. In fact, it is remarkable that 11, 17, 41, and 73 are the only four primes
    /// below one-hundred that can be a factor of R(10^n).
    ///
    /// Find the sum of all the primes below one-hundred thousand that will never be a
    /// factor of R(10^n).
    /// </summary>
    internal class Problem133 : Problem
    {
        private const int upper = 100000;

        public Problem133() : base(133) { }

        protected override string Action()
        {
            var p = new Prime(upper);
            long ret = 0;

            p.GenerateAll();
            ret += 2 + 3 + 5;
            /**
             * if A(n) == a, then only R(a), R(2a), R(3a), ... , R(na) is divisible by n
             */
            foreach (var n in p.Nums.Skip(3))
            {
                var a = RepUnit.GetA(n);

                while (a % 2 == 0)
                    a /= 2;
                while (a % 5 == 0)
                    a /= 5;

                if (a != 1)
                    ret += n;
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// Consider the consecutive primes p1 = 19 and p2 = 23. It can be verified that
    /// 1219 is the smallest number such that the last digits are formed by p1 whilst
    /// also being divisible by p2.
    ///
    /// In fact, with the exception of p1 = 3 and p2 = 5, for every pair of consecutive
    /// primes, p2 > p1, there exist values of n for which the last digits are formed
    /// by p1 and n is divisible by p2. Let S be the smallest of these values of n.
    ///
    /// Find Σ(S) for every pair of consecutive primes with 5 <= p1 <= 1000000.
    /// </summary>
    internal class Problem134 : Problem
    {
        private const int upper = 1000000;

        public Problem134() : base(134) { }

        protected override string Action()
        {
            var p = new Prime(upper + 1000);
            BigInteger sum = 0, tmp;
            int pow = 10;

            p.GenerateAll();

            for (int i = 2; p.Nums[i] <= upper; i++)
            {
                /**
                 * x*p2 = p1 (mod 10^d)
                 * http://www2.cc.niigata-u.ac.jp/~takeuchi/tbasic/BackGround/ExEuclid.html
                 * a*p2 + b*(10^n) = 1
                 * a*p1*p2 = p1 (mod 10^d)
                 * x = a*p1 mod 10^d
                 */
                int p1 = p.Nums[i], p2 = p.Nums[i + 1];

                if (p1 > pow)
                    pow *= 10;

                var ret = Factor.GetExtendedGCD(p2, pow);
                tmp = ((long)ret.Item1 * p1) % pow;

                if (tmp < 0)
                    tmp += pow;
                sum += tmp * p2;
            }

            return sum.ToString();
        }
    }

    /// <summary>
    /// Given the positive integers, x, y, and z, are consecutive terms of an
    /// arithmetic progression, the least value of the positive integer, n, for which
    /// the equation, x^2 - y^2 - z^2 = n, has exactly two solutions is n = 27:
    ///
    /// 34^2 - 27^2 - 20^2 = 12^2 - 9^2 - 6^2 = 27
    ///
    /// It turns out that n = 1155 is the least value which has exactly ten solutions.
    ///
    /// How many values of n less than one million have exactly ten distinct solutions?
    /// </summary>
    internal class Problem135 : Problem
    {
        private const int upper = 1000000;

        public Problem135() : base(135) { }

        protected override string Action()
        {
            var counter = new int[upper];
            var ret = 0;

            /**
             * assume x>y>z, x=z+2k, y=z+k, z=z
             * (z+2k)^2 - (z+k)^2 - z^2 = 2kz + 3k^2 - z^2 = (3k-z)(k+z) = n,
             * when z = k, n is maximized = 4k^2, when z = 3k-1, n = 4k-1 is minimal value.
             * for z:
             *   k and [2k,3k) is count once.
             *   (k,2k) is same as (0,k), count twice
             * also 0~z~k is same as k~z~2k
             */
            for (int k = 1; k <= (upper + 1) / 4; k++)
            {
                long tmp = (long)4 * k * k;
                int z = 3 * k - 1;

                if (tmp < upper)
                    counter[tmp]++;

                for (; z >= 2 * k; z--)
                {
                    tmp = (long)(3 * k - z) * (k + z);

                    if (tmp >= upper)
                        break;
                    counter[tmp]++;
                }
                if (z != 2 * k - 1)
                    continue;
                for (; z > k; z--)
                {
                    tmp = (long)(3 * k - z) * (k + z);

                    if (tmp >= upper)
                        break;
                    counter[tmp] += 2;
                }
            }

            foreach (var c in counter)
            {
                if (c == 10)
                    ret++;
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// The positive integers, x, y, and z, are consecutive terms of an arithmetic
    /// progression. Given that n is a positive integer, the equation,
    /// x^2 - y^2 - z^2 = n, has exactly one solution when n = 20:
    ///
    /// 13^2 - 10^2 - 7^2 = 20
    ///
    /// In fact there are twenty-five values of n below one hundred for which the
    /// equation has a unique solution.
    ///
    /// How many values of n less than fifty million have exactly one solution?
    /// </summary>
    internal class Problem136 : Problem
    {
        private const int upper = 50000000;

        public Problem136() : base(136) { }

        protected override string Action()
        {
            var counter = new int[upper];
            var ret = 0;

            for (int k = 1; k <= (upper + 1) / 4; k++)
            {
                long tmp = (long)4 * k * k;
                int z = 3 * k - 1;

                if (tmp < upper)
                    counter[tmp]++;

                for (; z >= 2 * k; z--)
                {
                    tmp = (long)(3 * k - z) * (k + z);

                    if (tmp >= upper)
                        break;
                    counter[tmp]++;
                }
                if (z != 2 * k - 1)
                    continue;
                for (; z > k; z--)
                {
                    tmp = (long)(3 * k - z) * (k + z);

                    if (tmp >= upper)
                        break;
                    counter[tmp] += 2;
                }
            }

            for (int i = 0; i < counter.Length; i++)
            {
                if (counter[i] == 1)
                    ret++;
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// Consider the infinite polynomial series AF(x) = xF1 + x^2F2 + x^3F3 + ...,
    /// where Fk is the kth term in the Fibonacci sequence: 1, 1, 2, 3, 5, 8, ... ;
    /// that is, Fk = Fk1 + Fk2, F1 = 1 and F2 = 1.
    ///
    /// For this problem we shall be interested in values of x for which AF(x) is a
    /// positive integer.
    ///
    /// Surprisingly AF(1/2)
    /// = (1/2)*1 + (1/2)^2*1 + (1/2)^3*2 + (1/2)^4*3 + (1/2)^5*5 + ...
    /// = 1/2 + 1/4 + 2/8 + 3/16 + 5/32 + ...
    /// = 2
    ///
    /// The corresponding values of x for the first five natural numbers are shown
    /// below.
    ///              x   AF(x)
    ///      sqrt(2)-1      1
    ///            1/2      2
    /// (sqrt(13)-2)/3      3
    /// (sqrt(89)-5)/8      4
    /// (sqrt(34)-3)/5      5
    ///
    /// We shall call AF(x) a golden nugget if x is rational, because they become
    /// increasingly rarer; for example, the 10th golden nugget is 74049690.
    ///
    /// Find the 15th golden nugget.
    /// </summary>
    internal class Problem137 : Problem
    {
        private const int index = 15;

        public Problem137() : base(137) { }

        protected override string Action()
        {
            /**
             * AF(x)*x^2 + AF(x)*x - AF(x)
             * =     0 +     0 + x^3F1 + x^4F2 + ...
             * +     0 + x^2F1 + x^3F2 + x^4F3 + ...
             * - x^1F1 - x^2F2 - x^3F3 - x^4F4 + ...
             * =    -x
             *
             * AF(x) = x/(1-x-x^2) = n
             * nx^2 + (n+1)x - n = 0, x is rational, so (n+1)^2 + 4n^2 is perfect square.
             * 5n^2+2n+1 = 5(n+1/5)^2 + 4/5 = y^2
             * 5y^2 = (5n+1)^2 + 4 => (5n+1)^2 - 5a^2 = -4
             * first solution: x=1, y=1
             */
            BigInteger x1 = 1, y1 = 1, xk = x1, yk = y1;
            int counter = 0;

            while (true)
            {
                var tmp = PellEquation.GetNextSolution(x1, y1, xk, yk, 5);

                // Extended Pell Equation
                // http://ja.wikipedia.org/wiki/%E3%83%9A%E3%83%AB%E6%96%B9%E7%A8%8B%E5%BC%8F
                // http://d.hatena.ne.jp/Rion778/20110515/1305463619
                xk = tmp.Item1 / 2;
                yk = tmp.Item2 / 2;

                if (xk % 5 == 1 && ++counter == index)
                    break;
            }

            return ((xk - 1) / 5).ToString();
        }
    }

    /// <summary>
    /// Consider the isosceles triangle with base length, b = 16, and legs, L = 17.
    ///
    /// By using the Pythagorean theorem it can be seen that the height of the
    /// triangle, h = sqrt(17^2 - 8^2) = 15, which is one less than the base length.
    ///
    /// With b = 272 and L = 305, we get h = 273, which is one more than the base
    /// length, and this is the second smallest isosceles triangle with the property
    /// that h = b +- 1.
    ///
    /// Find Σ(L) for the twelve smallest isosceles triangles for which h = b +- 1
    /// and b, L are positive integers.
    /// </summary>
    internal class Problem138 : Problem
    {
        private int index = 12;

        public Problem138() : base(138) { }

        protected override string Action()
        {
            BigInteger ret = 0;
            var counter = 0;

            /**
             * a,b,c where a^2+b^2=c^2 and a = 2b+-1.
             * according to pythagorean triples theory:
             * a = m^2-n^2, b = 2mn, c = m^2+n^2
             * m^2 - n^2 - 4mn = +-1
             * x^2 - 5y^2 = +-1
             * http://mathworld.wolfram.com/PellEquation.html, special case of Pell Equation
             * x = m-n, y = n, c = (x+2y)^2 + y^2
             * first solution is x=2, y=1
             */
            foreach (var ans in PellEquation.GetSolutions(5, 2, 1))
            {
                ret += (ans.Item1 + 2 * ans.Item2) * (ans.Item1 + 2 * ans.Item2) + ans.Item2 * ans.Item2;

                if (++counter == index)
                    break;
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// Let (a, b, c) represent the three sides of a right angle triangle with integral
    /// length sides. It is possible to place four such triangles together to form a
    /// square with length c.
    ///
    /// For example, (3, 4, 5) triangles can be placed together to form a 5 by 5 square
    /// with a 1 by 1 hole in the middle and it can be seen that the 5 by 5 square can
    /// be tiled with twenty-five 1 by 1 squares.
    ///
    /// However, if (5, 12, 13) triangles were used then the hole would measure 7 by 7
    /// and these could not be used to tile the 13 by 13 square.
    ///
    /// Given that the perimeter of the right triangle is less than one-hundred
    /// million, how many Pythagorean triangles would allow such a tiling to take
    /// place?
    /// </summary>
    internal class Problem139 : Problem
    {
        private const int upper = 100000000;

        public Problem139() : base(139) { }

        protected override string Action()
        {
            var counter = 0;

            /**
             * so abs(a-b) divides c
             */
            foreach (var triangle in PythagoreanTriple.GeneratePrimitive(upper))
            {
                var tmp = triangle[0] - triangle[1];

                if (tmp < 0)
                    tmp *= -1;
                if (triangle[2] % tmp == 0)
                    counter += upper / triangle.Sum();
            }

            return counter.ToString();
        }
    }
}