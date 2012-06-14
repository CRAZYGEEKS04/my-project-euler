using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.Miscellany
{
    public static class EulerPhi
    {
        public static int GetPhi(Prime prime, int n)
        {
            /**
             * Euler's Totient function:
             * if n = p1^a1 * p2^a2 * ... * pn^an, where [p1, p2, ..., pn] are primes
             * φ(n) = p1^(a1 - 1)(p1 - 1) * p2^(a2 - 1)(p2 - 1) * ... = n(1 - 1/p1)(1 - 1/p2)...
             */
            int ret = n;

            if (n > (long)prime.Upper * prime.Upper)
                throw new ArgumentException("Input n is too large");

            foreach (int i in prime)
            {
                if (n % i == 0)
                {
                    while (n % i == 0)
                        n /= i;
                    ret /= i;
                    ret *= i - 1;
                }

                if (n == 1)
                    break;
                if (n < prime.Upper && prime.Contains((int)n))
                {
                    ret /= n;
                    ret *= n - 1;
                    break;
                }
            }

            return ret;
        }
    }
}