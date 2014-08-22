using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;

namespace ProjectEuler.Solution
{
    public abstract class Problem
    {
        private static ResourceManager rm;
        private static List<string> answers;

        static Problem()
        {
            rm = Properties.Resources.ResourceManager;
            answers = (from answer in Properties.Resources.Answers.Split('\n')
                       select answer.Trim()).ToList();
        }

        public int ID { get; private set; }

        public long Ticks { get; private set; }

        public string QuestionUrl { get { return "http://projecteuler.net/problem=" + ID; } }

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

        protected virtual void PreAction(string data) { }

        protected virtual string Action() { return null; }

        protected Problem(int id)
        {
            ID = id;
            Ticks = 0;
            Answer = null;
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