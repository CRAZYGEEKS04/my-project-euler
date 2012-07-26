using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common
{
    public class Modulo
    {
        private long m;

        public Modulo(long modulo)
        {
            m = modulo;
        }

        public long Mod(long x)
        {
            return (x % m + m) % m;
        }

        public long Add(long x, long y)
        {
            return Mod(x + y);
        }

        public long Subtract(long x, long y)
        {
            return Mod(x - y);
        }

        public long Mul(long x, long y)
        {
            return Mod(x * y);
        }

        public long Pow(long x, long e)
        {
            long ret = 1;

            while (e != 0)
            {
                if ((e & 1) != 0)
                {
                    ret *= x;
                    ret %= m;
                }
                x *= x;
                x %= m;
                e >>= 1;
            }

            return ret;
        }
    }
}