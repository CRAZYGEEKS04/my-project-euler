using System;
using System.Collections.Generic;

namespace ProjectEuler.Common
{
    public class Prime
    {
        private int last, step, upper;

        private List<int> nums;
        private HashSet<int> s_nums;

        public int Upper { get { return upper; } }

        public List<int> Nums { get { return nums; } }

        public Prime(int upper)
        {
            last = 5;
            step = 2;
            this.upper = upper;

            nums = new List<int>(new int[] { 2, 3, 5 });
            s_nums = new HashSet<int>(new int[] { 2, 3, 5 });
        }

        public bool Contains(int n)
        {
            return s_nums.Contains(n);
        }

        public bool IsPrime(long n)
        {
            int sqrtn = (int)Math.Sqrt((double)n) + 1;

            if (sqrtn > upper)
                throw new ArgumentException("Input n is too large");
            while (sqrtn > last && GenerateNext() != 0) ;

            if (n < last)
                return s_nums.Contains((int)n);
            for (int i = 0; i < nums.Count && sqrtn > nums[i]; i++)
                if (n % nums[i] == 0)
                    return false;

            return true;
        }

        public int GenerateNext()
        {
            while ((last += step) < upper)
            {
                step = 6 - step;
                if (IsPrime(last))
                {
                    nums.Add(last);
                    s_nums.Add(last);
                    return last;
                }
            }

            return 0;
        }

        public IEnumerator<int> GetEnumerator()
        {
            foreach (int n in nums)
                yield return n;
            while (GenerateNext() != 0)
                yield return last;
        }

        public void GenerateAll()
        {
            byte[] flags = new byte[upper / 2];

            for (int i = 1; i < nums.Count; i++)
                for (int j = nums[i] * 3; j < upper; j += nums[i] * 2)
                    flags[j / 2] = 1;

            for (int i = last + 2; i < upper; i += step)
            {
                step = 6 - step;
                if (flags[i / 2] != 0)
                    continue;
                for (int j = i * 3; j < upper; j += i * 2)
                    flags[j / 2] = 1;
            }

            for (int i = last + 2; i < upper; i += 2)
            {
                if (flags[i / 2] == 0)
                {
                    nums.Add(i);
                    s_nums.Add(i);
                }
            }
            last = upper;
        }
    }
}