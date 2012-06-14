using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using ProjectEuler.Common;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// An irrational decimal fraction is created by concatenating the positive
    /// integers:
    ///
    /// 0.12345678910(1)112131415161718192021...
    ///
    /// It can be seen that the 12th digit of the fractional part is 1.
    ///
    /// If d(n) represents the nth digit of the fractional part, find the value of the
    /// following expression.
    ///
    /// d(1) * d(10) * d(100) * d(1000) * d(10000) * d(100000) * d(1000000)
    /// </summary>
    internal sealed class Problem40 : Problem
    {
        public Problem40() : base(40) { }

        private int GetNumber(int idx)
        {
            int step = 1, n = 10, current = 0;

            while (idx > step * n)
            {
                idx -= step * n;
                current += n;
                n = (int)Math.Pow(10, step++) * 9;
            }

            current += idx / step;

            return current.ToString()[idx % step] - '0';
        }

        protected override string Action()
        {
            return (GetNumber(1) * GetNumber(10) * GetNumber(100) * GetNumber(1000) * GetNumber(10000) * GetNumber(100000)
                * GetNumber(1000000)).ToString();
        }
    }

    /// <summary>
    /// We shall say that an n-digit number is pandigital if it makes use of all the
    /// digits 1 to n exactly once. For example, 2143 is a 4-digit pandigital and is
    /// also prime.
    ///
    /// What is the largest n-digit pandigital prime that exists?
    /// </summary>
    internal sealed class Problem41 : Problem
    {
        private const int upper = 100000000;

        public Problem41() : base(41) { }

        protected override string Action()
        {
            // Can't be 9 digits because it's divisible by 3.
            Prime p = new Prime(upper);
            p.GenerateAll();

            foreach (int l in Itertools.Range(8, 4))
            {
                foreach (var nums in Itertools.Permutations(Itertools.Range(l, 1), l))
                {
                    var n = int.Parse(string.Join("", nums));
                    if (p.Contains(n))
                        return n.ToString();
                }
            }

            return null;
        }
    }

    /// <summary>
    /// The nth term of the sequence of triangle numbers is given by, t(n) =
    /// n(n + 1) / 2; so the first ten triangle numbers are:
    ///
    /// 1, 3, 6, 10, 15, 21, 28, 36, 45, 55, ...
    ///
    /// By converting each letter in a word to a number corresponding to its
    /// alphabetical position and adding these values we form a word value. For
    /// example, the word value for SKY is 19 + 11 + 25 = 55 = t(10). If the word value
    /// is a triangle number then we shall call the word a triangle word.
    ///
    /// Using [file D0042.txt], a 16K text file containing nearly two-thousand common
    /// English words, how many are triangle words?
    /// </summary>
    internal sealed class Problem42 : Problem
    {
        public Problem42() : base(42) { }

        private List<string> names;

        protected override void PreAction(string data)
        {
            names = (from word in data.Split(',')
                     select word.Substring(1, word.Length - 2)).ToList();
        }

        protected override string Action()
        {
            var tNumbers = new HashSet<int>();
            int counter = 0;

            for (int i = 1; i < 100; i++)
                tNumbers.Add(i * (i + 1) / 2);

            foreach (var n in names)
            {
                var tmp = 0;

                foreach (var c in n)
                    tmp += c - 'A' + 1;
                if (tNumbers.Contains(tmp))
                    counter++;
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// The number, 1406357289, is a 0 to 9 pandigital number because it is made up of
    /// each of the digits 0 to 9 in some order, but it also has a rather interesting
    /// sub-string divisibility property.
    ///
    /// Let d1 be the 1st digit, d2 be the 2nd digit, and so on. In this way, we note
    /// the following:
    ///
    /// d2d3d4=406 is divisible by 2
    /// d3d4d5=063 is divisible by 3
    /// d4d5d6=635 is divisible by 5
    /// d5d6d7=357 is divisible by 7
    /// d6d7d8=572 is divisible by 11
    /// d7d8d9=728 is divisible by 13
    /// d8d9d10=289 is divisible by 17
    ///
    /// Find the sum of all 0 to 9 pandigital numbers with this property.
    /// </summary>
    internal sealed class Problem43 : Problem
    {
        public Problem43() : base(43) { }

        private List<long> GetValidNumbers(List<int> digits, int[] divisors, long number)
        {
            var ret = new List<long>();

            if (digits.Count == 0)
            {
                ret.Add(number);
                return ret;
            }

            foreach (int n in digits)
            {
                var tmp = number * 10 + n;

                if (tmp % 1000 % divisors[7 - digits.Count] == 0)
                    ret.AddRange(GetValidNumbers(digits.Where(it => it != n).ToList(), divisors, tmp));
            }

            return ret;
        }

        protected override string Action()
        {
            var divisors = new int[] { 2, 3, 5, 7, 11, 13, 17 };
            var ret = new List<long>();

            foreach (var nums in Itertools.Permutations(Itertools.Range(0, 9), 3))
            {
                if (nums[0] == 0)
                    continue;
                ret.AddRange(GetValidNumbers(Itertools.Range(0, 9).Where(it => !nums.Contains(it)).ToList(), divisors,
                    nums[0] * 100 + nums[1] * 10 + nums[2]));
            }

            return ret.Sum().ToString();
        }
    }

    /// <summary>
    /// Pentagonal numbers are generated by the formula, P(n)=n(3n-1)/2. The first ten
    /// pentagonal numbers are:
    ///
    /// 1, 5, 12, 22, 35, 51, 70, 92, 117, 145, ...
    ///
    /// It can be seen that P4 + P7 = 22 + 70 = 92 = P8. However, their difference,
    /// 70 - 22 = 48, is not pentagonal.
    ///
    /// Find the pair of pentagonal numbers, Pj and Pk, for which their sum and
    /// difference is pentagonal and D = |Pk - Pj| is minimised; what is the value of
    /// D?
    /// </summary>
    internal sealed class Problem44 : Problem
    {
        private const int upper = 2500;

        public Problem44() : base(44) { }

        protected override string Action()
        {
            var pNumbers = new List<int>();
            HashSet<int> pNumberSet;

            for (int i = 1; i < upper; i++)
                pNumbers.Add(i * (3 * i - 1) / 2);
            pNumberSet = new HashSet<int>(pNumbers);

            for (int diff = 0; diff < pNumbers.Count; diff++)
            {
                for (int sums = diff + 1; sums < pNumbers.Count; sums++)
                {
                    var pj = pNumbers[sums] - pNumbers[diff];
                    var pk = pNumbers[sums] + pNumbers[diff];

                    if (pj % 2 != 0 || pk % 2 != 0)
                        continue;
                    if (pNumberSet.Contains(pj / 2) && pNumberSet.Contains(pk / 2))
                        return pNumbers[diff].ToString();
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Triangle, pentagonal, and hexagonal numbers are generated by the following
    /// formulae:
    ///
    /// Triangle        T(n)=n(n+1)/2   1, 3, 6, 10, 15, ...
    /// Pentagonal      P(n)=n(3n-1)/2  1, 5, 12, 22, 35, ...
    /// Hexagonal       H(n)=n(2n-1)    1, 6, 15, 28, 45, ...
    ///
    /// It can be verified that T(285) = P(165) = H(143) = 40755.
    ///
    /// Find the next triangle number that is also pentagonal and hexagonal.
    /// </summary>
    internal sealed class Problem45 : Problem
    {
        public Problem45() : base(45) { }

        protected override string Action()
        {
            long tn, pn, hn;
            long ti = 285, pi = 165, hi = 143;

            tn = pn = hn = 40755;

            while (true)
            {
                ti++;
                tn = ti * (ti + 1) / 2;

                if (tn == pn && tn == hn)
                    return tn.ToString();

                if (tn > pn)
                {
                    pi++;
                    pn = pi * (3 * pi - 1) / 2;
                }
                if (tn > hn)
                {
                    hi++;
                    hn = hi * (2 * hi - 1);
                }
            }
        }
    }

    /// <summary>
    /// It was proposed by Christian Goldbach that every odd composite number can be
    /// written as the sum of a prime and twice a square.
    ///
    /// 9 = 7 + 2 * 1^2
    /// 15 = 7 + 2 * 2^2
    /// 21 = 3 + 2 * 3^2
    /// 25 = 7 + 2 * 3^2
    /// 27 = 19 + 2 * 2^2
    /// 33 = 31 + 2 * 1^2
    ///
    /// It turns out that the conjecture was false.
    ///
    /// What is the smallest odd composite that cannot be written as the sum of a prime
    /// and twice a square?
    /// </summary>
    internal sealed class Problem46 : Problem
    {
        private const int upper = 10000;

        public Problem46() : base(46) { }

        protected override string Action()
        {
            var p = new Prime(upper);
            var s = (from n in Itertools.Range(1, (int)Math.Sqrt(upper / 2))
                     select n * n * 2).ToList();

            p.GenerateAll();
            var nums = new HashSet<int>(Itertools.Range(9, upper).Where(it => it % 2 == 1 && !p.Contains(it)));

            foreach (var n in Itertools.Product(p.Nums, s))
            {
                if (nums.Contains(n[0] + n[1]))
                    nums.Remove(n[0] + n[1]);
            }

            var ret = nums.ToList();
            ret.Sort();

            return ret[0].ToString();
        }
    }

    /// <summary>
    /// The first two consecutive numbers to have two distinct prime factors are:
    ///
    /// 14 = 2 * 7
    /// 15 = 3 * 5
    ///
    /// The first three consecutive numbers to have three distinct prime factors are:
    ///
    /// 644 = 2^2 * 7 * 23
    /// 645 = 3 * 5 * 43
    /// 646 = 2 * 17 * 19.
    ///
    /// Find the first four consecutive integers to have four distinct primes factors.
    /// What is the first of these numbers?
    /// </summary>
    internal sealed class Problem47 : Problem
    {
        private const int upper = 1000000;

        public Problem47() : base(47) { }

        private int GetDistinctPrimeFactorNumbers(Prime p, int number)
        {
            int ret = 0;

            foreach (var n in p)
            {
                if (p.Contains(number))
                    return ret + 1;
                if (number == 1)
                    return ret;
                if (number % n == 0)
                {
                    ret++;
                    while (number % n == 0)
                        number /= n;
                }
            }

            return -1;
        }

        protected override string Action()
        {
            var p = new Prime(upper);
            var counter = 0;

            p.GenerateAll();
            for (int i = 644; i < upper; i++)
            {
                if (GetDistinctPrimeFactorNumbers(p, i) == 4)
                    counter++;
                else
                    counter = 0;

                if (counter == 4)
                    return (i - 3).ToString();
            }

            return null;
        }
    }

    /// <summary>
    /// The series, 1^1 + 2^2 + 3^3 + ... + 10^10 = 10405071317.
    ///
    /// Find the last ten digits of the series, 1^1 + 2^2 + 3^3 + ... + 1000^1000.
    /// </summary>
    internal sealed class Problem48 : Problem
    {
        public Problem48() : base(48) { }

        protected override string Action()
        {
            BigInteger sum = 0;

            for (var i = new BigInteger(1); i < 1000; i++)
                sum += BigInteger.Pow(i, (int)i);

            var ret = sum.ToString();

            return ret.Substring(ret.Length - 10);
        }
    }

    /// <summary>
    /// The arithmetic sequence, 1487, 4817, 8147, in which each of the terms increases
    /// by 3330, is unusual in two ways: (i) each of the three terms are prime, and,
    /// (ii) each of the 4-digit numbers are permutations of one another.
    ///
    /// There are no arithmetic sequences made up of three 1-, 2-, or 3-digit primes,
    /// exhibiting this property, but there is one other 4-digit increasing sequence.
    ///
    /// What 12-digit number do you form by concatenating the three terms in this
    /// sequence?
    /// </summary>
    internal sealed class Problem49 : Problem
    {
        public Problem49() : base(49) { }

        private int[] GetPermutations(int n)
        {
            return (from d in Itertools.Permutations(n.ToString(), 4)
                    select int.Parse(string.Join("", d))).ToArray();
        }

        protected override string Action()
        {
            var p = new Prime(10000);

            p.GenerateAll();

            foreach (int n in p)
            {
                if (n <= 1487)
                    continue;

                if (n == 2969)
                {
                    var x = n;
                    x++;
                }
                var nums = GetPermutations(n);
                if (nums.Contains(n + 3330) && nums.Contains(n + 6660) && p.Contains(n + 3330) && p.Contains(n + 6660))
                    return n.ToString() + (n + 3330).ToString() + (n + 6660).ToString();
            }

            return null;
        }
    }
}