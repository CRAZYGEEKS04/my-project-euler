using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using ProjectEuler.Common;
using ProjectEuler.Common.Miscellany;
using ProjectEuler.Common.Partition;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// In the following equation x, y, and n are positive integers.
    ///
    /// 1/x + 1/y = 1/n
    ///
    /// It can be verified that when n = 1260 there are 113 distinct solutions and this
    /// is the least value of n for which the total number of distinct solutions
    /// exceeds one hundred.
    ///
    /// What is the least value of n for which the number of distinct solutions exceeds
    /// four million?
    /// </summary>
    internal class Problem110 : Problem
    {
        private const int upper = 4000000;

        public Problem110() : base(110) { }

        protected override string Action()
        {
            var p = new Prime(10000);

            p.GenerateAll();

            return Misc.Sqrt(Factor.GetMinimalSquareNumber(p.Nums, upper * 2)).ToString();
        }
    }

    /// <summary>
    /// Considering 4-digit primes containing repeated digits it is clear that they
    /// cannot all be the same: 1111 is divisible by 11, 2222 is divisible by 22, and
    /// so on. But there are nine 4-digit primes containing three ones:
    ///
    /// 1117, 1151, 1171, 1181, 1511, 1811, 2111, 4111, 8111
    ///
    /// We shall say that M(n, d) represents the maximum number of repeated digits for
    /// an n-digit prime where d is the repeated digit, N(n, d) represents the number
    /// of such primes, and S(n, d) represents the sum of these primes.
    ///
    /// So M(4, 1) = 3 is the maximum number of repeated digits for a 4-digit prime
    /// where one is the repeated digit, there are N(4, 1) = 9 such primes, and the sum
    /// of these primes is S(4, 1) = 22275. It turns out that for d = 0, it is only
    /// possible to have M(4, 0) = 2 repeated digits, but there are N(4, 0) = 13 such
    /// cases.
    ///
    /// In the same way we obtain the following results for 4-digit primes.
    ///
    /// Digit, d M(4, d) N(4, d) S(4, d)
    ///        0      2      13   67061
    ///        1      3       9   22275
    ///        2      3       1    2221
    ///        3      3      12   46214
    ///        4      3       2    8888
    ///        5      3       1    5557
    ///        6      3       1    6661
    ///        7      3       9   57863
    ///        8      3       1    8887
    ///        9      3       7   48073
    ///
    /// For d = 0 to 9, the sum of all S(4, d) is 273700.
    ///
    /// Find the sum of all S(10, d).
    /// </summary>
    internal class Problem111 : Problem
    {
        private const int length = 10;

        public Problem111() : base(111) { }

        private long Check(Prime p, int d, int[] fixedd)
        {
            long ret = 0;

            foreach (var other in Itertools.PermutationsWithReplacement(Itertools.Range(0, 9).Where(it => it != d),
                length - fixedd.Length))
            {
                long tmp = 0;
                int idf = 0, ido = 0;

                // leading 0
                if (fixedd[0] != 0 && other[0] == 0)
                    continue;

                for (int i = 0; i < length; i++)
                {
                    if (idf < fixedd.Length && fixedd[idf] == i)
                    {
                        tmp = tmp * 10 + d;
                        idf++;
                    }
                    else
                    {
                        tmp = tmp * 10 + other[ido];
                        ido++;
                    }
                }

                if (p.IsPrime(tmp))
                    ret += tmp;
            }

            return ret;
        }

        private long Count(Prime p, int d)
        {
            for (int l = length - 1; l > 0; l--)
            {
                long counter = 0;

                foreach (var pos in Itertools.Combinations(Itertools.Range(0, length - 1), l))
                {
                    // leading zero
                    if (d == 0 && pos[0] == 0)
                        continue;
                    counter += Check(p, d, pos);
                }

                if (counter != 0)
                    return counter;
            }

            return 0;
        }

        protected override string Action()
        {
            var p = new Prime((int)Misc.Sqrt(BigInteger.Pow(10, length)) + 1);
            long counter = 0;

            p.GenerateAll();
            for (int i = 0; i < 10; i++)
                counter += Count(p, i);

            return counter.ToString();
        }
    }

    /// <summary>
    /// Working from left-to-right if no digit is exceeded by the digit to its left it
    /// is called an increasing number; for example, 134468.
    ///
    /// Similarly if no digit is exceeded by the digit to its right it is called a
    /// decreasing number; for example, 66420.
    ///
    /// We shall call a positive integer that is neither increasing nor decreasing a
    /// "bouncy" number; for example, 155349.
    ///
    /// Clearly there cannot be any bouncy numbers below one-hundred, but just over
    /// half of the numbers below one-thousand (525) are bouncy. In fact, the least
    /// number for which the proportion of bouncy numbers first reaches 50% is 538.
    ///
    /// Surprisingly, bouncy numbers become more and more common and by the time we
    /// reach 21780 the proportion of bouncy numbers is equal to 90%.
    ///
    /// Find the least number for which the proportion of bouncy numbers is exactly
    /// 99%.
    /// </summary>
    internal class Problem112 : Problem
    {
        public Problem112() : base(112) { }

        protected override string Action()
        {
            int counter = 0;

            for (int i = 100; ; i++)
            {
                if (BouncyNumber.IsBouncyNumber(i.ToString()))
                    counter++;

                if (counter * 100 == i * 99)
                    return i.ToString();
            }
        }
    }

    /// <summary>
    /// Working from left-to-right if no digit is exceeded by the digit to its left it
    /// is called an increasing number; for example, 134468.
    ///
    /// Similarly if no digit is exceeded by the digit to its right it is called a
    /// decreasing number; for example, 66420.
    ///
    /// We shall call a positive integer that is neither increasing nor decreasing a
    /// "bouncy" number; for example, 155349.
    ///
    /// As n increases, the proportion of bouncy numbers below n increases such that
    /// there are only 12951 numbers below one-million that are not bouncy and only
    /// 277032 non-bouncy numbers below 10^10.
    ///
    /// How many numbers below a googol (10^100) are not bouncy?
    /// </summary>
    internal class Problem113 : Problem
    {
        private const int digits = 100;

        public Problem113() : base(113) { }

        protected override string Action()
        {
            var bn = new BouncyNumber(digits);

            return (BigInteger.Pow(10, digits) - bn.CountByDigits(digits) - 1).ToString();
        }
    }

    /// <summary>
    /// A row measuring seven units in length has red blocks with a minimum length of
    /// three units placed on it, such that any two red blocks (which are allowed to be
    /// different lengths) are separated by at least one black square. There are
    /// exactly seventeen ways of doing this.
    ///
    /// How many ways can a row measuring fifty units in length be filled?
    ///
    /// NOTE: Although the example above does not lend itself to the possibility, in
    /// general it is permitted to mix block sizes. For example, on a row measuring
    /// eight units in length you could use red (3), black (1), and red (4).
    /// </summary>
    internal class Problem114 : Problem
    {
        private const int length = 50;

        public Problem114() : base(114) { }

        protected override string Action()
        {
            var sepblock = new SeparateBlock(3);

            sepblock.Generate(length);

            return sepblock[length].ToString();
        }
    }

    /// <summary>
    /// A row measuring n units in length has red blocks with a minimum length of m
    /// units placed on it, such that any two red blocks (which are allowed to be
    /// different lengths) are separated by at least one black square.
    ///
    /// Let the fill-count function, F(m, n), represent the number of ways that a row
    /// can be filled.
    ///
    /// For example, F(3, 29) = 673135 and F(3, 30) = 1089155.
    ///
    /// That is, for m = 3, it can be seen that n = 30 is the smallest value for which
    /// the fill-count function first exceeds one million.
    ///
    /// In the same way, for m = 10, it can be verified that F(10, 56) = 880711 and
    /// F(10, 57) = 1148904, so n = 57 is the least value for which the fill-count
    /// function first exceeds one million.
    ///
    /// For m = 50, find the least value of n for which the fill-count function first
    /// exceeds one million.
    /// </summary>
    internal class Problem115 : Problem
    {
        private const int upper = 1000000;

        public Problem115() : base(115) { }

        protected override string Action()
        {
            var sepblock = new SeparateBlock(50);

            for (int l = 50; l < upper; l++)
            {
                sepblock.Generate(l);
                if (sepblock[l] > upper)
                    return l.ToString();
            }

            return null;
        }
    }

    /// <summary>
    /// A row of five black square tiles is to have a number of its tiles replaced with
    /// coloured oblong tiles chosen from red (length two), green (length three), or
    /// blue (length four).
    ///
    /// If red tiles are chosen there are exactly seven ways this can be done.
    ///
    /// If green tiles are chosen there are three ways.
    ///
    /// And if blue tiles are chosen there are two ways.
    ///
    /// Assuming that colours cannot be mixed there are 7 + 3 + 2 = 12 ways of
    /// replacing the black tiles in a row measuring five units in length.
    ///
    /// How many different ways can the black tiles in a row measuring fifty units in
    /// length be replaced if colours cannot be mixed and at least one coloured tile
    /// must be used?
    /// </summary>
    internal class Problem116 : Problem
    {
        private const int length = 50;

        public Problem116() : base(116) { }

        protected override string Action()
        {
            BigInteger counter = 0;

            for (int i = 2; i < 5; i++)
            {
                var db = new DivideBlock(new int[] { 1, i });

                db.Generate(length);
                counter += db[length] - 1;
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// Using a combination of black square tiles and oblong tiles chosen from: red
    /// tiles measuring two units, green tiles measuring three units, and blue tiles
    /// measuring four units, it is possible to tile a row measuring five units in
    /// length in exactly fifteen different ways.
    ///
    /// How many ways can a row measuring fifty units in length be tiled?
    /// </summary>
    internal class Problem117 : Problem
    {
        private const int length = 50;

        public Problem117() : base(117) { }

        protected override string Action()
        {
            var db = new DivideBlock(new int[] { 1, 2, 3, 4 });

            db.Generate(50);

            return db[50].ToString();
        }
    }

    /// <summary>
    /// Using all of the digits 1 through 9 and concatenating them freely to form
    /// decimal integers, different sets can be formed. Interestingly with the set
    /// {2,5,47,89,631}, all of the elements belonging to it are prime.
    ///
    /// How many distinct sets containing each of the digits one through nine exactly
    /// once contain only prime elements?
    /// </summary>
    internal class Problem118 : Problem
    {
        public Problem118() : base(118) { }

        private HashSet<string> sets;

        private void AddSet(List<int> numbers)
        {
            var tmp = new List<int>(numbers);
            var sb = new StringBuilder();

            tmp.Sort();
            foreach (var n in tmp)
            {
                sb.Append(n);
                sb.Append('|');
            }

            sets.Add(sb.ToString());
        }

        private void Count(List<List<int>> primes, HashSet<int> digits, List<int> numbers, int minl)
        {
            if (digits.Count == 9)
            {
                AddSet(numbers);
                return;
            }

            for (int l = minl; l <= 9 - digits.Count; l++)
            {
                foreach (var p in primes[l])
                {
                    var nd = new HashSet<int>(digits);
                    int tmp = p;

                    while (tmp != 0)
                    {
                        if (digits.Contains(tmp % 10))
                            break;
                        nd.Add(tmp % 10);
                        tmp /= 10;
                    }

                    if (tmp != 0)
                        continue;

                    numbers.Add(p);
                    Count(primes, nd, numbers, l);
                    numbers.RemoveAt(numbers.Count - 1);
                }
            }
        }

        protected override string Action()
        {
            var p = new Prime((int)Misc.Sqrt(987654321) + 1);
            var primes = new List<List<int>>();

            sets = new HashSet<string>();
            p.GenerateAll();

            primes.Add(new List<int>());
            for (int l = 1; l < 9; l++)
            {
                primes.Add(new List<int>());
                foreach (var digits in Itertools.Permutations(Itertools.Range(1, 9), l))
                {
                    var tmp = 0;
                    foreach (var d in digits)
                        tmp = tmp * 10 + d;

                    if (p.IsPrime(tmp))
                        primes[l].Add(tmp);
                }
            }
            // length 9 numbers must be divisable by 3
            primes.Add(new List<int>());

            Count(primes, new HashSet<int>(), new List<int>(), 1);

            return sets.Count.ToString();
        }
    }

    /// <summary>
    /// The number 512 is interesting because it is equal to the sum of its digits
    /// raised to some power: 5 + 1 + 2 = 8, and 8^3 = 512. Another example of a number
    /// with this property is 614656 = 284.
    ///
    /// We shall define an to be the nth term of this sequence and insist that a number
    /// must contain at least two digits to have a sum.
    ///
    /// You are given that a(2) = 512 and a(10) = 614656.
    ///
    /// Find a(30).
    /// </summary>
    internal class Problem119 : Problem
    {
        private const int index = 30;

        public Problem119() : base(119) { }

        private void Find(List<BigInteger> numbers, int l)
        {
            var lower = BigInteger.Pow(10, l - 1);
            var upper = BigInteger.Pow(10, l);

            for (int i = l; i <= l * 9; i++)
            {
                BigInteger tmp = i;

                while (tmp < lower)
                    tmp *= i;

                while (tmp < upper)
                {
                    int sum = 0;

                    foreach (var c in tmp.ToString())
                        sum += c - '0';
                    if (sum == i)
                        numbers.Add(tmp);
                    tmp *= i;
                }
            }
        }

        protected override string Action()
        {
            var numbers = new List<BigInteger>();

            for (int l = 2; numbers.Count < index; l++)
                Find(numbers, l);
            numbers.Sort();

            return numbers[index - 1].ToString();
        }
    }
}