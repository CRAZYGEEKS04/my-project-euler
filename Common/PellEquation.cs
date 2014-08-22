using ProjectEuler.Common.Miscellany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ProjectEuler.Common
{
    /// <summary>
    /// http://en.wikipedia.org/wiki/Pell's_equation
    /// http://mathworld.wolfram.com/PellEquation.html
    /// </summary>
    public static class PellEquation
    {
        public static Tuple<BigInteger, BigInteger> GetFundamentalSolution(BigInteger D)
        {
            var f = ContinuedFraction.CreateFromSquareRoot(D);

            if (f.Loop.Count == 0)
                return null;
            for (int i = 0; ; i++)
            {
                var tmp = f.GetFraction(i);
                var x = tmp.Numerator;
                var y = tmp.Denominator;

                if (x * x - D * y * y == 1)
                    return new Tuple<BigInteger, BigInteger>(x, y);
            }
        }

        public static IEnumerable<Tuple<BigInteger, BigInteger>> GetSolutions(BigInteger D)
        {
            var ret = GetFundamentalSolution(D);
            BigInteger x = ret.Item1, y = ret.Item2, xk = x, yk = y, tmpx;

            yield return new Tuple<BigInteger, BigInteger>(x, y);
            while (true)
            {
                tmpx = x * xk + D * y * yk;
                yk = x * yk + y * xk;
                xk = tmpx;

                yield return new Tuple<BigInteger, BigInteger>(xk, yk);
            }
        }

        public static IEnumerable<Tuple<BigInteger, BigInteger>> GetSolutions(BigInteger D, BigInteger x, BigInteger y)
        {
            BigInteger xk = x, yk = y, tmpx;

            yield return new Tuple<BigInteger, BigInteger>(x, y);
            while (true)
            {
                tmpx = x * xk + D * y * yk;
                yk = x * yk + y * xk;
                xk = tmpx;

                yield return new Tuple<BigInteger, BigInteger>(xk, yk);
            }
        }

        public static Tuple<BigInteger, BigInteger> GetNextSolution(BigInteger x1, BigInteger y1, BigInteger xk, BigInteger yk,
            BigInteger D)
        {
            return new Tuple<BigInteger, BigInteger>(x1 * xk + D * y1 * yk, x1 * yk + y1 * xk);
        }
    }
}