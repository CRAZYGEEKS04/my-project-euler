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
        private const long radius = 1000000000;

        public Problem210() : base(210) { }

        protected override string Action()
        {
            long counter = 0;

            /**
             * OBC is obtuse, O(0, 0), B(x, y), C(r/4, r/4)
             * OB^2 = x^2 + y^2, OC^2 = r^2/8, BC^2 = x^2 + y^2 + r^2/8 - (x+y)r/2
             *
             * case 1: OB^2 > OC^2 + BC^2
             *   0 > r^2/4 - (x+y)r/2 => x+y > r/2
             *   All points in right side of x+y=r/2
             *
             * case 2: BC^2 > OB^2 + OC^2
             *   -(x+y)r/2 > 0 => 0 > x+y
             *   All points in left side of x+y = 0
             *
             * case 3: OC^2 > OB^2 + BC^2
             *   0 > 2x^2 - xr/2 + 2y^2 - yr/2 => (x+y)r/4 > x^2 + y^2
             *   All points inside the circle at (r/8, r/8), radius r/8*sqrt(2)
             */

            if (radius % 8 != 0)
                throw new ArgumentException("Can's use gaussian circle formula");
            // case 1, minus points where x = y
            counter += (radius + 1 + radius) * radius / 4;
            // case 2, minus points where x = y
            counter += (radius + 1 + radius) * radius / 2;
            // case 3, minus points where x = y, must not on the circle
            counter += GaussianCircle.Count(radius * radius / 32 - 1);
            // remove points at (n, n), (0, 0) and (r/4, r/4) is already excluded
            counter -= radius - 1;

            return counter.ToString();
        }
    }
}