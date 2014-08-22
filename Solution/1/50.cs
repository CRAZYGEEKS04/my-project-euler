using ProjectEuler.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// In a triangular array of positive and negative integers, we wish to find a
    /// sub-triangle such that the sum of the numbers it contains is the smallest
    /// possible.
    ///
    /// In the example below, it can be easily verified that the marked triangle
    /// satisfies this condition having a sum of -42.
    ///
    ///  15 |
    /// -14 | -7\
    ///  20 |-13  -5\
    ///  -3 |  8  23 -26\
    ///   1 | -4  -5 -18   5\
    ///     ------------------
    /// -16   31   2   9  28   3
    ///
    /// We wish to make such a triangular array with one thousand rows, so we generate
    /// 500500 pseudo-random numbers sk in the range +-2^19, using a type of random
    /// number generator (known as a Linear Congruential Generator) as follows:
    ///
    /// t := 0
    /// for k = 1 up to k = 500500:
    ///   t := (615949*t + 797807) modulo 2^20
    ///   sk := t-2^19
    ///
    /// Thus: s1 = 273519, s2 = -153582, s3 = 450905 etc
    ///
    /// Our triangular array is then formed using the pseudo-random numbers thus:
    ///
    /// s1
    /// s2  s3
    /// s4  s5  s6
    /// s7  s8  s9 s10
    /// ...
    ///
    /// Sub-triangles can start at any element of the array and extend down as far as
    /// we like (taking-in the two elements directly below it from the next row, the
    /// three elements directly below from the row after that, and so on).
    ///
    /// The "sum of a sub-triangle" is defined as the sum of all the elements it
    /// contains.
    ///
    /// Find the smallest possible sub-triangle sum.
    /// </summary>
    internal class Problem150 : Problem
    {
        private const int rows = 1000;

        public Problem150() : base(150) { }

        private void Generate(int[][] array)
        {
            Modulo modulo = new Modulo(0x100000);
            long t = 0;

            for (int i = 0; i < rows; i++)
            {
                array[i] = new int[i + 1];
                for (int j = 0; j <= i; j++)
                {
                    t = modulo.Mod(615949 * t + 797807);
                    array[i][j] = (int)t - 0x80000;
                }
            }
        }

        protected override string Action()
        {
            var array = new int[rows][];
            long min = 0;

            Generate(array);
            for (int r = 0; r < rows; r++)
            {
                min = Math.Min(min, array[r].Min());
                for (int i = 1; i <= r; i++)
                    array[r][i] += array[r][i - 1];
            }

            for (int r = 0; r < rows - 1; r++)
            {
                long tmp = array[r][0];

                // i = 0
                for (int l = r + 1; l < rows; l++)
                {
                    tmp += array[l][l - r];

                    if (tmp < min)
                        min = tmp;
                }
                for (int i = 1; i <= r; i++)
                {
                    tmp = array[r][i];

                    tmp -= array[r][i - 1];
                    for (int l = r + 1; l < rows; l++)
                    {
                        tmp += array[l][i + l - r];
                        tmp -= array[l][i - 1];

                        if (tmp < min)
                            min = tmp;
                    }
                }
            }

            return min.ToString();
        }
    }

    /// <summary>
    /// A printing shop runs 16 batches (jobs) every week and each batch requires a
    /// sheet of special colour-proofing paper of size A5.
    ///
    /// Every Monday morning, the foreman opens a new envelope, containing a large
    /// sheet of the special paper with size A1.
    ///
    /// He proceeds to cut it in half, thus getting two sheets of size A2. Then he cuts
    /// one of them in half to get two sheets of size A3 and so on until he obtains the
    /// A5-size sheet needed for the first batch of the week.
    ///
    /// All the unused sheets are placed back in the envelope.
    ///
    /// At the beginning of each subsequent batch, he takes from the envelope one sheet
    /// of paper at random. If it is of size A5, he uses it. If it is larger, he
    /// repeats the 'cut-in-half' procedure until he has what he needs and any
    /// remaining sheets are always placed back in the envelope.
    ///
    /// Excluding the first and last batch of the week, find the expected number of
    /// times (during each week) that the foreman finds a single sheet of paper in the
    /// envelope.
    ///
    /// Give your answer rounded to six decimal places using the format x.xxxxxx .
    /// </summary>
    internal class Problem151 : Problem
    {
        public Problem151() : base(151) { }

        private string GetIDX(int[] papers)
        {
            return string.Join("", papers);
        }

        private Fraction Count(Dictionary<string, Fraction> dict, int[] papers)
        {
            var idx = GetIDX(papers);
            var factor = papers.Sum();
            Fraction ret = 0;

            if (dict.ContainsKey(idx))
                return dict[idx];

            if (factor == 1)
                ret += 1;
            for (int i = 0; i < papers.Length; i++)
            {
                if (papers[i] <= 0)
                    continue;

                papers[i]--;
                for (int j = i + 1; j < papers.Length; j++)
                    papers[j]++;
                ret += Count(dict, papers) / factor * (papers[i] + 1);
                for (int j = i + 1; j < papers.Length; j++)
                    papers[j]--;
                papers[i]++;
            }

            dict[idx] = ret;

            return ret;
        }

        protected override string Action()
        {
            var dict = new Dictionary<string, Fraction>();
            Fraction ret = 0;

            dict.Add("0001", 0);
            ret = Count(dict, new int[] { 1, 1, 1, 1 });

            // don't forget to round up
            var tmp = ret.Numerator * 10000000 / ret.Denominator;
            if (tmp % 10 >= 5)
                tmp += 10;

            return ((double)tmp / 10000000).ToString().Substring(0, 8);
        }
    }

    /// <summary>
    /// There are several ways to write the number 1/2 as a sum of inverse squares
    /// using distinct integers.
    ///
    /// For instance, the numbers {2,3,4,5,7,12,15,20,28,35} can be used:
    ///
    /// 1/2 = 1/2^2 + 1/3^2 + 1/4^2 + 1/5^2 + 1/7^2 + 1/12^2 + 1/15^2 + 1/20^2 + 1/28^2
    /// + 1/35^2
    ///
    /// In fact, only using integers between 2 and 45 inclusive, there are exactly
    /// three ways to do it, the remaining two being: {2,3,4,6,7,9,10,20,28,35,36,45}
    /// and {2,3,4,6,7,9,12,15,28,30,35,36,45}.
    ///
    /// How many ways are there to write the number 1/2 as a sum of inverse squares
    /// using distinct integers between 2 and 80 inclusive?
    /// </summary>
    internal class Problem152 : Problem
    {
        private const int upper = 80;

        public Problem152() : base(152) { }

        public static int GetMaxFactor(Prime prime, BigInteger number)
        {
            for (int i = prime.Nums.Count - 1; i >= 0; i--)
            {
                if (number % prime.Nums[i] == 0)
                    return prime.Nums[i];
            }

            throw new ArgumentException();
        }

        private bool PredicateVector(Tuple<SmallFraction, BitVector>[] collection, int length)
        {
            BitVector v = new BitVector(upper + 1);

            for (int i = 0; i < length; i++)
            {
                if (!(v & collection[i].Item2).IsAllClear())
                    return false;
                v |= collection[i].Item2;
            }

            return true;
        }

        private void Reduce(Prime p, List<Tuple<SmallFraction, BitVector>>[] dict, int n)
        {
            for (int l = 2; l <= dict[n].Count; l++)
            {
                foreach (var collection in Itertools.Combinations(dict[n], l, PredicateVector))
                {
                    var vector = new BitVector(upper + 1);
                    var value = new SmallFraction(0, 1);
                    int factor;

                    foreach (var c in collection)
                    {
                        value += c.Item1;
                        vector |= c.Item2;
                    }
                    factor = GetMaxFactor(p, value.Denominator);
                    if (factor < n)
                    {
                        lock (dict[factor])
                            dict[factor].Add(new Tuple<SmallFraction, BitVector>(value, vector));
                    }
                }
            }
        }

        private List<BitVector> validSums;

        private void CheckSum(List<Tuple<SmallFraction, BitVector>> values, SmallFraction[] sums, int pos,
            SmallFraction current, BitVector cb)
        {
            BitVector tcb;
            SmallFraction tc;
            var ret = new List<BitVector>();
            var half = new SmallFraction(1, 2);

            if (current == half)
            {
                lock (validSums)
                    validSums.Add(cb);
            }
            if (pos >= sums.Length || current > half || current + sums[pos] < half)
                return;

            if (pos < 5)
            {
                Parallel.For(pos, sums.Length, (i) =>
                {
                    if ((cb & values[i].Item2).IsAllClear())
                    {
                        tc = current + values[i].Item1;
                        tcb = cb | values[i].Item2;
                        CheckSum(values, sums, i + 1, tc, tcb);
                    }
                });
            }
            else
            {
                for (int i = pos; i < sums.Length; i++)
                {
                    if ((cb & values[i].Item2).IsAllClear())
                    {
                        current += values[i].Item1;
                        tcb = cb | values[i].Item2;
                        CheckSum(values, sums, i + 1, current, tcb);
                        current -= values[i].Item1;
                    }
                }
            }
        }

        protected override string Action()
        {
            var dict = new List<Tuple<SmallFraction, BitVector>>[upper + 1];
            var p = new Prime(upper);

            p.GenerateAll();
            foreach (var n in p)
                dict[n] = new List<Tuple<SmallFraction, BitVector>>();
            for (int i = 2; i <= upper; i++)
            {
                var value = new SmallFraction(1, i * i);
                var vector = new BitVector(upper + 1);

                vector.Set(i);
                dict[GetMaxFactor(p, i)].Add(new Tuple<SmallFraction, BitVector>(value, vector));
            }
            for (int n = upper; n > 5; n--)
            {
                if (dict[n] == null)
                    continue;
                Reduce(p, dict, n);
            }

            var valid = new List<Tuple<SmallFraction, BitVector>>();
            var sums = new SmallFraction[dict[2].Count + dict[3].Count + dict[5].Count];

            valid.AddRange(dict[2]);
            valid.AddRange(dict[3]);
            valid.AddRange(dict[5]);
            valid = valid.OrderByDescending(it => it.Item1).ToList();
            sums[valid.Count - 1] = valid[valid.Count - 1].Item1;
            for (int i = valid.Count - 2; i >= 0; i--)
                sums[i] = valid[i].Item1 + sums[i + 1];

            validSums = new List<BitVector>(upper + 1);
            CheckSum(valid, sums, 0, 0, new BitVector(upper + 1));

            return validSums.Distinct().Count().ToString();
        }
    }

    /// <summary>
    /// As we all know the equation x^2=-1 has no solutions for real x.
    /// If we however introduce the imaginary number i this equation has two solutions:
    /// x=i and x=-i.
    /// If we go a step further the equation (x-3)^2=-4 has two complex solutions:
    /// x=3+2i and x=3-2i.
    /// x=3+2i and x=3-2i are called each others' complex conjugate.
    /// Numbers of the form a+bi are called complex numbers.
    /// In general a+bi and a-bi are each other's complex conjugate.
    ///
    /// A Gaussian Integer is a complex number a+bi such that both a and b are
    /// integers.
    /// The regular integers are also Gaussian integers (with b=0).
    /// To distinguish them from Gaussian integers with b != 0 we call such integers
    /// "rational integers."
    /// A Gaussian integer is called a divisor of a rational integer n if the result is
    /// also a Gaussian integer.
    /// If for example we divide 5 by 1+2i we can simplify in the following manner:
    /// Multiply numerator and denominator by the complex conjugate of 1+2i: 1-2i.
    /// The result is: 5/(1+2i)=5(1-2i)/(1+2i)(1-2i)=5(1-2i)/5=1-2i
    /// So 1+2i is a divisor of 5.
    /// Note that 1+i is not a divisor of 5 because 5/(1+i)=5/2-5i/2.
    /// Note also that if the Gaussian Integer (a+bi) is a divisor of a rational
    /// integer n, then its complex conjugate (a-bi) is also a divisor of n.
    ///
    /// In fact, 5 has six divisors such that the real part is positive: {1, 1 + 2i,
    /// 1 - 2i, 2 + i, 2 - i, 5}.
    /// The following is a table of all of the divisors for the first five positive
    /// rational integers:
    ///
    /// n  Gaussian integer divisors with positive real part  Sum s(n) of these divisors
    /// 1                                                  1                           1
    /// 2                                     1, 1+i, 1-i, 2                           5
    /// 3                                               1, 3                           4
    /// 4                      1, 1+i, 1-i, 2, 2+2i, 2-2i, 4                          13
    /// 5                         1, 1+2i, 1-2i, 2+i, 2-i, 5                          12
    ///
    /// For divisors with positive real parts, then, we have: Σ(s(n))=35 (1<=n<=5)
    ///
    /// For 1 <= n <= 10^5, Σ(s(n))=17924657155.
    ///
    /// What is Σ(s(n)) for 1 <= n <= 10^8?
    /// </summary>
    internal class Problem153 : Problem
    {
        private const int upper = 100000000;

        public Problem153() : base(153) { }

        protected override string Action()
        {
            // http://mathworld.wolfram.com/GaussianPrime.html
            var dict = new long[upper + 1];
            var lck = new object();
            BigInteger sum = 0;

            // special case n+ni, n-ni for 2n
            for (int i = 2; i <= upper; i += 2)
                dict[i] += i;
            Parallel.For(1, (int)Misc.Sqrt(upper / 2) + 1, (a) =>
            {
                var step = 2 - (a & 1);

                for (int b = a + 1; ; b += step)
                {
                    // a+bi, a-bi, b+ai, b-ai
                    var tmp = 2 * (a + b);
                    var s = a * a + b * b;

                    if (s > upper)
                        break;
                    if (Factor.GetCommonFactor(a, b) != 1)
                        continue;
                    for (int i = 1; i <= upper / s; i++)
                        Interlocked.Add(ref dict[s * i], tmp * i);
                }
            });
            Parallel.For(0, upper / 100, (n100) =>
            {
                BigInteger tmp = 0;

                for (int n = n100 * 100 + 1; n <= n100 * 100 + 100; n++)
                {
                    var l = upper / n;

                    tmp += l * n;
                    tmp += l * dict[n];
                }

                lock (lck)
                    sum += tmp;
            });

            return sum.ToString();
        }
    }

    /// <summary>
    /// A triangular pyramid is constructed using spherical balls so that each ball
    /// rests on exactly three balls of the next lower level.
    ///
    /// 1
    /// n=0
    ///
    ///  1
    /// 1 1
    /// n=1
    ///
    ///   1
    ///  2 2
    /// 1 2 1
    /// n=2
    ///
    ///    1
    ///   3 3
    ///  3 6 3
    /// 1 3 3 1
    /// n=3
    ///
    /// Then, we calculate the number of paths leading from the apex to each position:
    ///
    /// A path starts at the apex and progresses downwards to any of the three spheres
    /// directly below the current position.
    ///
    /// Consequently, the number of paths to reach a certain position is the sum of the
    /// numbers immediately above it (depending on the position, there are up to three
    /// numbers above it).
    ///
    /// The result is Pascal's pyramid and the numbers at each level n are the
    /// coefficients of the trinomial expansion (x + y + z)^n.
    ///
    /// How many coefficients in the expansion of (x + y + z)^200000 are multiples of
    /// 10^12?
    /// </summary>
    internal class Problem154 : Problem
    {
        private const int upper = 200000;
        private const int pow = 12;

        public Problem154() : base(154) { }

        protected override string Action()
        {
            /**
             * http://en.wikipedia.org/wiki/Pascal's_pyramid
             *
             * coefficients is 200000! / a!b!c!, where a+b+c = 200000
             */
            int[] fives = new int[upper + 1], twos = new int[upper + 1];
            int current, tmp;
            long counter = 0;

            // calculate how many 0's does n! have based on factor 2 and 5
            current = 0;
            for (int i = 1; i <= upper; i++)
            {
                tmp = i;
                while (tmp % 5 == 0)
                {
                    current++;
                    tmp /= 5;
                }
                fives[i] = current;
            }
            current = 0;
            for (int i = 1; i <= upper; i++)
            {
                tmp = i;
                while (tmp % 2 == 0)
                {
                    current++;
                    tmp /= 2;
                }
                twos[i] = current;
            }

            // a,b,c are symmetric, assume a<=b<=c
            Parallel.For(0, upper / 3 + 1, (a) =>
            {
                var leftfive = fives[upper] - fives[a];
                var lefttwo = twos[upper] - twos[a];
                var tmpc = 0;

                if (Math.Min(leftfive - fives[a] - fives[upper - a - a], lefttwo - twos[a] - twos[upper - a - a]) >= pow)
                    tmpc += 3;
                for (int b = a + 1; b <= (upper - a) / 2; b++)
                {
                    int c = upper - a - b;

                    if (Math.Min(leftfive - fives[b] - fives[c], lefttwo - twos[b] - twos[c]) >= pow)
                    {
                        if (b == c)
                            tmpc += 3;
                        else
                            tmpc += 6;
                    }
                }
                Interlocked.Add(ref counter, tmpc);
            });

            return counter.ToString();
        }
    }

    /// <summary>
    /// An electric circuit uses exclusively identical capacitors of the same value C.
    /// The capacitors can be connected in series or in parallel to form sub-units,
    /// which can then be connected in series or in parallel with other capacitors or
    /// other sub-units to form larger sub-units, and so on up to a final circuit.
    ///
    /// Using this simple procedure and up to n identical capacitors, we can make
    /// circuits having a range of different total capacitances. For example, using up
    /// to n=3 capacitors of 60uF each, we can obtain the following 7 distinct total
    /// capacitance values:
    ///
    /// C+C+C = 180uF
    /// C+C = 120uF
    /// C+C|C = 90uF
    /// C = 60uF
    /// (C+C)|C = 40uF
    /// C|C = 30uF
    /// C|C|C = 20uF
    ///
    /// If we denote by D(n) the number of distinct total capacitance values we can
    /// obtain when using up to n equal-valued capacitors and the simple procedure
    /// described above, we have: D(1)=1, D(2)=3, D(3)=7 ...
    ///
    /// Find D(18).
    /// </summary>
    internal class Problem155 : Problem
    {
        private const int upper = 18;

        public Problem155() : base(155) { }

        private HashSet<SmallFraction> Calculate(HashSet<SmallFraction>[] dict, int l)
        {
            var ret = new HashSet<SmallFraction>();

            Parallel.For(1, l / 2 + 1, (i) =>
            {
                var tmp = new HashSet<SmallFraction>();

                foreach (var pair in Itertools.Product(dict[i], dict[l - i]))
                {
                    tmp.Add(pair[0] + pair[1]);
                    tmp.Add(1 / (1 / pair[0] + 1 / pair[1]));
                }

                lock (ret)
                    ret.UnionWith(tmp);
            });

            return ret;
        }

        protected override string Action()
        {
            var dict = new HashSet<SmallFraction>[upper + 1];

            dict[1] = new HashSet<SmallFraction>();
            dict[1].Add(60);

            for (int i = 2; i <= upper; i++)
                dict[i] = Calculate(dict, i);

            for (int i = 1; i < upper; i++)
                dict[upper].UnionWith(dict[i]);

            return dict[upper].Count.ToString();
        }
    }

    /// <summary>
    /// Starting from zero the natural numbers are written down in base 10 like this:
    /// 0 1 2 3 4 5 6 7 8 9 10 11 12....
    ///
    /// Consider the digit d=1. After we write down each number n, we will update the
    /// number of ones that have occurred and call this number f(n,1). The first values
    /// for f(n,1), then, are as follows:
    ///
    ///  n f(n,1)
    ///  0     0
    ///  1     1
    ///  2     1
    ///  3     1
    ///  4     1
    ///  5     1
    ///  6     1
    ///  7     1
    ///  8     1
    ///  9     1
    /// 10     2
    /// 11     4
    /// 12     5
    ///
    /// Note that f(n,1) never equals 3.
    /// So the first two solutions of the equation f(n,1)=n are n=0 and n=1. The next
    /// solution is n=199981.
    ///
    /// In the same manner the function f(n,d) gives the total number of digits d that
    /// have been written down after the number n has been written.
    /// In fact, for every digit d != 0, 0 is the first solution of the equation
    /// f(n,d)=n.
    ///
    /// Let s(d) be the sum of all the solutions for which f(n,d)=n.
    /// You are given that s(1)=22786974071.
    ///
    /// Find Σ(s(d)) for 1 <= d <= 9.
    ///
    /// Note: if, for some n, f(n,d)=n for more than one value of d this value of n is
    /// counted again for every value of d for which f(n,d)=n.
    /// </summary>
    internal class Problem156 : Problem
    {
        private const int maxLength = 16;

        public Problem156() : base(156) { }

        private int CountDigits(long header, int digit)
        {
            return header.ToString().Where(it => it == '0' + digit).Count();
        }

        private long GetSum(long[] array, ref long n, ref long fn, long pow, int digit)
        {
            if (pow == 1)
            {
                n++;
                fn += CountDigits(n, digit);
                if (n == fn)
                    return n;
                else
                    return 0;
            }

            long sum = 0, step = pow.ToString().Length - 1;
            int h = CountDigits(n + 1, digit);

            if (h == 0)
            {
                if (n > fn + array[step] || fn > n + pow)
                {
                    n += pow;
                    fn += array[step];

                    return 0;
                }
            }
            else
            {
                if (n < fn || n > fn + array[step] + h * pow)
                {
                    n += pow;
                    fn += h * pow + array[step];

                    return 0;
                }
            }

            for (int i = 0; i < 10; i++)
                sum += GetSum(array, ref n, ref fn, pow / 10, digit);

            return sum;
        }

        private long GetSum(long[] array, int digit)
        {
            long sum = 0, n = -1, fn = 0, value = digit;

            sum += GetSum(array, ref n, ref fn, (long)BigInteger.Pow(10, array.Length - 2), digit);

            return sum;
        }

        protected override string Action()
        {
            BigInteger ret = 0;
            var array = new long[maxLength];

            array[0] = 0;
            array[1] = 1;

            for (int i = 2; i < array.Length; i++)
                array[i] = i * array[i - 1] * 10 / (i - 1);

            for (int d = 1; d <= 9; d++)
                ret += GetSum(array, d);

            return ret.ToString();
        }
    }

    /// <summary>
    /// Consider the diophantine equation 1/a+1/b= p/10^n with a, b, p, n positive
    /// integers and a <= b.
    /// For n=1 this equation has 20 solutions that are listed below:
    ///
    /// 1/1+1/1=20/10
    /// 1/1+1/2=15/10
    /// 1/1+1/5=12/10
    /// 1/1+1/10=11/10
    /// 1/2+1/2=10/10
    /// 1/2+1/5=7/10
    /// 1/2+1/10=6/10
    /// 1/3+1/6=5/10
    /// 1/3+1/15=4/10
    /// 1/4+1/4=5/10
    /// 1/4+1/20=3/10
    /// 1/5+1/5=4/10
    /// 1/5+1/10=3/10
    /// 1/6+1/30=2/10
    /// 1/10+1/10=2/10
    /// 1/11+1/110=1/10
    /// 1/12+1/60=1/10
    /// 1/14+1/35=1/10
    /// 1/15+1/30=1/10
    /// 1/20+1/20=1/10
    ///
    /// How many solutions has this equation for 1 <= n <= 9?
    /// </summary>
    internal class Problem157 : Problem
    {
        private const int upper = 9;

        public Problem157() : base(157) { }

        private int Count(Prime p, Tuple<long, long> value)
        {
            var sum = new SmallFraction(value.Item1 + value.Item2, value.Item1 * value.Item2);
            long two = 0, five = 0, tmp = sum.Denominator, num = sum.Numerator;
            var counter = 0;

            while (tmp % 2 == 0)
            {
                tmp /= 2;
                two++;
            }
            while (tmp % 5 == 0)
            {
                tmp /= 5;
                five++;
            }
            while (two < five)
            {
                two++;
                num *= 2;
            }
            while (five < two)
            {
                five++;
                num *= 5;
            }
            if (five == 0)
            {
                five++;
                num *= 10;
            }

            for (long l = five; l <= upper; l++)
            {
                counter += Factor.GetFactorNumber(p, (int)num, true, true);
                num *= 10;
            }

            return counter;
        }

        protected override string Action()
        {
            var p = new Prime((int)Misc.Sqrt(BigInteger.Pow(10, upper) * 2) + 1);
            var values = new HashSet<Tuple<long, long>>();
            var twos = new long[upper + 1];
            var fives = new long[upper + 1];
            var counter = 0;

            p.GenerateAll();
            /**
             * (a+b)/ab = p/10^n, 10^n*a + 10^n*b = p*a*b
             * ab must contain same non-2,5 factor, or only 2,5 factors
             *
             * Calculate possible common factor of a,b
             */
            twos[0] = 1;
            fives[0] = 1;
            for (int i = 1; i <= upper; i++)
            {
                twos[i] = twos[i - 1] * 2;
                fives[i] = fives[i - 1] * 5;
            }
            foreach (var idx in Itertools.PermutationsWithReplacement(Itertools.Range(0, upper), 2))
            {
                values.Add(new Tuple<long, long>(1, twos[idx[0]] * fives[idx[1]]));
                values.Add(new Tuple<long, long>(Math.Min(twos[idx[0]], fives[idx[1]]), Math.Max(twos[idx[0]], fives[idx[1]])));
            }
            foreach (var value in values)
                counter += Count(p, value);

            return counter.ToString();
        }
    }

    /// <summary>
    /// Taking three different letters from the 26 letters of the alphabet, character
    /// strings of length three can be formed.
    /// Examples are 'abc', 'hat' and 'zyx'.
    /// When we study these three examples we see that for 'abc' two characters come
    /// lexicographically after its neighbour to the left.
    /// For 'hat' there is exactly one character that comes lexicographically after its
    /// neighbour to the left. For 'zyx' there are zero characters that come
    /// lexicographically after its neighbour to the left.
    /// In all there are 10400 strings of length 3 for which exactly one character
    /// comes lexicographically after its neighbour to the left.
    ///
    /// We now consider strings of n <= 26 different characters from the alphabet.
    /// For every n, p(n) is the number of strings of length n for which exactly one
    /// character comes lexicographically after its neighbour to the left.
    ///
    /// What is the maximum value of p(n)?
    /// </summary>
    internal class Problem158 : Problem
    {
        private const int upper = 26;

        public Problem158() : base(158) { }

        private BigInteger PickNone(Dictionary<Tuple<int, int, int>, BigInteger> none, Tuple<int, int, int> current)
        {
            BigInteger sum = 0;

            if (none.ContainsKey(current))
                return none[current];

            if (current.Item3 == 1)
            {
                sum = current.Item1;
            }
            else
            {
                for (int i = 0; i < current.Item1; i++)
                    sum += PickNone(none, new Tuple<int, int, int>(i, current.Item1 + current.Item2 + i - 1, current.Item3 - 1));
            }

            none.Add(current, sum);

            return none[current];
        }

        private BigInteger PickOne(Dictionary<Tuple<int, int, int>, BigInteger> one,
            Dictionary<Tuple<int, int, int>, BigInteger> none, Tuple<int, int, int> current)
        {
            BigInteger sum = 0;

            if (one.ContainsKey(current))
                return one[current];
            if (current.Item3 == 1)
            {
                sum = current.Item2;
            }
            else
            {
                for (int i = 0; i < current.Item1; i++)
                    sum += PickOne(one, none, new Tuple<int, int, int>(i, current.Item1 + current.Item2 - i - 1, current.Item3 - 1));
                for (int i = 0; i < current.Item2; i++)
                    sum += PickNone(none, new Tuple<int, int, int>(current.Item1 + current.Item2 - i - 1, i, current.Item3 - 1));
            }

            one.Add(current, sum);

            return one[current];
        }

        protected override string Action()
        {
            var one = new Dictionary<Tuple<int, int, int>, BigInteger>();
            var none = new Dictionary<Tuple<int, int, int>, BigInteger>();
            var counter = new BigInteger[upper + 1];

            for (int length = 2; length <= upper; length++)
            {
                for (int i = 0; i < upper; i++)
                    counter[length] += PickOne(one, none, new Tuple<int, int, int>(i, upper - i - 1, length - 1));
            }

            return counter.Max().ToString();
        }
    }

    /// <summary>
    /// A composite number can be factored many different ways. For instance, not
    /// including multiplication by one, 24 can be factored in 7 distinct ways:
    ///
    /// 24 = 2x2x2x3
    /// 24 = 2x3x4
    /// 24 = 2x2x6
    /// 24 = 4x6
    /// 24 = 3x8
    /// 24 = 2x12
    /// 24 = 24
    ///
    /// Recall that the digital root of a number, in base 10, is found by adding
    /// together the digits of that number, and repeating that process until a number
    /// is arrived at that is less than 10. Thus the digital root of 467 is 8.
    ///
    /// We shall call a Digital Root Sum (DRS) the sum of the digital roots of the
    /// individual factors of our number.
    /// The chart below demonstrates all of the DRS values for 24.
    ///
    /// Factorisation  Digital Root Sum
    ///       2x2x2x3                 9
    ///         2x3x4                 9
    ///         2x2x6                10
    ///           4x6                10
    ///           3x8                11
    ///          2x12                 5
    ///            24                 6
    ///
    /// The maximum Digital Root Sum of 24 is 11.
    /// The function mdrs(n) gives the maximum Digital Root Sum of n. So mdrs(24)=11.
    /// Find Σ(mdrs(n)) for 1 < n < 1,000,000.
    /// </summary>
    internal class Problem159 : Problem
    {
        private const int upper = 1000000;

        public Problem159() : base(159) { }

        private void CalculateMDRS(Prime prime, int[] array, int n)
        {
            var max = Misc.GetDigitalRoot(n);

            if (prime.Contains(n))
            {
                array[n] = max;
                return;
            }

            for (int i = 2; i <= Misc.Sqrt(n); i++)
            {
                if (n % i != 0)
                    continue;

                if (array[i] + array[n / i] > max)
                    max = array[i] + array[n / i];
            }

            array[n] = max;
        }

        protected override string Action()
        {
            var p = new Prime(upper);
            var array = new int[upper];

            p.GenerateAll();
            for (int i = 2; i < upper; i++)
                CalculateMDRS(p, array, i);

            return array.Sum().ToString();
        }
    }
}