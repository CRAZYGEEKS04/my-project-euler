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
}