using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.Miscellany
{
    public static class RomanNumerals
    {
        private static string[] letters = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
        private static int[] numbers = new int[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };

        public static int GetNumber(string roman)
        {
            int number = 0;
            int pos = 0;

            for (int i = 0; i < letters.Length && pos < roman.Length; i++)
            {
                while (roman.IndexOf(letters[i], pos) == pos)
                {
                    pos += letters[i].Length;
                    number += numbers[i];
                }
            }

            if (pos != roman.Length)
                throw new ArgumentException("invalid roman numberals");

            return number;
        }

        public static string GetRoman(int number)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < numbers.Length && number > 0; i++)
            {
                while (number >= numbers[i])
                {
                    number -= numbers[i];
                    sb.Append(letters[i]);
                }
            }

            return sb.ToString();
        }
    }
}