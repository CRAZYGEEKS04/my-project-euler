using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ProjectEuler.Common.Miscellany
{
    public class BouncyNumber
    {
        public static bool IsBouncyNumber(string number)
        {
            bool i = false, d = false;

            for (int n = 0; n < number.Length - 1; n++)
            {
                if (number[n + 1] > number[n])
                    d = true;
                if (number[n + 1] < number[n])
                    i = true;
                if (d && i)
                    return true;
            }

            return false;
        }

        private BigInteger[][] iNumbers;
        private BigInteger[][] dNumbers;

        private void GenerateI(int nDigits)
        {
            iNumbers[0] = new BigInteger[10];
            for (int d = 0; d < 10; d++)
                iNumbers[0][d] = 1;

            for (int n = 1; n <= nDigits; n++)
            {
                iNumbers[n] = new BigInteger[10];
                iNumbers[n][9] = 1;
                for (int d = 8; d >= 0; d--)
                    iNumbers[n][d] = iNumbers[n][d + 1] + iNumbers[n - 1][d];
            }
        }

        private void GenerateD(int nDigits)
        {
            dNumbers[0] = new BigInteger[10];
            for (int d = 0; d < 10; d++)
                dNumbers[0][d] = 1;

            for (int n = 1; n <= nDigits; n++)
            {
                dNumbers[n] = new BigInteger[10];
                dNumbers[n][0] = 1;
                for (int d = 1; d < 10; d++)
                    dNumbers[n][d] = dNumbers[n][d - 1] + dNumbers[n - 1][d];
            }
        }

        public BouncyNumber(int nDigits)
        {
            iNumbers = new BigInteger[nDigits + 1][];
            dNumbers = new BigInteger[nDigits + 1][];

            GenerateI(nDigits);
            GenerateD(nDigits);
        }

        public BigInteger CountByDigits(int nDigits)
        {
            BigInteger counter = 0;

            if (nDigits > iNumbers.Length)
                throw new ArgumentException("too much digits");

            counter += BigInteger.Pow(10, nDigits);
            counter -= iNumbers[nDigits][0];
            for (int i = nDigits; i > 0; i--)
            {
                counter -= dNumbers[i][9];
                // nn..[n,i] contains in iNumber
                counter += 10;
            }

            return counter;
        }
    }
}