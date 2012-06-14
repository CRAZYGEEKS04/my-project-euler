using System.Numerics;

namespace ProjectEuler.Common
{
    public static class Probability
    {
        public static long CountPermutations(int n, int c)
        {
            long ret = 1;

            if (c > n)
                return 0;
            for (int i = n; i > n - c; i--)
                ret *= i;

            return ret;
        }

        public static long CountCombinations(int n, int c)
        {
            long ret = 1;

            if (c > n)
                return 0;
            if (c > n / 2)
                c = n - c;
            for (int i = n; i > n - c; i--)
                ret *= i;
            for (int i = c; i > 0; i--)
                ret /= i;

            return ret;
        }

        public static BigInteger CountPermutations(BigInteger n, BigInteger c)
        {
            BigInteger ret = 1;

            if (c > n)
                return 0;
            for (BigInteger i = n; i > n - c; i--)
                ret *= i;

            return ret;
        }

        public static BigInteger CountCombinations(BigInteger n, BigInteger c)
        {
            BigInteger ret = 1;

            if (c > n)
                return 0;
            if (c > n / 2)
                c = n - c;
            for (BigInteger i = n; i > n - c; i--)
                ret *= i;
            for (BigInteger i = c; i > 0; i--)
                ret /= i;

            return ret;
        }
    }
}