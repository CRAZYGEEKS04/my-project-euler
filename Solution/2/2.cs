using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using ProjectEuler.Common;

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
}