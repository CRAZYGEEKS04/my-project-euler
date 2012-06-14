using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common
{
    /// <summary>
    /// http://en.wikipedia.org/wiki/System_of_linear_equations
    /// </summary>
    public static class LinearEquation
    {
        public static Fraction[] Solve(Matrix a, IEnumerable<Fraction> b)
        {
            var ret = b.ToArray();

            if (a.Rows != a.Columns || ret.Length != a.Rows)
                throw new InvalidOperationException("invalid size");

            // Row Reduction
            for (int r = 0; r < a.Rows; r++)
            {
                if (a[r, r].Numerator != 1 || a[r, r].Denominator != 1)
                {
                    var factor = new Fraction(a[r, r].Numerator, a[r, r].Denominator);

                    for (int i = r; i < a.Rows; i++)
                        a[r, i] /= factor;
                    ret[r] /= factor;
                }

                for (int dstr = r + 1; dstr < a.Rows; dstr++)
                {
                    var factor = a[dstr, r] / a[r, r];

                    for (int c = r; c < a.Columns; c++)
                        a[dstr, c] -= factor * a[r, c];
                    ret[dstr] -= factor * ret[r];
                }
            }
            for (int r = a.Rows - 1; r >= 0; r--)
            {
                for (int dstr = r - 1; dstr >= 0; dstr--)
                {
                    var factor = a[dstr, r] / a[r, r];

                    a[dstr, r] -= factor * a[r, r];
                    ret[dstr] -= factor * ret[r];
                }
            }

            return ret;
        }
    }
}