using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.Miscellany
{
    public static class LaggedFibonacci
    {
        private const int modulo = 1000000;

        public static IEnumerable<int> Generate()
        {
            var cq = new int[55];
            int id = 54;

            for (int i = 1; i <= 55; i++)
            {
                cq[i - 1] = (int)((100003 - 200003 * i + (long)300007 * i * i * i) % modulo);
                yield return cq[i - 1];
            }
            while (true)
            {
                id = (id + 1) % 55;
                cq[id] = ((id >= 24 ? cq[id - 24] : cq[id + 31]) + cq[id]) % modulo;
                yield return cq[id];
            }
        }
    }
}