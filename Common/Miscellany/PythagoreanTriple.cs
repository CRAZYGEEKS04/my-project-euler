using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.Miscellany
{
    /// <summary>
    /// http://en.wikipedia.org/wiki/Formulas_for_generating_Pythagorean_triples
    /// </summary>
    public static class PythagoreanTriple
    {
        public static IEnumerable<int[]> GeneratePrimitive(int maxPerimeter)
        {
            var queue = new Queue<int[]>();

            queue.Enqueue(new int[] { 3, 4, 5 });
            while (queue.Count != 0)
            {
                var tmp = queue.Dequeue();

                foreach (var n in TrinaryTree.GenerateNext(tmp))
                {
                    if (n.Sum() <= maxPerimeter)
                        queue.Enqueue(n);
                }
                yield return tmp;
            }
        }

        public static IEnumerable<int[]> GeneratePrimitive2(int maxPerimeter)
        {
            // http://en.wikipedia.org/wiki/Pythagorean_triple
            var ret = new int[3];

            for (int m = 1; m <= (int)Misc.Sqrt(maxPerimeter); m++)
            {
                for (int n = m % 2 == 0 ? 1 : 2; n < m; n += 2)
                {
                    if (Factor.GetCommonFactor(m, n) != 1)
                        continue;

                    ret[0] = m * m - n * n;
                    ret[1] = 2 * m * n;
                    ret[2] = m * m + n * n;

                    if (ret.Sum() > maxPerimeter)
                        break;
                    yield return ret;
                }
            }
        }

        public static IEnumerable<int[]> GeneratePrimitive3(int maxPerimeter)
        {
            /**
             * Given the integers u and v, (see: http://www.math.rutgers.edu/~erowland/pythagoreantriples.html)
             * a = u^2 + 2uv, b = 2v^2 + 2uv, c = u^2 + 2v^2 + 2uv
             *
             * Example: For u = 3 and v = 5, a = 39, b = 80, c = 89.
             * For the resulting triple to be primitive, u and v must be co-prime and u must be odd.
             * A particularly elegant version of this method is to calculate
             * g = u^2, h = 2v^2, i = 2uv
             * Then
             * a = g + i, b = h + i, c = g + h + i
             */

            var ret = new int[3];

            for (int u = 1; u <= Misc.Sqrt(maxPerimeter); u += 2)
            {
                for (int v = 1; ; v++)
                {
                    if (Factor.GetCommonFactor(u, v) != 1)
                        continue;

                    int g = u * u, h = 2 * v * v, i = 2 * u * v;

                    ret[0] = g + i;

                    ret[1] = h + i;

                    ret[2] = g + h + i;

                    if (ret.Sum() > maxPerimeter)
                        break;

                    yield return ret;
                }
            }
        }
    }
}