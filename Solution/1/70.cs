using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using ProjectEuler.Common;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// Take the number 6 and multiply it by each of 1273 and 9854:
    ///
    /// 6 * 1273 = 7638
    /// 6 * 9854 = 59124
    ///
    /// By concatenating these products we get the 1 to 9 pandigital 763859124. We will
    /// call 763859124 the "concatenated product of 6 and (1273,9854)". Notice too,
    /// that the concatenation of the input numbers, 612739854, is also 1 to 9
    /// pandigital.
    ///
    /// The same can be done for 0 to 9 pandigital numbers.
    ///
    /// What is the largest 0 to 9 pandigital 10-digit concatenated product of an
    /// integer with two or more other integers, such that the concatenation of the
    /// input numbers is also a 0 to 9 pandigital 10-digit number?
    /// </summary>
    internal class Problem170 : Problem
    {
        public Problem170() : base(170) { }

        private bool Check(long value, int f)
        {
            var flags = new int[10];
            var nums = new List<string>();
            var tmp = value.ToString();
            var tmpf = f;

            while (value > 0)
            {
                flags[value % 10]++;
                value /= 10;
            }
            tmpf = f;
            while (tmpf > 0)
            {
                flags[tmpf % 10]++;
                tmpf /= 10;
            }

            if (flags[0] == 0)
                return false;
            for (int i = 1; i < 10; i++)
            {
                if (flags[i] != 1)
                    return false;
            }

            // check whether value/f can be divided into two or more numbers
            int idx = 0, next = 0;

            while (idx < tmp.Length)
            {
                while (next < tmp.Length && tmp[next] == '0')
                    next++;
                next = tmp.IndexOf('0', next);
                if (next == -1)
                    break;
                nums.Add(tmp.Substring(idx, next - idx));
                idx = next;
            }
            nums.Add(tmp.Substring(idx));
            for (int i = 1; i < nums.Count; i++)
            {
                var num = int.Parse(nums[i]);

                if (num == 0)
                    continue;
                if ((num * f).ToString().Length <= nums[i].Length)
                    return true;
            }

            return false;
        }

        protected override string Action()
        {
            /**
             * f must be a multiple of 3 and less than 49
             * in number f and number/f, 1-9 must occur only once and 0 at least once
             */
            foreach (var digits in Itertools.Permutations(Itertools.Range(9, 0), 10))
            {
                long value = 0;

                foreach (var d in digits)
                    value = value * 10 + d;
                for (int f = 3; f < 49; f += 3)
                {
                    if (value % f != 0)
                        continue;
                    var tmp = value / f;

                    if (Check(tmp, f))
                        return value.ToString();
                }
            }

            return null;
        }
    }

    /// <summary>
    /// For a positive integer n, let f(n) be the sum of the squares of the digits (in
    /// base 10) of n, e.g.
    ///
    /// f(3) = 3^2 = 9,
    /// f(25) = 2^2 + 5^2 = 4 + 25 = 29,
    /// f(442) = 4^2 + 4^2 + 2^2 = 16 + 16 + 4 = 36
    ///
    /// Find the last nine digits of the sum of all n, 0 < n < 10^20, such that f(n) is
    /// a perfect square.
    /// </summary>
    internal class Problem171 : Problem
    {
        private const int length = 20;
        private long factorial;

        public Problem171() : base(171) { }

        private long CalculateFactor(int[] counter)
        {
            long ret = 0, f = factorial;

            // calculate combinations of counter
            foreach (var c in counter)
            {
                for (int i = 2; i <= c; i++)
                    f /= i;
            }
            // factor is 1111.....1, so divide factor by length
            for (int i = 1; i < 10; i++)
                ret += i * f * counter[i] / length;

            return ret;
        }

        private long Calculate(List<int> pow, int[] counter, int d, int l, int value)
        {
            long ret = 0;

            if (pow[d] * l < value)
                return 0;
            if (d == 1)
            {
                counter[1] = value;
                counter[0] = l - value;

                return CalculateFactor(counter);
            }

            for (int i = 0; i <= value / pow[d]; i++)
            {
                counter[d] = i;
                ret += Calculate(pow, counter, d - 1, l - i, value - pow[d] * i);
                ret %= 1000000000;
            }

            return ret;
        }

        protected override string Action()
        {
            var pow = new List<int>();
            long ret = 0;

            factorial = 1;
            for (int i = 1; i <= length; i++)
                factorial *= i;

            for (int i = 0; ; i++)
            {
                pow.Add(i * i);
                if (i * i > 9 * 9 * length)
                    break;
            }
            foreach (var p in pow.Skip(1))
            {
                ret += Calculate(pow, new int[10], 9, 20, p);
                ret %= 1000000000;
            }

            return (ret * 111111111 % 1000000000).ToString();
        }
    }

    /// <summary>
    /// How many 18-digit numbers n (without leading zeros) are there such that no
    /// digit occurs more than three times in n?
    /// </summary>
    internal class Problem172 : Problem
    {
        private const int length = 18;
        private long factorial;

        public Problem172() : base(172) { }

        private long CountPermutation(int[] partitions)
        {
            var ret = factorial;

            for (int i = 0; i < partitions[2]; i++)
                ret /= 2;
            for (int i = 0; i < partitions[3]; i++)
                ret /= 6;

            return ret;
        }

        private long Count(int[] partitions)
        {
            var total = partitions.Sum();
            long ret = 0;

            // no zero exists
            if (total != 10)
            {
                var n = total;
                long factor = 1;

                // select digits from 9 digits
                factor = Probability.CountCombinations(9, n);
                // select digits appears only once
                factor *= Probability.CountCombinations(n, partitions[1]);
                n -= partitions[1];
                // select digits appears twice
                factor *= Probability.CountCombinations(n, partitions[2]);
                // other digits appears three times

                ret += factor * CountPermutation(partitions);
            }
            // zero exists once
            if (partitions[1] != 0)
            {
                var n = total;
                long factor = 1;

                factor = Probability.CountCombinations(9, n - 1);
                factor *= Probability.CountCombinations(n - 1, partitions[1] - 1);
                n -= partitions[1];
                factor *= Probability.CountCombinations(n, partitions[2]);

                ret += factor * CountPermutation(partitions) * (length - 1) / length;
            }
            // zero exist twice
            if (partitions[2] != 0)
            {
                var n = total;
                long factor = 1;

                factor = Probability.CountCombinations(9, n - 1);
                factor *= Probability.CountCombinations(n - 1, partitions[1]);
                n -= partitions[1];
                factor *= Probability.CountCombinations(n - 1, partitions[2] - 1);

                ret += factor * CountPermutation(partitions) * (length - 2) / length;
            }
            // zero exist three times
            if (partitions[3] != 0)
            {
                var n = total;
                long factor = 1;

                factor = Probability.CountCombinations(9, n - 1);
                factor *= Probability.CountCombinations(n - 1, partitions[1]);
                n -= partitions[1];
                factor *= Probability.CountCombinations(n - 1, partitions[2]);

                ret += factor * CountPermutation(partitions) * (length - 3) / length;
            }

            return ret;
        }

        protected override string Action()
        {
            var p = new int[4];
            long ret = 0;

            factorial = 1;
            for (int i = 1; i <= length; i++)
                factorial *= i;

            for (p[3] = 0; p[3] <= length / 3; p[3]++)
            {
                for (p[2] = 0; p[2] <= (length - p[3] * 3) / 2; p[2]++)
                {
                    p[1] = length - p[3] * 3 - p[2] * 2;

                    if (p.Sum() > 10)
                        continue;
                    ret += Count(p);
                }
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// We shall define a square lamina to be a square outline with a square "hole" so
    /// that the shape possesses vertical and horizontal symmetry. For example, using
    /// exactly thirty-two square tiles we can form two different square laminae:
    ///
    /// With one-hundred tiles, and not necessarily using all of the tiles at one time,
    /// it is possible to form forty-one different square laminae.
    ///
    /// Using up to one million tiles how many different square laminae can be formed?
    /// </summary>
    internal class Problem173 : Problem
    {
        private const int tiles = 1000000;

        public Problem173() : base(173) { }

        private int FindMinInner(List<long> pow, long value, int lower, int upper)
        {
            int mid = (lower + upper) / 2;

            while (lower <= upper)
            {
                mid = (lower + upper) / 2;

                if (value - pow[mid] == tiles)
                    return mid;
                if (value - pow[mid] > tiles)
                    lower = mid + 1;
                else
                    upper = mid - 1;
            }

            if (value - pow[mid] > tiles && value - pow[mid + 1] <= tiles)
                return mid + 1;
            if (value - pow[mid] <= tiles && (mid < 1 || value - pow[mid - 1] > tiles))
                return mid;

            throw new Exception();
        }

        protected override string Action()
        {
            var powe = new List<long>();
            var powo = new List<long>();
            var counter = 0;

            powe.Add(4);
            powo.Add(1);
            for (long i = 4; ; i += 2)
            {
                long tmp = i * i;

                if (tmp - powe[powe.Count - 1] > tiles)
                    break;
                powe.Add(tmp);
            }
            for (long i = 3; ; i += 2)
            {
                long tmp = i * i;

                if (tmp - powo[powo.Count - 1] > tiles)
                    break;
                powo.Add(tmp);
            }

            for (int outer = 1; outer < powe.Count; outer++)
                counter += outer - FindMinInner(powe, powe[outer], 0, outer);
            for (int outer = 1; outer < powo.Count; outer++)
                counter += outer - FindMinInner(powo, powo[outer], 0, outer);

            return counter.ToString();
        }
    }

    /// <summary>
    /// We shall define a square lamina to be a square outline with a square "hole" so
    /// that the shape possesses vertical and horizontal symmetry.
    ///
    /// Given eight tiles it is possible to form a lamina in only one way: 3x3 square
    /// with a 1x1 hole in the middle. However, using thirty-two tiles it is possible
    /// to form two distinct laminae.
    ///
    /// If t represents the number of tiles used, we shall say that t = 8 is type L(1)
    /// and t = 32 is type L(2).
    ///
    /// Let N(n) be the number of t <= 1000000 such that t is type L(n); for example,
    /// N(15) = 832.
    ///
    /// What is Σ(N(n)) for 1 <= n <= 10?
    /// </summary>
    internal class Problem174 : Problem
    {
        private const int tiles = 1000000;

        public Problem174() : base(174) { }

        protected override string Action()
        {
            var powe = new List<long>();
            var powo = new List<long>();
            var counter = new int[tiles + 1];

            powe.Add(4);
            powo.Add(1);
            for (long i = 4; ; i += 2)
            {
                long tmp = i * i;

                if (tmp - powe[powe.Count - 1] > tiles)
                    break;
                powe.Add(tmp);
            }
            for (long i = 3; ; i += 2)
            {
                long tmp = i * i;

                if (tmp - powo[powo.Count - 1] > tiles)
                    break;
                powo.Add(tmp);
            }

            for (int outer = 1; outer < powe.Count; outer++)
            {
                for (int inner = outer - 1; inner >= 0; inner--)
                {
                    var tmp = powe[outer] - powe[inner];

                    if (tmp > tiles)
                        break;
                    counter[tmp]++;
                }
            }
            for (int outer = 1; outer < powo.Count; outer++)
            {
                for (int inner = outer - 1; inner >= 0; inner--)
                {
                    var tmp = powo[outer] - powo[inner];

                    if (tmp > tiles)
                        break;
                    counter[tmp]++;
                }
            }

            return counter.Where(it => it >= 1 && it <= 10).Count().ToString();
        }
    }

    /// <summary>
    /// Define f(0)=1 and f(n) to be the number of ways to write n as a sum of powers
    /// of 2 where no power occurs more than twice.
    ///
    /// For example, f(10)=5 since there are five different ways to express 10:
    /// 10 = 8+2 = 8+1+1 = 4+4+2 = 4+2+2+1+1 = 4+4+1+1
    ///
    /// It can be shown that for every fraction p/q (p > 0, q > 0) there exists at
    /// least one integer n such that
    /// f(n)/f(n-1)=p/q.
    ///
    /// For instance, the smallest n for which f(n)/f(n-1)=13/17 is 241.
    /// The binary expansion of 241 is 11110001.
    /// Reading this binary number from the most significant bit to the least
    /// significant bit there are 4 one's, 3 zeroes and 1 one. We shall call the string
    /// 4,3,1 the Shortened Binary Expansion of 241.
    ///
    /// Find the Shortened Binary Expansion of the smallest n for which
    /// f(n)/f(n-1)=123456789/987654321.
    ///
    /// Give your answer as comma separated integers, without any whitespaces.
    /// </summary>
    internal class Problem175 : Problem
    {
        private const int numerator = 123456789;
        private const int denominator = 987654321;

        public Problem175() : base(175) { }

        private string MyArcf(int x, int y)
        {
            /**
             * http://2000clicks.com/mathhelp/BasicRecurrenceRelationsSumsOfPowersOfTwo.aspx
             * Lemma 3
             */
            var list = new List<int>();
            var idx = 0;
            int prev;

            if (x >= y)
                prev = 0;
            else
                prev = 1;
            list.Add(0);

            while (x != 0)
            {
                if (x >= y)
                {
                    var k = (x - y) / y;

                    if (k == 0)
                        k = 1;
                    if (prev == 1)
                    {
                        list.Add(0);
                        prev = 0;
                        idx++;
                    }
                    x = x - k * y;
                    list[idx] += k;
                }
                else
                {
                    var k = (y - x) / x;

                    if (k == 0)
                        k = 1;
                    if (prev == 0)
                    {
                        list.Add(0);
                        prev = 1;
                        idx++;
                    }
                    y = y - k * x;
                    list[idx] += k;
                }
            }
            list.Reverse();

            return string.Join(",", list);
        }

        protected override string Action()
        {
            var f = new SmallFraction(numerator, denominator);
            int x, y;

            /**
             * http://2000clicks.com/mathhelp/BasicRecurrenceRelationsSumsOfPowersOfTwo.aspx
             *
             * when n is odd, assume n=2k+1
             * f(2k+1) = f(k)
             * f(2k) = f(k)+f(k-1)
             * when n is even, assume n=2k
             * f(2k) = f(k)+f(k-1)
             * f(2k-1) = f(k-1), numerator is bigger than denominator
             *
             * Lemma: f(n) and f(n-1) are coprime
             */
            x = (int)f.Denominator;
            y = (int)f.Numerator;

            return MyArcf(x, y).ToString();
        }
    }

    /// <summary>
    /// The four rectangular triangles with sides (9,12,15), (12,16,20), (5,12,13) and
    /// (12,35,37) all have one of the shorter sides (catheti) equal to 12. It can be
    /// shown that no other integer sided rectangular triangle exists with one of the
    /// catheti equal to 12.
    ///
    /// Find the smallest integer that can be the length of a cathetus of exactly 47547
    /// different integer sided rectangular triangles.
    /// </summary>
    internal class Problem176 : Problem
    {
        private const int kinds = 47547;

        public Problem176() : base(176) { }

        protected override string Action()
        {
            var factors = new List<int>();
            var prime = new Prime(10000);
            var list = new List<int>();
            BigInteger min;

            /**
             * catheti^2 = x^2 - y^2 = (x+y)(x-y), assume x+y=A, x-y=B, combinations of AB is 47547
             * x=(A+B)/2, y=(A-B)/2, so AB must be both even or both odd, A>B.
             * so the number of factors of catheti^2 which is smaller than catheti (B) is 47547
             * number of factors of catheti^2 is 47547
             */
            prime.GenerateAll();

            // if catheti^2 is odd, AB are both odd, (f(catheti^2) - 1) / 2 = 47547
            var n = kinds * 2 + 1;
            foreach (var p in prime)
            {
                while (n % p == 0)
                {
                    n /= p;
                    list.Add(p);
                }
            }
            list.Sort();
            list.Reverse();
            min = 1;
            for (int i = 0; i < list.Count; i++)
                min *= BigInteger.Pow(prime.Nums[i + 1], list[i] - 1);

            /**
             * if catheti^2 contains 2n factor of 2, remove case when one of AB is odd,
             * so factor must contains only 1~2n-1 of factor 2, the true number of factor of 2 must +2
             */
            for (int p2 = 0; p2 < list.Count; p2++)
            {
                var exp = list[p2];
                var tmp = BigInteger.Pow(2, exp + 1);

                list.RemoveAt(p2);
                for (int i = 0; i < list.Count; i++)
                    tmp *= BigInteger.Pow(prime.Nums[i + 1], list[i] - 1);
                list.Insert(p2, exp);
                if (tmp < min)
                    min = tmp;
            }

            return Misc.Sqrt(min).ToString();
        }
    }

    /// <summary>
    /// Let ABCD be a convex quadrilateral, with diagonals AC and BD. At each vertex
    /// the diagonal makes an angle with each of the two sides, creating eight corner
    /// angles.
    ///
    /// For example, at vertex A, the two angles are CAD, CAB.
    ///
    /// We call such a quadrilateral for which all eight corner angles have integer
    /// values when measured in degrees an "integer angled quadrilateral". An example
    /// of an integer angled quadrilateral is a square, where all eight corner angles
    /// are 45°. Another example is given by DAC = 20°, BAC = 60°, ABD = 50°,
    /// CBD = 30°, BCA = 40°, DCA = 30°, CDB = 80°, ADB = 50°.
    ///
    /// What is the total number of non-similar integer angled quadrilaterals?
    ///
    /// Note: In your calculations you may assume that a calculated angle is integral
    /// if it is within a tolerance of 10^-9 of an integer value.
    /// </summary>
    internal class Problem177 : Problem
    {
        private double epson = Math.Pow(0.1, 9);

        public Problem177() : base(177) { }

        protected override string Action()
        {
            var sin = (from i in Itertools.Range(0, 180) select Math.Sin(i * Math.PI / 180)).ToArray();
            var cos = (from i in Itertools.Range(0, 180) select Math.Cos(i * Math.PI / 180)).ToArray();
            var counter = 0;
            double ab = 1;

            /**
             * resize and rotate quadrilateral such that A is at (0,0), B at (1, 0),
             * and DAB is the minimal angle and CAB is smaller or equal to DAC, like:
             *  D  C
             * A  B
             */
            for (int a = 2; a <= 90; a++)
            {
                for (int abd = 1; abd < 180 - a; abd++)
                {
                    /**
                     * http://en.wikipedia.org/wiki/Law_of_sines
                     * a/sin(a) = b/sin(b) = c/sin(c)
                     */
                    double ad = ab * sin[abd] / sin[180 - a - abd];

                    for (int b = Math.Max(a, abd + 1); b <= 180 - a; b++)
                    {
                        for (int bac = b == a ? abd : 1; bac < Math.Min(a, 180 - b); bac++)
                        {
                            double ac = ab * sin[b] / sin[180 - b - bac];
                            double x = Math.Atan2(sin[bac] * ac - sin[a] * ad, cos[bac] * ac - cos[a] * ad) * 180 / Math.PI;
                            int cdx = (int)Math.Round(x), d = 180 - a + cdx, c = 180 - b - cdx;

                            if (c < a)
                                break;
                            if (d < b)
                                continue;
                            if (b == d && bac > a / 2)
                                continue;
                            if (c == a && abd > b / 2)
                                continue;
                            if (Math.Abs(x - cdx) < epson)
                                counter++;
                        }
                    }
                }
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// Consider the number 45656.
    /// It can be seen that each pair of consecutive digits of 45656 has a difference
    /// of one.
    /// A number for which every pair of consecutive digits has a difference of one is
    /// called a step number.
    /// A pandigital number contains every decimal digit from 0 to 9 at least once.
    /// How many pandigital step numbers less than 10^40 are there?
    /// </summary>
    internal class Problem178 : Problem
    {
        private const int length = 40;

        public Problem178() : base(178) { }

        private string GetIDX(int[] digits, int prev, int len)
        {
            var flags = 0;

            for (int i = 0; i < 10; i++)
            {
                if (digits[i] != 0)
                    flags |= 1 << (i);
            }

            return string.Join("|", new int[] { len, prev, flags });
        }

        private long Count(Dictionary<string, long> dict, int[] digits, int prev, int len)
        {
            long counter = 0;

            if (digits.Where(it => it == 0).Count() > len)
                return 0;
            if (len == 0)
                return 1;

            var key = GetIDX(digits, prev, len);

            if (dict.ContainsKey(key))
                return dict[key];
            if (prev > 0)
            {
                digits[prev - 1]++;
                counter += Count(dict, digits, prev - 1, len - 1);
                digits[prev - 1]--;
            }
            if (prev < 9)
            {
                digits[prev + 1]++;
                counter += Count(dict, digits, prev + 1, len - 1);
                digits[prev + 1]--;
            }

            dict.Add(key, counter);

            return counter;
        }

        protected override string Action()
        {
            var dict = new Dictionary<string, long>();
            var digits = new int[10];
            long counter = 0;

            for (int l = 10; l <= length; l++)
            {
                for (int i = 1; i < 10; i++)
                {
                    digits[i] = 1;
                    counter += Count(dict, digits, i, l - 1);
                    digits[i] = 0;
                }
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// Find the number of integers 1 < n < 10^7, for which n and n + 1 have the same
    /// number of positive divisors. For example, 14 has the positive divisors 1, 2, 7,
    /// 14 while 15 has 1, 3, 5, 15.
    /// </summary>
    internal class Problem179 : Problem
    {
        private const int upper = 10000000;

        public Problem179() : base(179) { }

        protected override string Action()
        {
            var nfactors = new int[upper + 1];
            var counter = 0;

            for (int f = 1; f <= upper; f++)
            {
                for (int i = f; i <= upper; i += f)
                    nfactors[i]++;
            }
            for (int n = 1; n < upper; n++)
            {
                if (nfactors[n] == nfactors[n + 1])
                    counter++;
            }

            return counter.ToString();
        }
    }
}