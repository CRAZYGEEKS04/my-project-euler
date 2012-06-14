using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ProjectEuler.Common.Miscellany
{
    public class ContinuedFraction
    {
        public static ContinuedFraction CreateFromSquareRoot(BigInteger number)
        {
            var loop = new List<BigInteger>();
            BigInteger start = Misc.Sqrt(number);
            BigInteger left = -start, denominator = 1, tmp = 0;

            if (number == start * start)
                return new ContinuedFraction(new BigInteger[] { start }, new List<BigInteger>());

            do
            {
                left *= -1;
                denominator = (number - left * left) / denominator;
                tmp = (left + start) / denominator;
                left -= tmp * denominator;
                loop.Add(tmp);
            } while (denominator != 1 || left != -start);

            return new ContinuedFraction(new BigInteger[] { start }, loop);
        }

        public static ContinuedFraction CreateE(int precise)
        {
            var start = new List<BigInteger>();

            start.Add(2);
            foreach (var i in Itertools.Range(1, precise / 3))
            {
                start.Add(1);
                start.Add(i * 2);
                start.Add(1);
            }

            return new ContinuedFraction(start, new List<BigInteger>());
        }

        public List<BigInteger> Start { get; private set; }

        public List<BigInteger> Loop { get; private set; }

        public ContinuedFraction(IEnumerable<BigInteger> start, IEnumerable<BigInteger> loop)
        {
            Start = start.ToList();
            Loop = loop.ToList();
        }

        public Fraction GetFraction(int ith)
        {
            if (ith < Start.Count)
            {
                var ret = new Fraction(Start[ith], 1);
                for (int i = ith - 1; i >= 0; i--)
                    ret = Start[i] + 1 / ret;

                return ret;
            }
            else
            {
                var left = ith - Start.Count;
                var ret = new Fraction(Loop[left % Loop.Count], 1);

                for (int i = left % Loop.Count; i > 0; i--)
                    ret = Loop[i - 1] + 1 / ret;
                for (int n = 0; n < left / Loop.Count; n++)
                    for (int i = Loop.Count - 1; i >= 0; i--)
                        ret = Loop[i] + 1 / ret;
                for (int i = Start.Count - 1; i >= 0; i--)
                    ret = Start[i] + 1 / ret;

                return ret;
            }
        }
    }
}