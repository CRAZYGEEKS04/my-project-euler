﻿using ProjectEuler.Common;
using ProjectEuler.Common.Miscellany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// If we list all the natural numbers below 10 that are multiples of 3 or 5, we
    /// get 3, 5, 6 and 9. The sum of these multiples is 23.
    ///
    /// Find the sum of all the multiples of 3 or 5 below 1000.
    /// </summary>
    internal sealed class Problem1 : Problem
    {
        private BigInteger upper = 999;

        public Problem1() : base(1) { }

        private BigInteger SumDivisibleBy(BigInteger divisor)
        {
            BigInteger n = upper / divisor;

            return divisor * (n + 1) * n / 2;
        }

        protected override string Action()
        {
            return (SumDivisibleBy(3) + SumDivisibleBy(5) - SumDivisibleBy(15)).ToString();
        }
    }

    /// <summary>
    /// Each new term in the Fibonacci sequence is generated by adding the previous two
    /// terms. By starting with 1 and 2, the first 10 terms will be:
    ///
    /// 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, ...
    ///
    /// By considering the terms in the Fibonacci sequence whose values do not exceed
    /// four million, find the sum of the even-valued terms.
    /// </summary>
    internal sealed class Problem2 : Problem
    {
        private BigInteger upper = 4000000;

        public Problem2() : base(2) { }

        protected override string Action()
        {
            Fibonacci f = new Fibonacci(1, 2, upper);
            BigInteger sum = 0;

            foreach (BigInteger n in f)
                if (n % 2 == 0)
                    sum += n;

            return sum.ToString();
        }
    }

    /// <summary>
    /// The prime factors of 13195 are 5, 7, 13 and 29.
    ///
    /// What is the largest prime factor of the number 600851475143 ?
    /// </summary>
    internal sealed class Problem3 : Problem
    {
        private BigInteger number = 600851475143L;

        public Problem3() : base(3) { }

        protected override string Action()
        {
            BigInteger n = number;
            Prime prime = new Prime((int)Math.Sqrt((double)n) + 1);

            foreach (int p in prime)
            {
                while (n % p == 0)
                    n /= p;
                if (n == 1)
                    return p.ToString();
            }

            return null;
        }
    }

    /// <summary>
    /// A palindromic number reads the same both ways. The largest palindrome made from
    /// the product of two 2-digit numbers is 9009 = 91 * 99.
    ///
    /// Find the largest palindrome made from the product of two 3-digit numbers.
    /// </summary>
    internal sealed class Problem4 : Problem
    {
        public Problem4() : base(4) { }

        protected override string Action()
        {
            int ret = 0;

            for (int i = 999; i >= 100; i--)
            {
                if (i * i <= ret)
                    break;
                for (int j = i; j >= 100; j--)
                {
                    int tmp = i * j;
                    if (tmp <= ret)
                        break;
                    if (Misc.IsPalindromic(tmp.ToString()))
                        ret = tmp;
                }
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// 2520 is the smallest number that can be divided by each of the numbers from 1
    /// to 10 without any remainder.
    ///
    /// What is the smallest positive number that is evenly divisible by all of the
    /// numbers from 1 to 20?
    /// </summary>
    internal sealed class Problem5 : Problem
    {
        private const int upper = 20;

        public Problem5() : base(5) { }

        protected override string Action()
        {
            List<int> factors = new List<int>();
            BigInteger ret = 1;

            for (int i = 2; i < upper; i++)
                factors.Add(i);

            for (int i = 0; i < factors.Count; i++)
            {
                if (factors[i] == 1)
                    continue;
                ret *= factors[i];
                for (int j = i + 1; j < factors.Count; j++)
                    if (factors[j] % factors[i] == 0)
                        factors[j] /= factors[i];
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// The sum of the squares of the first ten natural numbers is,
    /// 1^2 + 2^2 + ... + 10^2 = 385
    ///
    /// The square of the sum of the first ten natural numbers is,
    /// (1 + 2 + ... + 10)^2 = 55^2 = 3025
    ///
    /// Hence the difference between the sum of the squares of the first ten natural
    /// numbers and the square of the sum is 3025 - 385 = 2640.
    ///
    /// Find the difference between the sum of the squares of the first one hundred
    /// natural numbers and the square of the sum.
    /// </summary>
    internal sealed class Problem6 : Problem
    {
        private BigInteger upper = 100;

        public Problem6() : base(6) { }

        protected override string Action()
        {
            BigInteger ret;

            ret = upper * (upper + 1) / 2;
            ret *= ret;
            for (BigInteger i = 1; i <= upper; i++)
                ret -= i * i;

            return ret.ToString();
        }
    }

    /// <summary>
    /// By listing the first six prime numbers: 2, 3, 5, 7, 11, and 13, we can see that
    /// the 6th prime is 13.
    ///
    /// What is the 10001st prime number?
    /// </summary>
    internal sealed class Problem7 : Problem
    {
        private const int index = 10001;

        public Problem7() : base(7) { }

        protected override string Action()
        {
            Prime prime = new Prime(index * 100);

            for (int i = prime.Nums.Count; i < index - 1; i++)
                prime.GenerateNext();

            return prime.GenerateNext().ToString();
        }
    }

    /// <summary>
    /// Find the greatest product of five consecutive digits in the 1000-digit number
    ///
    /// [file D0008.txt]
    /// </summary>
    internal sealed class Problem8 : Problem
    {
        public Problem8() : base(8) { }

        private int[] digits;

        protected override void PreAction(string data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string s in data.Split('\n'))
                sb.Append(s.Trim());
            digits = (from c in sb.ToString()
                      select (int)(c - '0')).ToArray();
        }

        protected override string Action()
        {
            int tmp, ret = 0;

            for (int i = 0; i < digits.Length - 4; i++)
            {
                tmp = digits[i] * digits[i + 1] * digits[i + 2] * digits[i + 3] * digits[i + 4];
                if (tmp > ret)
                    ret = tmp;
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// A Pythagorean triplet is a set of three natural numbers, a  b  c, for which,
    /// a^2 + b^2 = c^2
    /// For example, 3^2 + 4^2 = 9 + 16 = 25 = 5^2.
    ///
    /// There exists exactly one Pythagorean triplet for which a + b + c = 1000.
    /// Find the product abc.
    /// </summary>
    internal sealed class Problem9 : Problem
    {
        private const int sum = 1000;

        public Problem9() : base(9) { }

        protected override string Action()
        {
            int i, j, k, tmp;

            for (i = 3; i < sum / 3; i++)
                for (j = i + 1; j < (sum - i) / 2; j++)
                {
                    k = sum - i - j;
                    tmp = i * i + j * j;
                    if (tmp < k * k)
                        continue;
                    if (tmp > k * k)
                        break;
                    return (i * j * k).ToString();
                }

            return null;
        }
    }
}