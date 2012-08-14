using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.Miscellany
{
    /// <summary>
    /// http://www.cut-the-knot.org/pythagoras/PT_matrix.shtml
    /// </summary>
    public static class TrinaryTree
    {
        private static int[] GetAnotherPair(int a, int b, int c)
        {
            /**
             * a=q+m, b=q+n, c=q+m+n
             * (q+m)^2 + (q+n)^2 = (q+m+n)^2 + 1
             * q = +-sqrt(2*m*n+1)
             * m = c-b
             * n = c-a
             * q = a+b-c
             * -q = -a-b+c
             *
             * a' = -q+m = -a-2b+2c
             * b' = -q+n = -2a-b+2c
             * c' = -q+m+n = -2a-2b+3c
             */
            return new int[] { -a - 2 * b + 2 * c, -2 * a - b + 2 * c, -2 * a - 2 * b + 3 * c };
        }

        public static IEnumerable<int[]> GenerateNext(int[] ppt)
        {
            yield return GetAnotherPair(-ppt[0], ppt[1], ppt[2]);
            yield return GetAnotherPair(-ppt[0], -ppt[1], ppt[2]);
            if (ppt[0] != ppt[1])
                yield return GetAnotherPair(ppt[0], -ppt[1], ppt[2]);
        }
    }
}