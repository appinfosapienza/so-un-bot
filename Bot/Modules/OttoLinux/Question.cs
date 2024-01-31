using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SoUnBot.Modules.OttoLinux
{
    public class Question
    {
        [JsonProperty("quest")]
        public String Quest { get; set; }
        [JsonProperty("answers")]
        public List<string> Answers { get; }
        [JsonProperty("correct")]
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
