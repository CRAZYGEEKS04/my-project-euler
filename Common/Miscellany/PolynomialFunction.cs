using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.Miscellany
{
    public static class PolynomialFunction
    {
        public static Fraction Calculate(Fraction x, IEnumerable<Fraction> coefficients)
        {
            Fraction ret = 0;
            Fraction factor = 1;

            foreach (var co in coefficients)
            {
                ret += co * factor;
                factor *= x;
            }

            return ret;
        }
    }
}