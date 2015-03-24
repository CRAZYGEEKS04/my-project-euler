using ProjectEuler.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// A square piece of paper with integer dimensions N*N is placed with a corner at
    /// the origin and two of its sides along the x- and y-axes. Then, we cut it up
    /// respecting the following rules:
    ///
    /// We only make straight cuts between two points lying on different sides of the
    /// square, and having integer coordinates.
    /// Two cuts cannot cross, but several cuts can meet at the same border point.
    /// Proceed until no more legal cuts can be made.
    ///
    /// Counting any reflections or rotations as distinct, we call C(N) the number of
    /// ways to cut an N*N square. For example, C(1) = 2 and C(2) = 30 (shown below).
    ///
    /// What is C(30) mod 10^8?
    /// </summary>
    internal class Problem270 : Problem
    {
        private const int size = 30;
        private static Modulo m = new Modulo(Misc.Pow(10, 8));

        public Problem270()
            : base(270)
        {
        }

        private long Calculate(Dictionary<string, long> dict, List<int> polygon)
        {
            string key = string.Join(",", polygon);
            List<int> tmp = new List<int>();
            long ret = 0, u;

            if (polygon.Count < 3)
                return 0;
            if (dict.ContainsKey(key))
                return dict[key];

            if (polygon[0] > 0 && polygon[polygon.Count - 1] > 0)
            {
                tmp.Add(0);
                tmp.AddRange(polygon);
                tmp[1]--;
                tmp[tmp.Count - 1]--;
                ret = m.Add(ret, Calculate(dict, tmp));
            }
            else if (polygon[0] > 0 && polygon[polygon.Count - 1] == 0)
            {
                tmp.Add(0);
                tmp.AddRange(polygon.Take(polygon.Count - 1));
                tmp[1]--;
                ret = m.Add(ret, Calculate(dict, tmp));
            }
            else if (polygon[0] == 0 && polygon[polygon.Count - 1] > 0)
            {
                tmp.Add(0);
                tmp.AddRange(polygon.Skip(1));
                tmp[tmp.Count - 1]--;
                ret = m.Add(ret, Calculate(dict, tmp));
            }
            else if (polygon[0] == 0 && polygon[polygon.Count - 1] == 0 && polygon.Count >= 4)
            {
                tmp.Add(0);
                tmp.AddRange(polygon.Skip(1).Take(polygon.Count - 2));
                ret = m.Add(ret, Calculate(dict, tmp));
            }
            tmp.Clear();

            if (polygon[0] == 0 && polygon[1] > 0)
            {
                tmp.AddRange(polygon);
                tmp[0] = 0;
                tmp[1]--;
                ret = m.Add(ret, Calculate(dict, tmp));
            }
            else if (polygon[0] == 0 && polygon[1] == 0 && polygon.Count >= 4)
            {
                tmp.Add(0);
                tmp.AddRange(polygon.Skip(2));
                ret = m.Add(ret, Calculate(dict, tmp));
            }
            tmp.Clear();

            for (int i = 1; i < polygon.Count - 1; i++)
            {
                for (int j = 0; j < polygon[i]; j++)
                {
                    tmp.Add(0);
                    tmp.Add(polygon[i] - j - 1);
                    tmp.AddRange(polygon.Skip(i + 1));
                    u = Calculate(dict, tmp);
                    tmp.Clear();
                    if (polygon[0] > 0)
                    {
                        tmp.Add(0);
                        tmp.AddRange(polygon.Take(i));
                        tmp[1]--;
                        tmp.Add(j);
                        ret = m.Add(ret, m.Mul(u, Calculate(dict, tmp)));
                    }
                    else if (polygon[0] == 0 && i > 1)
                    {
                        tmp.AddRange(polygon.Take(i));
                        tmp[0] = 0;
                        tmp.Add(j);
                        ret = m.Add(ret, m.Mul(u, Calculate(dict, tmp)));
                    }
                    tmp.Clear();
                }
            }

            for (int i = 1; i < polygon.Count - 2; i++)
            {
                tmp.Add(0);
                tmp.AddRange(polygon.Skip(i + 1));
                u = Calculate(dict, tmp);
                tmp.Clear();
                if (polygon[0] > 0)
                {
                    tmp.Add(0);
                    tmp.AddRange(polygon.Take(i + 1));
                    tmp[1]--;
                    ret = m.Add(ret, m.Mul(u, Calculate(dict, tmp)));
                }
                else if (polygon[0] == 0 && i > 1)
                {
                    tmp.AddRange(polygon.Take(i + 1));
                    tmp[0] = 0;
                    ret = m.Add(ret, m.Mul(u, Calculate(dict, tmp)));
                }
                tmp.Clear();
            }

            dict.Add(key, ret);

            return ret;
        }

        protected override string Action()
        {
            /**
             * http://garethrees.org/2013/06/14/euler/
             * Describe shape using number of available cut points on each side.
             * C(n) = [n-1,n-1,n-1,n-1]
             */
            var dict = new Dictionary<string, long>();
            var poly = new List<int>() { size - 1, size - 1, size - 1, size - 1 };

            // basic triangle
            dict.Add("0,0,0", 1);

            return Calculate(dict, poly).ToString();
        }
    }

    /// <summary>
    /// For a positive number n, define S(n) as the sum of the integers x, for which
    /// 1 < x < n and x ^ 3 ≡ 1 mod n.
    ///
    /// When n = 91, there are 8 possible values for x, namely: 9, 16, 22, 29, 53, 74,
    /// 79, 81.
    /// Thus, S(91) = 9+16+22+29+53+74+79+81 = 363.
    ///
    /// Find S(13082761331670030).
    /// </summary>
    internal class Problem271 : Problem
    {
        private const long upper = 13082761331670030;

        public Problem271()
            : base(271)
        {
        }

        protected override string Action()
        {
            /**
             * x^3 = 1 mod n, x^3 = 1 mod every prime factor of n
             * upper = 2*3*5*7*11*13*17*19*23*29*31*37*41*43
             * a = x - 1
             * a^3 + 3*a^2 + 3*a + 1 = 1 mod p
             * a * (a^2 + 3a + 3) = 0 mod p,
             * for p = 2:
             */
            BigInteger sum = 0;

            for (BigInteger i = 1 + 153416670; i < upper; i += 153416670)
            {
                if (i * i * i % upper == 1)
                    sum += i;
            }

            return sum.ToString();
        }
    }

    internal class Problem272 : Problem
    {
        public Problem272()
            : base(272)
        {
        }
    }

    /// <summary>
    /// Consider equations of the form: a^2 + b^2 = N, 0 ≤ a ≤ b, a, b and N integer.
    ///
    /// For N=65 there are two solutions:
    ///
    /// a=1, b=8 and a=4, b=7.
    ///
    /// We call S(N) the sum of the values of a of all solutions of a^2 + b^2 = N,
    /// 0 ≤ a ≤ b, a, b and N integer.
    ///
    /// Thus S(65) = 1 + 4 = 5.
    ///
    /// Find ∑S(N), for all squarefree N only divisible by primes of the form 4k+1
    /// with 4k+1 < 150.
    /// </summary>
    internal class Problem273 : Problem
    {
        private const int upper = 150;

        public Problem273()
            : base(273)
        {
        }

        private void GenerateList(List<long> a, List<long> b)
        {
            var p = new Prime(upper);
            List<int> nums;
            int idx;

            p.GenerateAll();
            nums = p.Nums.Where(it => it % 4 == 1).ToList();
            a.AddRange(nums.Select(it => 0L));
            b.AddRange(nums.Select(it => 0L));
            for (int i = 1; i < Misc.Sqrt(upper) + 1; i++)
            {
                for (int j = i + 1; j < Misc.Sqrt(upper) + 1; j++)
                {
                    idx = nums.IndexOf(i * i + j * j);

                    if (idx < 0)
                        continue;
                    nums[idx] = 0;
                    a[idx] = i;
                    b[idx] = j;
                }
            }
        }

        private long Calculate(List<long> a, List<long> b, int id, List<long> c, List<long> d)
        {
            long sum = c.Sum();

            if (id >= a.Count - 1)
                return sum;

            for (int i = id + 1; i < a.Count; i++)
            {
                List<long> nc = new List<long>(), nd = new List<long>();
                long tmpa, tmpb;

                for (int j = 0; j < c.Count; j++)
                {
                    tmpa = Math.Abs(a[i] * c[j] - b[i] * d[j]);
                    tmpb = a[i] * d[j] + b[i] * c[j];
                    nc.Add(Math.Min(tmpa, tmpb));
                    nd.Add(Math.Max(tmpa, tmpb));
                    tmpa = a[i] * c[j] + b[i] * d[j];
                    tmpb = Math.Abs(a[i] * d[j] - b[i] * c[j]);
                    nc.Add(Math.Min(tmpa, tmpb));
                    nd.Add(Math.Max(tmpa, tmpb));
                }
                sum += Calculate(a, b, i, nc, nd);
            }

            return sum;
        }

        protected override string Action()
        {
            /**
             * every prime number modulo 4 equals 1 can be expressed by sum of two squares.
             * http://en.wikipedia.org/wiki/Brahmagupta–Fibonacci_identity
             */
            List<long> a = new List<long>(), b = new List<long>(), c, d;
            long sum = 0;

            GenerateList(a, b);

            for (int i = 0; i < a.Count; i++)
            {
                c = new List<long>() { a[i] };
                d = new List<long>() { b[i] };
                sum += Calculate(a, b, i, c, d);
            }

            return sum.ToString();
        }
    }

    /// <summary>
    /// For each integer p > 1 coprime to 10 there is a positive divisibility multiplier
    /// m < p which preserves divisibility by p for the following function on any
    /// positive integer, n:
    ///
    /// f(n) = (all but the last digit of n) + (the last digit of n) * m
    ///
    /// That is, if m is the divisibility multiplier for p, then f(n) is divisible by p
    /// if and only if n is divisible by p.
    ///
    /// (When n is much larger than p, f(n) will be less than n and repeated application
    /// of f provides a multiplicative divisibility test for p.)
    ///
    /// For example, the divisibility multiplier for 113 is 34.
    ///
    /// f(76275) = 7627 + 5 * 34 = 7797 : 76275 and 7797 are both divisible by 113
    /// f(12345) = 1234 + 5 * 34 = 1404 : 12345 and 1404 are both not divisible by 113
    ///
    /// The sum of the divisibility multipliers for the primes that are coprime to 10
    /// and less than 1000 is 39517. What is the sum of the divisibility multipliers
    /// for the primes that are coprime to 10 and less than 10^7?
    /// </summary>
    internal class Problem274 : Problem
    {
        private const int upper = 10000000;

        public Problem274()
            : base(274)
        {
        }

        private int CalculateM(int p)
        {
            int n = p - p / 10, d = p % 10;
            HashSet<int> m = new HashSet<int>();

            for (int i = 0; n / d < p; i++)
            {
                if (n % d == 0 && n != d)
                    m.Add(n / d);
                n += p;
            }

            if (m.Count != 1)
                throw new ArgumentException();

            return m.First();
        }

        protected override string Action()
        {
            var p = new Prime(upper);
            long sum = 1 + 5; // for p = 3 and 7

            p.GenerateAll();
            foreach (var prime in p.Nums.Skip(4))
                sum += CalculateM(prime);

            return sum.ToString();
        }
    }
}