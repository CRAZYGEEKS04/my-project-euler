using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectEuler.Common.GLPK
{
    public static class LinealProgramming
    {
        private const string InputFile = @"glpkin.txt";
        private const string OutputFile = @"glpkout.txt";

        public static string[] Solve(string input, int nVariables)
        {
            Process process;
            string result;
            string[] ret;

            if (File.Exists(InputFile))
                File.Delete(InputFile);
            if (File.Exists(OutputFile))
                File.Delete(OutputFile);

            using (var file = File.CreateText(InputFile))
                file.WriteLine(input);
            process = Process.Start(Path.Combine("GLPK", "glpsol.exe"), "-m " + InputFile + " -w " + OutputFile);
            process.WaitForExit();
            using (var file = File.OpenText(OutputFile))
                result = file.ReadToEnd();
            if (File.Exists(InputFile))
                File.Delete(InputFile);
            if (File.Exists(OutputFile))
                File.Delete(OutputFile);

            ret = result.Split('\n').Select(it => it.Trim()).Where(it => !string.IsNullOrWhiteSpace(it)).ToArray();

            return ret.Skip(ret.Length - nVariables).ToArray();
        }
    }
}