using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using ProjectEuler.Common;
using ProjectEuler.Common.Miscellany;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// n! means n * (n  1) * ... * 3 * 2 * 1
    ///
    /// Find the sum of the digits in the number 100!
    /// </summary>
    internal sealed class Problem20 : Problem
    {
        private const int upper = 100;

        public Problem20() : base(20) { }

        protected override string Action()
        {
            BigInteger num = 1;

            for (int i = 1; i <= upper; i++)
                num *= i;

            return (from c in num.ToString()
                    select (int)(c - '0')).Sum().ToString();
        }
    }

    /// <summary>
    /// Let d(n) be defined as the sum of proper divisors of n (numbers less than n
    /// which divide evenly into n).
    /// If d(a) = b and d(b) = a, where a != b, then a and b are an amicable pair and
    /// each of a and b are called amicable numbers.
    ///
    /// For example, the proper divisors of 220 are 1, 2, 4, 5, 10, 11, 20, 22, 44, 55
    /// and 110; therefore d(220) = 284. The proper divisors of 284 are 1, 2, 4, 71
    /// and 142; so d(284) = 220.
    ///
    /// Evaluate the sum of all the amicable numbers under 10000.
    /// </summary>
    internal sealed class Problem21 : Problem
    {
        private const int upper = 10000;

        public Problem21() : base(21) { }

        protected override string Action()
        {
            HashSet<int> amicableNums = new HashSet<int>();
            Prime p = new Prime(upper);
            int tmp;

            p.GenerateAll();
            for (int i = 2; i < upper; i++)
            {
                if (amicableNums.Contains(i))
                    continue;
                tmp = (int)Factor.GetFactorSum(p, i);
                if (tmp != i && Factor.GetFactorSum(p, tmp) == i)
                {
                    amicableNums.Add(i);
                    amicableNums.Add(tmp);
                }
            }

            return amicableNums.Sum().ToString();
        }
    }

    /// <summary>
    /// Using [file D0022.txt], a 46K text file containing over five-thousand first
    /// names, begin by sorting it into alphabetical order. Then working out the
    /// alphabetical value for each name, multiply this value by its alphabetical
    /// position in the list to obtain a name score.
    ///
    /// For example, when the list is sorted into alphabetical order, COLIN, which is
    /// worth 3 + 15 + 12 + 9 + 14 = 53, is the 938th name in the list. So, COLIN would
    /// obtain a score of 938 * 53 = 49714.
    ///
    /// What is the total of all the name scores in the file?
    /// </summary>
    internal sealed class Problem22 : Problem
    {
        public Problem22() : base(22) { }

        private List<string> names;

        protected override void PreAction(string data)
        {
            names = (from word in data.Split(',')
                     select word.Substring(1, word.Length - 2)).ToList();
        }

        protected override string Action()
        {
            BigInteger ret = 0, score;

            names.Sort();
            for (int i = 0; i < names.Count; i++)
            {
                score = 0;
                foreach (char c in names[i])
                    score += c - 'A' + 1;
                ret += score * (i + 1);
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// A perfect number is a number for which the sum of its proper divisors is
    /// exactly equal to the number. For example, the sum of the proper divisors of 28
    /// would be 1 + 2 + 4 + 7 + 14 = 28, which means that 28 is a perfect number.
    ///
    /// A number n is called deficient if the sum of its proper divisors is less than
    /// n and it is called abundant if this sum exceeds n.
    ///
    /// As 12 is the smallest abundant number, 1 + 2 + 3 + 4 + 6 = 16, the smallest
    /// number that can be written as the sum of two abundant numbers is 24. By
    /// mathematical analysis, it can be shown that all integers greater than 28123 can
    /// be written as the sum of two abundant numbers. However, this upper limit cannot
    /// be reduced any further by analysis even though it is known that the greatest
    /// number that cannot be expressed as the sum of two abundant numbers is less than
    /// this limit.
    ///
    /// Find the sum of all the positive integers which cannot be written as the sum of
    /// two abundant numbers.
    /// </summary>
    internal sealed class Problem23 : Problem
    {
        private const int upper = 28124;

        public Problem23() : base(23) { }

        protected override string Action()
        {
            List<int> abundantNums = new List<int>();
            HashSet<int> numbers = new HashSet<int>();
            Prime p = new Prime(upper);
            BigInteger ret = 0;

            p.GenerateAll();
            for (int i = 1; i < upper; i++)
            {
                numbers.Add(i);
                if (Factor.GetFactorSum(p, i) > i)
                    abundantNums.Add(i);
            }

            for (int i = 0; i < abundantNums.Count; i++)
                for (int j = i; j < abundantNums.Count; j++)
                {
                    if (numbers.Contains(abundantNums[i] + abundantNums[j]))
                        numbers.Remove(abundantNums[i] + abundantNums[j]);
                }

            foreach (int i in numbers)
                ret += i;

            return ret.ToString();
        }
    }

    /// <summary>
    /// A permutation is an ordered arrangement of objects. For example, 3124 is one
    /// possible permutation of the digits 1, 2, 3 and 4. If all of the permutations
    /// are listed numerically or alphabetically, we call it lexicographic order. The
    /// lexicographic permutations of 0, 1 and 2 are:
    /// 012  021  102  120  201  210
    ///
    /// What is the millionth lexicographic permutation of the digits 0, 1, 2, 3, 4, 5,
    /// 6, 7, 8 and 9?
    /// </summary>
    internal sealed class Problem24 : Problem
    {
        private const int index = 1000000;

        public Problem24() : base(24) { }

        protected override string Action()
        {
            List<int> counters = new List<int>();
            List<int> digits = new List<int>();
            BigInteger ret = 0;
            int tmp = index, pos;

            for (int i = 0; i < 10; i++)
            {
                digits.Add(i);
                counters.Add((int)Probability.CountPermutations(i + 1, i + 1));
            }

            counters.Reverse();
            counters.RemoveAt(0);
            tmp--;
            foreach (int counter in counters)
            {
                pos = tmp / counter;
                tmp = tmp % counter;
                ret = ret * 10 + digits[pos];
                digits.RemoveAt(pos);
            }
            ret = ret * 10 + digits[0];

            return ret.ToString();
        }
    }

    /// <summary>
    /// The Fibonacci sequence is defined by the recurrence relation:
    ///
    /// Fn = F(n1) + F(n2), where F(1) = 1 and F(2) = 1.
    /// Hence the first 12 terms will be:
    ///
    /// F(1) = 1
    /// F(2) = 1
    /// F(3) = 2
    /// F(4) = 3
    /// F(5) = 5
    /// F(6) = 8
    /// F(7) = 13
    /// F(8) = 21
    /// F(9) = 34
    /// F(10) = 55
    /// F(11) = 89
    /// F(12) = 144
    /// The 12th term, F(12), is the first term to contain three digits.
    ///
    /// What is the first term in the Fibonacci sequence to contain 1000 digits?
    /// </summary>
    internal sealed class Problem25 : Problem
    {
        private const int length = 1000;

        public Problem25() : base(25) { }

        protected override string Action()
        {
            Fibonacci f = new Fibonacci(1, 1, 0);
            int idx = 0;

            foreach (BigInteger num in f)
            {
                idx++;
                if (num.ToString().Length >= length)
                    break;
            }

            return idx.ToString();
        }
    }

    /// <summary>
    /// A unit fraction contains 1 in the numerator. The decimal representation of the
    /// unit fractions with denominators 2 to 10 are given:
    ///
    /// 1/2 = 0.5
    /// 1/3 = 0.(3)
    /// 1/4 = 0.25
    /// 1/5 = 0.2
    /// 1/6 = 0.1(6)
    /// 1/7 = 0.(142857)
    /// 1/8 = 0.125
    /// 1/9 = 0.(1)
    /// 1/10 =  0.1
    ///
    /// Where 0.1(6) means 0.166666..., and has a 1-digit recurring cycle. It can be
    /// seen that 1/7 has a 6-digit recurring cycle.
    ///
    /// Find the value of d < 1000 for which 1/d contains the longest recurring cycle
    /// in its decimal fraction part.
    /// </summary>
    internal sealed class Problem26 : Problem
    {
        private const int upper = 1000;

        public Problem26() : base(26) { }

        private int GetCycleLength(int n)
        {
            StringBuilder num = new StringBuilder("9");

            while (n % 2 == 0)
                n /= 2;
            while (n % 5 == 0)
                n /= 5;

            while (true)
            {
                if (BigInteger.Parse(num.ToString()) % n == 0)
                    break;
                num.Append("9");
            }

            return num.Length;
        }

        protected override string Action()
        {
            Prime prime = new Prime(upper);
            int max = 0, idx = 0;

            foreach (int i in prime)
            {
                int tmp = GetCycleLength(i);
                if (tmp > max)
                {
                    max = tmp;
                    idx = i;
                }
            }

            return idx.ToString();
        }
    }

    /// <summary>
    /// Euler published the remarkable quadratic formula:
    ///
    /// n ^ 2 + n + 41
    ///
    /// It turns out that the formula will produce 40 primes for the consecutive values
    /// n = 0 to 39. However, when n = 40, 40 ^ 2 + 40 + 41 = 40 * (40 + 1) + 41 is
    /// divisible by 41, and certainly when n = 41, 41 ^ 2 + 41 + 41 is clearly
    /// divisible by 41.
    ///
    /// Using computers, the incredible formula  n ^ 2 - 79n + 1601 was discovered,
    /// which produces 80 primes for the consecutive values n = 0 to 79. The product
    /// of the coefficients, 79 and 1601, is 126479.
    ///
    /// Considering quadratics of the form:
    ///
    /// n ^ 2 + an + b, where |a| < 1000 and |b| < 1000
    ///
    /// where |n| is the modulus/absolute value of n
    /// e.g. |11| = 11 and |-4| = 4
    ///
    /// Find the product of the coefficients, a and b, for the quadratic expression
    /// that produces the maximum number of primes for consecutive values of n,
    /// starting with n = 0.
    /// </summary>
    internal sealed class Problem27 : Problem
    {
        private const int upper = 1000;

        public Problem27() : base(27) { }

        private int CountCosecutivePrimes(Prime p, int a, int b)
        {
            int tmp;
            for (int x = 0; ; x++)
            {
                tmp = x * x + a * x + b;
                if ((tmp <= p.Upper && !p.Contains(x * x + a * x + b))
                    || !p.IsPrime(tmp))
                    return x;
            }
        }

        protected override string Action()
        {
            Prime prime = new Prime(upper * upper);
            int ret = 0, max = 0;

            prime.GenerateAll();
            foreach (int[] tuples in Itertools.PermutationsWithReplacement(
                    Itertools.Range(1 - upper, upper - 1), 2))
            {
                int i = tuples[0], j = tuples[1];
                int tmp = CountCosecutivePrimes(prime, tuples[0], tuples[1]);
                if (tmp > max)
                {
                    max = tmp;
                    ret = tuples[0] * tuples[1];
                }
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// Starting with the number 1 and moving to the right in a clockwise direction a
    /// 5 by 5 spiral is formed as follows:
    ///
    /// 21 22 23 24 25
    /// 20  7  8  9 10
    /// 19  6  1  2 11
    /// 18  5  4  3 12
    /// 17 16 15 14 13
    ///
    /// It can be verified that the sum of the numbers on the diagonals is 101.
    ///
    /// What is the sum of the numbers on the diagonals in a 1001 by 1001 spiral formed
    /// in the same way?
    /// </summary>
    internal sealed class Problem28 : Problem
    {
        private const int upper = 1001;

        public Problem28() : base(28) { }

        protected override string Action()
        {
            BigInteger sum = 1;
            int current = 1, step = 2;

            while (step <= upper)
            {
                for (int i = 0; i < 4; i++)
                {
                    current += step;
                    sum += current;
                }
                step += 2;
            }

            return sum.ToString();
        }
    }

    /// <summary>
    /// Consider all integer combinations of a ^ b for 2 <= a <= 5 and 2 <= b <= 5:
    ///
    /// 2^2=4, 2^3=8, 2^4=16, 2^5=32
    /// 3^2=9, 3^3=27, 3^4=81, 3^5=243
    /// 4^2=16, 4^3=64, 4^4=256, 4^5=1024
    /// 5^2=25, 5^3=125, 5^4=625, 5^5=3125
    ///
    /// If they are then placed in numerical order, with any repeats removed, we get
    /// the following sequence of 15 distinct terms:
    ///
    /// 4, 8, 9, 16, 25, 27, 32, 64, 81, 125, 243, 256, 625, 1024, 3125
    ///
    /// How many distinct terms are in the sequence generated by a^b for 2 <= a <= 100
    /// and 2 <= b <= 100?
    /// </summary>
    internal sealed class Problem29 : Problem
    {
        private static int upper = 100;

        public Problem29() : base(29) { }

        protected override string Action()
        {
            HashSet<BigInteger> numbers = new HashSet<BigInteger>();

            foreach (int[] tuples in Itertools.PermutationsWithReplacement(
                Itertools.Range(2, upper), 2))
                numbers.Add(BigInteger.Pow(tuples[0], tuples[1]));

            return numbers.Count.ToString();
        }
    }
}