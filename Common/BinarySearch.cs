using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common
{
    public static class BinarySearch
    {
        public static int Search<T>(IList<T> sortedList, T item)
            where T : IComparable
        {
            int lower = 0, upper = sortedList.Count - 1;
            int tmp = 0;

            while (lower <= upper)
            {
                tmp = (lower + upper) / 2;
                var ret = item.CompareTo(sortedList[tmp]);
                if (ret == 0)
                    return tmp;
                if (ret > 0)
                    lower = tmp + 1;
                else
                    upper = tmp - 1;
            }

            return -1;
        }

        public static int SearchLeft<T>(IList<T> sortedList, T item)
            where T : IComparable
        {
            int lower = 0, upper = sortedList.Count - 1;
            int tmp = 0;

            while (lower <= upper)
            {
                tmp = (lower + upper) / 2;
                var ret = item.CompareTo(sortedList[tmp]);
                if (ret == 0)
                    return tmp;
                if (ret > 0)
                    lower = tmp + 1;
                else
                    upper = tmp - 1;
            }

            if (item.CompareTo(sortedList[tmp]) < 0)
                return tmp - 1;
            else
                return tmp;
        }

        public static int SearchRight<T>(IList<T> sortedList, T item)
            where T : IComparable
        {
            int lower = 0, upper = sortedList.Count - 1;
            int tmp = 0;

            while (lower <= upper)
            {
                tmp = (lower + upper) / 2;
                var ret = item.CompareTo(sortedList[tmp]);
                if (ret == 0)
                    return tmp;
                if (ret > 0)
                    lower = tmp + 1;
                else
                    upper = tmp - 1;
            }

            if (item.CompareTo(sortedList[tmp]) < 0)
                return tmp;
            else
                return tmp + 1;
        }
    }
}