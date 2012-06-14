using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;

namespace ProjectEuler.Solution
{
    public abstract class Problem
    {
        private static ResourceManager rm;
        private static List<string> answers;
        private static List<string> questions;

        static Problem()
        {
            string s = Encoding.UTF8.GetString(Properties.Resources.Questions);
            int pos = 0;

            rm = Properties.Resources.ResourceManager;
            answers = (from answer in Properties.Resources.Answers.Split('\n')
                       select answer.Trim()).ToList();

            questions = new List<string>();
            foreach (Match match in Regex.Matches(s, "=========="))
            {
                questions.Add(s.Substring(pos, match.Index - pos).Trim());
                pos = match.Index + match.Length;
            }
            questions.Add(s.Substring(pos).Trim());
        }

        protected abstract string Action();

        protected virtual void PreAction(string data) { }

        protected Problem(int id)
        {
            ID = id;
            Ticks = 0;
            Answer = null;
        }

        public int ID { get; private set; }

        public long Ticks { get; private set; }

        public string Answer { get; private set; }

        public bool IsCorrect
        {
            get
            {
                if (ID >= answers.Count)
                    return false;
                return (Answer == answers[ID]);
            }
        }

        public string Question
        {
            get
            {
                if (ID >= questions.Count)
                    return "Problem Description Missing";
                return questions[ID];
            }
        }

        public void Solve()
        {
            string data = rm.GetString(string.Format("D{0:0000}", ID));
            long start;

            PreAction(data);
            start = DateTime.Now.Ticks;
            Answer = Action();
            Ticks = DateTime.Now.Ticks - start;
        }

        public sealed override string ToString()
        {
            return string.Format("Problem {0}", ID);
        }
    }
}