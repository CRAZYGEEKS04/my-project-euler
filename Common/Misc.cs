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
        public static int Modulo(int x, int m)
        {
            return (x % m + m) % m;
        }

        public static long Modulo(long x, long m)
        {
            return (x % m + m) % m;
        }

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
            return (long)(Math.Sqrt(number) + 0.1);
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
    }
}