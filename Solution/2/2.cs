﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using ProjectEuler.Common;
using ProjectEuler.Common.Miscellany;

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
}