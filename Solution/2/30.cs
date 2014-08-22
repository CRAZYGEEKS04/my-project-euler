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
    /// For any two strings of digits, A and B, we define F(A,B) to be the sequence
    /// (A,B,AB,BAB,ABBAB,...) in which each term is the concatenation of the previous
    /// two.
    ///
    /// Further, we define D(A,B)(n) to be the nth digit in the first term of F(A,B) that
    /// contains at least n digits.
    ///
    /// Example:
    ///
    /// Let A=1415926535, B=8979323846. We wish to find D(A,B)(35), say.
    ///
    /// The first few terms of F(A,B) are:
    /// 1415926535
    /// 8979323846
    /// 14159265358979323846
    /// 897932384614159265358979323846
    /// 14159265358979323846897932384614159265358979323846
    ///
    /// Then D(A,B)(35) is the 35th digit in the fifth term, which is 9.
    ///
    /// Now we use for A the first 100 digits of π behind the decimal point:
    ///
    /// 14159265358979323846264338327950288419716939937510
    /// 58209749445923078164062862089986280348253421170679
    ///
    /// and for B the next hundred digits:
    ///
    /// 82148086513282306647093844609550582231725359408128
    /// 48111745028410270193852110555964462294895493038196.
    ///
    /// Find sum(10^n * D(A,B)((127+19n)*7^n) n = 0, 1, ..., 17
    /// </summary>
    internal class Problem230 : Problem
    {
        private abstract class FibonacciNode
        {
            public long Length { get; protected set; }

            public abstract char GetIthChar(long idx);
        }

        private class NodeA : FibonacciNode
        {
            private const string A =
                "1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679";

            public NodeA() { Length = A.Length; }

            public override char GetIthChar(long idx) { return A[(int)idx]; }
        }

        private class NodeB : FibonacciNode
        {
            private const string B =
                "8214808651328230664709384460955058223172535940812848111745028410270193852110555964462294895493038196";

            public NodeB() { Length = B.Length; }

            public override char GetIthChar(long idx) { return B[(int)idx]; }
        }

        private class CommonNode : FibonacciNode
        {
            private FibonacciNode Left;
            private FibonacciNode Right;

            public CommonNode(FibonacciNode left, FibonacciNode right)
            {
                Left = left;
                Right = right;
                Length = left.Length + right.Length;
            }

            public override char GetIthChar(long idx)
            {
                if (idx < Left.Length)
                    return Left.GetIthChar(idx);
                else
                    return Right.GetIthChar(idx - Left.Length);
            }
        }

        public Problem230() : base(230) { }

        private char GetChar(List<FibonacciNode> list, long idx)
        {
            while (list[list.Count - 1].Length < idx)
                list.Add(new CommonNode(list[list.Count - 2], list[list.Count - 1]));

            return list[list.Count - 1].GetIthChar(idx - 1);
        }

        protected override string Action()
        {
            var list = new List<FibonacciNode>();
            long sum = 0, p10 = 1, p7 = 1;

            list.Add(new NodeA());
            list.Add(new NodeB());

            for (int n = 0; n <= 17; n++)
            {
                sum += p10 * (GetChar(list, (127 + 19 * n) * p7) - '0');
                p10 *= 10;
                p7 *= 7;
            }

            return sum.ToString();
        }
    }

    /// <summary>
    /// The binomial coefficient C(10, 3) = 120.
    /// 120 = 2^3 * 3 * 5 = 2 * 2 * 2 * 3 * 5, and 2 + 2 + 2 + 3 + 5 = 14.
    /// So the sum of the terms in the prime factorisation of C(10, 3) is 14.
    ///
    /// Find the sum of the terms in the prime factorisation of C(20000000, 15000000).
    /// </summary>
    internal class Problem231 : Problem
    {
        private const int N = 20000000;
        private const int C = 15000000;

        public Problem231() : base(231) { }

        private long GetSum(Prime prime, int l)
        {
            long sum = 0, factor;

            foreach (var p in prime)
            {
                if (p > l)
                    break;

                factor = p;
                while (factor <= l)
                {
                    sum += p * (l / factor);
                    factor *= p;
                }
            }

            return sum;
        }

        protected override string Action()
        {
            var p = new Prime(N);

            p.GenerateAll();

            return (GetSum(p, N) - GetSum(p, N - C) - GetSum(p, C)).ToString();
        }
    }

    /// <summary>
    /// Two players share an unbiased coin and take it in turns to play "The Race". On
    /// Player 1's turn, he tosses the coin once: if it comes up Heads, he scores one
    /// point; if it comes up Tails, he scores nothing. On Player 2's turn, she chooses
    /// a positive integer T and tosses the coin T times: if it comes up all Heads, she
    /// scores 2^(T-1) points; otherwise, she scores nothing. Player 1 goes first. The
    /// winner is the first to 100 or more points.
    ///
    /// On each turn Player 2 selects the number, T, of coin tosses that maximises the
    /// probability of her winning.
    ///
    /// What is the probability that Player 2 wins?
    ///
    /// Give your answer rounded to eight decimal places in the form 0.abcdefgh .
    /// </summary>
    internal class Problem232 : Problem
    {
        private const int scores = 100;

        public Problem232() : base(232) { }

        protected override string Action()
        {
            double[,] array = new double[scores + 1, scores + 1];

            /**
             * Array[a,b] stores the probability for Player 2 to win before Player 2 tosses
             * when Player 1 has a points left and Player 2 has b points left
             *
             * when player 2 choose t' coins to toss to get t points with the probability p
             *
             * if b>t:
             * array[a,b] = p * (array[a,b-t] / 2 + array[a-1,b-t]/2) + (1-p) * (array[a-1,b]/2 + array[a,b]/2)
             * array[a,b] * (1+p) = array[a,b-t] * p + array[a-1,b-t] * p + array[a-1,b] * (1-p)
             * else:
             * array[a,b] = p + (1-p) * (array[a-1,b]/2 + array[a,b]/2)
             * array[a,b] * (1+p) = 2p + array[a-1,b]*(1-p)
             */
            for (int a = 1; a <= scores; a++)
                array[a, 0] = 1;
            for (int b = 1; b <= scores; b++)
                array[0, b] = 0;

            for (int b = 1; b <= scores; b++)
            {
                for (int a = 1; a <= scores; a++)
                {
                    double p = 0.5, tmp;

                    array[a, b] = 0;
                    for (int t = 1; t < b; t *= 2)
                    {
                        tmp = (array[a, b - t] * p + array[a - 1, b - t] * p + array[a - 1, b] * (1 - p)) / (1 + p);
                        if (tmp > array[a, b])
                            array[a, b] = tmp;
                        p /= 2;
                    }
                    tmp = (2 * p + array[a - 1, b] * (1 - p)) / (1 + p);
                    if (tmp > array[a, b])
                        array[a, b] = tmp;
                }
            }

            return Math.Round(array[scores - 1, scores] / 2 + array[scores, scores] / 2, 8).ToString("F8");
        }
    }

    /// <summary>
    /// Let f(N) be the number of points with integer coordinates that are on a circle
    /// passing through (0,0), (N,0), (0,N), and (N,N).
    ///
    /// It can be shown that f(10000) = 36.
    ///
    /// What is the sum of all positive integers N <= 10^11 such that f(N) = 420 ?
    /// </summary>
    internal class Problem233 : Problem
    {
        private const long upper = 100000000000;

        public Problem233() : base(233) { }

        private List<long> GenerateNumbers(List<int> p)
        {
            var list = new List<long>();
            long n1, n2, n3;

            // case 1
            foreach (var p1 in p)
            {
                n1 = Misc.Pow(p1, 3);
                if (n1 > upper)
                    break;

                foreach (var p2 in p)
                {
                    if (p2 == p1)
                        continue;
                    n2 = n1 * p2 * p2;
                    if (n2 > upper)
                        break;

                    foreach (var p3 in p)
                    {
                        if (p3 == p1 || p3 == p2)
                            continue;
                        n3 = n2 * p3;
                        if (n3 > upper)
                            break;
                        list.Add(n3);
                    }
                }
            }

            // case 2
            foreach (var p1 in p)
            {
                n1 = Misc.Pow(p1, 7);
                if (n1 > upper)
                    break;

                foreach (var p2 in p)
                {
                    if (p2 == p1)
                        continue;
                    n2 = n1 * Misc.Pow(p2, 3);
                    if (n2 > upper)
                        break;
                    list.Add(n2);
                }
            }

            // case 3
            foreach (var p1 in p)
            {
                n1 = Misc.Pow(p1, 10);
                if (n1 > upper)
                    break;

                foreach (var p2 in p)
                {
                    if (p2 == p1)
                        continue;
                    n2 = n1 * p2 * p2;
                    if (n2 > upper)
                        break;
                    list.Add(n2);
                }
            }

            return list;
        }

        private List<long> GenerateCounter(List<int> p, int length)
        {
            var list = new List<long>();
            var flags = new bool[length + 1];

            foreach (var f in p)
            {
                for (int i = f; i <= length; i += f)
                    flags[i] = true;
            }

            list.Add(0);
            for (int i = 1; i <= length; i++)
            {
                if (flags[i])
                    list.Add(list[i - 1]);
                else
                    list.Add(list[i - 1] + i);
            }

            return list;
        }

        protected override string Action()
        {
            /**
             * Center of the circle is (N/2, N/2)
             * Think about a lattice point on the left side of the arc between the top corners of the square.
             * By symmetry there is another such point directly to the right of it and a third directly below.
             * The hypotenuse The triangle formed by these three points is the diameter of the circle
             * N^2+N^2 = 2N^2 => 4 points
             * Finding ways to represent 2N^2 = a^2+b^2
             *
             * http://mathworld.wolfram.com/SchinzelsTheorem.html
             * assume N = p1^a1 * p2^a2 * ... * pn^an = p11^a11 * ... * p1n^a1n * p31^a31 *... * p3n^a3n * pi^ai * ...
             * f(N) = 4((a1*2+1)*(a2*2+1)*...*(an*2+1)) where pn mod 4 = 1
             *
             * f(N) = 420 => 4 * 3*5*7
             * case 1: N = p1 * p2^2 * p3^3
             * case 2: N = p1^3 * p2^7
             * case 3: N = p1^2 * p2^10
             * case 4: N = p1 * p2^17, impossible
             * case 5: N = p1^52, impossible
             */
            var p = new Prime((int)(upper / 5 / 5 / 5 / 13 / 13));
            var nums = new List<long>();
            long sum = 0;

            p.GenerateAll();

            var p5 = p.Nums.Where(it => it % 4 == 1).ToList();
            var sums = GenerateCounter(p5, (int)(upper / 5 / 5 / 5 / 13 / 13 / 17));
            var list = GenerateNumbers(p5);

            foreach (var n in list)
                sum += n * sums[(int)(upper / n)];

            return sum.ToString();
        }
    }

    /// <summary>
    /// For an integer n >= 4, we define the lower prime square root of n, denoted by
    /// lps(n), as the largest prime <= sqrt(n) and the upper prime square root of n,
    /// ups(n), as the smallest prime >= sqrt(n).
    ///
    /// So, for example, lps(4) = 2 = ups(4), lps(1000) = 31, ups(1000) = 37.
    /// Let us call an integer n >= 4 semidivisible, if one of lps(n) and ups(n)
    /// divides n, but not both.
    ///
    /// The sum of the semidivisible numbers not exceeding 15 is 30, the numbers are
    /// 8, 10 and 12.
    /// 15 is not semidivisible because it is a multiple of both lps(15) = 3 and
    /// ups(15) = 5.
    /// As a further example, the sum of the 92 semidivisible numbers up to 1000 is
    /// 34825.
    ///
    /// What is the sum of all semidivisible numbers not exceeding 999966663333 ?
    /// </summary>
    internal class Problem234 : Problem
    {
        private const long upper = 999966663333;

        public Problem234() : base(234) { }

        private long CalculateSum(long start, long end, long lps, long ups)
        {
            return Misc.SumOfDivisor(start, end, lps) + Misc.SumOfDivisor(start, end, ups)
                - Misc.SumOfDivisor(start, end, lps * ups) * 2;
        }

        protected override string Action()
        {
            var p = new Prime((int)Misc.Sqrt(upper) + 1000);
            long sum = 0, start, end;

            p.GenerateAll();
            for (int i = 0; ; i++)
            {
                start = (long)p.Nums[i] * p.Nums[i] + 1;
                end = (long)p.Nums[i + 1] * p.Nums[i + 1] - 1;

                if (start > upper)
                    break;
                if (end > upper)
                    end = upper;
                sum += CalculateSum(start, end, p.Nums[i], p.Nums[i + 1]);
            }

            return sum.ToString();
        }
    }

    /// <summary>
    /// Given is the arithmetic-geometric sequence u(k) = (900-3k)r^(k-1).
    /// Let s(n) = Σ(k=1...n)u(k).
    ///
    /// Find the value of r for which s(5000) = -600,000,000,000.
    ///
    /// Give your answer rounded to 12 places behind the decimal point.
    /// </summary>
    internal class Problem235 : Problem
    {
        private const long value = -200000000000;

        public Problem235() : base(235) { }

        private double GetValue(double r)
        {
            double sum = 0, rk = 1;

            for (int i = 1; i <= 5000; i++)
            {
                sum += (300 - i) * rk;
                rk *= r;
            }

            return sum;
        }

        protected override string Action()
        {
            double min = 1, max = 2, r = (min + max) / 2;

            while (true)
            {
                var tmp = GetValue(r);

                if (Math.Abs(value - tmp) < 1)
                    break;
                if (tmp > value)
                    min = r;
                else
                    max = r;

                r = (min + max) / 2;
            }

            return Math.Round(r, 12).ToString();
        }
    }

    /// <summary>
    /// Suppliers 'A' and 'B' provided the following numbers of products for the luxury
    /// hamper market:
    ///
    /// Product              'A'      'B'
    /// Beluga Caviar        5248     640
    /// Christmas Cake       1312    1888
    /// Gammon Joint         2624    3776
    /// Vintage Port         5760    3776
    /// Champagne Truffles   3936    5664
    ///
    /// Although the suppliers try very hard to ship their goods in perfect condition,
    /// there is inevitably some spoilage - i.e. products gone bad.
    ///
    /// The suppliers compare their performance using two types of statistic:
    ///
    /// The five per-product spoilage rates for each supplier are equal to the number
    /// of products gone bad divided by the number of products supplied, for each of
    /// the five products in turn.
    /// The overall spoilage rate for each supplier is equal to the total number of
    /// products gone bad divided by the total number of products provided by that
    /// supplier.
    ///
    /// To their surprise, the suppliers found that each of the five per-product
    /// spoilage rates was worse (higher) for 'B' than for 'A' by the same factor
    /// (ratio of spoilage rates), m>1; and yet, paradoxically, the overall spoilage
    /// rate was worse for 'A' than for 'B', also by a factor of m.
    ///
    /// There are thirty-five m>1 for which this surprising result could have occurred,
    /// the smallest of which is 1476/1475.
    ///
    /// What's the largest possible value of m?
    /// Give your answer as a fraction reduced to its lowest terms, in the form u/v.
    /// </summary>
    internal class Problem236 : Problem
    {
        // Merge Product 2,3 and 5
        //private static int[] A = new int[] { 5248, 1312, 2624, 5760, 3936 };
        //private static int[] B = new int[] { 640, 1888, 3776, 3776, 5664 };

        private static int[] A = new int[] { 5248, 5760, 7872 };
        private static int[] B = new int[] { 640, 3776, 11328 };

        public Problem236() : base(236) { }

        private bool Check(int badA, int badB, SmallFraction m, int id)
        {
            long num, den, lcf;
            int a, b;

            if (id == A.Length - 1)
                return ((badA * B[id] * m.Numerator) == (badB * A[id] * m.Denominator));

            den = A[id] * m.Denominator;
            num = B[id] * m.Numerator;
            lcf = Factor.GetCommonFactor(den, num);
            a = (int)(den / lcf);
            b = (int)(num / lcf);

            for (int i = 1; ; i++)
            {
                if (a * i >= badA || a * i >= A[id] || b * i >= badB || b * i >= B[id])
                    break;
                if (Check(badA - a * i, badB - b * i, m, id + 1))
                    return true;
            }

            return false;
        }

        protected override string Action()
        {
            int sumA = A.Sum(), sumB = B.Sum();
            SmallFraction m, maxm = 1;

            for (int badA = 5; badA <= sumA; badA++)
            {
                for (int badB = 5; badB <= sumB; badB++)
                {
                    m = new SmallFraction(badA * sumB, badB * sumA);

                    if (m <= maxm)
                        break;
                    if (Check(badA, badB, m, 0))
                        maxm = m;
                }
            }

            return maxm.ToString();
        }
    }

    /// <summary>
    /// Let T(n) be the number of tours over a 4 x n playing board such that:
    ///  •The tour starts in the top left corner.
    ///  •The tour consists of moves that are up, down, left, or right one square.
    ///  •The tour visits each square exactly once.
    ///  •The tour ends in the bottom left corner.
    ///
    /// T(10) is 2329. What is T(10^12) modulo 10^8?
    /// </summary>
    internal class Problem237 : Problem
    {
        private const long e = 1000000000000;
        private const long modulo = 100000000;

        public Problem237() : base(237) { }

        protected override string Action()
        {
            /**
             * A Matrix Method for Counting Hamiltonian Cycles on Grid Graphs (very impressive!)
             *
             * http://www.sciencedirect.com/science/article/pii/S0195669884710316
             *
             * h(n) = 2h(n-1) + 2h(n-2) - 2h(n-3) + h(n-4)
             */
            long[] m = new long[] {
                1, 1, 0, 0,
                2, 0, 1, 1,
                2, 0, 1, 0,
                0, 1, 0, 0,
            };
            var matrix = new SmallMatrix(m, 4, 4);
            var h = new SmallMatrix(new long[] { 0, 1, 1, 0 }, 4, 1);
            var ret = SmallMatrix.ModPow(matrix, e - 1, modulo);

            ret = ret * h;

            return (ret[1, 0] % modulo).ToString();
        }
    }

    /// <summary>
    /// Create a sequence of numbers using the "Blum Blum Shub" pseudo-random number
    /// generator:
    ///
    /// s(0) = 14025256
    /// s(n+1) = s(n)^2 mod 20300713
    ///
    /// Concatenate these numbers s(0)s(1)s(2)... to create a string w of infinite
    /// length.
    /// Then, w = 14025256741014958470038053646...
    ///
    /// For a positive integer k, if no substring of w exists with a sum of digits
    /// equal to k, p(k) is defined to be zero. If at least one substring of w exists
    /// with a sum of digits equal to k, we define p(k) = z, where z is the starting
    /// position of the earliest such substring.
    ///
    /// For instance:
    ///
    /// The substrings 1, 14, 1402, ...
    /// with respective sums of digits equal to 1, 5, 7, ...
    /// start at position 1, hence p(1) = p(5) = p(7) = ... = 1.
    ///
    /// The substrings 4, 402, 4025, ...
    /// with respective sums of digits equal to 4, 6, 11, ...
    /// start at position 2, hence p(4) = p(6) = p(11) = ... = 2.
    ///
    /// The substrings 02, 0252, ...
    /// with respective sums of digits equal to 2, 9, ...
    /// start at position 3, hence p(2) = p(9) = ... = 3.
    ///
    /// Note that substring 025 starting at position 3, has a sum of digits equal to 7,
    /// but there was an earlier substring (starting at position 1) with a sum of
    /// digits equal to 7, so p(7) = 1, not 3.
    ///
    /// We can verify that, for 0 < k < 10^3, sum(p(k)) = 4742.
    ///
    /// Find sum(p(k)), for 0 < k < 2*10^15.
    /// </summary>
    internal class Problem238 : Problem
    {
        private static long upper = 2 * Misc.Pow(10, 15);

        public Problem238() : base(238) { }

        private long GetSum(List<byte> list, int period)
        {
            BitVector flags = new BitVector(period + 1);
            int left = period, k;
            long sum = 0, mul = upper / period, mod = upper % period, counter;

            // p(k) = p(k % period)
            for (int i = 0; left > 0; i++)
            {
                counter = 0;
                k = 0;

                for (int j = i; j < i + list.Count; j++)
                {
                    k += list[j % list.Count];
                    if (k == 0 || flags[k])
                        continue;

                    flags.Set(k);
                    left--;
                    counter += mul;
                    if (mod >= k)
                        counter++;
                }

                sum += (i + 1) * counter;
            }

            return sum;
        }

        protected override string Action()
        {
            List<byte> list = new List<byte>();
            int period, start;

            // S(n) is only related to S(n-1), there must be a loop with length less than 20300713
            start = (PseudoNumberGenerator.GenerateBlumBlumShub().Take(1).ToList()[0]);
            list = start.ToString().Select(it => (byte)(it - '0')).ToList();
            period = Misc.GetDigitalSum(start);
            foreach (var num in PseudoNumberGenerator.GenerateBlumBlumShub().Skip(1))
            {
                if (num == start)
                    break;
                list.AddRange(num.ToString().Select(it => (byte)(it - '0')).ToList());
                period += Misc.GetDigitalSum(num);
            }

            return GetSum(list, period).ToString();
        }
    }

    /// <summary>
    /// A set of disks numbered 1 through 100 are placed in a line in random order.
    ///
    /// What is the probability that we have a partial derangement such that exactly 22
    /// prime number discs are found away from their natural positions?
    /// (Any number of non-prime disks may also be found in or out of their natural
    /// positions.)
    ///
    /// Give your answer rounded to 12 places behind the decimal point in the form
    /// 0.abcdefghijkl.
    /// </summary>
    internal class Problem239 : Problem
    {
        private const int upper = 100;
        private const int nFool = 22;

        public Problem239() : base(239) { }

        private BigInteger CountPartialDerangement(Derangement d, int total, int deranged)
        {
            // The probability of n deranged number containing 22 foolish primes is Fool/Total
            BigInteger Total = Probability.CountCombinations(new BigInteger(total), deranged);
            BigInteger Fool = Probability.CountCombinations(new BigInteger(total - nFool), deranged - nFool);
            BigInteger sum = d[deranged];

            return sum * Fool / Total;
        }

        protected override string Action()
        {
            BigInteger total = Probability.CountPermutations(new BigInteger(upper), upper), sum = 0, mul = 1;
            var p = new Prime(upper);
            Derangement d;

            // Select foolish primes
            p.GenerateAll();
            mul = Probability.CountCombinations(new BigInteger(p.Nums.Count), nFool);

            // Count Partial Derangement
            d = new Derangement(upper - p.Nums.Count + nFool);
            for (int i = nFool; i <= d.N; i++)
                sum += CountPartialDerangement(d, d.N, i);

            return Math.Round((double)(sum * mul) / ((double)total), 12).ToString();
        }
    }
}