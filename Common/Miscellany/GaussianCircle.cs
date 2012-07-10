using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.Miscellany
{
    /// <summary>
    /// http://mathworld.wolfram.com/GausssCircleProblem.html
    /// </summary>
    public static class GaussianCircle
    {
        public static long Count(long rr)
        {
            long radius = Misc.Sqrt(rr);
            long counter = 0;

            counter = radius * 4 + 1;
            for (long i = 1; i <= radius; i++)
                counter += 4 * Misc.Sqrt(rr - i * i);

            return counter;
        }
    }
}