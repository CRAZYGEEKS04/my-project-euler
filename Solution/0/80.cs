using ProjectEuler.Common;
using ProjectEuler.Common.Graph;
using ProjectEuler.Common.Miscellany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// It is well known that if the square root of a natural number is not an integer,
    /// then it is irrational. The decimal expansion of such square roots is infinite
    /// without any repeating pattern at all.
    ///
    /// The square root of two is 1.41421356237309504880..., and the digital sum of the
    /// first one hundred decimal digits is 475.
    ///
    /// For the first one hundred natural numbers, find the total of the digital sums
    /// of the first one hundred decimal digits for all the irrational square roots.
    /// </summary>
    internal class Problem80 : Problem
    {
        private const int upper = 100;
        private const int precision = 100;

        public Problem80() : base(80) { }

        protected override string Action()
        {
            var counter = 0;

            for (int n = 1; n <= upper; n++)
            {
                var num = SquareRoot.GetDecimal(n, precision);
                var start = num.IndexOf('.');

                if (start == -1)
                    continue;

                for (int id = 0; id < start; id++)
                    counter += num[id] - '0';
                for (int id = 0; id < precision - start; id++)
                    counter += num[start + 1 + id] - '0';
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// In the 5 by 5 matrix below, the minimal path sum from the top left to the
    /// bottom right, by only moving to the right and down, is indicated in bold red
    /// and is equal to 2427.
    ///
    /// 131 673 234 103  18
    /// 201  96 342 965 150
    /// 630 803 746 422 111
    /// 537 699 497 121 956
    /// 805 732 524  37 331
    ///
    /// Find the minimal path sum, in [file D0081.txt], a 31K text file containing a
    /// 80 by 80 matrix, from the top left to the bottom right by only moving right
    /// and down.
    /// </summary>
    internal class Problem81 : Problem
    {
        public Problem81() : base(81) { }

        private List<List<int>> numbers;

        protected override void PreAction(string data)
        {
            numbers = (from line in data.Split('\n')
                       select (from number in line.Trim().Split(',')
                               select int.Parse(number.Trim())).ToList()).ToList();
        }

        protected override string Action()
        {
            for (int i = 1; i < numbers[0].Count; i++)
                numbers[0][i] += numbers[0][i - 1];
            for (int j = 1; j < numbers.Count; j++)
                numbers[j][0] += numbers[j - 1][0];
            for (int i = 1; i < numbers[0].Count; i++)
            {
                for (int j = 1; j < numbers.Count; j++)
                {
                    if (numbers[i - 1][j] < numbers[i][j - 1])
                        numbers[i][j] += numbers[i - 1][j];
                    else
                        numbers[i][j] += numbers[i][j - 1];
                }
            }

            return numbers[numbers.Count - 1][numbers[0].Count - 1].ToString();
        }
    }

    /// <summary>
    /// The minimal path sum in the 5 by 5 matrix below, by starting in any cell in the
    /// left column and finishing in any cell in the right column, and only moving up,
    /// down, and right, is indicated in red and bold; the sum is equal to 994.
    ///
    /// 131 673 234 103  18
    /// 201  96 342 965 150
    /// 630 803 746 422 111
    /// 537 699 497 121 956
    /// 805 732 524  37 331
    ///
    /// Find the minimal path sum, in [file D0082.txt], a 31K text file containing a
    /// 80 by 80 matrix, from the left column to the right column.
    /// </summary>
    internal class Problem82 : Problem
    {
        public Problem82() : base(82) { }

        private List<List<int>> numbers;

        protected override void PreAction(string data)
        {
            numbers = (from line in data.Split('\n')
                       select (from number in line.Trim().Split(',')
                               select int.Parse(number.Trim())).ToList()).ToList();
        }

        private void CalculateValue(int[,] mins, int i, int j)
        {
            if (i > 0 && mins[i, j] > mins[i - 1, j] + numbers[i][j])
                mins[i, j] = mins[i - 1, j] + numbers[i][j];
            if (i < numbers.Count - 1 && mins[i, j] > mins[i + 1, j] + numbers[i][j])
                mins[i, j] = mins[i + 1, j] + numbers[i][j];
        }

        private void CalculateColumn(int[,] mins, int j)
        {
            for (int i = 0; i < numbers.Count; i++)
                mins[i, j] = mins[i, j - 1] + numbers[i][j];

            // Calculate N-1 times.
            for (int n = 0; n < numbers.Count - 1; n++)
            {
                for (int i = 0; i < numbers.Count; i++)
                    CalculateValue(mins, i, j);
            }
        }

        protected override string Action()
        {
            var mins = new int[numbers.Count, numbers[0].Count];
            var min = int.MaxValue;

            for (int i = 0; i < numbers.Count; i++)
                mins[i, 0] = numbers[i][0];
            for (int j = 1; j < numbers[0].Count; j++)
                CalculateColumn(mins, j);

            for (int i = 0; i < numbers.Count; i++)
            {
                if (min > mins[i, numbers[0].Count - 1])
                    min = mins[i, numbers[0].Count - 1];
            }

            return min.ToString();
        }
    }

    /// <summary>
    /// In the 5 by 5 matrix below, the minimal path sum from the top left to the
    /// bottom right, by moving left, right, up, and down, is indicated in bold red and
    /// is equal to 2297.
    ///
    /// 131 673 234 103  18
    /// 201  96 342 965 150
    /// 630 803 746 422 111
    /// 537 699 497 121 956
    /// 805 732 524  37 331
    ///
    /// Find the minimal path sum, in [file D0083.txt], a 31K text file containing a
    /// 80 by 80 matrix, from the top left to the bottom right by moving left, right,
    /// up, and down.
    /// </summary>
    internal class Problem83 : Problem
    {
        public Problem83() : base(83) { }

        private List<List<int>> numbers;

        protected override void PreAction(string data)
        {
            numbers = (from line in data.Split('\n')
                       select (from number in line.Trim().Split(',')
                               select int.Parse(number.Trim())).ToList()).ToList();
        }

        private string GetIDX(int i, int j)
        {
            return string.Format("{0}X{1}", i, j);
        }

        protected override string Action()
        {
            // find minimal path of graph
            var g = new Graph<string, int>(VertexHelper.CreateStringHelper(), EdgeHelper.CreateIntHelper());
            int ret = 0;

            g.AddVertex("start");
            g.AddVertex("end");
            for (int i = 0; i < numbers.Count; i++)
                for (int j = 0; j < numbers[0].Count; j++)
                    g.AddVertex(GetIDX(i, j));

            g.AddEdge("start", GetIDX(0, 0), numbers[0][0]);
            g.AddEdge(GetIDX(numbers.Count - 1, numbers[0].Count - 1), "end", 0);
            for (int i = 0; i < numbers.Count; i++)
            {
                for (int j = 0; j < numbers[0].Count; j++)
                {
                    if (i > 0)
                        g.AddEdge(GetIDX(i, j), GetIDX(i - 1, j), numbers[i - 1][j]);
                    if (i < numbers.Count - 1)
                        g.AddEdge(GetIDX(i, j), GetIDX(i + 1, j), numbers[i + 1][j]);
                    if (j > 0)
                        g.AddEdge(GetIDX(i, j), GetIDX(i, j - 1), numbers[i][j - 1]);
                    if (j < numbers[0].Count - 1)
                        g.AddEdge(GetIDX(i, j), GetIDX(i, j + 1), numbers[i][j + 1]);
                }
            }

            g.PathFound += (obj, e) => { ret = e.Distance; };
            g.FindMinPath("start", "end");

            return ret.ToString();
        }
    }

    /// <summary>
    /// In the game, Monopoly, the standard board is set up in the following way:
    ///
    /// GO  A1 CC1  A2  T1  R1  B1 CH1  B2  B3 JAIL
    /// H2                                      C1
    /// T2                                      U1
    /// H1                                      C2
    /// CH3                                     C3
    /// R4                                      R2
    /// G3                                      D1
    /// CC3                                    CC2
    /// G2                                      D2
    /// G1                                      D3
    /// G2J F3  U2  F2  F1  R3  E3  E2 CH2  E1  FP
    ///
    /// A player starts on the GO square and adds the scores on two 6-sided dice to
    /// determine the number of squares they advance in a clockwise direction. Without
    /// any further rules we would expect to visit each square with equal probability:
    /// 2.5%. However, landing on G2J (Go To Jail), CC (community chest), and
    /// CH (chance) changes this distribution.
    ///
    /// In addition to G2J, and one card from each of CC and CH, that orders the player
    /// to go directly to jail, if a player rolls three consecutive doubles, they do
    /// not advance the result of their 3rd roll. Instead they proceed directly to
    /// jail.
    ///
    /// At the beginning of the game, the CC and CH cards are shuffled. When a player
    /// lands on CC or CH they take a card from the top of the respective pile and,
    /// after following the instructions, it is returned to the bottom of the pile.
    /// There are sixteen cards in each pile, but for the purpose of this problem we
    /// are only concerned with cards that order a movement; any instruction not
    /// concerned with movement will be ignored and the player will remain on the CC/CH
    /// square.
    ///
    /// •Community Chest (2/16 cards):
    /// 1.Advance to GO
    /// 2.Go to JAIL
    ///
    /// •Chance (10/16 cards):
    /// 1.Advance to GO
    /// 2.Go to JAIL
    /// 3.Go to C1
    /// 4.Go to E3
    /// 5.Go to H2
    /// 6.Go to R1
    /// 7.Go to next R (railway company)
    /// 8.Go to next R
    /// 9.Go to next U (utility company)
    /// 10.Go back 3 squares.
    ///
    /// The heart of this problem concerns the likelihood of visiting a particular
    /// square. That is, the probability of finishing at that square after a roll. For
    /// this reason it should be clear that, with the exception of G2J for which the
    /// probability of finishing on it is zero, the CH squares will have the lowest
    /// probabilities, as 5/8 request a movement to another square, and it is the final
    /// square that the player finishes at on each roll that we are interested in. We
    /// shall make no distinction between "Just Visiting" and being sent to JAIL, and
    /// we shall also ignore the rule about requiring a double to "get out of jail",
    /// assuming that they pay to get out on their next turn.
    ///
    /// By starting at GO and numbering the squares sequentially from 00 to 39 we can
    /// concatenate these two-digit numbers to produce strings that correspond with
    /// sets of squares.
    ///
    /// Statistically it can be shown that the three most popular squares, in order,
    /// are JAIL (6.24%) = Square 10, E3 (3.18%) = Square 24, and GO (3.09%) =
    /// Square 00. So these three most popular squares can be listed with the
    /// six-digit modal string: 102400.
    ///
    /// If, instead of using two 6-sided dice, two 4-sided dice are used, find the
    /// six-digit modal string.
    /// </summary>
    internal class Problem84 : Problem
    {
        private const int upper = 10000000;

        public Problem84() : base(84) { }

        private int GetDice(Random rd)
        {
            return rd.Next(4) + rd.Next(4) + 2;
        }

        private void Count(int[] counter)
        {
            var rd = new Random();
            int pos = 0, ccid = 0, chid = 0;

            for (int i = 0; i < upper; i++)
            {
                pos += GetDice(rd);
                pos %= 40;

                switch (pos)
                {
                    case 2:
                    case 17:
                    case 33:
                        switch (ccid)
                        {
                            case 0: pos = 0; break;
                            case 1: pos = 10; break;
                            default: break;
                        }
                        ccid++;
                        ccid %= 16;
                        break;
                    case 7:
                    case 22:
                    case 36:
                        switch (chid)
                        {
                            case 0: pos = 0; break;
                            case 1: pos = 10; break;
                            case 2: pos = 11; break;
                            case 3: pos = 24; break;
                            case 4: pos = 39; break;
                            case 5: pos = 5; break;
                            case 6:
                            case 7:
                                if (pos == 7)
                                    pos = 15;
                                if (pos == 22)
                                    pos = 25;
                                if (pos == 36)
                                    pos = 5;
                                break;
                            case 8:
                                if (pos == 22)
                                    pos = 28;
                                else
                                    pos = 12;
                                break;
                            case 9:
                                pos -= 3;
                                if (pos < 0)
                                    pos += 40;
                                break;
                            default: break;
                        }
                        chid++;
                        chid %= 16;
                        break;
                    case 30: pos = 10; break;
                    default: break;
                }

                counter[pos]++;
            }
        }

        protected override string Action()
        {
            var counter = new int[40];

            Count(counter);

            var sorted = (from i in Itertools.Range(0, 39) select new KeyValuePair<int, int>(i, counter[i])).ToList();
            sorted.Sort((x, y) => { return y.Value.CompareTo(x.Value); });
            var sb = new StringBuilder();
            for (int i = 0; i < 3; i++)
            {
                if (sorted[i].Key < 10)
                    sb.Append("0");

                sb.Append(sorted[i].Key.ToString());
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// By counting carefully it can be seen that a rectangular grid measuring 3 by 2
    /// contains eighteen rectangles:
    ///
    /// * - -  * * -  * * *
    /// - - -  - - -  - - -
    ///   6      4      2
    /// * - -  * * -  * * *
    /// * - -  * * -  * * *
    ///   3      2      1
    ///
    /// Although there exists no rectangular grid that contains exactly two million
    /// rectangles, find the area of the grid with the nearest solution.
    /// </summary>
    internal class Problem85 : Problem
    {
        private int value = 2000000;

        public Problem85() : base(85) { }

        protected override string Action()
        {
            /**
             * For a m*n board:
             * 1*1-1*n squares: (1+2+...+n)*m = m*n*(n+1)/2
             * 2*1-2*n squares = (m-1)*n*(n+1)/2
             * m*1-m*n squares = 1*n*(n+1)/2
             * number of rectangles is m*(m+1)*n*(n+1)/4
             */
            var minarea = 0;
            var min = value;

            for (int n = 1; n < value; n++)
                for (int m = (int)Math.Sqrt(value * 4 / n / n); ; m++)
                {
                    var counter = n * (n + 1) * m * (m + 1) / 4;
                    int tmp;

                    if (counter > value)
                        tmp = counter - value;
                    else
                        tmp = value - counter;

                    if (tmp < min)
                    {
                        min = tmp;
                        minarea = m * n;
                    }

                    if (counter > value)
                        break;
                }

            return minarea.ToString();
        }
    }

    /// <summary>
    /// A spider, S, sits in one corner of a cuboid room, measuring 6 by 5 by 3, and a
    /// fly, F, sits in the opposite corner. By travelling on the surfaces of the room
    /// the shortest "straight line" distance from S to F is 10 and the path is shown
    /// on the diagram.
    ///
    /// [Unfold 6*(5+3) panel, so path length = sqrt(6^2+8^2) = 10
    ///
    /// However, there are up to three "shortest" path candidates for any given cuboid
    /// and the shortest route is not always integer.
    ///
    /// By considering all cuboid rooms with integer dimensions, up to a maximum size
    /// of M by M by M, there are exactly 2060 cuboids for which the shortest distance
    /// is integer when M=100, and this is the least value of M for which the number of
    /// solutions first exceeds two thousand; the number of solutions is 1975 when
    /// M=99.
    ///
    /// Find the least value of M such that the number of solutions first exceeds one
    /// million.
    /// </summary>
    internal class Problem86 : Problem
    {
        private const int upper = 1000000;

        public Problem86() : base(86) { }

        protected override string Action()
        {
            int c;
            int counter = 0;

            for (c = 1; ; c++)
            {
                // Shortest path, check a+b and c
                for (var ab = 2; ab <= c * 2; ab++)
                {
                    if (Misc.IsPerfectSquare(ab * ab + c * c))
                    {
                        if (c >= ab)
                            counter += ab / 2;
                        else
                            counter += c - (ab - 1) / 2;
                    }
                }

                if (counter >= upper)
                    break;
            }

            return c.ToString();
        }
    }

    /// <summary>
    /// The smallest number expressible as the sum of a prime square, prime cube, and
    /// prime fourth power is 28. In fact, there are exactly four numbers below fifty
    /// that can be expressed in such a way:
    ///
    /// 28 = 2^2 + 2^3 + 2^4
    /// 33 = 3^2 + 2^3 + 2^4
    /// 49 = 5^2 + 2^3 + 2^4
    /// 47 = 2^2 + 3^3 + 2^4
    ///
    /// How many numbers below fifty million can be expressed as the sum of a prime
    /// square, prime cube, and prime fourth power?
    /// </summary>
    internal class Problem87 : Problem
    {
        private const int upper = 50000000;

        public Problem87() : base(87) { }

        protected override string Action()
        {
            var p = new Prime((int)Math.Sqrt(upper) + 1);
            var numbers = new HashSet<int>();

            p.GenerateAll();
            foreach (var p1 in p.Nums)
            {
                var square = p1 * p1;

                if (square >= upper)
                    break;
                foreach (var p2 in p.Nums)
                {
                    var tri = p2 * p2 * p2;

                    if (square + tri >= upper)
                        break;
                    foreach (var p3 in p.Nums)
                    {
                        var tmp = p3 * p3 * p3 * p3 + square + tri;

                        if (tmp >= upper)
                            break;

                        numbers.Add(tmp);
                    }
                }
            }

            return numbers.Count.ToString();
        }
    }

    /// <summary>
    /// A natural number, N, that can be written as the sum and product of a given set
    /// of at least two natural numbers, {a1, a2, ... , ak} is called a product-sum
    /// number: N = a1 + a2 + ... + ak = a1  a2  ...  ak.
    ///
    /// For example, 6 = 1 + 2 + 3 = 1 * 2 * 3.
    ///
    /// For a given set of size, k, we shall call the smallest N with this property a
    /// minimal product-sum number. The minimal product-sum numbers for sets of size,
    /// k = 2, 3, 4, 5, and 6 are as follows.
    ///
    /// k=2: 4 = 2 * 2 = 2 + 2
    /// k=3: 6 = 1 * 2 * 3 = 1 + 2 + 3
    /// k=4: 8 = 1 * 1 * 2 * 4 = 1 + 1 + 2 + 4
    /// k=5: 8 = 1 * 1 * 2 * 2 * 2 = 1 + 1 + 2 + 2 + 2
    /// k=6: 12 = 1 * 1 * 1 * 1 * 2 * 6 = 1 + 1 + 1 + 1 + 2 + 6
    ///
    /// Hence for 2 <= k <= 6, the sum of all the minimal product-sum numbers is
    /// 4+6+8+12 = 30; note that 8 is only counted once in the sum.
    ///
    /// In fact, as the complete set of minimal product-sum numbers for 2 <= k <= 12 is
    /// {4, 6, 8, 12, 15, 16}, the sum is 61.
    ///
    /// What is the sum of all the minimal product-sum numbers for 2 <= k <= 12000?
    /// </summary>
    internal class Problem88 : Problem
    {
        private const int upper = 12000;

        public Problem88() : base(88) { }

        private void Count(Dictionary<int, int> dict, int cvalue, int csum, int cproduct, int clength)
        {
            var ctmp = cproduct - csum + clength;

            /**
             * VERY IMPORTANT: without this problem will run over 1-minutes
             * for any number N=2k is a guaranteed solution. k=k:2k = 2*k*1*1*...*1=2+k+1+1+...+1
             */
            if (clength == 0 && cvalue > Math.Sqrt(upper))
                return;
            if (cvalue * cproduct >= upper * 2)
                return;

            while (ctmp <= upper)
            {
                Count(dict, cvalue + 1, csum, cproduct, clength);
                cproduct *= cvalue;
                csum += cvalue;
                clength++;
                ctmp = cproduct - csum + clength;

                if (clength > 1 && (!dict.ContainsKey(ctmp) || dict[ctmp] > cproduct))
                    dict[ctmp] = cproduct;
            }
        }

        protected override string Action()
        {
            var dict = new Dictionary<int, int>();
            var factors = new List<int>();
            var values = new HashSet<int>();

            Count(dict, 2, 0, 1, 0);
            foreach (var pair in dict)
            {
                if (pair.Key >= 2 && pair.Key <= upper)
                    values.Add(pair.Value);
            }

            return values.Sum().ToString();
        }
    }

    /// <summary>
    /// The rules for writing Roman numerals allow for many ways of writing each number
    /// (see http://projecteuler.net/about=roman_numerals). However, there is always a
    /// "best" way of writing a particular number.
    ///
    /// For example, the following represent all of the legitimate ways of writing the
    /// number sixteen:
    ///
    /// IIIIIIIIIIIIIIII
    /// VIIIIIIIIIII
    /// VVIIIIII
    /// XIIIIII
    /// VVVI
    /// XVI
    ///
    /// The last example being considered the most efficient, as it uses the least
    /// number of numerals.
    ///
    /// [file D0089.txt], contains one thousand numbers written in valid, but not
    /// necessarily minimal, Roman numerals; that is, they are arranged in descending
    /// units and obey the subtractive pair rule (see
    /// http://projecteuler.net/about=roman_numerals for the definitive rules for
    /// this problem).
    ///
    /// Find the number of characters saved by writing each of these in their minimal
    /// form.
    ///
    /// Note: You can assume that all the Roman numerals in the file contain no more
    /// than four consecutive identical units.
    /// </summary>
    internal class Problem89 : Problem
    {
        public Problem89() : base(89) { }

        private List<string> numerals;

        protected override void PreAction(string data)
        {
            numerals = data.Split('\n').Select(it => it.Trim()).ToList();
        }

        protected override string Action()
        {
            var numbers = numerals.Select(it => RomanNumerals.GetNumber(it)).ToList();
            var mins = numbers.Select(it => RomanNumerals.GetRoman(it)).ToList();
            var counter = 0;

            for (int i = 0; i < mins.Count; i++)
                counter += numerals[i].Length - mins[i].Length;

            return counter.ToString();
        }
    }
}