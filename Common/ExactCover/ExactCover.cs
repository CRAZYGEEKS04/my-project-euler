using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.ExactCover
{
    /// <summary>
    /// http://en.wikipedia.org/wiki/Exact_cover
    /// </summary>
    internal static class ExactCover
    {
        private static List<int> solution;

        private static void FoundSingleSolution(object sender, DancingLinks.SolutionFoundEventArgs args)
        {
            solution = args.Solution;
            args.Terminate = true;
        }

        public static List<int> GetSingleSolution(int[][] matrix)
        {
            var dlx = new DancingLinks(matrix);

            solution = null;
            dlx.SolutionFound += FoundSingleSolution;
            dlx.Search();

            return solution;
        }
    }
}