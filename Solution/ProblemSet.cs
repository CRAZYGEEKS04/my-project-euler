using System;
using System.Collections.Generic;

namespace ProjectEuler.Solution
{
    public static class ProblemSet
    {
        private static List<Problem> problems = null;

        static ProblemSet()
        {
            Type ptype = null;

            problems = new List<Problem>();
            for (int i = 1; ; i++)
            {
                ptype = Type.GetType("ProjectEuler.Solution.Problem" + i.ToString());
                if (ptype == null)
                    break;
                object tmp = Activator.CreateInstance(ptype);
                problems.Add((Problem)tmp);
            }
        }

        public static Problem Get(int id)
        {
            if (id >= problems.Count)
                return null;
            return problems[id];
        }
    }
}