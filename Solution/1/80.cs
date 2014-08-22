using ProjectEuler.Common;
using ProjectEuler.Common.GLPK;
using ProjectEuler.Common.Graph;
using ProjectEuler.Common.Miscellany;
using ProjectEuler.Common.Partition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// For any integer n, consider the three functions
    ///
    /// f(1,n)(x,y,z) = x^(n+1) + y^(n+1) - z^(n+1)
    /// f(2,n)(x,y,z) = (xy + yz + zx)*(x^(n-1) + y^(n-1) - z^(n-1))
    /// f(3,n)(x,y,z) = xyz*(x^(n-2) + y^(n-2) - z^(n-2))
    ///
    /// and their combination
    ///
    /// fn(x,y,z) = f(1,n)(x,y,z) + f(2,n)(x,y,z) - f(3,n)(x,y,z)
    ///
    /// We call (x,y,z) a golden triple of order k if x, y, and z are all rational
    /// numbers of the form a / b with 0 < a < b <= k and there is (at least) one
    /// integer n, so that fn(x,y,z) = 0.
    ///
    /// Let s(x,y,z) = x + y + z.
    /// Let t = u / v be the sum of all distinct s(x,y,z) for all golden triples
    /// (x,y,z) of order 35.
    /// All the s(x,y,z) and t must be in reduced form.
    ///
    /// Find u + v.
    /// </summary>
    internal class Problem180 : Problem
    {
        private const int k = 35;

        public Problem180() : base(180) { }

        protected override string Action()
        {
            var sxyz = new HashSet<Fraction>();
            var t = new Fraction(0, 1);

            /**
             * fn(x,y,z) = x^(n-2)(xxx+xxy+xyz+xxz-xyz) + y^(n-2)(yyy+xyy+yyz+xyz-xyz) - z^(n-2)(zzz+xyz+yzz+xzz-xyz)
             *           = x^n(x+y+z) + y^n(x+y+z) - z^n(x+y+z)
             *           = (x+y+z)(x^n+y^n-z^n) = 0
             * x, y, z > 0, so x^n+y^n-z^n = 0
             *
             * According to Fermat, x^n+y^n=z^n has no solutions when n > 2, also valid for rational numbers
             * possible values for n are:
             * when n = 1, z = x + y
             * when n = 2, z = sqrt(x^2 + y^2)
             * when n = -1, yz+xz = xy, z = xy/(x+y)
             * when n = -2, yyzz+xxzz = xxyy, z = xy / sqrt(x^2 + y^2)
             */
            foreach (var tx in Itertools.Combinations(Itertools.Range(1, k), 2))
            {
                var x = new Fraction(tx[0], tx[1]);

                foreach (var ty in Itertools.Combinations(Itertools.Range(1, k), 2))
                {
                    var y = new Fraction(ty[0], ty[1]);
                    var z = x + y;

                    if (z.Denominator > z.Numerator && z.Denominator <= k)
                        sxyz.Add(x + y + z);
                    z = x * y / z;
                    if (z.Denominator > z.Numerator && z.Denominator <= k)
                        sxyz.Add(x + y + z);

                    z = x * x + y * y;
                    if (Misc.IsPerfectSquare(z.Numerator) && Misc.IsPerfectSquare(z.Denominator))
                    {
                        z = new Fraction(Misc.Sqrt(z.Numerator), Misc.Sqrt(z.Denominator));
                        if (z.Denominator > z.Numerator && z.Denominator <= k)
                            sxyz.Add(x + y + z);
                        z = x * y / z;
                        if (z.Denominator > z.Numerator && z.Denominator <= k)
                            sxyz.Add(x + y + z);
                    }
                }
            }

            foreach (var f in sxyz)
                t += f;

            return (t.Numerator + t.Denominator).ToString();
        }
    }

    /// <summary>
    /// Having three black objects B and one white object W they can be grouped in 7
    /// ways like this:
    ///
    /// (BBBW) (B,BBW) (B,B,BW) (B,B,B,W) (B,BB,W) (BBB,W) (BB,BW)
    ///
    /// In how many ways can sixty black objects B and forty white objects W be thus
    /// grouped?
    /// </summary>
    internal class Problem181 : Problem
    {
        private const int totalblack = 60;
        private const int totalwhite = 40;

        public Problem181() : base(181) { }

        private string GetCompositeIDX(int black, int white, int maxblack, int maxwhite)
        {
            return string.Join("|", new int[] { black, white, maxblack, maxwhite });
        }

        private long CountComposite(Dictionary<string, long> dict, int black, int white, int maxblack, int maxwhite)
        {
            if (black == 0 && white == 0)
                return 1;

            var key = GetCompositeIDX(black, white, maxblack, maxwhite);
            long counter = 0;

            if (dict.ContainsKey(key))
                return dict[key];

            // create a new composite block, sort composite blocks by B1W1, B1W2, ..., B2W1, B2W2, ...
            if (maxblack <= black)
            {
                for (int w = Math.Min(white, maxwhite); w > 0; w--)
                    counter += CountComposite(dict, black - maxblack, white - w, maxblack, w);
            }
            for (int b = Math.Min(black, maxblack - 1); b > 0; b--)
            {
                for (int w = Math.Min(white, totalwhite); w > 0; w--)
                    counter += CountComposite(dict, black - b, white - w, b, w);
            }
            // no more composite parts exist
            counter += Partition.Generate(black) * Partition.Generate(white);

            dict.Add(key, counter);

            return counter;
        }

        protected override string Action()
        {
            var dict = new Dictionary<string, long>();
            long counter = 0;

            counter = CountComposite(dict, totalblack, totalwhite, totalblack, totalwhite);

            return counter.ToString();
        }
    }

    /// <summary>
    /// The RSA encryption is based on the following procedure:
    ///
    /// Generate two distinct primes p and q.
    /// Compute n=pq and φ=(p-1)(q-1).
    /// Find an integer e, 1 < e < φ, such that gcd(e,φ)=1.
    ///
    /// A message in this system is a number in the interval [0,n-1].
    /// A text to be encrypted is then somehow converted to messages (numbers in the
    /// interval [0,n-1]).
    /// To encrypt the text, for each message, m, c=m^e mod n is calculated.
    ///
    /// To decrypt the text, the following procedure is needed: calculate d such that
    /// ed=1 mod φ, then for each encrypted message, c, calculate m=c^d mod n.
    ///
    /// There exist values of e and m such that m^e mod n=m.
    /// We call messages m for which m^e mod n=m unconcealed messages.
    ///
    /// An issue when choosing e is that there should not be too many unconcealed
    /// messages.
    /// For instance, let p=19 and q=37.
    /// Then n=19*37=703 and φ=18*36=648.
    /// If we choose e=181, then, although gcd(181,648)=1 it turns out that all
    /// possible messages
    /// m (0 <= m <= n-1) are unconcealed when calculating m^e mod n.
    /// For any valid choice of e there exist some unconcealed messages.
    /// It's important that the number of unconcealed messages is at a minimum.
    ///
    /// Choose p=1009 and q=3643.
    /// Find the sum of all values of e, 1 < e < φ(1009,3643) and gcd(e,φ)=1, so that
    /// the number of unconcealed messages for this value of e is at a minimum.
    /// </summary>
    internal class Problem182 : Problem
    {
        private const int p = 1009;
        private const int q = 3643;
        private const int n = p * q;
        private const int phi = (p - 1) * (q - 1);

        public Problem182() : base(182) { }

        protected override string Action()
        {
            long sum = 0;

            /**
             * The number of unconcealed messages is given by:
             * (1 + gcd (e-1) (p-1)) * (1 + gcd (e-1) (q-1))
             * Furthermore, it is also true (page 290 of Handbook of applied cryptography)
             * that since e-1, p-1 and q-1 are all even, the number of unconcealed messages
             * is always at least 9. So if we want to make sure that we sum the values of e
             * so that the number of unconcealed messages are at a minimum:
             * (1 + gcd (e-1) (p-1)) * (1 + gcd (e-1) (q-1)) == 9
             * From this formula we can derive that in order to have 9 as a result:
             * gcd(e-1)(p-1) = gcd(e-1)(q-1) = 2
             */
            for (int e = 2; e < phi; e++)
            {
                if (Factor.GetCommonFactor(phi, e) != 1)
                    continue;
                if (Factor.GetCommonFactor(e - 1, p - 1) == 2 && Factor.GetCommonFactor(e - 1, q - 1) == 2)
                    sum += e;
            }

            return sum.ToString();
        }
    }

    /// <summary>
    /// Let N be a positive integer and let N be split into k equal parts, r = N/k, so
    /// that N = r + r + ... + r.
    /// Let P be the product of these parts, P = r*r*...*r = r^k.
    ///
    /// For example, if 11 is split into five equal parts, 11 = 2.2 + 2.2 + 2.2 + 2.2
    /// + 2.2, then P = 2.2^5 = 51.53632.
    ///
    /// Let M(N) = Pmax for a given value of N.
    ///
    /// It turns out that the maximum for N = 11 is found by splitting eleven into four
    /// equal parts which leads to Pmax = (11/4)^4; that is, M(11) = 14641/256 =
    /// 57.19140625, which is a terminating decimal.
    ///
    /// However, for N = 8 the maximum is achieved by splitting it into three equal
    /// parts, so M(8) = 512/27, which is a non-terminating decimal.
    ///
    /// Let D(N) = N if M(N) is a non-terminating decimal and D(N) = -N if M(N) is a
    /// terminating decimal.
    ///
    /// For example, Σ(D(N)) for 5 <= N <= 100 is 2438.
    ///
    /// Find Σ(D(N)) for 5 <= N <= 10000.
    /// </summary>
    internal class Problem183 : Problem
    {
        private const int upper = 10000;

        public Problem183() : base(183) { }

        protected override string Action()
        {
            var ktok = new double[upper + 1];
            int k = 1, n = 5, sum = 0, tmp;

            /**
             * P(N,k) = N^k / k^k, P(N,3)>P(N,2)
             * P(N,k+1)/P(N,k) = N * k^k/(k+1)^(k+1), calculate max k when P(N,k+1)/P(N,k)>1
             * calculate k^k/(k+1)^(k+1)
             */
            for (int i = 1; i <= upper; i++)
                ktok[i] = Math.Pow((double)i / (i + 1), i) / (i + 1);
            while (n <= upper)
            {
                while (n * ktok[k] >= 1)
                    k++;
                tmp = k / Factor.GetCommonFactor(n, k);
                while (tmp % 2 == 0)
                    tmp /= 2;
                while (tmp % 5 == 0)
                    tmp /= 5;
                if (tmp == 1)
                    sum -= n;
                else
                    sum += n;
                n++;
            }

            return sum.ToString();
        }
    }

    /// <summary>
    /// Consider the set Ir of points (x,y) with integer co-ordinates in the interior
    /// of the circle with radius r, centered at the origin, i.e. x^2 + y^2 < r2.
    ///
    /// For a radius of 2, I2 contains the nine points (0,0), (1,0), (1,1), (0,1),
    /// (-1,1), (-1,0), (-1,-1), (0,-1) and (1,-1). There are eight triangles having
    /// all three vertices in I2 which contain the origin in the interior. Two of them
    /// are shown below, the others are obtained from these by rotation.
    ///
    /// For a radius of 3, there are 360 triangles containing the origin in the
    /// interior and having all vertices in I3 and for I5 the number is 10600.
    ///
    /// How many triangles are there containing the origin in the interior and having
    /// all three vertices in I105?
    /// </summary>
    internal class Problem184 : Problem
    {
        private const int radius = 105;

        public Problem184() : base(184) { }

        private int[] GenerateBoundary()
        {
            var bound = new int[radius];
            int x = radius, y = 0;

            while (y < radius)
            {
                while (y < radius && x * x + y * y < radius * radius)
                {
                    bound[y] = x;
                    y++;
                }
                x--;
            }

            return bound;
        }

        private List<int> SortAndCumulateByTangent(int[] bound)
        {
            var list = new SortedList<SmallFraction, int>();

            for (int y = 1; y < radius; y++)
            {
                for (int x = 1; x <= bound[y]; x++)
                {
                    var tan = new SmallFraction(y, x);

                    if (list.ContainsKey(tan))
                        list[tan]++;
                    else
                        list.Add(tan, 1);
                }
            }

            return list.Select(it => it.Value).ToList();
        }

        private long CountA1B1C3(List<int> list, List<long> clist)
        {
            long ret = 0;

            foreach (var t in Itertools.Combinations(Itertools.Range(0, list.Count - 1), 2))
                ret += list[t[0]] * list[t[1]] * (clist[t[1] - 1] - clist[t[0]]);

            return ret;
        }

        private long CountA1B2C3(List<int> list, List<long> clist)
        {
            long ret = 0;

            for (int a = 0; a < list.Count; a++)
                ret += clist[clist.Count - 1] * list[a] * (clist[clist.Count - 1] - clist[a]);

            return ret;
        }

        private long CountAxB1C3(List<int> list, List<long> clist)
        {
            long ret = 0;

            for (int b = 1; b < list.Count; b++)
                ret += (radius - 1) * list[b] * clist[b - 1];

            return ret;
        }

        private long CountAxB2C3(List<int> list, List<long> clist)
        {
            return (radius - 1) * clist[clist.Count - 1] * clist[clist.Count - 1];
        }

        private long CountAxByC3(List<int> list, List<long> clist)
        {
            return (radius - 1) * (radius - 1) * clist[clist.Count - 1];
        }

        protected override string Action()
        {
            var bound = GenerateBoundary();
            var list = SortAndCumulateByTangent(bound);
            var clist = new List<long>();
            long counter = 0;

            clist.Add(list[0]);
            for (int i = 1; i < list.Count; i++)
                clist.Add(list[i] + clist[i - 1]);

            counter += CountA1B1C3(list, clist) * 4;
            counter += CountA1B2C3(list, clist) * 4;
            counter += CountAxB1C3(list, clist) * 8;
            counter += CountAxB2C3(list, clist) * 4;
            counter += CountAxByC3(list, clist) * 4;

            return counter.ToString();
        }
    }

    /// <summary>
    /// The game Number Mind is a variant of the well known game Master Mind.
    ///
    /// Instead of coloured pegs, you have to guess a secret sequence of digits. After
    /// each guess you're only told in how many places you've guessed the correct
    /// digit. So, if the sequence was 1234 and you guessed 2036, you'd be told that
    /// you have one correct digit; however, you would NOT be told that you also have
    /// another digit in the wrong place.
    ///
    /// For instance, given the following guesses for a 5-digit secret sequence,
    ///
    /// 90342 ;2 correct
    /// 70794 ;0 correct
    /// 39458 ;2 correct
    /// 34109 ;1 correct
    /// 51545 ;2 correct
    /// 12531 ;1 correct
    ///
    /// The correct sequence 39542 is unique.
    ///
    /// Based on the following guesses,
    ///
    /// 5616185650518293 ;2 correct
    /// 3847439647293047 ;1 correct
    /// 5855462940810587 ;3 correct
    /// 9742855507068353 ;3 correct
    /// 4296849643607543 ;3 correct
    /// 3174248439465858 ;1 correct
    /// 4513559094146117 ;2 correct
    /// 7890971548908067 ;3 correct
    /// 8157356344118483 ;1 correct
    /// 2615250744386899 ;2 correct
    /// 8690095851526254 ;3 correct
    /// 6375711915077050 ;1 correct
    /// 6913859173121360 ;1 correct
    /// 6442889055042768 ;2 correct
    /// 2321386104303845 ;0 correct
    /// 2326509471271448 ;2 correct
    /// 5251583379644322 ;2 correct
    /// 1748270476758276 ;3 correct
    /// 4895722652190306 ;1 correct
    /// 3041631117224635 ;3 correct
    /// 1841236454324589 ;3 correct
    /// 2659862637316867 ;2 correct
    ///
    /// Find the unique 16-digit secret sequence.
    /// </summary>
    internal class Problem185 : Problem
    {
        private string[] guesses = new string[] { "5616185650518293", "3847439647293047", "5855462940810587",
            "9742855507068353", "4296849643607543", "3174248439465858", "4513559094146117", "7890971548908067",
            "8157356344118483", "2615250744386899", "8690095851526254", "6375711915077050", "6913859173121360",
            "6442889055042768", "2321386104303845", "2326509471271448", "5251583379644322", "1748270476758276",
            "4895722652190306", "3041631117224635", "1841236454324589", "2659862637316867", };

        private int[] corrects = new int[] { 2, 1, 3, 3, 3, 1, 2, 3, 1, 2, 3, 1, 1, 2, 0, 2, 2, 3, 1, 3, 3, 2 };

        public Problem185() : base(185) { }

        private string GenerateVariable()
        {
            var sb = new StringBuilder();

            for (int l = 0; l < guesses[0].Length; l++)
            {
                for (int d = 0; d < 10; d++)
                    sb.AppendLine(string.Format("var x{0}{1}, binary;", l, d));
            }

            return sb.ToString();
        }

        private string GenerateDigitsConstraints()
        {
            var sb = new StringBuilder();

            for (int l = 0; l < guesses[0].Length; l++)
            {
                sb.Append(string.Format("s.t. d{0} :", l));
                sb.Append(string.Join("+", (from d in Itertools.Range(0, 9) select string.Format("x{0}{1}", l, d))));
                sb.AppendLine("=1;");
            }

            return sb.ToString();
        }

        private string GenerateGuessConstraints()
        {
            var sb = new StringBuilder();

            for (int g = 0; g < guesses.Length; g++)
            {
                sb.Append(string.Format("s.t. g{0} :", g));
                sb.Append(string.Join("+", (from l in Itertools.Range(0, guesses[g].Length - 1)
                                            select string.Format("x{0}{1}", l, guesses[g][l]))));
                sb.AppendLine(string.Format("={0};", corrects[g]));
            }

            return sb.ToString();
        }

        private string GetAnswer(string[] answer)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < answer.Length; i++)
            {
                if (answer[i] == "1")
                    sb.Append(i % 10);
            }

            return sb.ToString();
        }

        protected override string Action()
        {
            var sb = new StringBuilder();

            sb.AppendLine(GenerateVariable());
            sb.AppendLine(GenerateDigitsConstraints());
            sb.AppendLine(GenerateGuessConstraints());
            sb.AppendLine("solve;");
            sb.AppendLine("end;");

            return GetAnswer(LinealProgramming.Solve(sb.ToString(), 10 * guesses[0].Length));
        }
    }

    /// <summary>
    /// Here are the records from a busy telephone system with one million users:
    ///
    /// RecNr    Caller    Called
    ///     1    200007    100053
    ///     2    600183    500439
    ///     3    600863    701497
    ///   ...       ...       ...
    ///
    /// The telephone number of the caller and the called number in record n are
    /// Caller(n) = S(2n-1) and Called(n) = S(2n) where S1,2,3,... come from the
    /// "Lagged Fibonacci Generator":
    ///
    /// For 1 <= k <= 55, Sk = [100003 - 200003k + 300007k^3] (modulo 1000000)
    ///
    /// For 56 <= k, Sk = [S(k-24) + S(k-55)] (modulo 1000000)
    ///
    /// If Caller(n) = Called(n) then the user is assumed to have misdialled and the
    /// call fails; otherwise the call is successful.
    ///
    /// From the start of the records, we say that any pair of users X and Y are
    /// friends if X calls Y or vice-versa. Similarly, X is a friend of a friend of Z
    /// if X is a friend of Y and Y is a friend of Z; and so on for longer chains.
    ///
    /// The Prime Minister's phone number is 524287. After how many successful calls,
    /// not counting misdials, will 99% of the users (including the PM) be a friend, or
    /// a friend of a friend etc., of the Prime Minister?
    /// </summary>
    internal class Problem186 : Problem
    {
        private const int modulo = 1000000;
        private const int pm = 524287;

        public Problem186() : base(186) { }

        private bool Befriend(DistjointSetNode[] array, int caller, int callee)
        {
            DisjointSet.Union(array[caller], array[callee]);

            return DisjointSet.FindSet(array[pm]).Size >= modulo * 99 / 100;
        }

        protected override string Action()
        {
            var array = new DistjointSetNode[modulo];
            int caller = 0, counter = 0;

            for (int i = 0; i < modulo; i++)
                array[i] = DisjointSet.MakeSet();
            foreach (var i in PseudoNumberGenerator.GenerateLaggedFibonacci())
            {
                if (counter % 2 == 0)
                    caller = i;
                else
                {
                    if (Befriend(array, caller, i))
                        break;
                    if (caller == i)
                        counter -= 2;
                }
                counter++;
            }

            return ((counter + 1) / 2).ToString();
        }
    }

    /// <summary>
    /// A composite is a number containing at least two prime factors. For example,
    /// 15 = 3 * 5; 9 = 3 * 3; 12 = 2 * 2 * 3.
    ///
    /// There are ten composites below thirty containing precisely two, not necessarily
    /// distinct, prime factors: 4, 6, 9, 10, 14, 15, 21, 22, 25, 26.
    ///
    /// How many composite integers, n < 10^8, have precisely two, not necessarily
    /// distinct, prime factors?
    /// </summary>
    internal class Problem187 : Problem
    {
        private const int upper = 100000000;

        public Problem187() : base(187) { }

        protected override string Action()
        {
            var p = new Prime(upper / 2);
            int counter = 0, lid, uid;

            p.GenerateAll();
            lid = 0;
            uid = p.Nums.Count - 1;
            while (lid <= uid)
            {
                while (p.Nums[lid] * p.Nums[uid] >= upper)
                    uid--;
                counter += uid - lid + 1;
                lid++;
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// The hyperexponentiation or tetration of a number a by a positive integer b,
    /// denoted by a↑↑b, is recursively defined by:
    ///
    /// a↑↑1 = a,
    /// a↑↑(k+1) = a^(a↑↑k).
    ///
    /// Thus we have e.g. 3↑↑2 = 3^3 = 27, hence 3↑↑3 = 3^27 = 7625597484987 and 3↑↑4
    /// is roughly 10^(3.6383346400240996*10^12).
    ///
    /// Find the last 8 digits of 1777↑↑1855.
    /// </summary>
    internal class Problem188 : Problem
    {
        private const int a = 1777;
        private const int b = 1855;
        private const int divisor = 100000000;

        public Problem188() : base(188) { }

        protected override string Action()
        {
            var list = new List<int>();
            long ret = a;

            /**
             * Find the number e where a^e = 1(mod divisor), gcd(a, divisor) = 1
             */
            list.Add(1);
            for (int i = 1; ; i++)
            {
                long tmp = a;

                tmp *= list[i - 1];
                tmp %= divisor;
                if (tmp == 1)
                    break;
                list.Add((int)tmp);
            }

            for (int i = 1; i < b; i++)
            {
                long pow = ret;

                pow %= list.Count;
                ret = list[(int)pow];
            }

            return ret.ToString();
        }
    }

    /// <summary>
    /// Consider the following configuration of 64 triangles:
    ///
    ///        A
    ///       AVA
    ///      AVAVA
    ///     AVAVAVA
    ///    AVAVAVAVA
    ///   AVAVAVAVAVA
    ///  AVAVAVAVAVAVA
    /// AVAVAVAVAVAVAVA
    ///
    /// We wish to colour the interior of each triangle with one of three colours: red,
    /// green or blue, so that no two neighbouring triangles have the same colour. Such
    /// a colouring shall be called valid. Here, two triangles are said to be
    /// neighbouring if they share an edge.
    /// Note: if they only share a vertex, then they are not neighbours.
    ///
    /// For example, here is a valid colouring of the above grid:
    ///        R
    ///       BGB
    ///      GRBRB
    ///     GRBGBRB
    ///    BRGRBGRBG
    ///   RGRBRGBGBRB
    ///  GBGBRGBGRGRBR
    /// BRGBRBGRGBRBGBR
    ///
    /// A colouring C' which is obtained from a colouring C by rotation or reflection
    /// is considered distinct from C unless the two are identical.
    ///
    /// How many distinct valid colourings are there for the above configuration?
    /// </summary>
    internal class Problem189 : Problem
    {
        private const int height = 8;
        private const int nColors = 3;

        public Problem189() : base(189) { }

        private string GetIDX(int level, int[] colors)
        {
            long ret = 0;

            foreach (var c in colors)
                ret = ret * nColors + c;

            return level + "|" + ret;
        }

        private long Count(Dictionary<string, long> dict, int[] upper, int[] current, int level, int pos)
        {
            string key = null;
            long ret = 0;

            if (level == height)
                return 1;
            if (pos == 0)
            {
                key = GetIDX(level, upper);
                if (dict.ContainsKey(key))
                    return dict[key];
            }

            for (int color = 0; color < nColors; color++)
            {
                if (pos != 0 && color == current[pos - 1])
                    continue;
                if (pos % 2 == 1 && color == upper[pos - 1])
                    continue;

                current[pos] = color;
                if (pos == level * 2)
                    ret += Count(dict, current, new int[level * 2 + 3], level + 1, 0);
                else
                    ret += Count(dict, upper, current, level, pos + 1);
            }

            if (key != null)
                dict.Add(key, ret);

            return ret;
        }

        protected override string Action()
        {
            var dict = new Dictionary<string, long>();

            return Count(dict, new int[0], new int[1], 0, 0).ToString();
        }
    }
}