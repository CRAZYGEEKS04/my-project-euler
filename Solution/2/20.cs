using ProjectEuler.Common;
using ProjectEuler.Common.Miscellany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// Let D(0) be the two-letter string "Fa". For n >= 1, derive D(n) from D(n-1) by
    /// the string-rewriting rules:
    ///
    /// "a" -> "aRbFR"
    /// "b" -> "LFaLb"
    ///
    /// Thus, D(0) = "Fa", D(1) = "FaRbFR", D(2) = "FaRbFRRLFaLbFR", and so on.
    ///
    /// These strings can be interpreted as instructions to a computer graphics
    /// program, with "F" meaning "draw forward one unit", "L" meaning "turn left 90
    /// degrees", "R" meaning "turn right 90 degrees", and "a" and "b" being ignored.
    /// The initial position of the computer cursor is (0,0), pointing up towards
    /// (0,1).
    ///
    /// Then D(n) is an exotic drawing known as the Heighway Dragon of order n. For
    /// example, D(10) is shown below; counting each "F" as one step, the highlighted
    /// spot at (18,16) is the position reached after 500 steps.
    ///
    /// What is the position of the cursor after 10^12 steps in D(50)?
    /// Give your answer in the form x,y with no spaces.
    /// </summary>
    internal class Problem220 : Problem
    {
        private class Move
        {
            public static readonly Move Forward = new Move() { Y = 1 };

            // 0->Up, 1->Right, 2->Down, 3->Left
            public int Direction { get; private set; }
            public int X { get; private set; }
            public int Y { get; private set; }

            public Move()
            {
                X = 0;
                Y = 0;
                Direction = 0;
            }

            public Move(string pattern, Move a, Move b)
                : this()
            {
                foreach (var c in pattern)
                {
                    switch (c)
                    {
                        case 'a': Go(a); break;
                        case 'b': Go(b); break;
                        case 'L': TurnLeft(); break;
                        case 'R': TurnRight(); break;
                        case 'F': Go(Move.Forward); break;
                        default: throw new ArgumentException("Invalid pattern");
                    }
                }
            }

            public void TurnLeft()
            {
                Direction = (Direction + 3) % 4;
            }

            public void TurnRight()
            {
                Direction = (Direction + 1) % 4;
            }

            public void Go(Move move)
            {
                switch (Direction)
                {
                    case 0:
                        X += move.X;
                        Y += move.Y;
                        break;
                    case 1:
                        X += move.Y;
                        Y -= move.X;
                        break;
                    case 2:
                        X -= move.X;
                        Y -= move.Y;
                        break;
                    case 3:
                        X -= move.Y;
                        Y += move.X;
                        break;
                    default:
                        throw new ArgumentException("Impossible");
                }
                Direction = (Direction + move.Direction) % 4;
            }
        }

        private const long steps = 1000000000000;
        private const int D = 50;

        public Problem220() : base(220) { }

        private Move Traverse(string pattern, List<Move> a, List<Move> b, long left, int level)
        {
            Move ret = new Move();
            long counter = ((long)1 << level) - 1;

            foreach (var c in pattern)
            {
                if (left == 0)
                    return ret;
                switch (c)
                {
                    case 'L':
                        ret.TurnLeft();
                        break;
                    case 'R':
                        ret.TurnRight();
                        break;
                    case 'F':
                        ret.Go(Move.Forward);
                        left--;
                        break;
                    case 'a':
                        if (left >= counter)
                        {
                            ret.Go(a[level]);
                            left -= counter;
                        }
                        else
                        {
                            ret.Go(Traverse("aRbFR", a, b, left, level - 1));
                            left = 0;
                        }
                        break;
                    case 'b':
                        if (left >= counter)
                        {
                            ret.Go(a[level]);
                            left -= counter;
                        }
                        else
                        {
                            ret.Go(Traverse("LFaLb", a, b, left, level - 1));
                            left = 0;
                        }
                        break;
                    default: throw new ArgumentException("Invalid pattern");
                }
            }

            if (left != 0)
                throw new ArgumentException("Impossible");

            return ret;
        }

        protected override string Action()
        {
            List<Move> a = new List<Move>(), b = new List<Move>();
            Move ret;

            // Generate move pattern for a and b
            a.Add(new Move());
            b.Add(new Move());
            for (int i = 0; i < D; i++)
            {
                a.Add(new Move("aRbFR", a[i], b[i]));
                b.Add(new Move("LFaLb", a[i], b[i]));
            }

            ret = Traverse("Fa", a, b, steps, D);

            return string.Format("{0},{1}", ret.X, ret.Y);
        }
    }

    /// <summary>
    /// We shall call a positive integer A an "Alexandrian integer", if there exist
    /// integers p, q, r such that:
    ///
    /// A = p * q * r and 1/A = 1/p + 1/q + 1/r
    ///
    /// For example, 630 is an Alexandrian integer (p = 5, q = -7, r = -18). In fact,
    /// 630 is the 6th Alexandrian integer, the first 6 Alexandrian integers being: 6,
    /// 42, 120, 156, 420 and 630.
    ///
    /// Find the 150000th Alexandrian integer.
    /// </summary>
    internal class Problem221 : Problem
    {
        private const int index = 150000;

        public Problem221() : base(221) { }

        protected override string Action()
        {
            var queue = new SortedList<long, Tuple<long, long, long>>();
            long al, a, b, c;

            /**
             * A = p*q*r = A(q*r + p*r + p*q)
             * => Find all p,q,r where p*q + p*r + q*r = 1
             *
             * assume r > 0 > p, q, r(p+q) = 1-p*q => r = (1-p*q)/(p+q)
             * => r = (p*p + 1 - p*(p+q))/(p+q)
             *
             * so p+q divides p*p+1, find all factors of p*p+1
             *
             * --- The following algorithm comes from Assato in P221 Thread ---
             *
             * If (a,b,c) is an answer, then (b, 2b-a, 2b+c) and (c, 2c-a, 2c+b) are answers too,
             * and from (1,2,3) we can generate every Alexandrian integer
             */
            queue.Add(6, new Tuple<long, long, long>(1, 2, 3));

            for (int i = 0; i < index; i++)
            {
                var triple = queue.Values[i];

                a = triple.Item2;
                b = a * 2 - triple.Item1;
                c = a * 2 + triple.Item3;
                al = a * b * c;
                if (long.MaxValue / c > a * b && !queue.ContainsKey(al))
                    queue.Add(al, new Tuple<long, long, long>(a, b, c));

                a = triple.Item3;
                b = a * 2 - triple.Item1;
                c = a * 2 + triple.Item2;
                al = a * b * c;
                if (long.MaxValue / c > a * b && !queue.ContainsKey(al))
                    queue.Add(al, new Tuple<long, long, long>(a, b, c));
            }

            return queue.Keys[index - 1].ToString();
        }
    }

    /// <summary>
    /// What is the length of the shortest pipe, of internal radius 50mm, that can
    /// fully contain 21 balls of radii 30mm, 31mm, ..., 50mm?
    ///
    /// Give your answer in micrometres (10^-6 m) rounded to the nearest integer.
    /// </summary>
    internal class Problem222 : Problem
    {
        private static int[] balls = Itertools.Range(30, 50).ToArray();

        public Problem222() : base(222) { }

        private double GetIntersection(int ab)
        {
            /**
             * Pick two balls of radii a and b, the pipe length is
             * a + b + sqrt((a+b)^2-(100-a-b)^2)
             * the intersection part is a+b-sqrt((a+b)^2-(100-a-b)^2)
             */
            return ab - Math.Sqrt(ab * ab - (100 - ab) * (100 - ab));
        }

        protected override string Action()
        {
            var intersect = Itertools.Range(1, balls.Length).Select(it => new double[balls.Length]).ToArray();
            int total = balls.Sum() * 2;
            double value = 0;

            for (int i = 0; i < balls.Length; i++)
            {
                for (int j = i + 1; j < balls.Length; j++)
                    intersect[i][j] = intersect[j][i] = GetIntersection(balls[i] + balls[j]);
            }

            // By observation from less balls, pattern 49, 47, 45, 43, ..., 31, 30, 32, ... 50 is minimal sequence
            var list = new List<int>(Itertools.Range(balls.Length % 2 == 0 ? balls.Length - 1 : balls.Length - 2, 0, 2));
            list.AddRange(Itertools.Range(0, balls.Length - 1, 2));

            for (int i = 0; i < list.Count - 1; i++)
                value += intersect[list[i]][list[i + 1]];

            return Math.Round((total - value) * 1000, 0).ToString();
        }
    }

    /// <summary>
    /// Let us call an integer sided triangle with sides a <= b <= c barely acute if
    /// the sides satisfy
    /// a^2 + b^2 = c^2 + 1.
    ///
    /// How many barely acute triangles are there with perimeter <= 25,000,000?
    /// </summary>
    internal class Problem223 : Problem
    {
        private const int length = 25000000;

        public Problem223() : base(223) { }

        protected override string Action()
        {
            var queue = new Queue<int[]>();
            var counter = 0;

            queue.Enqueue(new int[] { 1, 1, 1 });

            // 1,1,1 will not generate 1,2n,2n
            queue.Enqueue(new int[] { 1, 2, 2 });
            while (queue.Count != 0)
            {
                var tmp = queue.Dequeue();

                counter++;
                foreach (var n in TrinaryTree.GenerateNext(tmp))
                {
                    if (n.Sum() <= length)
                        queue.Enqueue(n);
                }
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// Let us call an integer sided triangle with sides a <= b <= c barely obtuse if
    /// the sides satisfy
    /// a^2 + b^2 = c^2 - 1.
    ///
    /// How many barely obtuse triangles are there with perimeter <= 75,000,000?
    /// </summary>
    internal class Problem224 : Problem
    {
        private const int length = 75000000;

        public Problem224() : base(224) { }

        protected override string Action()
        {
            var queue = new Queue<int[]>();
            var counter = 0;

            queue.Enqueue(new int[] { 2, 2, 3 });
            while (queue.Count != 0)
            {
                var tmp = queue.Dequeue();

                counter++;
                foreach (var n in TrinaryTree.GenerateNext(tmp))
                {
                    if (n.Sum() <= length)
                        queue.Enqueue(n);
                }
            }

            return counter.ToString();
        }
    }

    /// <summary>
    /// The sequence 1, 1, 1, 3, 5, 9, 17, 31, 57, 105, 193, 355, 653, 1201 ...
    /// is defined by T1 = T2 = T3 = 1 and T(n) = T(n-1) + T(n-2) + T(n-3).
    ///
    /// It can be shown that 27 does not divide any terms of this sequence.
    /// In fact, 27 is the first odd number with this property.
    ///
    /// Find the 124th odd number that does not divide any terms of the above sequence.
    /// </summary>
    internal class Problem225 : Problem
    {
        private const int index = 124;

        public Problem225() : base(225) { }

        private string GetIDX(int[] num, int idx)
        {
            return string.Join(",", num[idx % 3], num[(idx + 1) % 3], num[(idx + 2) % 3]);
        }

        private bool IsNonDivisor(int n)
        {
            var set = new HashSet<string>() { "1,1,1" };
            var num = new int[3] { 1, 1, 1 };
            int idx = 0;
            string key;

            while (true)
            {
                num[idx] = (num[idx] + num[(idx + 1) % 3] + num[(idx + 2) % 3]) % n;
                if (num[idx] == 0)
                    return false;
                idx = (idx + 1) % 3;

                key = GetIDX(num, idx);
                if (set.Contains(key))
                    return true;
                set.Add(key);
            }
        }

        protected override string Action()
        {
            int counter = 0, n;

            for (n = 3; ; n += 2)
            {
                if (IsNonDivisor(n))
                    counter++;
                if (counter == index)
                    return n.ToString();
            }
        }
    }

    /// <summary>
    /// The blancmange curve is the set of points (x,y) such that 0 <= x <= 1 and
    /// y = Sum(s(2^n*x)/(2^n)), n in [0, inf)
    /// where s(x) = the distance from x to the nearest integer.
    ///
    /// The area under the blancmange curve is equal to 1/2, shown in pink in the
    /// diagram below.
    ///
    /// Let C be the circle with centre (1/4, 1/2) and radius 1/4, shown in black in
    /// the diagram.
    ///
    /// What area under the blancmange curve is enclosed by C?
    /// Give your answer rounded to eight decimal places in the form 0.abcdefgh
    /// </summary>
    internal class Problem226 : Problem
    {
        public Problem226() : base(226) { }

        private double Gety(double x, int n)
        {
            double y = 0;
            double den = 1.0;
            double num = x;
            long ix;

            for (int i = 0; i < n; i++)
            {
                ix = (long)num;
                if (num - ix >= 0.5)
                    ix++;
                y += (double)Math.Abs(ix - num) / den;
                num *= 2.0;
                den *= 2.0;
            }
            return y;
        }

        protected override string Action()
        {
            /**
             * http://en.wikipedia.org/wiki/Blancmange_curve
             *
             * http://en.wikipedia.org/wiki/Circular_segment
             */
            double uc = 0.25 - Math.PI * (1.0 / 16) / 2;
            int n = (int)Math.Ceiling(Math.Log(10, 2) * 8);
            double step = Math.Pow(10, -8);
            double wuc = 0;
            double y, x = step, t;

            while (true)
            {
                y = Gety(x, n);

                // t is y on circle - y on curve
                t = 0.5 * (1 - Math.Sqrt(2 * x * (-2 * x + 1))) - y;
                if (t > 0)
                    wuc += t * step;
                else
                    break;
                x += step;
            }

            return (0.25 - uc + wuc).ToString("F8");
        }
    }

    /// <summary>
    /// "The Chase" is a game played with two dice and an even number of players.
    ///
    /// The players sit around a table; the game begins with two opposite players
    /// having one die each. On each turn, the two players with a die roll it.
    /// If a player rolls a 1, he passes the die to his neighbour on the left; if he
    /// rolls a 6, he passes the die to his neighbour on the right; otherwise, he keeps
    /// the die for the next turn.
    /// The game ends when one player has both dice after they have been rolled and
    /// passed; that player has then lost.
    ///
    /// In a game with 100 players, what is the expected number of turns the game
    /// lasts?
    ///
    /// Give your answer rounded to ten significant digits.
    /// </summary>
    internal class Problem227 : Problem
    {
        private const int nPlayers = 100;

        public Problem227() : base(227) { }

        private void AddCoefficients(Fraction[,] data, Fraction[] value, int n, int nn, Fraction v)
        {
            if (nn < 0)
                nn *= -1;
            if (nn > nPlayers / 2)
                nn = nPlayers - nn;

            data[n, nn] += v;
        }

        protected override string Action()
        {
            /**
             * The starting distance between two dice is 50, after every roll,
             * there is a 1/36 chance distance -= 2, a 1/36 chance distance += 2,
             * a 2/9 chance distance -=1, a 2/9 chance distance += 1
             *
             * assume the distance is n, the expected number of turns needs for game to end is x(n)
             * x(n) = (x(n-2)+1)/36 + (x(n-1)+1)*2/9 + (x(n)+1)/2 + (x(n+1)+1)*2/9 + (x(n+2)+1)/36
             * x(n)/2 - x(n-2)/36 - x(n-1)*2/9 - x(n+1)*2/9 - x(n+2)/36 = 1, n in [1, nPlayer/2]
             */
            var data = new Fraction[nPlayers / 2 + 1, nPlayers / 2 + 1];
            var value = new Fraction[nPlayers / 2 + 1];
            var list = new List<Fraction>();

            // x(0) = 0
            for (int i = 1; i <= nPlayers / 2; i++)
                data[0, i] = 0;
            data[0, 0] = 1;
            value[0] = 0;
            for (int n = 1; n <= nPlayers / 2; n++)
            {
                value[n] = 1;

                for (int i = 0; i <= nPlayers / 2; i++)
                    data[n, i] = 0;
                AddCoefficients(data, value, n, n, new Fraction(1, 2));
                AddCoefficients(data, value, n, n - 2, new Fraction(-1, 36));
                AddCoefficients(data, value, n, n - 1, new Fraction(-2, 9));
                AddCoefficients(data, value, n, n + 1, new Fraction(-2, 9));
                AddCoefficients(data, value, n, n + 2, new Fraction(-1, 36));
            }
            for (int i = 0; i <= nPlayers / 2; i++)
            {
                for (int j = 0; j <= nPlayers / 2; j++)
                    list.Add(data[i, j]);
            }

            var ret = LinearEquation.Solve(new Matrix(list, nPlayers / 2 + 1, nPlayers / 2 + 1), value)[nPlayers / 2];

            return ((double)ret.Numerator / (double)ret.Denominator).ToString("G10");
        }
    }

    /// <summary>
    /// Let Sn be the regular n-sided polygon – or shape – whose vertices vk
    /// (k = 1,2,…,n) have coordinates:
    ///
    /// xk = cos((2k-1)/n X 180°)
    /// yk = sin((2k-1)/n X 180°)
    ///
    /// Each Sn is to be interpreted as a filled shape consisting of all points on the
    /// perimeter and in the interior.
    ///
    /// The Minkowski sum, S+T, of two shapes S and T is the result of adding every
    /// point in S to every point in T, where point addition is performed
    /// coordinate-wise: (u, v) + (x, y) = (u+x, v+y).
    ///
    /// For example, the sum of S3 and S4 is the six-sided shape shown in pink below:
    ///
    /// How many sides does S1864 + S1865 + … + S1909 have?
    /// </summary>
    internal class Problem228 : Problem
    {
        private const int start = 1864;
        private const int end = 1909;

        public Problem228() : base(228) { }

        protected override string Action()
        {
            /**
             * http://en.wikipedia.org/wiki/Minkowski_addition#Two_convex_polygons_in_the_plane
             *
             * idea - sort all by polar angles. For an n-gon, the angles are 0, 1/n, 2/n, ...
             * of the way around, so this is the same as finding all unique fractions
             * j/k where k goes 1864 to 1909, j goes 0 to k-1.
             */

            var vals = new HashSet<int>();
            for (int k = start; k <= end; k++)
            {
                for (int j = 0; j < k; j++)
                {
                    int t = Factor.GetCommonFactor(j, k);
                    vals.Add((k / t) * 10000 + (j / t));
                }
            }

            return vals.Count().ToString();
        }
    }

    /// <summary>
    /// Consider the number 3600. It is very special, because
    ///
    /// 3600 = 48^2 + 36^2
    /// 3600 = 20^2 + 2 * 40^2
    /// 3600 = 30^2 + 3 * 30^2
    /// 3600 = 45^2 + 7 * 15^2
    ///
    /// Similarly, we find that 88201 = 99^2 + 280^2 = 287^2 + 2 * 54^2
    /// = 283^2 + 3 * 52^2 = 197^2 + 7 * 84^2.
    ///
    /// In 1747, Euler proved which numbers are representable as a sum of two squares.
    /// We are interested in the numbers n which admit representations of all of the
    /// following four types:
    ///
    /// n = a1^2 + b1^2
    /// n = a2^2 + 2 * b2^2
    /// n = a3^2 + 3 * b3^2
    /// n = a7^2 + 7 * b7^2,
    ///
    /// where the ak and bk are positive integers.
    ///
    /// There are 75373 such numbers that do not exceed 10^7.
    /// How many such numbers are there that do not exceed 2*10^9?
    /// </summary>
    internal class Problem229 : Problem
    {
        private const int upper = 2000000000;

        public Problem229() : base(229) { }

        private void N1(byte[] flags)
        {
            int n, a2;

            for (int a = 1; ; a++)
            {
                a2 = a * a;
                if (a2 > upper / 2)
                    break;
                for (int b = a; ; b++)
                {
                    n = a2 + b * b;
                    if (n > upper)
                        break;

                    if (n % 2 == 0)
                        flags[(n - 1) / 2] |= 0x1;
                    else
                        flags[(n - 1) / 2] |= 0x10;
                }
            }
        }

        private void N2(byte[] flags)
        {
            int n, a2;

            for (int a = 1; ; a++)
            {
                a2 = a * a;
                if (a2 >= upper)
                    break;
                for (int b = 1; ; b++)
                {
                    n = a2 + 2 * b * b;
                    if (n > upper)
                        break;

                    if (n % 2 == 0)
                        flags[(n - 1) / 2] |= 0x2;
                    else
                        flags[(n - 1) / 2] |= 0x20;
                }
            }
        }

        private void N3(byte[] flags)
        {
            int n, a2;

            for (int a = 1; ; a++)
            {
                a2 = a * a;
                if (a2 >= upper)
                    break;
                for (int b = 1; ; b++)
                {
                    n = a2 + 3 * b * b;
                    if (n > upper)
                        break;

                    if (n % 2 == 0)
                        flags[(n - 1) / 2] |= 0x4;
                    else
                        flags[(n - 1) / 2] |= 0x40;
                }
            }
        }

        private void N4(byte[] flags)
        {
            int n, a2;

            for (int a = 1; ; a++)
            {
                a2 = a * a;
                if (a2 >= upper)
                    break;
                for (int b = 1; ; b++)
                {
                    n = a2 + 7 * b * b;
                    if (n > upper)
                        break;

                    if (n % 2 == 0)
                        flags[(n - 1) / 2] |= 0x8;
                    else
                        flags[(n - 1) / 2] |= 0x80;
                }
            }
        }

        protected override string Action()
        {
            var flags = new byte[upper / 2];
            var counter = 0;

            N1(flags);
            N2(flags);
            N3(flags);
            N4(flags);

            for (int i = 0; i < upper / 2; i++)
            {
                if ((flags[i] & 0xF) == 0xF)
                    counter++;
                if ((flags[i] & 0xF0) == 0xF0)
                    counter++;
            }

            return counter.ToString();
        }
    }
}