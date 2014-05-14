using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectEuler.Common;

namespace ProjectEuler.Solution
{
    /// <summary>
    /// A square piece of paper with integer dimensions N*N is placed with a corner at
    /// the origin and two of its sides along the x- and y-axes. Then, we cut it up
    /// respecting the following rules:
    ///
    /// We only make straight cuts between two points lying on different sides of the
    /// square, and having integer coordinates.
    /// Two cuts cannot cross, but several cuts can meet at the same border point.
    /// Proceed until no more legal cuts can be made.
    ///
    /// Counting any reflections or rotations as distinct, we call C(N) the number of
    /// ways to cut an N*N square. For example, C(1) = 2 and C(2) = 30 (shown below).
    ///
    /// What is C(30) mod 10^8?
    /// </summary>
    internal class Problem270 : Problem
    {
        private const int size = 30;
        private static Modulo m = new Modulo(Misc.Pow(10, 8));

        public Problem270() : base(270) { }

        private long Calculate(Dictionary<string, long> dict, List<int> polygon)
        {
            string key = string.Join(",", polygon);
            List<int> tmp = new List<int>();
            long ret = 0, u;

            if (polygon.Count < 3)
                return 0;
            if (dict.ContainsKey(key))
                return dict[key];

            if (polygon[0] > 0 && polygon[polygon.Count - 1] > 0)
            {
                tmp.Add(0);
                tmp.AddRange(polygon);
                tmp[1]--;
                tmp[tmp.Count - 1]--;
                ret = m.Add(ret, Calculate(dict, tmp));
            }
            else if (polygon[0] > 0 && polygon[polygon.Count - 1] == 0)
            {
                tmp.Add(0);
                tmp.AddRange(polygon.Take(polygon.Count - 1));
                tmp[1]--;
                ret = m.Add(ret, Calculate(dict, tmp));
            }
            else if (polygon[0] == 0 && polygon[polygon.Count - 1] > 0)
            {
                tmp.Add(0);
                tmp.AddRange(polygon.Skip(1));
                tmp[tmp.Count - 1]--;
                ret = m.Add(ret, Calculate(dict, tmp));
            }
            else if (polygon[0] == 0 && polygon[polygon.Count - 1] == 0 && polygon.Count >= 4)
            {
                tmp.Add(0);
                tmp.AddRange(polygon.Skip(1).Take(polygon.Count - 2));
                ret = m.Add(ret, Calculate(dict, tmp));
            }
            tmp.Clear();

            if (polygon[0] == 0 && polygon[1] > 0)
            {
                tmp.AddRange(polygon);
                tmp[0] = 0;
                tmp[1]--;
                ret = m.Add(ret, Calculate(dict, tmp));
            }
            else if (polygon[0] == 0 && polygon[1] == 0 && polygon.Count >= 4)
            {
                tmp.Add(0);
                tmp.AddRange(polygon.Skip(2));
                ret = m.Add(ret, Calculate(dict, tmp));
            }
            tmp.Clear();

            for (int i = 1; i < polygon.Count - 1; i++)
            {
                for (int j = 0; j < polygon[i]; j++)
                {
                    tmp.Add(0);
                    tmp.Add(polygon[i] - j - 1);
                    tmp.AddRange(polygon.Skip(i + 1));
                    u = Calculate(dict, tmp);
                    tmp.Clear();
                    if (polygon[0] > 0)
                    {
                        tmp.Add(0);
                        tmp.AddRange(polygon.Take(i));
                        tmp[1]--;
                        tmp.Add(j);
                        ret = m.Add(ret, m.Mul(u, Calculate(dict, tmp)));
                    }
                    else if (polygon[0] == 0 && i > 1)
                    {
                        tmp.AddRange(polygon.Take(i));
                        tmp[0] = 0;
                        tmp.Add(j);
                        ret = m.Add(ret, m.Mul(u, Calculate(dict, tmp)));
                    }
                    tmp.Clear();
                }
            }

            for (int i = 1; i < polygon.Count - 2; i++)
            {
                tmp.Add(0);
                tmp.AddRange(polygon.Skip(i + 1));
                u = Calculate(dict, tmp);
                tmp.Clear();
                if (polygon[0] > 0)
                {
                    tmp.Add(0);
                    tmp.AddRange(polygon.Take(i + 1));
                    tmp[1]--;
                    ret = m.Add(ret, m.Mul(u, Calculate(dict, tmp)));
                }
                else if (polygon[0] == 0 && i > 1)
                {
                    tmp.AddRange(polygon.Take(i + 1));
                    tmp[0] = 0;
                    ret = m.Add(ret, m.Mul(u, Calculate(dict, tmp)));
                }
                tmp.Clear();
            }

            dict.Add(key, ret);

            return ret;
        }

        protected override string Action()
        {
            /**
             * http://garethrees.org/2013/06/14/euler/
             * Describe shape using number of available cut points on each side.
             * C(n) = [n-1,n-1,n-1,n-1]
             */
            var dict = new Dictionary<string, long>();
            var poly = new List<int>() { size - 1, size - 1, size - 1, size - 1 };

            // basic triangle
            dict.Add("0,0,0", 1);

            return Calculate(dict, poly).ToString();
        }
    }
}