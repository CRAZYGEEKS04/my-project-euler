using System.Collections.Generic;
using System.Numerics;

namespace ProjectEuler.Common.Miscellany
{
    public class Fibonacci
    {
        private Dictionary<int, BigInteger> values;
        private BigInteger n1, n2, upper;

        public BigInteger this[int index]
        {
            get
            {
                // http://eli.thegreenplace.net/2009/03/01/project-euler-problem-104/
                if (values.ContainsKey(index))
                    return values[index];

                if (index % 2 == 0)
                {
                    var fn = this[index / 2];
                    var fn1 = this[index / 2 - 1];

                    values.Add(index, fn * (2 * fn1 + fn));
                }
                else
                {
                    var fn = this[index / 2 + 1];
                    var fn1 = this[index / 2];

                    values.Add(index, fn * fn + fn1 * fn1);
                }

                return values[index];
            }
        }

        public Fibonacci(BigInteger n1, BigInteger n2, BigInteger upper)
        {
            this.n1 = n1;
            this.n2 = n2;
            this.upper = upper;
            values = new Dictionary<int, BigInteger>();
            values.Add(1, n1);
            values.Add(2, n2);
        }

        public IEnumerator<BigInteger> GetEnumerator()
        {
            BigInteger a = n1, b = n2, c;

            while (a < upper || upper == 0)
            {
                yield return a;
                c = a + b;
                a = b;
                b = c;
            }
        }
    }
}