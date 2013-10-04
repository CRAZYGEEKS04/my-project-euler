using System;
using System.Numerics;

namespace ProjectEuler.Common.Miscellany
{
    public static class EnglishNumber
    {
        private static string[] To19 = new string[] { "zero", "one", "two", "three", "four",
            "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen",
            "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };

        private static string[] Tens = new string[] { "twenty", "thirty", "forty", "fifty",
            "sixty", "seventy", "eighty", "ninety" };

        private static string[] Denom = new string[] { "", "thousand", "million", "billion",
            "trillion", "quadrillion", "quintillion", "sextillion", "septillion", "octillion",
            "nonillion", "decillion", "undecillion", "duodecillion", "tredecillion",
            "quattuordecillion", "sexdecillion", "septendecillion", "octodecillion",
            "novemdecillion", "vigintillion" };

        private static string ConvertNN(int n)
        {
            if (n < 20)
                return To19[n];

            if (n % 10 == 0)
                return Tens[n / 10 - 2];
            else
                return Tens[n / 10 - 2] + "-" + To19[n % 10];
        }

        private static string ConvertNNN(int n)
        {
            String ret = "";

            if (n / 100 != 0)
            {
                ret = To19[n / 100] + " hundred";
                if (n % 100 != 0)
                    ret = ret + " and ";
            }
            if (n % 100 != 0)
                ret = ret + ConvertNN(n % 100);

            return ret;
        }

        public static string GetWord(BigInteger n)
        {
            BigInteger dval, l, r;
            string ret = "";

            if (n < 100)
                return ConvertNN((int)n);
            if (n < 1000)
                return ConvertNNN((int)n);

            for (int i = 1; i < Denom.Length; i++)
            {
                dval = BigInteger.Pow(1000, i);
                if (dval > n)
                {
                    l = n / (dval / 1000);
                    r = n - l * dval / 1000;
                    ret = ConvertNNN((int)l) + " " + Denom[i - 1];
                    if (r > 0)
                        return ret + ", " + GetWord(r);
                    break;
                }
            }

            return ret;
        }
    }
}