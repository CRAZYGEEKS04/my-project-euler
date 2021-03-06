﻿using ProjectEuler.Common;
using ProjectEuler.Common.Graph;
using ProjectEuler.Common.Miscellany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// If a box contains twenty-one coloured discs, composed of fifteen blue discs and
    /// six red discs, and two discs were taken at random, it can be seen that the
    /// probability of taking two blue discs, P(BB) = (15/21)(14/20) = 1/2.
    ///
    /// The next such arrangement, for which there is exactly 50% chance of taking two
    /// blue discs at random, is a box containing eighty-five blue discs and
    /// thirty-five red discs.
    ///
    /// By finding the first arrangement to contain over 10^12 = 1,000,000,000,000
    /// discs in total, determine the number of blue discs that the box would contain.
    /// </summary>
    internal class Problem100 : Problem
    {
        private const long lower = 1000000000000;

        public Problem100() : base(100) { }

        protected override string Action()
        {
            /**
             * 2*x^2 - 2*x - y^2 + y = 0
             * http://www.alpertron.com.ar/QUAD.HTM
             * hyperbolic case
             *
             * x(n) = 3*x(n-1) + 2*y(n-1) - 2
             * y(n) = 4*x(n-1) + 3*y(n-1) - 3
             * x(0) = 1, y(0) = 0
             */

            long x = 1, y = 0;

            while (y < lower)
            {
                long tmpx = 3 * x + 2 * y - 2;
                long tmpy = 4 * x + 3 * y - 3;

                x = tmpx;
                y = tmpy;
            }

            return x.ToString();
        }
    }

    /// <summary>
    /// If we are presented with the first k terms of a sequence it is impossible to
    /// say with certainty the value of the next term, as there are infinitely many
    /// polynomial functions that can model the sequence.
    ///
    /// As an example, let us consider the sequence of cube numbers. This is defined
    /// by the generating function,
    /// u(n) = n^3: 1, 8, 27, 64, 125, 216, ...
    ///
    /// Suppose we were only given the first two terms of this sequence. Working on the
    /// principle that "simple is best" we should assume a linear relationship and
    /// predict the next term to be 15 (common difference 7). Even if we were presented
    /// with the first three terms, by the same principle of simplicity, a quadratic
    /// relationship should be assumed.
    ///
    /// We shall define OP(k, n) to be the nth term of the optimum polynomial
    /// generating function for the first k terms of a sequence. It should be clear
    /// that OP(k, n) will accurately generate the terms of the sequence for n <= k,
    /// and potentially the first incorrect term (FIT) will be OP(k, k+1); in which
    /// case we shall call it a bad OP (BOP).
    ///
    /// As a basis, if we were only given the first term of sequence, it would be most
    /// sensible to assume constancy; that is, for n >= 2, OP(1, n) = u(1).
    ///
    /// Hence we obtain the following OPs for the cubic sequence:
    ///
    /// OP(1, n) = 1               1, 1, 1, 1, ...
    /// OP(2, n) = 7n - 6          1, 8, 15, ...
    /// OP(3, n) = 6n^2 - 11n + 6  1, 8, 27, 58, ...
    /// OP(4, n) = n^3             1, 8, 27, 64, 125, ...
    ///
    /// Clearly no BOPs exist for k >= 4.
    ///
    /// By considering the sum of FITs generated by the BOPs (indicated in red above),
    /// we obtain 1 + 15 + 58 = 74.
    ///
    /// Consider the following tenth degree polynomial generating function:
    ///
    /// u(n) = 1 - n + n^2 - n^3 + n^4 - n^5 + n^6 - n^7 + n^8 - n^9 + n^10
    ///
    /// Find the sum of FITs for the BOPs.
    /// </summary>
    internal class Problem101 : Problem
    {
        private static Fraction[] coefficients = new Fraction[] { 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1 };

        public Problem101() : base(101) { }

        private Fraction GetFIT(IEnumerable<Fraction> values, Fraction[] coefficients)
        {
            int x = 1;

            foreach (var v in values)
            {
                var tmp = PolynomialFunction.Calculate(x, coefficients);

                if (v != tmp)
                    return tmp;
                x++;
            }

            throw new ArgumentException("all values are correct");
        }

        protected override string Action()
        {
            var values = (from x in Itertools.Range(1, coefficients.Length * 2)
                          select PolynomialFunction.Calculate(x, coefficients)).ToArray();
            BigInteger ret = 0;

            for (int i = 1; i < coefficients.Length; i++)
            {
                var list = new List<Fraction>();

                for (int n = 1; n <= i; n++)
                {
                    Fraction tmp = 1;

                    list.Add(tmp);
                    for (int t = 0; t < i - 1; t++)
                    {
                        tmp *= n;
                        list.Add(tmp);
                    }
                }
                var a = new Matrix(list, i, i);
                var tmpco = LinearEquation.Solve(a, values.Take(i));
                var fit = GetFIT(values, tmpco);

                if (fit.Denominator != 1)
                    throw new ArgumentException();

                ret += fit.Numerator;
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// Three distinct points are plotted at random on a Cartesian plane, for which
    /// -1000 <= x, y <= 1000, such that a triangle is formed.
    ///
    /// Consider the following two triangles:
    ///
    /// A(-340,495), B(-153,-910), C(835,-947)
    ///
    /// X(-175,41), Y(-421,-714), Z(574,-645)
    ///
    /// It can be verified that triangle ABC contains the origin, whereas triangle XYZ
    /// does not.
    ///
    /// [file D0102.txt], a 27K text file containing the co-ordinates of one thousand
    /// "random" triangles, find the number of triangles for which the interior
    /// contains the origin.
    /// </summary>
    internal class Problem102 : Problem
    {
        public Problem102() : base(102) { }

        private List<int[]> triangles;

        protected override void PreAction(string data)
        {
            triangles = (from line in data.Split('\n')
                         select (from number in line.Split(',')
                                 select int.Parse(number)).ToArray()).ToList();
        }

        private bool IsSameSide(ThreeDimension.Point p1, ThreeDimension.Point p2, ThreeDimension.Point a, ThreeDimension.Point b)
        {
            var cp1 = (b - a) * (p1 - a);
            var cp2 = (b - a) * (p2 - a);

            return (cp1 ^ cp2) >= 0;
        }

        private bool Check(int[] points)
        {
            // http://www.blackpawn.com/texts/pointinpoly/default.html
            var a = new ThreeDimension.Point(points[0], points[1], 0);
            var b = new ThreeDimension.Point(points[2], points[3], 0);
            var c = new ThreeDimension.Point(points[4], points[5], 0);
            var p = new ThreeDimension.Point(0, 0, 0);

            return (IsSameSide(p, a, b, c) && IsSameSide(p, b, a, c) && IsSameSide(p, c, a, b));
        }

        protected override string Action()
        {
            var counter = 0;

            foreach (var triangle in triangles)
            {
                if (Check(triangle))
                    counter++;
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// Let S(A) represent the sum of elements in set A of size n. We shall call it a
    /// special sum set if for any two non-empty disjoint subsets, B and C, the
    /// following properties are true:
    ///
    /// * S(B) != S(C); that is, sums of subsets cannot be equal.
    /// * If B contains more elements than C then S(B) > S(C).
    ///
    /// If S(A) is minimised for a given n, we shall call it an optimum special sum
    /// set. The first five optimum special sum sets are given below.
    ///
    /// n = 1: {1}
    /// n = 2: {1, 2}
    /// n = 3: {2, 3, 4}
    /// n = 4: {3, 5, 6, 7}
    /// n = 5: {6, 9, 11, 12, 13}
    ///
    /// It seems that for a given optimum set, A = {a1, a2, ... , an}, the next optimum
    /// set is of the form B = {b, a1+b, a2+b, ... ,an+b}, where b is the "middle"
    /// element on the previous row.
    ///
    /// By applying this "rule" we would expect the optimum set for n = 6 to be A =
    /// {11, 17, 20, 22, 23, 24}, with S(A) = 117. However, this is not the optimum
    /// set, as we have merely applied an algorithm to provide a near optimum set. The
    /// optimum set for n = 6 is A = {11, 18, 19, 20, 22, 25}, with S(A) = 115 and
    /// corresponding set string: 111819202225.
    ///
    /// Given that A is an optimum special sum set for n = 7, find its set string.
    /// </summary>
    internal class Problem103 : Problem
    {
        private int[] s6 = new int[] { 11, 18, 19, 20, 22, 25 };

        public Problem103() : base(103) { }

        protected override string Action()
        {
            var s7 = OptimumSpecialSumSet.GetNext(s6);

            return string.Join("", s7);
        }
    }

    /// <summary>
    /// The Fibonacci sequence is defined by the recurrence relation:
    ///
    /// F(n) = F(n-1) + F(n-2), where F(1) = 1 and F(2) = 1.
    ///
    /// It turns out that F(541), which contains 113 digits, is the first Fibonacci
    /// number for which the last nine digits are 1-9 pandigital (contain all the
    /// digits 1 to 9, but not necessarily in order). And F(2749), which contains 575
    /// digits, is the first Fibonacci number for which the first nine digits are 1-9
    /// pandigital.
    ///
    /// Given that F(k) is the first Fibonacci number for which the first nine digits
    /// AND the last nine digits are 1-9 pandigital, find k.
    /// </summary>
    internal class Problem104 : Problem
    {
        private const int divisor = 1000000000;

        public Problem104() : base(104) { }

        protected override string Action()
        {
            var pandigitals = new HashSet<int>();
            var f = new Fibonacci(1, 1, 0);
            int k, a = 1, b = 1;

            foreach (var nums in Itertools.Permutations(Itertools.Range(1, 9), 9))
                pandigitals.Add(int.Parse(string.Join("", nums)));

            for (k = 2; ; k++)
            {
                var tmp = (a + b) % divisor;

                a = b;
                b = tmp;
                if (pandigitals.Contains(a))
                {
                    var value = f[k];

                    if (pandigitals.Contains(int.Parse(value.ToString().Substring(0, 9))))
                        break;
                }
            }

            return k.ToString();
        }
    }

    /// <summary>
    /// Let S(A) represent the sum of elements in set A of size n. We shall call it a
    /// special sum set if for any two non-empty disjoint subsets, B and C, the
    /// following properties are true:
    ///
    /// * S(B) != S(C); that is, sums of subsets cannot be equal.
    /// * If B contains more elements than C then S(B) > S(C).
    ///
    /// For example, {81, 88, 75, 42, 87, 84, 86, 65} is not a special sum set because
    /// 65 + 87 + 88 = 75 + 81 + 84, whereas {157, 150, 164, 119, 79, 159, 161, 139,
    /// 158} satisfies both rules for all possible subset pair combinations and S(A)
    /// = 1286.
    ///
    /// [file D0105.txt], a 4K text file with one-hundred sets containing seven to
    /// twelve elements (the two examples given above are the first two sets in the
    /// file), identify all the special sum sets, A1, A2, ..., Ak, and find the value
    /// of S(A1) + S(A2) + ... + S(Ak).
    /// </summary>
    internal class Problem105 : Problem
    {
        public Problem105() : base(105) { }

        private List<int[]> sets;

        protected override void PreAction(string data)
        {
            sets = (from line in data.Split('\n')
                    select (from n in line.Trim().Split(',')
                            select int.Parse(n)).ToArray()).ToList();
        }

        protected override string Action()
        {
            int ret = 0;

            foreach (var set in sets)
            {
                if (OptimumSpecialSumSet.IsOptimumSpecialSumSet(set))
                    ret += set.Sum();
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// Let S(A) represent the sum of elements in set A of size n. We shall call it a
    /// special sum set if for any two non-empty disjoint subsets, B and C, the
    /// following properties are true:
    ///
    /// * S(B) != S(C); that is, sums of subsets cannot be equal.
    /// * If B contains more elements than C then S(B) > S(C).
    ///
    /// For this problem we shall assume that a given set contains n strictly
    /// increasing elements and it already satisfies the second rule.
    ///
    /// Surprisingly, out of the 25 possible subset pairs that can be obtained from a
    /// set for which n = 4, only 1 of these pairs need to be tested for equality
    /// (first rule). Similarly, when n = 7, only 70 out of the 966 subset pairs need
    /// to be tested.
    ///
    /// For n = 12, how many of the 261625 subset pairs that can be obtained need to be
    /// tested for equality?
    /// </summary>
    internal class Problem106 : Problem
    {
        private const int n = 12;

        public Problem106() : base(106) { }

        private string GetIDX(int[] set1, int[] set2)
        {
            string s1 = "", s2 = "";

            foreach (var n in set1)
            {
                if (!set2.Contains(n))
                    s1 += n;
            }
            foreach (var n in set2)
            {
                if (!set1.Contains(n))
                    s2 += n;
            }

            return s1 + "|" + s2;
        }

        protected override string Action()
        {
            var idx = new HashSet<string>();

            // only test equality for subsets of same length, length of 1,n-1,n doesn't need compare
            for (int l = 2; l < n - 1; l++)
            {
                var subsets = Itertools.Combinations(Itertools.Range(1, n), l).ToList();

                foreach (var pair in Itertools.Combinations(subsets, 2))
                {
                    bool tmp = false;

                    for (int i = 0; i < l; i++)
                    {
                        if (pair[0][i] < pair[1][i])
                        {
                            tmp = true;
                            break;
                        }
                    }

                    if (!tmp)
                        continue;
                    tmp = false;
                    for (int i = l - 1; i >= 0; i--)
                    {
                        if (pair[0][i] > pair[1][i])
                        {
                            tmp = true;
                            break;
                        }
                    }

                    if (tmp)
                        idx.Add(GetIDX(pair[0], pair[1]));
                }
            }

            return idx.Count.ToString();
        }
    }

    /// <summary>
    /// The following undirected network consists of seven vertices and twelve edges
    /// with a total weight of 243.
    ///
    /// The same network can be represented by the matrix below.
    ///
    ///    A  B  C  D  E  F  G
    /// A  - 16 12 21  -  -  -
    /// B 16  -  - 17 20  -  -
    /// C 12  -  - 28  - 31  -
    /// D 21 17 28  - 18 19 23
    /// E  - 20  - 18  -  - 11
    /// F  -  - 31 19  -  - 27
    /// G  -  -  - 23 11 27  -
    ///
    /// However, it is possible to optimise the network by removing some edges and
    /// still ensure that all points on the network remain connected. The network which
    /// achieves the maximum saving is shown below. It has a weight of 93, representing
    /// a saving of 243 - 93 = 150 from the original network.
    ///
    /// Using [file D0107.txt], a 6K text file containing a network with forty
    /// vertices, and given in matrix form, find the maximum saving which can be
    /// achieved by removing redundant edges whilst ensuring that the network remains
    /// connected.
    /// </summary>
    internal class Problem107 : Problem
    {
        public Problem107() : base(107) { }

        private int[][] matrix;

        protected override void PreAction(string data)
        {
            matrix = (from line in data.Split('\n')
                      select (
                          from e in line.Trim().Split(',')
                          select e == "-" ? 0 : int.Parse(e)).ToArray()).ToArray();
        }

        protected override string Action()
        {
            var g = new Graph<int, int>(VertexHelper.CreateIntHelper(), EdgeHelper.CreateIntHelper());
            int counter = (from row in matrix select row.Sum()).Sum();

            for (int i = 0; i < matrix.Length; i++)
                g.AddVertex(i);
            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix[0].Length; j++)
                {
                    if (matrix[i][j] != 0)
                        g.AddEdge(i, j, matrix[i][j]);
                }

            var mst = g.FindMinSpinningTree();

            foreach (var p in mst.Data)
                foreach (var pp in p.Value)
                    counter -= pp.Value;

            return (counter / 2).ToString();
        }
    }

    /// <summary>
    /// In the following equation x, y, and n are positive integers.
    ///
    /// 1/x + 1/y = 1/n
    ///
    /// For n = 4 there are exactly three distinct solutions:
    ///
    /// 1/5 + 1/20 = 1/4
    /// 1/6 + 1/12 = 1/4
    /// 1/8 + 1/8 = 1/4
    ///
    /// What is the least value of n for which the number of distinct solutions exceeds
    /// one-thousand?
    /// </summary>
    internal class Problem108 : Problem
    {
        private const int upper = 1000;

        public Problem108() : base(108) { }

        protected override string Action()
        {
            /**
             * xy - nx - ny = 0
             * http://www.alpertron.com.ar/QUAD.HTM
             * simple hyperbolic case
             * DE-BF = n^2
             * number of solutions equals number of divisors of n^2, but there exists symmetric value of x and y.
             */
            var p = new Prime(100);

            p.GenerateAll();

            return Misc.Sqrt(Factor.GetMinimalSquareNumber(p.Nums, upper * 2)).ToString();
        }
    }

    /// <summary>
    /// In the game of darts a player throws three darts at a target board which is
    /// split into twenty equal sized sections numbered one to twenty.
    ///
    /// The score of a dart is determined by the number of the region that the dart
    /// lands in. A dart landing outside the red/green outer ring scores zero. The
    /// black and cream regions inside this ring represent single scores. However,
    /// the red/green outer ring and middle ring score double and treble scores
    /// respectively.
    ///
    /// At the centre of the board are two concentric circles called the bull region,
    /// or bulls-eye. The outer bull is worth 25 points and the inner bull is a double,
    /// worth 50 points.
    ///
    /// There are many variations of rules but in the most popular game the players
    /// will begin with a score 301 or 501 and the first player to reduce their running
    /// total to zero is a winner. However, it is normal to play a "doubles out"
    /// system, which means that the player must land a double (including the double
    /// bulls-eye at the centre of the board) on their final dart to win; any other
    /// dart that would reduce their running total to one or lower means the score for
    /// that set of three darts is "bust".
    ///
    /// When a player is able to finish on their current score it is called a
    /// "checkout" and the highest checkout is 170: T20 T20 D25 (two treble 20s and
    /// double bull).
    ///
    /// There are exactly eleven distinct ways to checkout on a score of 6:
    ///
    /// D3
    /// D1 D2
    /// S2 D2
    /// D2 D1
    /// S4 D1
    /// S1 S1 D2
    /// S1 T1 D1
    /// S1 S3 D1
    /// D1 D1 D1
    /// D1 S2 D1
    /// S2 S2 D1
    ///
    /// Note that D1 D2 is considered different to D2 D1 as they finish on different
    /// doubles. However, the combination S1 T1 D1 is considered the same as T1 S1 D1.
    ///
    /// In addition we shall not include misses in considering combinations; for
    /// example, D3 is the same as 0 D3 and 0 0 D3.
    ///
    /// Incredibly there are 42336 distinct ways of checking out in total.
    ///
    /// How many distinct ways can a player checkout with a score less than 100?
    /// </summary>
    internal class Problem109 : Problem
    {
        private int upper = 100;

        public Problem109() : base(109) { }

        protected override string Action()
        {
            var singleScores = new List<int>();
            var finishScores = new List<int>();
            int counter = 0;

            for (int i = 1; i <= 20; i++)
            {
                singleScores.Add(i);
                singleScores.Add(i * 2);
                singleScores.Add(i * 3);
                finishScores.Add(i * 2);
            }
            singleScores.Add(25);
            singleScores.Add(50);
            finishScores.Add(50);

            // Only single hit
            counter += finishScores.Count;

            // Two hits, only 50+50 is not less than 100
            foreach (var s in singleScores)
            {
                int ret = upper - s;

                if (finishScores.Contains(ret))
                    counter += BinarySearch.Search(finishScores, ret);
                else
                    counter += BinarySearch.SearchLeft(finishScores, ret) + 1;
            }

            // Three hits, distinct first two values
            foreach (var s in Itertools.Combinations(singleScores, 2))
            {
                int ret = upper - s[0] - s[1];

                if (finishScores.Contains(ret))
                    counter += BinarySearch.Search(finishScores, ret);
                else
                    counter += BinarySearch.SearchLeft(finishScores, ret) + 1;
            }
            // Three hits, same first two values
            foreach (var s in singleScores)
            {
                int ret = upper - s * 2;

                if (finishScores.Contains(ret))
                    counter += BinarySearch.Search(finishScores, ret);
                else
                    counter += BinarySearch.SearchLeft(finishScores, ret) + 1;
            }

            return counter.ToString();
        }
    }
}