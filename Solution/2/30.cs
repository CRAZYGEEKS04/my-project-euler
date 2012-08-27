using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectEuler.Common;

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
                    int t;

                    array[a, b] = 0;
                    for (t = 1; t < b; t *= 2)
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
}