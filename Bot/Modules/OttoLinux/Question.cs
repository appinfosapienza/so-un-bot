using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoUnBot.Modules.OttoLinux
{
    public class Question
    {
        public String Quest { get; set; }
        public List<string> Answers { get; }
        public int Correct { get; private set; }

        public Question(String quest)
        {
            Quest = quest;
            Answers = new List<string>();
            Correct = 0;
        }

        public void AddAnswer(String answer, bool correct)
        {
            Answers.Add(answer);
            Correct = correct ? Answers.Count - 1 : Correct;
        }

        public void Append(String quest)
        {
            Quest += quest;
        }
    }
}
