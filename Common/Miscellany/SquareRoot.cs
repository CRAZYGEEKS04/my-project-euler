using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ProjectEuler.Common.Miscellany
{
    public static class SquareRoot
    {
        private static int CalculateX(ref BigInteger p, ref BigInteger c)
        {
            int x = (int)(c / 20 / p);
            var tmp = (20 * p + x) * x;

            while (tmp > c)
            {
                x--;
                tmp = (20 * p + x) * x;
            }

            c -= tmp;
            p = p * 10 + x;

            return x;
        }

        /// <summary>
        /// http://en.wikipedia.org/wiki/Methods_of_computing_square_roots#Digit-by-digit_calculation
        /// </summary>
        public static string GetDecimal(BigInteger number, int precision)
        {
            List<int> pairs = new List<int>();
            var ret = new StringBuilder();
            BigInteger p = 0, c = 0;
            int x = 0;

            while (number != 0)
            {
                pairs.Add((int)(number % 100));
                number /= 100;
            }
            pairs.Reverse();

            foreach (var pair in pairs)
            {
                c = c * 100 + pair;
                if (p == 0)
                {
                    x = (int)Math.Sqrt(pair);
                    c = pair - x * x;
                    p = x;
                }
                else
                {
                    x = CalculateX(ref p, ref c);
                }

                ret.Append(x.ToString());
            }

            if (c == 0)
                return ret.ToString();

            ret.Append(".");
            for (int i = 0; i < precision; i++)
            {
                c *= 100;
                x = CalculateX(ref p, ref c);
                ret.Append(x.ToString());
            }

            return ret.ToString();
        }
    }
}