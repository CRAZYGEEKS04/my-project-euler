using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.Miscellany
{
    public class SmallContinuedFraction
    {
        public static SmallContinuedFraction CreateFromSquareRoot(int number)
        {
            var loop = new List<long>();
            long start = Misc.Sqrt(number);
            long left = -start, denominator = 1, tmp = 0;

            if (number == start * start)
                return new SmallContinuedFraction(new long[] { start }, new List<long>());

            do
            {
                left *= -1;
                denominator = (number - left * left) / denominator;
                tmp = (left + start) / denominator;
                left -= tmp * denominator;
                loop.Add(tmp);
            } while (denominator != 1 || left != -start);

            return new SmallContinuedFraction(new long[] { start }, loop);
        }

        public List<long> Start { get; private set; }

        public List<long> Loop { get; private set; }

        public SmallContinuedFraction(IEnumerable<long> start, IEnumerable<long> loop)
        {
            Start = start.ToList();
            Loop = loop.ToList();
        }

        public SmallFraction GetFraction(int ith)
        {
            if (ith < Start.Count)
            {
                var ret = new SmallFraction(Start[ith], 1);
                for (int i = ith - 1; i >= 0; i--)
                    ret = Start[i] + 1 / ret;

                return ret;
            }
            else
            {
                var left = ith - Start.Count;
                var ret = new SmallFraction(Loop[left % Loop.Count], 1);

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