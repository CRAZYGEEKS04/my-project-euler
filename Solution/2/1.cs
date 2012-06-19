using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectEuler.Common;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// Consider the set S(r) of points (x,y) with integer coordinates satisfying
    /// |x| + |y| <= r.
    /// Let O be the point (0, 0) and C the point (r/4, r/4).
    /// Let N(r) be the number of points B in S(r), so that the triangle OBC has an
    /// obtuse angle, i.e. the largest angle α satisfies 90° < α < 180°.
    /// So, for example, N(4) = 24 and N(8) = 100.
    /// What is N(1,000,000,000)?
    /// </summary>
    internal class Problem210 : Problem
    {
        // 12-226, 100-15944, 10000-159814790
        private const long radius = 4;

        public Problem210() : base(210) { }

        protected override string Action()
        {
            long counter = 0;

            /**
             * OBC is obtuse, O(0, 0), B(x, y), C(r/4, r/4)
             * OB^2 = x^2 + y^2, OC^2 = r^2/8, BC^2 = x^2 + y^2 + r^2/8 - (x+y)r/2
             *
             * case 1: OB^2 > OC^2 + BC^2
             *   0 > r^2/4 - (x+y)r/2
             *   x+y > r/2
             *
             * case 2: BC^2 > OB^2 + OC^2
             *   -(x+y)r/2 > 0 =>
             *   0 > x+y
             *
             * case 3: OC^2 > OB^2 + BC^2
             *   0 > 2x^2 - xr/2 + 2y^2 - yr/2 =>
             *   0 > x^2 + y^2 - (x+y)r/4
             */

            for (long x = -radius; x <= radius; x++)
            {
                long bound = radius - (x > 0 ? x : -x);
                long lower = -x, upper = radius / 2 - x;

                if (lower < -bound)
                    lower = -bound;
                if (upper > bound)
                    upper = bound;

                if (lower <= upper)
                {
                    counter += lower + bound;
                    counter += bound - upper;
                    if (lower > x || upper < x)
                        counter--;
                    for (long y = lower; y <= upper; y++)
                    {
                        if (x == y)
                            continue;
                        if (x * x + y * y - (x + y) * radius / 4 < 0)
                            counter++;
                    }
                }
                else
                {
                    counter += bound * 2 + 1;
                    if (x <= radius / 2 && x >= -radius / 2)
                        counter--;
                }
            }

            return counter.ToString();
        }
    }
}