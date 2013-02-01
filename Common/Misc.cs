using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace ProjectEuler.Common
{
    public static class Misc
    {
        public static bool IsPalindromic(string number)
        {
            for (int i = 0; i < number.Length / 2; i++)
            {
                if (number[i] != number[number.Length - i - 1])
                    return false;
            }

            return true;
        }

        public static bool IsPermutation(string lhs, string rhs)
        {
            var digits = new int[10];

            foreach (var c in lhs)
                digits[c - '0']++;
            foreach (var c in rhs)
            {
                if (digits[c - '0'] == 0)
                    return false;
                digits[c - '0']--;
            }

            return true;
        }

        public static long Pow(long x, int e)
        {
            long ret = 1;

            while (e != 0)
            {
                if ((e & 1) != 0)
                    ret *= x;
                x *= x;
                e >>= 1;
            }

            return ret;
        }

        public static bool IsPerfectSquare(long number)
        {
            var tmp = (long)(Math.Sqrt(number) + 0.1);

            return tmp * tmp == number;
        }

        public static bool IsPerfectSquare(BigInteger number)
        {
            var tmp = Sqrt(number);

            return tmp * tmp == number;
        }

        public static long Sqrt(long number)
        {
            var ret = (long)Math.Sqrt(number);

            while ((ret + 1) * (ret + 1) <= number)
                ret++;
            while (ret * ret > number)
                ret--;

            return ret;
        }

        public static BigInteger Sqrt(BigInteger number)
        {
            // Newton's method (N/g + g)/2
            BigInteger g = 1;

            while (true)
            {
                var newg = (number / g + g) / 2;
                var tmp = newg * newg;

                if (tmp <= number && (newg + 1) * (newg + 1) > number)
                    return newg;
                if (newg == g)
                {
                    if (tmp < number)
                        newg++;
                    else
                        newg--;
                }
                g = newg;
            }
        }

        public static BigInteger Random(BigInteger range)
        {
            var rng = new RNGCryptoServiceProvider();
            var tmp = new byte[range.ToByteArray().Length + 1];

            rng.GetBytes(tmp);

            return new BigInteger(tmp) % range;
        }

        public static int GetDigitalSum(int number)
        {
            int sum = 0;

            while (number > 0)
            {
                sum += number % 10;
                number /= 10;
            }

            return sum;
        }

        public static int GetDigitalRoot(int number)
        {
            int ret = 0;

            while (number > 0)
            {
                ret += number % 10;
                number /= 10;
            }

            if (ret >= 10)
                return GetDigitalRoot(ret);
            else
                return ret;
        }

        public static long SumOfDivisor(long start, long end, long divisor)
        {
            long p1 = start / divisor, p2 = end / divisor;

            if (start % divisor != 0)
                p1++;

            return (p1 + p2) * (p2 - p1 + 1) / 2 * divisor;
        }

        public static long Factorial(int n)
        {
            long ret = 1;

            for (int i = 2; i <= n; i++)
                ret *= i;

            return ret;
        }
    }
}