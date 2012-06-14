using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.Miscellany
{
    public static class RepUnit
    {
        public static int GetA(int n)
        {
            int num = 1, l = 1;

            for (; num % n != 0; l++)
                num = (num % n) * 10 + 1;

            return l;
        }
    }
}