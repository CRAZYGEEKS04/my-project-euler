using ProjectEuler.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// We shall define a sqube to be a number of the form, p^2q^3, where p and q are
    /// distinct primes.
    /// For example, 200 = 5^2*2^3 or 120072949 = 23^2*61^3.
    ///
    /// The first five squbes are 72, 108, 200, 392, and 500.
    ///
    /// Interestingly, 200 is also the first number for which you cannot change any
    /// single digit to make a prime; we shall call such numbers, prime-proof. The next
    /// prime-proof sqube which contains the contiguous sub-string "200" is 1992008.
    ///
    /// Find the 200th prime-proof sqube containing the contiguous sub-string "200".
    /// </summary>
    internal class Problem200 : Problem
    {
        private const long upper = 1000000000000;
        private const int index = 200;

        public Problem200() : base(200) { }

        private bool IsPrimeProof(Prime p, long num)
        {
            var array = num.ToString().ToCharArray();
            long tmp;

            for (int l = 0; l < array.Length; l++)
            {
                char digit = array[l];

                for (char d = '0'; d <= '9'; d++)
                {
                    if (d == digit)
                        continue;
                    array[l] = d;
                    tmp = long.Parse(new string(array));

                    if (p.IsPrime(tmp))
                        return false;
                }
                array[l] = digit;
            }

            return true;
        }

        protected override string Action()
        {
            var squares = new List<long>();
            var cubes = new List<long>();
            var nums = new List<long>();
            var p = new Prime((int)Math.Sqrt(upper));
            var counter = 0;

            p.GenerateAll();
            foreach (var n in p)
            {
                if (n > Math.Sqrt(upper))
                    break;
                squares.Add((long)n * n);
            }
            foreach (var n in p)
            {
                if (n > Math.Pow(upper, 0.34))
                    break;
                cubes.Add((long)n * n * n);
            }

            for (int i = 0; i < squares.Count; i++)
            {
                for (int j = 0; j < cubes.Count && cubes[j] < upper / squares[i]; j++)
                {
                    if (i == j)
                        continue;
                    long tmp = squares[i] * cubes[j];

                    if (tmp.ToString().Contains("200"))
                        nums.Add(tmp);
                }
            }
            nums.Sort();

            foreach (var num in nums)
            {
                if (IsPrimeProof(p, num))
                    counter++;
                if (counter == index)
                    return num.ToString();
            }

            return null;
        }
    }

    /// <summary>
    /// For any set A of numbers, let sum(A) be the sum of the elements of A.
    /// Consider the set B = {1,3,6,8,10,11}.
    /// There are 20 subsets of B containing three elements, and their sums are:
    ///
    /// sum({1,3,6}) = 10,
    /// sum({1,3,8}) = 12,
    /// sum({1,3,10}) = 14,
    /// sum({1,3,11}) = 15,
    /// sum({1,6,8}) = 15,
    /// sum({1,6,10}) = 17,
    /// sum({1,6,11}) = 18,
    /// sum({1,8,10}) = 19,
    /// sum({1,8,11}) = 20,
    /// sum({1,10,11}) = 22,
    /// sum({3,6,8}) = 17,
    /// sum({3,6,10}) = 19,
    /// sum({3,6,11}) = 20,
    /// sum({3,8,10}) = 21,
    /// sum({3,8,11}) = 22,
    /// sum({3,10,11}) = 24,
    /// sum({6,8,10}) = 24,
    /// sum({6,8,11}) = 25,
    /// sum({6,10,11}) = 27,
    /// sum({8,10,11}) = 29.
    ///
    /// Some of these sums occur more than once, others are unique.
    /// For a set A, let U(A,k) be the set of unique sums of k-element subsets of A, in
    /// our example we find U(B,3) = {10,12,14,18,21,25,27,29} and sum(U(B,3)) = 156.
    ///
    /// Now consider the 100-element set S = {1^2, 2^2, ... , 100^2}.
    /// S has 100891344545564193334812497256 50-element subsets.
    ///
    /// Determine the sum of all integers which are the sum of exactly one of the
    /// 50-element subsets of S, i.e. find sum(U(S,50)).
    /// </summary>
    internal class Problem201 : Problem
    {
        private const int nElements = 100;
        private const int nSelected = 50;

        public Problem201() : base(201) { }

        protected override string Action()
        {
            var nums = Itertools.Range(0, nElements).Select(it => it * it).ToArray();
            var array = new int[nSelected + 1][];
            int max = nums.Skip(nElements - nSelected).Sum();
            long sum = 0;

            /**
             * array[n,m] represents how many different ways can m be made with n summands
             * 0 - None
             * 1 - Unique
             * 2 - Many Ways
             */
            for (int i = 0; i <= nSelected; i++)
                array[i] = new int[max + 1];
            array[0][0] = 1;

            // ith element
            for (int i = 1; i <= nElements; i++)
            {
                // n summands, must in descending order to ensure the summand occurs only once
                for (int n = nSelected - 1; n >= 0; n--)
                {
                    for (int m = max - nums[i]; m >= 0; m--)
                    {
                        if (array[n][m] == 0)
                            continue;
                        array[n + 1][m + nums[i]] += array[n][m];
                    }
                }
            }

            for (int value = 0; value <= max; value++)
            {
                if (array[nSelected][value] == 1)
                    sum += value;
            }

            return sum.ToString();
        }
    }

    /// <summary>
    /// Three mirrors are arranged in the shape of an equilateral triangle, with their
    /// reflective surfaces pointing inwards. There is an infinitesimal gap at each
    /// vertex of the triangle through which a laser beam may pass.
    ///
    /// Label the vertices A, B and C. There are 2 ways in which a laser beam may enter
    /// vertex C, bounce off 11 surfaces, then exit through the same vertex: one way is
    /// shown below; the other is the reverse of that.
    ///
    /// There are 80840 ways in which a laser beam may enter vertex C, bounce off
    /// 1000001 surfaces, then exit through the same vertex.
    ///
    /// In how many ways can a laser beam enter at vertex C, bounce off 12017639147
    /// surfaces, then exit through the same vertex?
    /// </summary>
    internal class Problem202 : Problem
    {
        private const long nBouces = 12017639147;

        public Problem202() : base(202) { }

        protected override string Action()
        {
            /**
             * Unfold the shape into many equilaterals and the way to exit is a line of two vertices
             * The number of edges crossed equals the times it bounces.
             *
             * Only consider one sixth of the whole shape, also must exit at the same vertex
             *
             * Triangles        Bounce times    Same Vertex
             * ＶＶＶＶＶ        0 7 7 7 7 0     X S X X S X
             *  ＶＶＶＶ          0 5 X 5 0       X X S X X
             *   ＶＶＶ            0 3 3 0         S X X S
             *    ＶＶ              0 1 0           X S X
             *     Ｖ                0 0             X X
             *                        S               S
             *
             * Symmetric, every line must exclude vertices which cross vertex in lower lines,
             * Enter Point is (0, 0)
             * Line 1 Bounced  0 : (1, 1)
             * Line 2 Bounced  1: (0, 2), (2, 2)
             * Line 3 Bounced  3: (1, 3), (3, 3)
             * Line 4 Bounced  5: (0, 4), (2, 4), (4, 4)
             * Line 5 Bounced  7: (1, 5), (3, 5), (5, 5)
             * Line 6 Bounced  9: (0, 6), (2, 6), (4, 6), (6, 6)
             * Line 7 Bounced 11: (1, 7), (3, 7), (5, 7), (7, 7)
             * ....
             * Line n: (n, n), (n-2, n), ... (1, n)/(0, n)
             *
             * Must exit from the same vertex:
             * Line 0: (0, 0)
             * Line 1:
             * Line 2: (0, 2)
             * Line 3: (3, 3)
             * Line 4: (0, 4)
             * Line 5: (3, 5)
             * Line 6: (0, 6), (6, 6)
             */
            long nLine = (nBouces + 3) / 2;
            long counter = 0;

            if (nLine % 2 == 0)
            {
                for (long x = 6; x < nLine; x += 6)
                {
                    var gcd = Factor.GetCommonFactor(x, nLine);

                    if (gcd > 2 && gcd % 2 == 0)
                        continue;
                    if ((x + nLine) / gcd % 2 == 0)
                        continue;
                    counter++;
                }
            }
            else
            {
                for (long x = 3; x < nLine; x += 6)
                {
                    var gcd = Factor.GetCommonFactor(x, nLine);

                    // remove points blocked by previous point
                    if (gcd == 1)
                        counter++;
                }
            }

            return (counter * 2).ToString();
        }
    }

    /// <summary>
    /// The binomial coefficients C(n, k) can be arranged in triangular form, Pascal's
    /// triangle, like this:
    ///        1
    ///       1 1
    ///      1 2 1
    ///     1 3 3 1
    ///    1 4 6 4 1
    ///   1 51010 5 1
    ///  1 6152015 6 1
    /// 1 721353521 7 1
    /// .........
    /// It can be seen that the first eight rows of Pascal's triangle contain twelve
    /// distinct numbers: 1, 2, 3, 4, 5, 6, 7, 10, 15, 20, 21 and 35.
    ///
    /// A positive integer n is called squarefree if no square of a prime divides n.
    /// Of the twelve distinct numbers in the first eight rows of Pascal's triangle,
    /// all except 4 and 20 are squarefree. The sum of the distinct squarefree numbers
    /// in the first eight rows is 105.
    ///
    /// Find the sum of the distinct squarefree numbers in the first 51 rows of
    /// Pascal's triangle.
    /// </summary>
    internal class Problem203 : Problem
    {
        private const int upper = 51;

        public Problem203() : base(203) { }

        private bool IsSquareFree(Prime prime, long number)
        {
            // factor of C(n, k) must be less than or equal to n, only need to check up to n
            foreach (var p in prime)
            {
                int counter = 0;

                while (number % p == 0)
                {
                    number /= p;
                    counter++;
                }

                if (counter > 1)
                    return false;
            }

            return true;
        }

        protected override string Action()
        {
            var p = new Prime(upper + 1);
            var nums = new HashSet<long>();
            long[] prev = null, current = null;
            long sum = 0;

            p.GenerateAll();
            prev = new long[] { 1 };
            nums.Add(1);

            for (int l = 2; l <= upper; l++)
            {
                current = new long[l];
                current[0] = 1;
                for (int i = 1; i < l - 1; i++)
                {
                    current[i] = prev[i - 1] + prev[i];
                    nums.Add(current[i]);
                }
                current[l - 1] = 1;
                prev = current;
            }

            foreach (var num in nums)
            {
                if (IsSquareFree(p, num))
                    sum += num;
            }

            return sum.ToString();
        }
    }

    /// <summary>
    /// A Hamming number is a positive number which has no prime factor larger than 5.
    /// So the first few Hamming numbers are 1, 2, 3, 4, 5, 6, 8, 9, 10, 12, 15.
    /// There are 1105 Hamming numbers not exceeding 10^8.
    ///
    /// We will call a positive number a generalised Hamming number of type n, if it
    /// has no prime factor larger than n.
    /// Hence the Hamming numbers are the generalised Hamming numbers of type 5.
    ///
    /// How many generalised Hamming numbers of type 100 are there which don't exceed
    /// 10^9?
    /// </summary>
    internal class Problem204 : Problem
    {
        private const long upper = 1000000000;
        private const int type = 100;

        public Problem204() : base(204) { }

        private long Count(int[] factors, long num, int id)
        {
            long counter = 0;

            if (id == factors.Length)
                return 1;
            for (; num <= upper; num *= factors[id])
                counter += Count(factors, num, id + 1);

            return counter;
        }

        protected override string Action()
        {
            var p = new Prime(type);

            p.GenerateAll();

            return Count(p.Nums.ToArray(), 1, 0).ToString();
        }
    }

    /// <summary>
    /// Peter has nine four-sided (pyramidal) dice, each with faces numbered 1, 2, 3,
    /// 4.
    /// Colin has six six-sided (cubic) dice, each with faces numbered 1, 2, 3, 4, 5,
    /// 6.
    ///
    /// Peter and Colin roll their dice and compare totals: the highest total wins. The
    /// result is a draw if the totals are equal.
    ///
    /// What is the probability that Pyramidal Pete beats Cubic Colin? Give your answer
    /// rounded to seven decimal places in the form 0.abcdefg
    /// </summary>
    internal class Problem205 : Problem
    {
        public Problem205() : base(205) { }

        protected override string Action()
        {
            var Pete = new long[37];
            var Colin = new long[37];
            long win, total;

            foreach (var dices in Itertools.PermutationsWithReplacement(Itertools.Range(1, 4), 9))
                Pete[dices.Sum()]++;
            foreach (var dices in Itertools.PermutationsWithReplacement(Itertools.Range(1, 6), 6))
                Colin[dices.Sum()]++;

            total = Pete.Sum() * Colin.Sum();
            win = 0;
            for (int i = 1; i <= 36; i++)
                win += Pete[i] * Colin.Take(i).Sum();

            return Math.Round(((double)win / total), 7).ToString();
        }
    }

    /// <summary>
    /// Find the unique positive integer whose square has the form 1_2_3_4_5_6_7_8_9_0,
    /// where each “_” is a single digit.
    /// </summary>
    internal class Problem206 : Problem
    {
        public Problem206() : base(206) { }

        private bool Check(long number)
        {
            var str = number.ToString();

            for (int i = 9; i > 0; i--)
            {
                if (str[i * 2 - 2] != '0' + i)
                    return false;
            }

            return true;
        }

        protected override string Action()
        {
            long min = Misc.Sqrt(10203040506070809), max = Misc.Sqrt(19293949596979899);
            long square, n;

            /**
             * the number must be ********30 or ********70
             */
            n = min;
            while (n % 10 != 3)
                n++;
            for (; n <= max; n += 10)
            {
                square = n * n;
                if (Check(square))
                    return (n * 10).ToString();
            }
            n = min;
            while (n % 10 != 7)
                n++;
            for (; n <= max; n += 10)
            {
                square = n * n;
                if (Check(square))
                    return (n * 10).ToString();
            }

            return null;
        }
    }

    /// <summary>
    /// For some positive integers k, there exists an integer partition of the form
    /// 4^t = 2^t + k,
    /// where 4^t, 2^t, and k are all positive integers and t is a real number.
    ///
    /// The first two such partitions are 4^1 = 2^1 + 2 and 4^1.5849625... =
    /// 2^1.5849625... + 6.
    ///
    /// Partitions where t is also an integer are called perfect.
    /// For any m >= 1 let P(m) be the proportion of such partitions that are perfect
    /// with k <= m.
    /// Thus P(6) = 1/2.
    ///
    /// In the following table are listed some values of P(m)
    ///
    /// P(5) = 1/1
    /// P(10) = 1/2
    /// P(15) = 2/3
    /// P(20) = 1/2
    /// P(25) = 1/2
    /// P(30) = 2/5
    /// ...
    /// P(180) = 1/4
    /// P(185) = 3/13
    ///
    /// Find the smallest m for which P(m) < 1/12345
    /// </summary>
    internal class Problem207 : Problem
    {
        public Problem207() : base(207) { }

        protected override string Action()
        {
            long perfect = 0, x = 0, next = 2, ret;

            /**
             * 4^t = 2^t + k => 2^t(2^t-1) = k
             * x^2 - x - k = 0, x must be integer
             * x = (1 +- sqrt(4k+1)) / 2 must be integer, sqrt(4k+1) must be 1,3,5,7...
             * exclude x = 1 because k = 0
             */
            for (x = 2; ; x++)
            {
                if (x == next)
                {
                    perfect++;
                    next *= 2;
                }

                if (perfect * 12345 < (x - 1))
                    break;
            }
            ret = x * 2 - 1;

            return (((ret * ret) - 1) / 4).ToString();
        }
    }

    /// <summary>
    /// A robot moves in a series of one-fifth circular arcs (72°), with a free choice
    /// of a clockwise or an anticlockwise arc for each step, but no turning on the
    /// spot.
    ///
    /// One of 70932 possible closed paths of 25 arcs starting northward is:
    ///
    /// Given that the robot starts facing North, how many journeys of 70 arcs in
    /// length can it take that return it, after the final arc, to its starting
    /// position?
    /// (Any arc may be traversed multiple times.)
    /// </summary>
    internal class Problem208 : Problem
    {
        private int length = 70;

        public Problem208() : base(208) { }

        private long Count(Dictionary<string, long> dict, int[] angles)
        {
            long ret = 0;

            if (angles.Any(it => it < 0))
                return 0;
            if (angles.Sum() == 0)
                return 1;

            var key = string.Join(",", angles);

            if (dict.ContainsKey(key))
                return dict[key];
            ret += Count(dict, new int[] { angles[1], angles[2], angles[3], angles[4], angles[0] - 1 });
            ret += Count(dict, new int[] { angles[4] - 1, angles[0], angles[1], angles[2], angles[3] });
            dict.Add(key, ret);

            return ret;
        }

        protected override string Action()
        {
            /**
             * According to Problem thread 208, the robot should go in five directions, each direction exactly 14 times
             * to return to original point.
             * I don't understand why though.
             * Computing time reduced significantly.
             */
            var dict = new Dictionary<string, long>();

            return Count(dict, new int[] { length / 5, length / 5, length / 5, length / 5, length / 5 }).ToString();
        }
    }

    /// <summary>
    /// A k-input binary truth table is a map from k input bits (binary digits, 0
    /// [false] or 1 [true]) to 1 output bit. For example, the 2-input binary truth
    /// tables for the logical AND and XOR functions are:
    ///
    /// x	y	x AND y
    /// 0	0		0
    /// 0	1		0
    /// 1	0		0
    /// 1	1		1
    ///
    /// x	y	x XOR y
    /// 0	0		0
    /// 0	1		1
    /// 1	0		1
    /// 1	1		0
    ///
    /// How many 6-input binary truth tables, τ, satisfy the formula
    /// τ(a, b, c, d, e, f) AND τ(b, c, d, e, f, a XOR (b AND c)) = 0
    ///
    /// for all 6-bit inputs (a, b, c, d, e, f)?
    /// </summary>
    internal class Problem209 : Problem
    {
        public Problem209() : base(209) { }

        private Dictionary<int, int> GetLength(int[] x, int[] y)
        {
            var hash = new HashSet<int>();
            var dict = new Dictionary<int, int>();

            while (hash.Count < 64)
            {
                int start = Itertools.Range(0, 63).First(it => !hash.Contains(it));
                int i = start, len = 1;

                hash.Add(i);
                while (y[i] != x[start])
                {
                    i = y[i];
                    hash.Add(i);
                    len++;
                }

                if (dict.ContainsKey(len))
                    dict[len]++;
                else
                    dict[len] = 1;
            }

            return dict;
        }

        protected override string Action()
        {
            int[] x = Itertools.Range(0, 63).ToArray();
            int[] y = new int[64];
            Dictionary<int, int> dict;
            long counter = 1;

            for (int i = 0; i < 64; i++)
            {
                bool a = (x[i] & 0x20) != 0, b = (x[i] & 0x10) != 0, c = (x[i] & 0x08) != 0;

                y[i] = (x[i] << 1) & 0x3F;
                if (a != (b && c))
                    y[i] |= 1;
            }

            /**
             * x = τ(a, b, c, d, e, f), y = τ(b, c, d, e, f, a XOR (b AND c))
             * x, y contains 0~63 uniquely
             *
             *      x      y
             * 000000 000000
             * ......
             *
             * x1 and y1 = 0
             * x2 and y2 = 0
             * ...
             * x64 and y64 = 0
             *
             * Concatenate different clauses n and m where yn = xm => xnxmym => xnxmxpyp until yp = xn
             * every concatenated string must not contains consecutive 1's
             */
            dict = GetLength(x, y);

            /**
             * any string of length N, k(N) = 0k(N-1) + 1(0)k(N-2)
             * K(0) = 1, K(1) = 2, K(2) = 3
             */
            var list = new List<long> { 1, 2, 3 };
            for (int i = 2; i < 64; i++)
                list.Add(list[list.Count - 2] + list[list.Count - 1]);
            foreach (var cycle in dict)
            {
                long multiple;

                switch (cycle.Key)
                {
                    case 1:
                        multiple = 1;
                        break;
                    case 2:
                        multiple = 3;
                        break;
                    default:
                        // first and last can't be both 1, 10***0 or 0****
                        multiple = list[cycle.Key - 1] + list[cycle.Key - 3];
                        break;
                }

                for (int i = 0; i < cycle.Value; i++)
                    counter *= multiple;
            }

            return counter.ToString();
        }
    }
}