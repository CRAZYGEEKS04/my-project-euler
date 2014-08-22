using ProjectEuler.Common;
using ProjectEuler.Common.Graph;
using ProjectEuler.Common.Miscellany;
using ProjectEuler.Common.Partition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// Euler's Totient function, φ(n) [sometimes called the phi function], is used to
    /// determine the number of positive numbers less than or equal to n which are
    /// relatively prime to n. For example, as 1, 2, 4, 5, 7, and 8, are all less than
    /// nine and relatively prime to nine, φ(9)=6.
    /// The number 1 is considered to be relatively prime to every positive number, so
    /// φ(1)=1.
    ///
    /// Interestingly, φ(87109)=79180, and it can be seen that 87109 is a permutation
    /// of 79180.
    ///
    /// Find the value of n, 1 < n < 10^7, for which φ(n) is a permutation of n and the
    /// ratio n/φ(n) produces a minimum.
    /// </summary>
    internal class Problem70 : Problem
    {
        private const int upper = 10000000;

        public Problem70() : base(70) { }

        protected override string Action()
        {
            // * Euler's Totient function:
            // if n = p1^a1 * p2^a2 * ... * pn^an, where [p1, p2, ..., pn] are primes
            // φ(n) = p1^(a1 - 1)(p1 - 1) * p2^(a2 - 1)(p2 - 1) * ...
            //
            // * n / φ(n), will be p1/(p1 - 1) * p2/(p2 - 1) * ... for all prime factors
            // the result is smaller if p is bigger
            // * So fewer p and bigger p product smaller n/φ(n)
            // * a single prime will never be permutation of prime - 1
            // * Try two prime factors
            var p = new Prime(upper);
            double minphi = 3;
            int ret = 6;

            p.GenerateAll();
            var idx = BinarySearch.SearchRight(p.Nums, (int)Math.Sqrt(upper));

            for (int i = idx; i < p.Nums.Count; i++)
            {
                var start = BinarySearch.SearchLeft(p.Nums, upper / p.Nums[i]);
                for (int j = start; j >= 0; j--)
                {
                    var t1 = p.Nums[i] * p.Nums[j];
                    var t2 = (p.Nums[i] - 1) * (p.Nums[j] - 1);
                    var phi = (double)t1 / t2;

                    if (phi > minphi)
                        break;

                    if (Misc.IsPermutation(t1.ToString(), t2.ToString()))
                    {
                        minphi = phi;
                        ret = t1;
                        break;
                    }
                }
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// Consider the fraction, n/d, where n and d are positive integers. If n < d and
    /// HCF(n,d)=1, it is called a reduced proper fraction.
    ///
    /// If we list the set of reduced proper fractions for d <= 8 in ascending order of
    /// size, we get:
    ///
    /// 1/8, 1/7, 1/6, 1/5, 1/4, 2/7, 1/3, 3/8, 2/5, 3/7, 1/2, 4/7, 3/5, 5/8, 2/3, 5/7,
    /// 3/4, 4/5, 5/6, 6/7, 7/8
    ///
    /// It can be seen that 2/5 is the fraction immediately to the left of 3/7.
    ///
    /// By listing the set of reduced proper fractions for d <= 1,000,000 in ascending
    /// order of size, find the numerator of the fraction immediately to the left of
    /// 3/7.
    /// </summary>
    internal class Problem71 : Problem
    {
        private const int upper = 1000000;

        public Problem71() : base(71) { }

        protected override string Action()
        {
            // * a/b is less than 3/7
            // * 3/7 - a/b = (3b - 7a) / 7b is minimal
            // 3b-7a=1 and b is maximum available number
            for (int b = upper; b > 1; b--)
            {
                if (b * 3 % 7 != 1)
                    continue;

                return new Fraction((b * 3 - 1) / 7, b).Numerator.ToString();
            }

            return null;
        }
    }

    /// <summary>
    /// Consider the fraction, n/d, where n and d are positive integers. If n < d and
    /// HCF(n,d)=1, it is called a reduced proper fraction.
    ///
    /// If we list the set of reduced proper fractions for d <= 8 in ascending order of
    /// size, we get:
    ///
    /// 1/8, 1/7, 1/6, 1/5, 1/4, 2/7, 1/3, 3/8, 2/5, 3/7, 1/2, 4/7, 3/5, 5/8, 2/3, 5/7,
    /// 3/4, 4/5, 5/6, 6/7, 7/8
    ///
    /// It can be seen that there are 21 elements in this set.
    ///
    /// How many elements would be contained in the set of reduced proper fractions for
    /// d <= 1,000,000?
    /// </summary>
    internal class Problem72 : Problem
    {
        private const int upper = 1000000;

        public Problem72() : base(72) { }

        protected override string Action()
        {
            BigInteger counter = 0;
            var p = new Prime(upper);

            p.GenerateAll();
            for (int i = 2; i <= upper; i++)
                counter += EulerPhi.GetPhi(p, i);

            return counter.ToString();
        }
    }

    /// <summary>
    /// Consider the fraction, n/d, where n and d are positive integers. If n < d and
    /// HCF(n,d)=1, it is called a reduced proper fraction.
    ///
    /// If we list the set of reduced proper fractions for d <= 8 in ascending order of
    /// size, we get:
    ///
    /// 1/8, 1/7, 1/6, 1/5, 1/4, 2/7, 1/3, 3/8, 2/5, 3/7, 1/2, 4/7, 3/5, 5/8, 2/3, 5/7,
    /// 3/4, 4/5, 5/6, 6/7, 7/8
    ///
    /// It can be seen that there are 3 fractions between 1/3 and 1/2.
    ///
    /// How many fractions lie between 1/3 and 1/2 in the sorted set of reduced proper
    /// fractions for d <= 12,000?
    /// </summary>
    internal class Problem73 : Problem
    {
        private const int upper = 12000;

        public Problem73() : base(73) { }

        protected override string Action()
        {
            var sbt = new FareySequence(upper);

            return sbt.Count(1, 3, 1, 2).ToString();
        }
    }

    /// <summary>
    /// The number 145 is well known for the property that the sum of the factorial of
    /// its digits is equal to 145:
    ///
    /// 1! + 4! + 5! = 1 + 24 + 120 = 145
    ///
    /// Perhaps less well known is 169, in that it produces the longest chain of
    /// numbers that link back to 169; it turns out that there are only three such
    /// loops that exist:
    ///
    /// 169 -> 363601 -> 1454 -> 169
    /// 871 -> 45361 -> 871
    /// 872 -> 45362 -> 872
    ///
    /// It is not difficult to prove that EVERY starting number will eventually get
    /// stuck in a loop. For example,
    ///
    /// 69 -> 363600 -> 1454 -> 169 -> 363601 -> (1454)
    /// 78 -> 45360 -> 871 -> 45361 -> (871)
    /// 540 -> 145 -> (145)
    ///
    /// Starting with 69 produces a chain of five non-repeating terms, but the longest
    /// non-repeating chain with a starting number below one million is sixty terms.
    ///
    /// How many chains, with a starting number below one million, contain exactly
    /// sixty non-repeating terms?
    /// </summary>
    internal class Problem74 : Problem
    {
        private const int upper = 1000000;

        public Problem74() : base(74) { }

        private int[] Factorial;

        private BigInteger GetNext(string number)
        {
            BigInteger sum = 0;

            foreach (var c in number)
                sum += Factorial[c - '0'];

            return sum;
        }

        private int GetLength(int[] lens, BigInteger number)
        {
            var next = GetNext(number.ToString());

            if (number >= upper)
                return GetLength(lens, next) + 1;

            if (lens[(int)number] != 0)
                return lens[(int)number];

            var length = GetLength(lens, next) + 1;
            lens[(int)number] = length;

            return length;
        }

        protected override string Action()
        {
            var lens = new int[upper];
            var counter = 0;

            Factorial = new int[10];
            Factorial[0] = 1;
            for (int i = 1; i < 10; i++)
                Factorial[i] = Factorial[i - 1] * i;

            lens[1] = 1;
            lens[2] = 1;
            lens[145] = 1;
            lens[40585] = 1;
            lens[871] = 2;
            lens[872] = 2;
            lens[45361] = 2;
            lens[45362] = 2;
            lens[169] = 3;
            lens[1454] = 3;
            lens[363601] = 3;

            for (int i = 1; i < upper; i++)
                if (GetLength(lens, i) == 60)
                    counter++;

            return counter.ToString();
        }
    }

    /// <summary>
    /// It turns out that 12 cm is the smallest length of wire that can be bent to form
    /// an integer sided right angle triangle in exactly one way, but there are many
    /// more examples.
    ///
    /// 12 cm: (3,4,5)
    /// 24 cm: (6,8,10)
    /// 30 cm: (5,12,13)
    /// 36 cm: (9,12,15)
    /// 40 cm: (8,15,17)
    /// 48 cm: (12,16,20)
    ///
    /// In contrast, some lengths of wire, like 20 cm, cannot be bent to form an
    /// integer sided right angle triangle, and other lengths allow more than one
    /// solution to be found; for example, using 120 cm it is possible to form exactly
    /// three different integer sided right angle triangles.
    ///
    /// 120 cm: (30,40,50), (20,48,52), (24,45,51)
    ///
    /// Given that L is the length of the wire, for how many values of L <= 1,500,000
    /// can exactly one integer sided right angle triangle be formed?
    /// </summary>
    internal class Problem75 : Problem
    {
        private const int upper = 1500000;

        public Problem75() : base(75) { }

        protected override string Action()
        {
            var lens = new Dictionary<int, int>();
            var counter = 0;

            foreach (var triple in PythagoreanTriple.GeneratePrimitive(upper))
            {
                int l = triple.Sum();
                var tmp = l;

                while (tmp <= upper)
                {
                    if (lens.ContainsKey(tmp))
                        lens[tmp]++;
                    else
                        lens.Add(tmp, 1);
                    tmp += l;
                }
            }

            foreach (var pair in lens)
            {
                if (pair.Value == 1)
                    counter++;
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// It is possible to write five as a sum in exactly six different ways:
    ///
    /// 4 + 1
    /// 3 + 2
    /// 3 + 1 + 1
    /// 2 + 2 + 1
    /// 2 + 1 + 1 + 1
    /// 1 + 1 + 1 + 1 + 1
    ///
    /// How many different ways can one hundred be written as a sum of at least two
    /// positive integers?
    /// </summary>
    internal class Problem76 : Problem
    {
        public Problem76() : base(76) { }

        protected override string Action()
        {
            return (Partition.Generate(100) - 1).ToString();
        }
    }

    /// <summary>
    /// It is possible to write ten as the sum of primes in exactly five different ways:
    ///
    /// 7 + 3
    /// 5 + 5
    /// 5 + 3 + 2
    /// 3 + 3 + 2 + 2
    /// 2 + 2 + 2 + 2 + 2
    ///
    /// What is the first value which can be written as the sum of primes in over five
    /// thousand different ways?
    /// </summary>
    internal class Problem77 : Problem
    {
        private const int upper = 5000;

        public Problem77() : base(77) { }

        private int Count(Dictionary<Tuple<int, int>, int> dict, Prime p, int number, int maxp)
        {
            var key = new Tuple<int, int>(number, maxp);
            var tmp = 0;

            if (number == 0)
                return 1;
            if (number < 2 || maxp < 2)
                return 0;
            if (dict.ContainsKey(key))
                return dict[key];

            foreach (var n in p.Nums)
            {
                if (n > number || n > maxp)
                    break;
                tmp += Count(dict, p, number - n, n);
            }
            dict.Add(key, tmp);

            return tmp;
        }

        protected override string Action()
        {
            var dict = new Dictionary<Tuple<int, int>, int>();
            var p = new Prime(upper);

            p.GenerateAll();
            for (int n = 2; n < upper; n++)
            {
                if (Count(dict, p, n, n) > upper)
                    return n.ToString();
            }

            return null;
        }
    }

    /// <summary>
    /// Let p(n) represent the number of different ways in which n coins can be
    /// separated into piles. For example, five coins can separated into piles in
    /// exactly seven different ways, so p(5)=7.
    ///
    /// OOOOO
    /// OOOO O
    /// OOO OO
    /// OOO O O
    /// OO OO O
    /// OO O O O
    /// O O O O O
    ///
    /// Find the least value of n for which p(n) is divisible by one million.
    /// </summary>
    internal class Problem78 : Problem
    {
        private const int divisor = 1000000;

        public Problem78() : base(78) { }

        protected override string Action()
        {
            for (int i = 1; i < divisor; i++)
            {
                var ret = Partition.Generate(i);

                if (ret % 1000000 == 0)
                    return i.ToString();
            }

            return null;
        }
    }

    /// <summary>
    /// A common security method used for online banking is to ask the user for three
    /// random characters from a passcode. For example, if the passcode was 531278,
    /// they may ask for the 2nd, 3rd, and 5th characters; the expected reply would
    /// be: 317.
    ///
    /// [file D0079.txt], contains fifty successful login attempts.
    ///
    /// Given that the three characters are always asked for in order, analyse the file
    /// so as to determine the shortest possible secret passcode of unknown length.
    /// </summary>
    internal class Problem79 : Problem
    {
        public Problem79() : base(79) { }

        private string[] passwords;

        protected override void PreAction(string data)
        {
            passwords = (from s in data.Split('\n')
                         select s.Trim()).ToArray();
        }

        protected override string Action()
        {
            // Topological sorting of dag
            var g = new Graph<int, int>(VertexHelper.CreateIntHelper(), EdgeHelper.CreateIntHelper());
            string ret = null;

            foreach (var p in passwords)
            {
                g.AddVertex(p[0] - '0');
                g.AddVertex(p[1] - '0');
                g.AddVertex(p[2] - '0');
                g.AddEdge(p[0] - '0', p[1] - '0', 0);
                g.AddEdge(p[1] - '0', p[2] - '0', 0);
            }

            g.PathFound += (obj, e) => { ret = string.Join("", (from n in e.Path select n.ToString())); };
            g.TopologicalSort(TopologicalSortType.VertexValueMinFirst, EdgeHelper.CreateIntHelper());

            return ret;
        }
    }
}