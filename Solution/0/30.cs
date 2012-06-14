using System;
using System.Collections.Generic;
using ProjectEuler.Common;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// Surprisingly there are only three numbers that can be written as the sum of
    /// fourth powers of their digits:
    ///
    /// 1634 = 1^4 + 6^4 + 3^4 + 4^4
    /// 8208 = 8^4 + 2^4 + 0^4 + 8^4
    /// 9474 = 9^4 + 4^4 + 7^4 + 4^4
    /// As 1 = 1^4 is not a sum it is not included.
    ///
    /// The sum of these numbers is 1634 + 8208 + 9474 = 19316.
    ///
    /// Find the sum of all the numbers that can be written as the sum of fifth powers
    /// of their digits.
    /// </summary>
    internal sealed class Problem30 : Problem
    {
        public Problem30() : base(30) { }

        protected override string Action()
        {
            int[] powers = new int[10];
            int ret = 0;

            for (int i = 0; i < 10; i++)
                powers[i] = i * i * i * i * i;
            for (int i = 2; i < powers[9] * 6; i++)
            {
                int tmp = 0, num = i;
                while (num > 0)
                {
                    tmp += powers[num % 10];
                    num /= 10;
                }
                if (tmp == i)
                    ret += tmp;
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// In England the currency is made up of pound, £, and pence, p, and there are
    /// eight coins in general circulation:
    ///
    /// 1p, 2p, 5p, 10p, 20p, 50p, £1 (100p) and £2 (200p).
    /// It is possible to make £2 in the following way:
    ///
    /// 1£1 + 1 50p + 2 20p + 1 5p + 1 2p + 3 1p
    /// How many different ways can £2 be made using any number of coins?
    /// </summary>
    internal sealed class Problem31 : Problem
    {
        private const int totalamount = 200;
        private static int[] coins = new int[] { 200, 100, 50, 20, 10, 5, 2, 1 };

        public Problem31() : base(31) { }

        private int Divide(int value, int idx)
        {
            int ret = 0;

            if (idx == coins.Length - 1)
                return 1;

            while (value >= 0)
            {
                ret += Divide(value, idx + 1);
                value -= coins[idx];
            }

            return ret;
        }

        protected override string Action()
        {
            return Divide(totalamount, 0).ToString();
        }
    }

    /// <summary>
    /// We shall say that an n-digit number is pandigital if it makes use of all the
    /// digits 1 to n exactly once; for example, the 5-digit number, 15234, is 1
    /// through 5 pandigital.
    ///
    /// The product 7254 is unusual, as the identity, 39 * 186 = 7254, containing
    /// multiplicand, multiplier, and product is 1 through 9 pandigital.
    ///
    /// Find the sum of all products whose multiplicand/multiplier/product identity can
    /// be written as a 1 through 9 pandigital.
    ///
    /// HINT: Some products can be obtained in more than one way so be sure to only
    /// include it once in your sum.
    /// </summary>
    internal sealed class Problem32 : Problem
    {
        private static HashSet<int> digits = new HashSet<int>();

        public Problem32() : base(32) { }

        private bool Check(int a, int b)
        {
            int product = a * b;

            digits.Clear();
            foreach (int num in new int[] { a, b, product })
            {
                int tmp = num;

                while (tmp > 0)
                {
                    if (digits.Contains(tmp % 10))
                        return false;
                    digits.Add(tmp % 10);
                    tmp /= 10;
                }
            }

            if (digits.Contains(0) || digits.Count != 9)
                return false;
            else
                return true;
        }

        protected override string Action()
        {
            HashSet<int> numbers = new HashSet<int>();
            int ret = 0;

            foreach (int[] nums in Itertools.Product(Itertools.Range(1, 9),
                Itertools.Range(1234, 9876)))
            {
                if (Check(nums[0], nums[1]))
                    numbers.Add(nums[0] * nums[1]);
            }
            foreach (int[] nums in Itertools.Product(Itertools.Range(12, 98),
                Itertools.Range(123, 987)))
            {
                if (Check(nums[0], nums[1]))
                    numbers.Add(nums[0] * nums[1]);
            }

            foreach (int n in numbers)
                ret += n;

            return ret.ToString();
        }
    }

    /// <summary>
    /// The fraction 49/98 is a curious fraction, as an inexperienced mathematician in
    /// attempting to simplify it may incorrectly believe that 49/98 = 4/8, which is
    /// correct, is obtained by cancelling the 9s.
    ///
    /// We shall consider fractions like, 30/50 = 3/5, to be trivial examples.
    ///
    /// There are exactly four non-trivial examples of this type of fraction, less than
    /// one in value, and containing two digits in the numerator and denominator.
    ///
    /// If the product of these four fractions is given in its lowest common terms,
    /// find the value of the denominator.
    /// </summary>
    internal sealed class Problem33 : Problem
    {
        public Problem33() : base(33) { }

        private bool Check(int n, int d)
        {
            return ((n % 10 == d / 10 && n / 10 * d == d % 10 * n) || (n / 10 == d % 10 && n % 10 * d == d / 10 * n));
        }

        protected override string Action()
        {
            int n = 1, d = 1;
            foreach (var nums in Itertools.Combinations(Itertools.Range(11, 99), 2))
            {
                if (Check(nums[0], nums[1]))
                {
                    n *= nums[0];
                    d *= nums[1];
                    var c = (int)Factor.GetCommonFactor(n, d);
                    n /= c;
                    d /= c;
                }
            }

            return d.ToString();
        }
    }

    /// <summary>
    /// 145 is a curious number, as 1! + 4! + 5! = 1 + 24 + 120 = 145.
    ///
    /// Find the sum of all numbers which are equal to the sum of the factorial of
    /// their digits.
    ///
    /// Note: as 1! = 1 and 2! = 2 are not sums they are not included.
    /// </summary>
    internal sealed class Problem34 : Problem
    {
        public Problem34() : base(34) { }

        private bool IsCuriousNumber(int[] factorials, int number)
        {
            int sum = 0;

            foreach (var c in number.ToString())
                sum += factorials[c - '0'];

            return sum == number;
        }

        protected override string Action()
        {
            var factorials = new int[10];
            int ret = 0;

            factorials[0] = 1;
            for (int i = 1; i < 10; i++)
                factorials[i] = factorials[i - 1] * i;

            for (int n = 10; n < factorials[9] * 6; n++)
            {
                if (IsCuriousNumber(factorials, n))
                    ret += n;
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// The number, 197, is called a circular prime because all rotations of the
    /// digits: 197, 971, and 719, are themselves prime.
    ///
    /// There are thirteen such primes below 100: 2, 3, 5, 7, 11, 13, 17, 31, 37, 71,
    /// 73, 79, and 97.
    ///
    /// How many circular primes are there below one million?
    /// </summary>
    internal sealed class Problem35 : Problem
    {
        private const int upper = 1000000;

        public Problem35() : base(35) { }

        private HashSet<int> GetCircularNumbers(string number)
        {
            var ret = new HashSet<int>();

            for (int i = 0; i < number.ToString().Length; i++)
                ret.Add(int.Parse(number.Substring(i) + number.Substring(0, i)));

            return ret;
        }

        protected override string Action()
        {
            var primes = new Prime(upper);
            var ret = 0;

            primes.GenerateAll();
            var num = new HashSet<int>(primes.Nums);

            foreach (int p in primes)
            {
                var flag = true;

                if (!num.Contains(p))
                    continue;

                var cnums = GetCircularNumbers(p.ToString());
                foreach (var n in cnums)
                {
                    if (!num.Contains(n))
                    {
                        flag = false;
                        break;
                    }
                    num.Remove(n);
                }

                if (flag)
                    ret += cnums.Count;
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// The decimal number, 585 = 1001001001(binary), is palindromic in both bases.
    ///
    /// Find the sum of all numbers, less than one million, which are palindromic in
    /// base 10 and base 2.
    ///
    /// (Please note that the palindromic number, in either base, may not include
    /// leading zeros.)
    /// </summary>
    internal sealed class Problem36 : Problem
    {
        private const int upper = 1000000;

        public Problem36() : base(36) { }

        protected override string Action()
        {
            int ret = 0;

            for (int i = 1; i < upper; i++)
            {
                if (Misc.IsPalindromic(i.ToString()) && Misc.IsPalindromic(Convert.ToString(i, 2)))
                    ret += i;
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// The number 3797 has an interesting property. Being prime itself, it is possible
    /// to continuously remove digits from left to right, and remain prime at each
    /// stage: 3797, 797, 97, and 7. Similarly we can work from right to left: 3797,
    /// 379, 37, and 3.
    ///
    /// Find the sum of the only eleven primes that are both truncatable from left to
    /// right and right to left.
    ///
    /// NOTE: 2, 3, 5, and 7 are not considered to be truncatable primes.
    /// </summary>
    internal sealed class Problem37 : Problem
    {
        private const int upper = 1000000;

        public Problem37() : base(37) { }

        private bool Check(Prime p, string n)
        {
            for (int i = 1; i < n.Length; i++)
            {
                if (!p.Contains(int.Parse(n.Substring(i))) || !p.Contains(int.Parse(n.Substring(0, i))))
                    return false;
            }

            return true;
        }

        protected override string Action()
        {
            Prime p = new Prime(upper);
            int counter = 0, ret = 0, n;

            while ((n = p.GenerateNext()) < 10) ;

            while (counter < 11)
            {
                if (Check(p, n.ToString()))
                {
                    counter++;
                    ret += n;
                }
                n = p.GenerateNext();
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// Take the number 192 and multiply it by each of 1, 2, and 3:
    ///
    /// 192 * 1 = 192
    /// 192 * 2 = 384
    /// 192 * 3 = 576
    ///
    /// By concatenating each product we get the 1 to 9 pandigital, 192384576. We will
    /// call 192384576 the concatenated product of 192 and (1,2,3)
    ///
    /// The same can be achieved by starting with 9 and multiplying by 1, 2, 3, 4, and
    /// 5, giving the pandigital, 918273645, which is the concatenated product of 9 and
    /// (1,2,3,4,5).
    ///
    /// What is the largest 1 to 9 pandigital 9-digit number that can be formed as the
    /// concatenated product of an integer with (1,2, ... , n) where n > 1?
    /// </summary>
    internal sealed class Problem38 : Problem
    {
        public Problem38() : base(38) { }

        private bool Check(string number)
        {
            if (number.Length != 9)
                return false;

            for (int i = 1; i < 10; i++)
                if (!number.Contains(i.ToString()))
                    return false;

            return true;
        }

        protected override string Action()
        {
            var nums = new List<int>();

            for (int i = 9876; i >= 1234; i--)
            {
                var number = i.ToString() + (i * 2).ToString();
                if (Check(number))
                {
                    nums.Add(int.Parse(number));
                    break;
                }
            }
            for (int i = 987; i >= 123; i--)
            {
                var number = i.ToString() + (i * 2).ToString() + (i * 3).ToString();
                if (Check(number))
                {
                    nums.Add(int.Parse(number));
                    break;
                }
            }
            for (int i = 98; i >= 12; i--)
            {
                var number = i.ToString() + (i * 2).ToString() + (i * 3).ToString() + (i * 4).ToString();
                if (Check(number))
                {
                    nums.Add(int.Parse(number));
                    break;
                }
            }
            nums.Add(918273645);

            nums.Sort();
            nums.Reverse();

            return nums[0].ToString();
        }
    }

    /// <summary>
    /// If p is the perimeter of a right angle triangle with integral length sides,
    /// {a,b,c}, there are exactly three solutions for p = 120.
    ///
    /// {20,48,52}, {24,45,51}, {30,40,50}
    ///
    /// For which value of p <= 1000, is the number of solutions maximised?
    /// </summary>
    internal sealed class Problem39 : Problem
    {
        public Problem39() : base(39) { }

        protected override string Action()
        {
            var r = new Dictionary<int, int>();

            for (int a = 3; a <= 333; a++)
                for (int b = a + 1; b <= (1000 - a) / 2; b++)
                    for (int c = b + 1; c <= 1000 - a - b; c++)
                    {
                        if (a * a + b * b == c * c)
                        {
                            if (r.ContainsKey(a + b + c))
                                r[a + b + c]++;
                            else
                                r.Add(a + b + c, 1);
                        }
                    }

            int n = 0, id = 0;
            foreach (var p in r)
            {
                if (p.Value > n)
                {
                    n = p.Value;
                    id = p.Key;
                }
            }

            return id.ToString();
        }
    }
}