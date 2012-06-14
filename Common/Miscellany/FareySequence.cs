using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.Miscellany
{
    /// <summary>
    /// http://en.wikipedia.org/wiki/Farey_sequence
    /// </summary>
    public class FareySequence
    {
        public int MaxDenominator { get; private set; }

        public FareySequence(int upper)
        {
            MaxDenominator = upper;
        }

        public IEnumerator<SmallFraction> GetEnumerator()
        {
            long a, b, c, d, tmpa, tmpb, k;

            a = 0;
            b = 1;
            c = 1;
            d = MaxDenominator;

            yield return new SmallFraction(a, b);
            while (c <= MaxDenominator)
            {
                k = (MaxDenominator + b) / d;
                tmpa = a;
                tmpb = b;
                a = c;
                b = d;
                c = k * c - tmpa;
                d = k * d - tmpb;

                yield return new SmallFraction(a, b);
            }
        }

        /// <summary>
        /// Stern Brocot Tree
        /// http://www.cut-the-knot.org/blue/Stern.shtml
        /// </summary>
        /// <param name="leftN"></param>
        /// <param name="leftD"></param>
        /// <param name="rightN"></param>
        /// <param name="rightD"></param>
        /// <returns></returns>
        public int Count(int leftN, int leftD, int rightN, int rightD)
        {
            int midN = leftN + rightN, midD = leftD + rightD;
            int counter = 1;

            if (midD > MaxDenominator)
                return 0;

            counter += Count(leftN, leftD, midN, midD);
            counter += Count(midN, midD, rightN, rightD);

            return counter;
        }
    }
}