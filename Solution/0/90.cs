using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using ProjectEuler.Common;
using ProjectEuler.Common.Miscellany;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// Each of the six faces on a cube has a different digit (0 to 9) written on it;
    /// the same is done to a second cube. By placing the two cubes side-by-side in
    /// different positions we can form a variety of 2-digit numbers.
    ///
    /// For example, the square number 64 could be formed:
    ///
    /// {6}{4}
    ///
    /// In fact, by carefully choosing the digits on both cubes it is possible to
    /// display all of the square numbers below one-hundred: 01, 04, 09, 16, 25, 36,
    /// 49, 64, and 81.
    ///
    /// For example, one way this can be achieved is by placing {0, 5, 6, 7, 8, 9} on
    /// one cube and {1, 2, 3, 4, 8, 9} on the other cube.
    ///
    /// However, for this problem we shall allow the 6 or 9 to be turned upside-down so
    /// that an arrangement like {0, 5, 6, 7, 8, 9} and {1, 2, 3, 4, 6, 7} allows for
    /// all nine square numbers to be displayed; otherwise it would be impossible to
    /// obtain 09.
    ///
    /// In determining a distinct arrangement we are interested in the digits on each
    /// cube, not the order.
    ///
    /// {1, 2, 3, 4, 5, 6} is equivalent to {3, 6, 4, 1, 2, 5}
    /// {1, 2, 3, 4, 5, 6} is distinct from {1, 2, 3, 4, 5, 9}
    ///
    /// But because we are allowing 6 and 9 to be reversed, the two distinct sets in
    /// the last example both represent the extended set {1, 2, 3, 4, 5, 6, 9} for the
    /// purpose of forming 2-digit numbers.
    ///
    /// How many distinct arrangements of the two cubes allow for all of the square
    /// numbers to be displayed?
    /// </summary>
    internal class Problem90 : Problem
    {
        public Problem90() : base(90) { }

        private bool Check(int[] cube1, int[] cube2, int number)
        {
            int n1 = number / 10, n2 = number % 10;

            if (!(cube1.Contains(n1) || n1 == 6 && cube1.Contains(9) || n1 == 9 && cube1.Contains(6)))
                return false;
            if (!(cube2.Contains(n2) || n2 == 6 && cube2.Contains(9) || n2 == 9 && cube2.Contains(6)))
                return false;

            return true;
        }

        protected override string Action()
        {
            var squares = Itertools.Range(1, 9).Select(it => it * it).ToList();
            var cubes = new List<int[]>();
            var counter = 0;

            foreach (var cube in Itertools.Combinations(Itertools.Range(0, 9), 6))
                cubes.Add(cube.Clone() as int[]);

            foreach (var pair in Itertools.Combinations(cubes, 2))
            {
                var flags = true;

                foreach (var n in squares)
                {
                    if (!(Check(pair[0], pair[1], n) || Check(pair[1], pair[0], n)))
                    {
                        flags = false;
                        break;
                    }
                }
                if (flags)
                    counter++;
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// The points P (x1, y1) and Q (x2, y2) are plotted at integer co-ordinates and
    /// are joined to the origin, O(0,0), to form ΔOPQ.
    ///
    /// There are exactly fourteen triangles containing a right angle that can be
    /// formed when each co-ordinate lies between 0 and 2 inclusive; that is,
    /// 0 <= x1, y1, x2, y2 <= 2.
    ///           P     Q
    /// P     Q             p
    /// OQ   OP   OQ   OP   O Q
    ///
    ///           P      Q
    ///  P     Q            PQ
    /// O Q  O P  O Q  O P  O
    ///
    ///      P    PQ   P Q
    /// P Q   Q
    /// O    O    O    O
    ///
    /// Given that 0 <= x1, y1, x2, y2 <= 50, how many right triangles can be formed?
    /// </summary>
    internal class Problem91 : Problem
    {
        private const int upper = 50;

        public Problem91() : base(91) { }

        private bool Check(int x1, int y1, int x2, int y2)
        {
            var lens = new List<int>();

            lens.Add(x1 * x1 + y1 * y1);
            lens.Add(x2 * x2 + y2 * y2);
            lens.Add((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
            lens.Sort();

            return lens[0] + lens[1] == lens[2];
        }

        protected override string Action()
        {
            var counter = 0;
            var size = upper + 1;

            foreach (var p in Itertools.Combinations(Itertools.Range(1, size * size - 1), 2))
            {
                if (Check(p[0] / size, p[0] % size, p[1] / size, p[1] % size))
                    counter++;
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// A number chain is created by continuously adding the square of the digits in a
    /// number to form a new number until it has been seen before.
    ///
    /// For example,
    ///
    /// 44 => 32 => 13 => 10 => 1 => 1
    /// 85 => 89 => 145 => 42 => 20 => 4 => 16 => 37 => 58 => 89
    ///
    /// Therefore any chain that arrives at 1 or 89 will become stuck in an endless
    /// loop. What is most amazing is that EVERY starting number will eventually arrive
    /// at 1 or 89.
    ///
    /// How many starting numbers below ten million will arrive at 89?
    /// </summary>
    internal class Problem92 : Problem
    {
        private const int upper = 10000000;

        public Problem92() : base(92) { }

        private int GetNext(int[] squares, int number)
        {
            var ret = 0;

            while (number != 0)
            {
                ret += squares[number % 10];
                number /= 10;
            }

            return ret;
        }

        protected override string Action()
        {
            var squares = (from i in Itertools.Range(0, 9) select i * i).ToArray();
            var e1 = new HashSet<int>();
            var e89 = new HashSet<int>();
            var counter = 0;

            e1.Add(1);
            e89.Add(89);

            // 9*9=81, assume after first step the number will be less than 1000
            for (int i = 1; i < 1000; i++)
            {
                if (e1.Contains(i) || e89.Contains(i))
                    continue;
                var list = new List<int>();
                var tmp = i;

                while (true)
                {
                    list.Add(tmp);
                    tmp = GetNext(squares, tmp);

                    if (e1.Contains(tmp))
                    {
                        foreach (var n in list)
                            e1.Add(n);
                        break;
                    }
                    if (e89.Contains(tmp))
                    {
                        foreach (var n in list)
                            e89.Add(n);
                        break;
                    }
                }
            }
            for (int i = 1000; i < upper; i++)
            {
                if (e89.Contains(GetNext(squares, i)))
                    counter++;
            }

            return (e89.Count + counter).ToString();
        }
    }

    /// <summary>
    /// By using each of the digits from the set, {1, 2, 3, 4}, exactly once, and
    /// making use of the four arithmetic operations (+, -, *, /)
    /// and brackets/parentheses, it is possible to form different positive integer
    /// targets.
    ///
    /// For example,
    ///
    /// 8 = (4 * (1 + 3)) / 2
    /// 14 = 4 * (3 + 1 / 2)
    /// 19 = 4 * (2 + 3) - 1
    /// 36 = 3 * 4 * (2 + 1)
    ///
    /// Note that concatenations of the digits, like 12 + 34, are not allowed.
    ///
    /// Using the set, {1, 2, 3, 4}, it is possible to obtain thirty-one different
    /// target numbers of which 36 is the maximum, and each of the numbers 1 to 28 can
    /// be obtained before encountering the first non-expressible number.
    ///
    /// Find the set of four distinct digits, a < b < c < d, for which the longest set
    /// of consecutive positive integers, 1 to n, can be obtained, giving your answer
    /// as a string: abcd.
    /// </summary>
    internal class Problem93 : Problem
    {
        public Problem93() : base(93) { }

        private Fraction Calculate(Fraction n1, Fraction n2, int op)
        {
            switch (op)
            {
                case 0:
                    return n1 + n2;
                case 1:
                    return n1 - n2;
                case 2:
                    return n1 * n2;
                case 3:
                    return n1 / n2;
                default:
                    throw new Exception();
            }
        }

        private int Count(IEnumerable<int> digits)
        {
            var results = new HashSet<int>();

            // Brute-force: Possibility of number sequence
            foreach (var nums in Itertools.Permutations(digits, 4))
            {
                // Possibility of operator sequence
                foreach (var operators in Itertools.PermutationsWithReplacement(Itertools.Range(0, 3), 3))
                {
                    // Possibility of brackets
                    for (int seq = 0; seq < 5; seq++)
                    {
                        Fraction result;

                        switch (seq)
                        {
                            case 0: // 1 2 3
                                result = Calculate(Calculate(Calculate(nums[0], nums[1], operators[0]), nums[2], operators[1]),
                                    nums[3], operators[2]);
                                break;
                            case 1: // 1 3 2 and 2 3 1
                                result = Calculate(Calculate(nums[0], nums[1], operators[0]),
                                    Calculate(nums[2], nums[3], operators[2]), operators[1]);
                                break;
                            case 2: // 2 1 3
                                result = Calculate(Calculate(nums[0], Calculate(nums[1], nums[2], operators[1]), operators[0]),
                                    nums[3], operators[2]);
                                break;
                            case 3: // 3 1 2
                                result = Calculate(nums[0], Calculate(Calculate(nums[1], nums[2], operators[1]), nums[3], operators[2]),
                                    operators[0]);
                                break;
                            case 4: // 3 2 1
                                result = Calculate(nums[0], Calculate(nums[1], Calculate(nums[2], nums[3], operators[2]), operators[1]),
                                    operators[0]);
                                break;
                            default:
                                throw new Exception();
                        }

                        if (result.Denominator != 1 || result.Numerator <= 0)
                            continue;
                        results.Add((int)result.Numerator);
                    }
                }
            }

            for (int i = 0; i < results.Count; i++)
                if (!results.Contains(i + 1))
                    return i;

            return results.Count;
        }

        protected override string Action()
        {
            int maxlength = 0;
            string ret = null;

            foreach (var digits in Itertools.Combinations(Itertools.Range(1, 9), 4))
            {
                var tmp = Count(digits);

                if (tmp > maxlength)
                {
                    maxlength = tmp;
                    ret = string.Format("{0}{1}{2}{3}", digits[0], digits[1], digits[2], digits[3]);
                }
            }

            return ret;
        }
    }

    /// <summary>
    /// It is easily proved that no equilateral triangle exists with integral length
    /// sides and integral area. However, the almost equilateral triangle 5-5-6 has an
    /// area of 12 square units.
    ///
    /// We shall define an almost equilateral triangle to be a triangle for which two
    /// sides are equal and the third differs by no more than one unit.
    ///
    /// Find the sum of the perimeters of all almost equilateral triangles with
    /// integral side lengths and area and whose perimeters do not exceed one billion
    /// (1,000,000,000).
    /// </summary>
    internal class Problem94 : Problem
    {
        private const int upper = 1000000000;

        public Problem94() : base(94) { }

        protected override string Action()
        {
            BigInteger counter = 0;

            /**
             * Heron's formula:
             * S = sqrt(s*(s-a)*(s-b)*(s-c)) where s=(a+b+c)/2
             *
             * if i is even:
             * for i,i,i
             *   s = 3i/2
             *   S = sqrt(3i*i*i*i/16) impossible
             *
             * if i is odd:
             * for i,i,i-1:
             *   s = (3i-1)/2
             *   S = sqrt((3i-1)*(i-1)*(i-1)*(i+1)/16) => (3i-1)(i+1) must be square number
             * for i,i,i+1
             *   s = (3i+1)/2
             *   S = sqrt((3i+1)*(i+1)*(i+1)*(i-1)/16) => (3i+1)(i-1) must be square number
             */
            for (long i = 3; i <= upper / 3; i += 2)
            {
                long n1 = (3 * i - 1) * (i + 1);
                long n2 = (3 * i + 1) * (i - 1);

                if (Misc.IsPerfectSquare(n1))
                    counter += 3 * i - 1;
                if (Misc.IsPerfectSquare(n2))
                    counter += 3 * i + 1;
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// The proper divisors of a number are all the divisors excluding the number
    /// itself. For example, the proper divisors of 28 are 1, 2, 4, 7, and 14. As the
    /// sum of these divisors is equal to 28, we call it a perfect number.
    ///
    /// Interestingly the sum of the proper divisors of 220 is 284 and the sum of the
    /// proper divisors of 284 is 220, forming a chain of two numbers. For this reason,
    /// 220 and 284 are called an amicable pair.
    ///
    /// Perhaps less well known are longer chains. For example, starting with 12496, we
    /// form a chain of five numbers:
    ///
    /// 12496 - 14288 - 15472 - 14536 - 14264 (- 12496 - ...)
    ///
    /// Since this chain returns to its starting point, it is called an amicable chain.
    ///
    /// Find the smallest member of the longest amicable chain with no element
    /// exceeding one million.
    /// </summary>
    internal class Problem95 : Problem
    {
        private int upper = 1000000;

        public Problem95() : base(95) { }

        private void GetAmicableChain(Prime p, HashSet<int> existNums, int number, ref int maxlength, ref int startNumber)
        {
            var list = new List<int>();
            var set = new HashSet<int>();
            int tmp = number;

            list.Add(number);
            set.Add(number);

            while (true)
            {
                tmp = Factor.GetFactorSum(p, tmp);

                if (set.Contains(tmp))
                {
                    int id = list.IndexOf(tmp);

                    if (list.Count - id > maxlength)
                    {
                        maxlength = list.Count - id;
                        startNumber = list.Skip(id).Min();
                    }
                    break;
                }
                if (existNums.Contains(tmp) || tmp > upper)
                {
                    break;
                }
                list.Add(tmp);
                set.Add(tmp);
            }

            foreach (var n in list)
                existNums.Add(n);
        }

        protected override string Action()
        {
            var nums = new HashSet<int>();
            var p = new Prime(upper);
            int maxlength = 0, number = 0;

            nums.Add(1);
            p.GenerateAll();
            for (int i = 2; i <= upper; i++)
            {
                if (nums.Contains(i))
                    continue;
                GetAmicableChain(p, nums, i, ref maxlength, ref number);
            }

            return number.ToString();
        }
    }

    /// <summary>
    /// Su Doku (Japanese meaning number place) is the name given to a popular puzzle
    /// concept. Its origin is unclear, but credit must be attributed to Leonhard Euler
    /// who invented a similar, and much more difficult, puzzle idea called Latin
    /// Squares. The objective of Su Doku puzzles, however, is to replace the blanks
    /// (or zeros) in a 9 by 9 grid in such that each row, column, and 3 by 3 box
    /// contains each of the digits 1 to 9. Below is an example of a typical starting
    /// puzzle grid and its solution grid.
    ///
    /// 0 0 3 0 2 0 6 0 0     4 8 3 9 2 1 6 5 7
    /// 9 0 0 3 0 5 0 0 1     9 6 7 3 4 5 8 2 1
    /// 0 0 1 8 0 6 4 0 0     2 5 1 8 7 6 4 9 3
    /// 0 0 8 1 0 2 9 0 0     5 4 8 1 3 2 9 7 6
    /// 7 0 0 0 0 0 0 0 8     7 2 9 5 6 4 1 3 8
    /// 0 0 6 7 0 8 2 0 0     1 3 6 7 9 8 2 4 5
    /// 0 0 2 6 0 9 5 0 0     3 7 2 6 8 9 5 1 4
    /// 8 0 0 2 0 3 0 0 9     8 1 4 2 5 3 7 6 9
    /// 0 0 5 0 1 0 3 0 0     6 9 5 4 1 7 3 8 2
    ///
    /// A well constructed Su Doku puzzle has a unique solution and can be solved by
    /// logic, although it may be necessary to employ "guess and test" methods in order
    /// to eliminate options (there is much contested opinion over this). The
    /// complexity of the search determines the difficulty of the puzzle; the example
    /// above is considered easy because it can be solved by straight forward direct
    /// deduction.
    ///
    /// [file D0096.txt], contains fifty different Su Doku puzzles ranging in
    /// difficulty, but all with unique solutions (the first puzzle in the file is the
    /// example above).
    ///
    /// By solving all fifty puzzles find the sum of the 3-digit numbers found in the
    /// top left corner of each solution grid; for example, 483 is the 3-digit number
    /// found in the top left corner of the solution grid above.
    /// </summary>
    internal class Problem96 : Problem
    {
        public Problem96() : base(96) { }

        private List<int[][]> puzzles;

        protected override void PreAction(string data)
        {
            var lines = data.Split('\n');

            puzzles = new List<int[][]>();
            for (int i = 1; i < lines.Length; i += 10)
            {
                puzzles.Add((from line in lines.Skip(i).Take(9)
                             select (
                                 from c in line.Trim() select c - '0').ToArray()).ToArray());
            }
        }

        protected override string Action()
        {
            var counter = 0;

            foreach (var puzzle in puzzles)
            {
                SudokuSolver.Solve(puzzle);
                counter += puzzle[0][0] * 100 + puzzle[0][1] * 10 + puzzle[0][2];
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// The first known prime found to exceed one million digits was discovered in
    /// 1999, and is a Mersenne prime of the form 2^6972593 - 1; it contains exactly
    /// 2,098,960 digits. Subsequently other Mersenne primes, of the form 2^p - 1, have
    /// been found which contain more digits.
    ///
    /// However, in 2004 there was found a massive non-Mersenne prime which contains
    /// 2,357,207 digits: 28433 * 2^7830457 + 1.
    ///
    /// Find the last ten digits of this prime number.
    /// </summary>
    internal class Problem97 : Problem
    {
        private const long upper = 10000000000;

        public Problem97() : base(97) { }

        protected override string Action()
        {
            long number = 28433;

            for (int i = 0; i < 7830457; i++)
            {
                number *= 2;
                number %= upper;
            }
            number += 1;

            return number.ToString();
        }
    }

    /// <summary>
    /// By replacing each of the letters in the word CARE with 1, 2, 9, and 6
    /// respectively, we form a square number: 1296 = 36^2. What is remarkable is that,
    /// by using the same digital substitutions, the anagram, RACE, also forms a square
    /// number: 9216 = 96^2. We shall call CARE (and RACE) a square anagram word pair
    /// and specify further that leading zeroes are not permitted, neither may a
    /// different letter have the same digital value as another letter.
    ///
    /// [file D0098.txt], a 16K text file containing nearly two-thousand common English
    /// words, find all the square anagram word pairs (a palindromic word is NOT
    /// considered to be an anagram of itself).
    ///
    /// What is the largest square number formed by any member of such a pair?
    ///
    /// NOTE: All anagrams formed must be contained in the given text file.
    /// </summary>
    internal class Problem98 : Problem
    {
        public Problem98() : base(98) { }

        private List<string> words;

        protected override void PreAction(string data)
        {
            words = (from w in data.Split(',')
                     select w.Substring(1, w.Length - 2)).ToList();
        }

        private string GetIDX(string word)
        {
            var counter = new int[26];

            foreach (var c in word)
                counter[c - 'A']++;

            return string.Join("", counter.Select(it => it.ToString()));
        }

        private int GetMaxSquare(string id, List<string> anagram)
        {
            var map = new Dictionary<char, int>();
            int cid = 0;
            int max = 0;

            foreach (var c in anagram[0])
            {
                if (!map.ContainsKey(c))
                {
                    map.Add(c, cid++);
                }
            }

            foreach (var nums in Itertools.Permutations(Itertools.Range(0, 9), map.Count))
            {
                bool next = false;
                int tmpmax = 0;

                foreach (var word in anagram)
                {
                    // First letter must not be 0
                    if (nums[map[word[0]]] == 0)
                    {
                        next = true;
                        break;
                    }

                    int tmp = 0;
                    foreach (var c in word)
                    {
                        tmp *= 10;
                        tmp += nums[map[c]];
                    }

                    if (!Misc.IsPerfectSquare(tmp))
                    {
                        next = true;
                        break;
                    }
                    if (tmp > tmpmax)
                        tmpmax = tmp;
                }
                if (next)
                    continue;

                if (tmpmax > max)
                    max = tmpmax;
            }

            return max;
        }

        protected override string Action()
        {
            var anagrams = new Dictionary<string, List<string>>();
            var list = new List<KeyValuePair<string, List<string>>>();
            int maxlength = 0, max = 0;

            foreach (var word in words)
            {
                var id = GetIDX(word);

                if (!anagrams.ContainsKey(id))
                    anagrams[id] = new List<string>();

                anagrams[id].Add(word);
            }
            foreach (var p in anagrams)
            {
                if (p.Value.Count > 1)
                    list.Add(p);
            }
            list.Sort((x, y) => { return y.Value[0].Length.CompareTo(x.Value[0].Length); });

            foreach (var p in list)
            {
                if (p.Value[0].Length < maxlength)
                    break;

                var tmp = GetMaxSquare(p.Key, p.Value);

                if (tmp > max)
                {
                    max = tmp;
                    maxlength = p.Value[0].Length;
                }
            }

            return max.ToString();
        }
    }

    /// <summary>
    /// Comparing two numbers written in index form like 2^11 and 3^7 is not difficult,
    /// as any calculator would confirm that 2^11 = 2048 < 3^7 = 2187.
    ///
    /// However, confirming that 632382^518061 > 519432^525806 would be much more
    /// difficult, as both numbers contain over three million digits.
    ///
    /// Using [file D0099.txt], a 22K text file containing one thousand lines with a
    /// base/exponent pair on each line, determine which line number has the greatest
    /// numerical value.
    /// </summary>
    internal class Problem99 : Problem
    {
        public Problem99() : base(99) { }

        private List<int[]> values;

        protected override void PreAction(string data)
        {
            values = (from line in data.Split('\n')
                      select (
                          from num in line.Trim().Split(',') select int.Parse(num)).ToArray()).ToList();
        }

        protected override string Action()
        {
            int ret = 0;
            double max = 0;

            // Compare log(a^b) = b*log(a)
            for (int i = 0; i < values.Count; i++)
            {
                double tmp = Math.Log(values[i][0]) * values[i][1];

                if (tmp > max)
                {
                    max = tmp;
                    ret = i;
                }
            }

            return (ret + 1).ToString();
        }
    }
}