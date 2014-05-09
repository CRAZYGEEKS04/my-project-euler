using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ProjectEuler.Common
{
    public static class Factor
    {
        public static Tuple<int, int> GetExtendedGCD(int a, int b)
        {
            // http://en.wikipedia.org/wiki/Extended_Euclidean_algorithm
            int x = 0, lastx = 1, y = 1, lasty = 0, tmp, q;

            while (b != 0)
            {
                q = a / b;

                tmp = a;
                a = b;
                b = tmp % b;

                tmp = x;
                x = lastx - q * x;
                lastx = tmp;

                tmp = y;
                y = lasty - q * y;
                lasty = y;
            }

            return new Tuple<int, int>(lastx, lasty);
        }

        public static Tuple<BigInteger, BigInteger> GetExtendedGCD(BigInteger a, BigInteger b)
        {
            BigInteger x = 0, lastx = 1, y = 1, lasty = 0, tmp, q;

            while (b != 0)
            {
                q = a / b;

                tmp = a;
                a = b;
                b = tmp % b;

                tmp = x;
                x = lastx - q * x;
                lastx = tmp;

                tmp = y;
                y = lasty - q * y;
                lasty = y;
            }

            return new Tuple<BigInteger, BigInteger>(lastx, lasty);
        }

        public static int GetCommonFactor(int a, int b)
        {
            int tmp;

            while (b != 0)
            {
                a = a % b;
                tmp = b;
                b = a;
                a = tmp;
            }

            return a;
        }

        public static long GetCommonFactor(long a, long b)
        {
            long tmp;

            while (b != 0)
            {
                a = a % b;
                tmp = b;
                b = a;
                a = tmp;
            }

            return a;
        }

        public static long GetCommonMultiple(int a, int b)
        {
            return a * b / GetCommonFactor(a, b);
        }

        public static long GetCommonMultiple(long a, long b)
        {
            return a * b / GetCommonFactor(a, b);
        }

        public static BigInteger GetCommonFactor(BigInteger a, BigInteger b)
        {
            BigInteger tmp;

            while (b != 0)
            {
                a = a % b;
                tmp = b;
                b = a;
                a = tmp;
            }

            return a;
        }

        public static int GetFactorSum(Prime prime, int number, bool includeOne = true, bool includeSelf = false)
        {
            // http://planetmath.org/encyclopedia/FormulaForSumOfDivisors.html
            int ret = 1;
            int tmpn = number;

            if (number > (long)prime.Upper * prime.Upper)
                throw new ArgumentException("Input n is too large");

            foreach (var p in prime)
            {
                if (tmpn % p == 0)
                {
                    var sum = 1;
                    var tmp = 1;

                    while (tmpn % p == 0)
                    {
                        tmpn /= p;
                        tmp *= p;
                        sum += tmp;
                    }

                    ret *= sum;
                }

                if (tmpn == 1)
                    break;
                if (tmpn < prime.Upper && prime.Contains(tmpn))
                {
                    ret *= (tmpn + 1);
                    break;
                }
            }

            if (!includeOne)
                ret--;
            if (!includeSelf)
                ret -= number;

            return ret;
        }

        public static int GetRadical(Prime prime, int number)
        {
            int ret = 1;

            if (number > (long)prime.Upper * prime.Upper)
                throw new ArgumentException("Input n is too large");

            foreach (var p in prime)
            {
                if (number % p == 0)
                {
                    while (number % p == 0)
                        number /= p;
                    ret *= p;
                }

                if (number == 1)
                    break;
                if (number < prime.Upper && prime.Contains(number))
                {
                    ret *= number;
                    break;
                }
            }

            return ret;
        }

        public static int GetFactorNumber(Prime prime, int n, bool includeOne = true, bool includeSelf = false)
        {
            int tmp, ret = 1;

            if (n == 1)
                return includeOne || includeSelf ? 1 : 0;

            if (n > prime.Upper * prime.Upper)
                throw new ArgumentException("Input n is too large");

            foreach (int i in prime)
            {
                if (n % i == 0)
                {
                    tmp = 1;
                    while (n % i == 0)
                    {
                        n /= i;
                        tmp++;
                    }
                    ret *= tmp;
                }

                if (n == 1)
                    break;
                if (n < prime.Upper && prime.Contains(n))
                {
                    ret *= 2;
                    n = 1;
                    break;
                }
            }

            if (n != 1)
                ret *= 2;
            if (!includeOne)
                ret--;
            if (!includeSelf)
                ret--;

            return ret;
        }

        public static BigInteger GetFactorNumber(Prime prime, BigInteger n, bool includeOne = true, bool includeSelf = false)
        {
            BigInteger tmp, ret = 1;

            if (n == 1)
                return includeOne || includeSelf ? 1 : 0;

            if (n > (BigInteger)prime.Upper * prime.Upper)
                throw new ArgumentException("Input n is too large");

            foreach (int i in prime)
            {
                if (n % i == 0)
                {
                    tmp = 1;
                    while (n % i == 0)
                    {
                        n /= i;
                        tmp++;
                    }
                    ret *= tmp;
                }

                if (n == 1)
                    break;
                if (n < prime.Upper && prime.Contains((int)n))
                {
                    ret *= 2;
                    n = 1;
                    break;
                }
            }

            if (n != 1)
                ret *= 2;
            if (!includeOne)
                ret--;
            if (!includeSelf)
                ret--;

            return ret;
        }

        private static void GetMinimalNumber(List<int> factors, int pos, int prevexp, int nfactors, int cfactors,
            BigInteger current, ref BigInteger min)
        {
            if (pos == factors.Count)
                return;

            int tfactors = cfactors * 2, cexp = 1;
            current *= factors[pos];

            if (current >= min)
                return;
            if (tfactors > nfactors)
            {
                min = current;
                return;
            }

            while (true)
            {
                GetMinimalNumber(factors, pos + 1, cexp, nfactors, tfactors, current, ref min);
                current *= factors[pos];
                tfactors += cfactors;
                cexp++;

                if (current >= min || cexp > prevexp)
                    break;
                if (tfactors > nfactors)
                {
                    min = current;
                    break;
                }
            }
        }

        public static BigInteger GetMinimalNumber(List<int> factors, int nfactors)
        {
            // Calculate approximate value
            BigInteger ret = 1;
            int i, f = 1;
            for (i = 0; f <= nfactors; i++)
            {
                ret *= factors[i];
                f *= 2;
            }
            GetMinimalNumber(factors, 0, i, nfactors, 1, 1, ref ret);

            return ret;
        }

        private static void GetMinimalSquareNumber(List<int> factors, int pos, int prevexp, int nfactors, int cfactors,
            BigInteger current, ref BigInteger min)
        {
            if (pos == factors.Count)
                return;

            int tfactors = cfactors * 3, cexp = 2;
            current *= factors[pos] * factors[pos];

            if (current >= min)
                return;
            if (tfactors > nfactors)
            {
                min = current;
                return;
            }

            while (true)
            {
                GetMinimalSquareNumber(factors, pos + 1, cexp, nfactors, tfactors, current, ref min);
                current *= factors[pos] * factors[pos];
                tfactors += cfactors * 2;
                cexp += 2;

                if (current >= min || cexp > prevexp)
                    break;
                if (tfactors > nfactors)
                {
                    min = current;
                    break;
                }
            }
        }

        public static BigInteger GetMinimalSquareNumber(List<int> factors, int nfactors)
        {
            // Calculate approximate value
            BigInteger ret = 1;
            int i, f = 1;
            for (i = 0; f <= nfactors; i++)
            {
                ret *= factors[i] * factors[i];
                f *= 3;
            }
            GetMinimalSquareNumber(factors, 0, i * 2, nfactors, 1, 1, ref ret);

            return ret;
        }

        public static List<long> GetDivisors(Prime primes, long n)
        {
            List<long> ret = new List<long>() { 1 };

            foreach (var p in primes)
            {
                List<long> tmp = new List<long>(ret);
                long tmpp = 1;

                if (n == 1)
                    break;
                if (p * p > n)
                {
                    tmp.AddRange(ret.Select(it => it * n));
                    ret = tmp;
                    break;
                }
                if (n % p != 0)
                    continue;

                while (n % p == 0)
                {
                    n /= p;
                    tmpp *= p;
                    tmp.AddRange(ret.Select(it => it * tmpp));
                }
                ret = tmp;
            }

            return ret;
        }

        public static bool IsPracticalNumber(Prime primes, long n)
        {
            long theta = 1;

            foreach (var p in primes)
            {
                if (n == 1)
                    return true;
                if (n % p != 0)
                    continue;

                if (p > theta + 1)
                    return false;
                while (n % p == 0)
                {
                    theta *= p;
                    n /= p;
                }
                theta *= p;
                theta /= p - 1;
                if (n <= primes.Upper && primes.Contains((int)n))
                    return n <= theta + 1;
            }

            return false;
        }
    }
}