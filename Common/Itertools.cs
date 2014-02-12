using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectEuler.Common
{
    public static class Itertools
    {
        public static IEnumerable<int> Range(int start, int end, int step = 1)
        {
            if (start <= end)
            {
                for (int i = start; i <= end; i += step)
                    yield return i;
            }
            else
            {
                for (int i = start; i >= end; i -= step)
                    yield return i;
            }
        }

        public static IEnumerable<int> Repeat(int number, int counter)
        {
            for (int i = 0; i < counter; i++)
                yield return number;
        }

        public static IEnumerable<T> Concatenate<T>(params IEnumerable<T>[] enumerables)
        {
            foreach (var enumerable in enumerables)
            {
                foreach (T item in enumerable)
                    yield return item;
            }
        }

        public static IEnumerable<T[]> Permutations<T>(IEnumerable<T> enumerable, int nrepeats)
        {
            HashSet<int> existedIDs = new HashSet<int>();
            List<T> items = enumerable.ToList();
            T[] ret = new T[nrepeats];
            int[] indices = new int[nrepeats];
            int current = 0;

            while (true)
            {
                if (indices[current] < items.Count)
                {
                    ret[current] = items[indices[current]++];
                    if (current == nrepeats - 1 && !existedIDs.Contains(indices[current]))
                        yield return ret.Clone() as T[];
                    else
                    {
                        if (!existedIDs.Add(indices[current]))
                            continue;
                        indices[++current] = 0;
                    }
                }
                else
                {
                    if (current == 0)
                        break;
                    existedIDs.Remove(indices[--current]);
                }
            }
        }

        public static IEnumerable<T[]> PermutationsWithReplacement<T>(IEnumerable<T> enumerable,
            int nrepeats)
        {
            List<T> items = enumerable.ToList();
            T[] ret = new T[nrepeats];
            int[] indices = new int[nrepeats];
            int current = 0;

            while (true)
            {
                if (indices[current] < items.Count)
                {
                    ret[current] = items[indices[current]++];
                    if (current == nrepeats - 1)
                        yield return ret.Clone() as T[];
                    else
                        indices[++current] = 0;
                }
                else
                {
                    if (current == 0)
                        break;
                    current--;
                }
            }
        }

        public static IEnumerable<T[]> Combinations<T>(IEnumerable<T> enumerable, int nrepeats)
        {
            List<T> items = enumerable.ToList();
            T[] ret = new T[nrepeats];
            int[] indices = new int[nrepeats];
            int current = 0;

            while (true)
            {
                if (indices[current] < items.Count)
                {
                    ret[current] = items[indices[current]++];
                    if (current == nrepeats - 1)
                        yield return ret.Clone() as T[];
                    else
                        indices[++current] = indices[current - 1];
                }
                else
                {
                    if (current == 0)
                        break;
                    current--;
                }
            }
        }

        public static IEnumerable<T[]> Combinations<T>(IEnumerable<T> enumerable, int nrepeats, Func<T[], int, bool> predicate)
        {
            List<T> items = enumerable.ToList();
            T[] ret = new T[nrepeats];
            int[] indices = new int[nrepeats];
            int current = 0;

            while (true)
            {
                if (indices[current] < items.Count)
                {
                    ret[current] = items[indices[current]++];
                    if (!predicate(ret, current + 1))
                        continue;

                    if (current == nrepeats - 1)
                        yield return ret.Clone() as T[];
                    else
                        indices[++current] = indices[current - 1];
                }
                else
                {
                    if (current == 0)
                        break;
                    current--;
                }
            }
        }

        public static IEnumerable<T[]> CombinationsWithReplacement<T>(IEnumerable<T> enumerable,
            int nrepeats)
        {
            List<T> items = enumerable.ToList();
            T[] ret = new T[nrepeats];
            int[] indices = new int[nrepeats];
            int current = 0;

            while (true)
            {
                if (indices[current] < items.Count)
                {
                    ret[current] = items[indices[current]++];
                    if (current == nrepeats - 1)
                        yield return ret.Clone() as T[];
                    else
                        indices[++current] = indices[current - 1] - 1;
                }
                else
                {
                    if (current == 0)
                        break;
                    current--;
                }
            }
        }

        public static IEnumerable<T[]> Product<T>(params IEnumerable<T>[] enumerables)
        {
            List<T>[] items = (from enumerable in enumerables
                               select enumerable.ToList()).ToArray();
            T[] ret = new T[enumerables.Length];
            int[] indices = new int[enumerables.Length];
            int current = 0;

            while (true)
            {
                if (indices[current] < items[current].Count)
                {
                    ret[current] = items[current][indices[current]++];
                    if (current == enumerables.Length - 1)
                        yield return ret.Clone() as T[];
                    else
                        indices[++current] = 0;
                }
                else
                {
                    if (current == 0)
                        break;
                    current--;
                }
            }
        }
    }
}