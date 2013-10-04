using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.Partition
{
    /// <summary>
    /// http://en.wikipedia.org/wiki/Partition_%28number_theory%29
    /// </summary>
    public static class Partition
    {
        public static List<long> Results { get; private set; }

        private static List<int> magicNumbers;

        static Partition()
        {
            Results = new List<long>();
            magicNumbers = new List<int>();
            Results.Add(1);
            Results.Add(1);
        }

        private static void GeneratePentagonalNumbers(int number)
        {
            int tmp, n = magicNumbers.Count / 2 + 1;

            while ((tmp = n * (3 * n - 1) / 2) <= number)
            {
                magicNumbers.Add(tmp);
                magicNumbers.Add(n * (3 * n + 1) / 2);
                n++;
            }
        }

        public static long Generate(int number)
        {
            if (number < Results.Count)
                return Results[number];

            GeneratePentagonalNumbers(number);
            for (int i = Results.Count; i <= number; i++)
            {
                long counter = 0;
                var seq = 0;

                foreach (var id in magicNumbers)
                {
                    if (i < id)
                        break;
                    if (seq < 2)
                        counter += Results[i - id];
                    else
                        counter -= Results[i - id];

                    seq = (seq + 1) % 4;
                }
                Results.Add(counter);
            }

            return Results[number];
        }
    }
}