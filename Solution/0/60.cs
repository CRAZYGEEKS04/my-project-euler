﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ProjectEuler.Common;
using ProjectEuler.Common.Miscellany;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// The primes 3, 7, 109, and 673, are quite remarkable. By taking any two primes
    /// and concatenating them in any order the result will always be prime. For
    /// example, taking 7 and 109, both 7109 and 1097 are prime. The sum of these four
    /// primes, 792, represents the lowest sum for a set of four primes with this
    /// property.
    ///
    /// Find the lowest sum for a set of five primes for which any two primes
    /// concatenate to produce another prime.
    /// </summary>
    internal class Problem60 : Problem
    {
        private const int upper = 10000;

        public Problem60() : base(60) { }

        private bool Check(Prime p, int[] nums, int id)
        {
            for (int i = 0; i < id; i++)
            {
                if (!p.IsPrime(int.Parse(nums[i].ToString() + nums[id].ToString()))
                    || !p.IsPrime(int.Parse(nums[id].ToString() + nums[i].ToString())))
                    return false;
            }

            return true;
        }

        protected override string Action()
        {
            var nums = new Prime(upper);
            var p = new Prime(upper * 10);
            var maxsum = upper * 5;
            var lck = new object();

            nums.GenerateAll();
            p.GenerateAll();

            Parallel.For(0, nums.Nums.Count, (pi, pls) =>
            {
                var i = new int[5];
                var n = new int[5];

                i[0] = pi;
                n[0] = nums.Nums[i[0]];
                var tmp = n[0];
                if (tmp >= maxsum)
                    pls.Break();
                for (i[1] = i[0] + 1; i[1] < nums.Nums.Count; i[1]++)
                {
                    n[1] = nums.Nums[i[1]];
                    if (tmp + n[1] >= maxsum)
                        break;
                    if (!Check(p, n, 1))
                        continue;

                    tmp += n[1];
                    for (i[2] = i[1] + 1; i[2] < nums.Nums.Count; i[2]++)
                    {
                        n[2] = nums.Nums[i[2]];
                        if (tmp + n[2] >= maxsum)
                            break;
                        if (!Check(p, n, 2))
                            continue;

                        tmp += n[2];
                        for (i[3] = i[2] + 1; i[3] < nums.Nums.Count; i[3]++)
                        {
                            n[3] = nums.Nums[i[3]];
                            if (tmp + n[3] >= maxsum)
                                break;
                            if (!Check(p, n, 3))
                                continue;

                            tmp += n[3];
                            for (i[4] = i[3] + 1; i[4] < nums.Nums.Count; i[4]++)
                            {
                                n[4] = nums.Nums[i[4]];
                                if (tmp + n[4] >= maxsum)
                                    break;
                                if (!Check(p, n, 4))
                                    continue;

                                lock (lck)
                                    maxsum = tmp + n[4];
                                break;
                            }
                            tmp -= n[3];
                        }
                        tmp -= n[2];
                    }
                    tmp -= n[1];
                }
            });

            return maxsum.ToString();
        }
    }

    /// <summary>
    /// Triangle, square, pentagonal, hexagonal, heptagonal, and octagonal numbers are
    /// all figurate (polygonal) numbers and are generated by the following formulae:
    ///
    /// Triangle    P(3,n)=n(n+1)/2     1, 3, 6, 10, 15, ...
    /// Square      P(4,n)=n^2          1, 4, 9, 16, 25, ...
    /// Pentagonal  P(5,n)=n(3n-1)/2    1, 5, 12, 22, 35, ...
    /// Hexagonal   P(6,n)=n(2n-1)      1, 6, 15, 28, 45, ...
    /// Heptagonal  P(7,n)=n(5n-3)/2    1, 7, 18, 34, 55, ...
    /// Octagonal   P(8,n)=n(3n-2)      1, 8, 21, 40, 65, ...
    ///
    /// The ordered set of three 4-digit numbers: 8128, 2882, 8281, has three
    /// interesting properties.
    ///
    /// 1. The set is cyclic, in that the last two digits of each number is the first
    /// two digits of the next number (including the last number with the first).
    /// 2. Each polygonal type: triangle (P(3,127)=8128), square (P(4,91)=8281), and
    /// pentagonal (P(5,44)=2882), is represented by a different number in the set.
    /// 3. This is the only set of 4-digit numbers with this property.
    ///
    /// Find the sum of the only ordered set of six cyclic 4-digit numbers for which
    /// each polygonal type: triangle, square, pentagonal, hexagonal, heptagonal, and
    /// octagonal, is represented by a different number in the set.
    /// </summary>
    internal class Problem61 : Problem
    {
        public Problem61() : base(61) { }

        private Dictionary<int, List<int>> Generate(int i)
        {
            var ret = new Dictionary<int, List<int>>();

            for (int n = 1; ; n++)
            {
                var tmp = n * ((i - 2) * n + 4 - i) / 2;

                if (tmp < 1000)
                    continue;
                if (tmp >= 10000)
                    break;
                if (ret.ContainsKey(tmp / 100))
                    ret[tmp / 100].Add(tmp);
                else
                    ret.Add(tmp / 100, new List<int>(new int[] { tmp }));
            }

            return ret;
        }

        private List<int> Check(List<Dictionary<int, List<int>>> numbers, int upper, int lower)
        {
            if (numbers.Count == 1)
            {
                if (numbers[0].ContainsKey(lower) && numbers[0][lower].Contains(lower * 100 + upper))
                    return new List<int>(new int[] { lower * 100 + upper });
                else
                    return null;
            }

            foreach (var dict in numbers)
            {
                if (dict.ContainsKey(lower))
                {
                    var tmp = new List<Dictionary<int, List<int>>>(numbers);

                    tmp.Remove(dict);
                    foreach (var num in dict[lower])
                    {
                        var ret = Check(tmp, upper, num % 100);
                        if (ret != null)
                        {
                            ret.Add(num);

                            return ret;
                        }
                    }
                }
            }

            return null;
        }

        protected override string Action()
        {
            var numbers = (from i in Itertools.Range(3, 7)
                           select Generate(i)).ToList();
            var oct = Generate(8);

            foreach (var pair in oct)
                foreach (var num in pair.Value)
                {
                    var ret = Check(numbers, pair.Key, num % 100);

                    if (ret != null)
                        return (ret.Sum() + num).ToString();
                }

            return null;
        }
    }

    /// <summary>
    /// The cube, 41063625 (345^3), can be permuted to produce two other cubes:
    /// 56623104 (384^3) and 66430125 (405^3). In fact, 41063625 is the smallest cube
    /// which has exactly three permutations of its digits which are also cube.
    ///
    /// Find the smallest cube for which exactly five permutations of its digits are
    /// cube.
    /// </summary>
    internal class Problem62 : Problem
    {
        private const int upper = 10000;

        public Problem62() : base(62) { }

        private string GetIDX(string number)
        {
            var idx = new int[10];

            foreach (var c in number)
                idx[c - '0']++;

            return string.Join("", (from i in idx select Convert.ToChar('0' + i)));
        }

        protected override string Action()
        {
            var dict = new Dictionary<string, long[]>();

            for (long i = 1; i < upper; i++)
            {
                var tmp = i * i * i;
                var idx = GetIDX(tmp.ToString());

                if (dict.ContainsKey(idx))
                    dict[idx][1]++;
                else
                    dict.Add(idx, new long[] { tmp, 1 });

                if (dict[idx][1] == 5)
                    return dict[idx][0].ToString();
            }

            return null;
        }
    }

    /// <summary>
    /// The 5-digit number, 16807=7^5, is also a fifth power. Similarly, the 9-digit
    /// number, 134217728=8^9, is a ninth power.
    ///
    /// How many n-digit positive integers exist which are also an nth power?
    /// </summary>
    internal class Problem63 : Problem
    {
        public Problem63() : base(63) { }

        protected override string Action()
        {
            int counter = 0, l = 1;

            while (BigInteger.Pow(9, l).ToString().Length >= l)
            {
                for (int i = 1; i < 10; i++)
                {
                    if (BigInteger.Pow(i, l).ToString().Length == l)
                        counter++;
                }
                l++;
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// All square roots are periodic when written as continued fractions and can be
    /// written in the form:
    ///
    /// sqrt(N) = a0 + 1 / (a1 + 1 / (a2 + 1 / (a3 + ... )))
    ///
    /// For example, let us consider 23:
    ///
    /// sqrt(23) = 4 + sqrt(23) — 4 = 4 + 1 / (1 / (sqrt(23) - 4)) =
    /// 4 + 1 / (1 + (sqrt(23) – 3) / 7)
    ///
    /// If we continue we would get the following expansion:
    ///
    /// sqrt(23) = 4 + 1 / (1 + 1 / (3 + 1 / (1 + 1 / (8 + ... ))))
    ///
    /// The process can be summarised as follows:
    ///
    /// a0 = 4, 1 / (sqrt(23) - 4) = (sqrt(23) + 4) / 7 = 1 + (sqrt(23) - 3) / 7
    /// a1 = 1, 7 / (sqrt(23) - 3) = 7(sqrt(23) + 3) / 14 = 3 + (sqrt(23) - 3) / 2
    /// a2 = 3, 2 / (sqrt(23) - 3) = 2(sqrt(23) + 3) / 14 = 1 + (sqrt(23) - 4) / 7
    /// a3 = 1, 7 / (sqrt(23) - 4) = 7(sqrt(23) + 4) / 7 = 8 + sqrt(23) - 4
    /// a4 = 8, 1 / (sqrt(23) - 4) = (sqrt(23) + 4) / 7 = 1 + (sqrt(23) - 3) / 7
    /// a5 = 1, 7 / (sqrt(23) - 3) = 7(sqrt(23) + 3) / 14 = 3 + (sqrt(23) - 3) / 2
    /// a6 = 3, 2 / (sqrt(23) - 3) = 2(sqrt(23) + 3) / 14 = 1 + (sqrt(23) - 4) / 7
    /// a7 = 1, 7 / (sqrt(23) - 4) = 7(sqrt(23) + 4) / 7 = 8 + sqrt(23) - 4
    ///
    /// It can be seen that the sequence is repeating. For conciseness, we use the
    /// notation sqrt(23) = [4;(1,3,1,8)], to indicate that the block (1,3,1,8) repeats
    /// indefinitely.
    ///
    /// The first ten continued fraction representations of (irrational) square roots
    /// are:
    ///
    /// 2=[1;(2)], period=1
    /// 3=[1;(1,2)], period=2
    /// 5=[2;(4)], period=1
    /// 6=[2;(2,4)], period=2
    /// 7=[2;(1,1,1,4)], period=4
    /// 8=[2;(1,4)], period=2
    /// 10=[3;(6)], period=1
    /// 11=[3;(3,6)], period=2
    /// 12= [3;(2,6)], period=2
    /// 13=[3;(1,1,1,1,6)], period=5
    ///
    /// Exactly four continued fractions, for N <= 13, have an odd period.
    ///
    /// How many continued fractions for N <= 10000 have an odd period?
    /// </summary>
    internal class Problem64 : Problem
    {
        private const int upper = 10001;

        public Problem64() : base(64) { }

        protected override string Action()
        {
            int counter = 0;

            for (int i = 1; i < upper; i++)
            {
                if (SmallContinuedFraction.CreateFromSquareRoot(i).Loop.Count % 2 == 1)
                    counter++;
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// The square root of 2 can be written as an infinite continued fraction.
    ///
    /// 2 = 1 + 1 / (2 + 1 / (2 + 1 / (2 + 1 / (2 + ... ))))
    ///
    /// The infinite continued fraction can be written, 2 = [1;(2)], (2) indicates that
    /// 2 repeats ad infinitum. In a similar way, 23 = [4;(1,3,1,8)].
    ///
    /// It turns out that the sequence of partial values of continued fractions for
    /// square roots provide the best rational approximations. Let us consider the
    /// convergents for 2.
    ///
    /// 1 + 1/2 = 3/2
    /// 1 + 1/(2 + 1/2) = 7/5
    /// 1 + 1/(2 + 1/(2 + 1/2)) = 17/12
    /// 1 + 1/(2 + 1/(2 + 1/(2 + 1/2))) = 41/29
    ///
    /// Hence the sequence of the first ten convergents for 2 are:
    ///
    /// 1, 3/2, 7/5, 17/12, 41/29, 99/70, 239/169, 577/408, 1393/985, 3363/2378, ...
    /// What is most surprising is that the important mathematical constant,
    /// e = [2; 1,2,1, 1,4,1, 1,6,1 , ... , 1,2k,1, ...].
    ///
    /// The first ten terms in the sequence of convergents for e are:
    ///
    /// 2, 3, 8/3, 11/4, 19/7, 87/32, 106/39, 193/71, 1264/465, 1457/536, ...
    /// The sum of digits in the numerator of the 10th convergent is 1+4+5+7=17.
    ///
    /// Find the sum of digits in the numerator of the 100th convergent of the
    /// continued fraction for e.
    /// </summary>
    internal class Problem65 : Problem
    {
        public Problem65() : base(65) { }

        protected override string Action()
        {
            var e = ContinuedFraction.CreateE(100);
            var num = e.GetFraction(99).Numerator;

            return (from c in num.ToString() select c - '0').Sum().ToString();
        }
    }

    /// <summary>
    /// Consider quadratic Diophantine equations of the form:
    ///
    /// x^2 - D * y^2 = 1
    ///
    /// For example, when D=13, the minimal solution in x is 649^2 - 13 * 180^2 = 1.
    ///
    /// It can be assumed that there are no solutions in positive integers when D is
    /// square.
    ///
    /// By finding minimal solutions in x for D = {2, 3, 5, 6, 7}, we obtain the
    /// following:
    ///
    /// 3^2 - 2 * 2^2 = 1
    /// 2^2 - 3 * 1^2 = 1
    /// 9^2 - 5 * 4^2 = 1
    /// 5^2 - 6 * 2^2 = 1
    /// 8^2 - 7 * 3^2 = 1
    ///
    /// Hence, by considering minimal solutions in x for D <= 7, the largest x is
    /// obtained when D=5.
    ///
    /// Find the value of D <= 1000 in minimal solutions of x for which the largest
    /// value of x is obtained.
    /// </summary>
    internal class Problem66 : Problem
    {
        private const int upper = 1000;

        public Problem66() : base(66) { }

        protected override string Action()
        {
            BigInteger max = 0;
            var ret = 0;

            for (int i = 1; i <= upper; i++)
            {
                var tmp = PellEquation.GetFundamentalSolution(i);

                if (tmp != null && tmp.Item1 > max)
                {
                    max = tmp.Item1;
                    ret = i;
                }
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// By starting at the top of the triangle below and moving to adjacent numbers on
    /// the row below, the maximum total from top to bottom is 23.
    ///
    /// 3
    /// 7 4
    /// 2 4 6
    /// 8 5 9 3
    ///
    /// That is, 3 + 7 + 4 + 9 = 23.
    ///
    /// Find the maximum total from top to bottom in [file D0067.txt], a 15K text file
    /// containing a triangle with one-hundred rows.
    ///
    /// NOTE: This is a much more difficult version of Problem 18. It is not possible
    /// to try every route to solve this problem, as there are 299 altogether! If you
    /// could check one trillion (1012) routes every second it would take over twenty
    /// billion years to check them all. There is an efficient algorithm to solve it.
    /// ;o)
    /// </summary>
    internal class Problem67 : Problem
    {
        public Problem67() : base(67) { }

        private int[][] numbers;

        protected override void PreAction(string data)
        {
            string[] lines = (from line in data.Split('\n')
                              select line.Trim()).ToArray();

            numbers = new int[lines.Length][];
            for (int i = 0; i < lines.Length; i++)
            {
                numbers[i] = (from num in lines[i].Split(' ')
                              select int.Parse(num.Trim())).ToArray();
            }
        }

        protected override string Action()
        {
            for (int i = numbers.Length - 2; i >= 0; i--)
                for (int j = 0; j < numbers[i].Length; j++)
                {
                    numbers[i][j] += numbers[i + 1][j] > numbers[i + 1][j + 1] ?
                        numbers[i + 1][j] : numbers[i + 1][j + 1];
                }

            return numbers[0][0].ToString();
        }
    }

    /// <summary>
    /// Consider the following "magic" 3-gon ring, filled with the numbers 1 to 6, and
    /// each line adding to nine.
    ///
    ///     4
    ///         3
    ///     1       2       6
    /// 5
    ///
    /// Working clockwise, and starting from the group of three with the numerically
    /// lowest external node (4,3,2 in this example), each solution can be described
    /// uniquely. For example, the above solution can be described by the set: 4,3,2;
    /// 6,2,1; 5,1,3.
    ///
    /// It is possible to complete the ring with four different totals: 9, 10, 11, and
    /// 12. There are eight solutions in total.
    ///
    /// Total Solution Set
    /// 9  4,2,3; 5,3,1; 6,1,2
    /// 9  4,3,2; 6,2,1; 5,1,3
    /// 10 2,3,5; 4,5,1; 6,1,3
    /// 10 2,5,3; 6,3,1; 4,1,5
    /// 11 1,4,6; 3,6,2; 5,2,4
    /// 11 1,6,4; 5,4,2; 3,2,6
    /// 12 1,5,6; 2,6,4; 3,4,5
    /// 12 1,6,5; 3,5,4; 2,4,6
    ///
    /// By concatenating each group it is possible to form 9-digit strings; the maximum
    /// string for a 3-gon ring is 432621513.
    ///
    /// Using the numbers 1 to 10, and depending on arrangements, it is possible to
    /// form 16- and 17-digit strings. What is the maximum 16-digit string for a
    /// "magic" 5-gon ring?
    ///
    ///     X        X
    ///         X
    ///     X       X
    /// X
    ///      X     X     X
    ///
    ///       X
    /// </summary>
    internal class Problem68 : Problem
    {
        public Problem68() : base(68) { }

        private BigInteger GetNumber(int[] outer, int[] inner)
        {
            var sum = outer[0] + inner[0] + inner[1];

            for (int i = 1; i < 5; i++)
            {
                if (outer[i] + inner[i] + inner[(i + 1) % 5] != sum)
                    return 0;
            }

            var id = outer.ToList().IndexOf(outer.Min());
            var sb = new StringBuilder();
            for (int i = id; i < id + 5; i++)
            {
                sb.Append(outer[i % 5].ToString());
                sb.Append(inner[i % 5].ToString());
                sb.Append(inner[(i + 1) % 5].ToString());
            }

            return BigInteger.Parse(sb.ToString());
        }

        protected override string Action()
        {
            BigInteger max = 0;

            // len is 16, so 10 must in outer circle
            foreach (var outer in Itertools.Permutations(Itertools.Range(1, 10), 5))
            {
                if (!outer.Contains(10))
                    continue;
                foreach (var inner in Itertools.Permutations(Itertools.Range(1, 9).Where(it => !outer.Contains(it)), 5))
                {
                    var tmp = GetNumber(outer, inner);

                    if (tmp > max)
                        max = tmp;
                }
            }

            return max.ToString();
        }
    }

    /// <summary>
    /// Euler's Totient function, φ(n) [sometimes called the phi function], is used to
    /// determine the number of numbers less than n which are relatively prime to n.
    /// For example, as 1, 2, 4, 5, 7, and 8, are all less than nine and relatively
    /// prime to nine, φ(9)=6.
    ///
    /// n   Relatively Prime    φ(n)    n/φ(n)
    /// 2   1                   1       2
    /// 3   1,2                 2       1.5
    /// 4   1,3                 2       2
    /// 5   1,2,3,4             4       1.25
    /// 6   1,5                 2       3
    /// 7   1,2,3,4,5,6         6       1.1666...
    /// 8   1,3,5,7             4       2
    /// 9   1,2,4,5,7,8         6       1.5
    /// 10  1,3,7,9             4       2.5
    ///
    /// It can be seen that n=6 produces a maximum n/φ(n) for n <= 10.
    ///
    /// Find the value of n <= 1,000,000 for which n/φ(n) is a maximum.
    /// </summary>
    internal class Problem69 : Problem
    {
        private const int upper = 1000000;

        public Problem69() : base(69) { }

        protected override string Action()
        {
            var p = new Prime((int)Math.Sqrt(upper));
            var ret = 1;

            p.GenerateAll();

            // n / φ(n), will be p1/(p1 - 1) * p2/(p2 - 1) * ... for all prime factors
            foreach (var n in p)
            {
                if (ret * n > upper)
                    break;
                ret *= n;
            }

            return ret.ToString();
        }
    }
}