using System.Collections;
using Newtonsoft.Json;
using SoUnBot.AccessControl;
using SoUnBot.ModuleLoader;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SoUnBot.Modules.OttoLinux
{
    public class BotGame : IModule
    {
        private AccessManager _accessManager;
        private List<Question> _questions;
        private string _questionsPath;
        private string _name;
        private bool _lock;

        private Dictionary<long, OttoScore> _scores;
        private Dictionary<long, Question> _playingQuestions;
        private Dictionary<long, List<int>> _playedQuestions;
        private Dictionary<Question, OttoScore> _questionStats;

        private static Random _rng = new Random();

        public BotGame(AccessManager accessManager, string name, string path, bool locke, int version = 1)
        {
            _accessManager = accessManager;
            _questionsPath = path;
            _name = name;
            _lock = locke;

            _questions = new List<Question>();
            _scores = new Dictionary<long, OttoScore>();
            _playingQuestions = new Dictionary<long, Question>();
            _questionStats = new Dictionary<Question, OttoScore>();
            _playedQuestions = new Dictionary<long, List<int>>();

            if (version == 2) LoadQuestionsV2();
            else if (version == 3) LoadQuestionsJSON();
            else LoadQuestions();
        }
        public BotGame(AccessManager accessManager)
        {
            _accessManager = accessManager;
            _questions = new List<Question>();
            _scores = new Dictionary<long, OttoScore>();
            _playingQuestions = new Dictionary<long, Question>();
            _questionStats = new Dictionary<Question, OttoScore>();
            _playedQuestions = new Dictionary<long, List<int>>();

            LoadQuestions();
        }
        private void LoadQuestions()
        {
            var lines = System.IO.File.ReadAllLines(_questionsPath);
            Question cur = null;
            var preEmpty = true;
            foreach (var line in lines)
            {
                if (line.Equals("")) preEmpty = true;
                else preEmpty = false;
                if (line.StartsWith(">")) cur.AddAnswer(line.Substring(2), false);
                else if (line.StartsWith("v")) cur.AddAnswer(line.Substring(2), true);
                else
                {
                    if (!preEmpty && cur != null)
                    {
                        cur.Append("\n" + line);
                        continue;
                    }
                    if (cur != null) _questions.Add(cur);
                    cur = new Question(line);
                }
            }
            _questions.Add(cur);
            SanitizeQuestions();
        }

        private void LoadQuestionsV2()
        {
            var questions = System.IO.Directory.GetFileSystemEntries(_questionsPath);
            foreach (var questPath in questions)
            {
                var quest = System.IO.File.ReadAllText(questPath + "/quest.txt");

                if (quest.StartsWith("img=")) quest = quest.Insert(quest.IndexOf("\n") + 1, questPath.Split(new char[] { '/', '\\' }).Last() + ". ");
                else quest = questPath.Split(new char[] { '/', '\\' }).Last() + ". " + quest;

                Question cur = new Question(quest);

                var shuffledAnsPaths = Directory.GetFiles(questPath).OrderBy(a => _rng.Next()).ToArray();

                foreach (var ansPath in shuffledAnsPaths)
                {
                    if (ansPath.EndsWith("quest.txt")) continue;
                    if (ansPath.EndsWith(".png")) continue;

                    var ans = System.IO.File.ReadAllText(ansPath);
                    if (ansPath.EndsWith("correct.txt")) cur.AddAnswer(ans, true);
                    else cur.AddAnswer(ans, false);
                }
                _questions.Add(cur);
            }
            SanitizeQuestions();
        }

        private void LoadQuestionsJSON()
        {
            var json = System.IO.File.ReadAllText(_questionsPath);
            var quests = JsonConvert.DeserializeObject<Question[]>(json);
            if (quests != null) _questions = quests.ToList();
        }

        private void SanitizeQuestions()
        {
            var invalidQuestions = new List<Question>();
            foreach (var qst in _questions)
            {
                while (qst.Quest.StartsWith("\n")) qst.Quest = qst.Quest.Substring(1);
                for (int i = 0; i < qst.Answers.Count; i++)
                {
                    while (qst.Answers[i].StartsWith("\n")) qst.Answers[i] = qst.Answers[i].Substring(1);
                }
                if (qst.Quest == "")
                {
                    invalidQuestions.Add(qst);
                    Console.WriteLine("an empty question was found, skipping it");
                }
                else if(qst.Answers.Count == 0)
                {
                    invalidQuestions.Add(qst);
                    Console.WriteLine($"The following question: {qst.Quest} \nhas no answers, skipping it");
                }
            }
            _questions = _questions.Except(invalidQuestions).ToList();
        }

        public Question PickRandomQuestion(long player, ITelegramBotClient botClient)
        {
            //WebRequest w = WebRequest.Create($"https://www.random.org/integers/?num=1&min=1&max={_questions.Count - 1 }&col=1&base=10&format=plain&rnd=new");
            //w.Method = "GET";

            //var number = int.Parse(new StreamReader(w.GetResponse().GetResponseStream()).ReadToEnd());
            var number = _rng.Next(0, _questions.Count - 1);
            while (_questions[number].Quest == "")
            {
                number = _rng.Next(0, _questions.Count - 1);
            }

            if (!_playedQuestions.ContainsKey(player)) _playedQuestions.Add(player, new List<int>());

            if (_playedQuestions[player].Count >= _questions.Count)
            {
                _playedQuestions[player].Clear();
                botClient.SendTextMessageAsync(
                    chatId: player,
                    text: $"🥳🥳🥳 Congratulazioni! Hai risposto a tutte le {_questions.Count} domande!"
                    );
            }

            while (_playedQuestions[player].Contains(number))
            {
                if (number < _questions.Count - 1) number++;
                else number = 0;
            }
            _playedQuestions[player].Add(number);

            return _questions[number];
        }

        public string Cmd()
        {
            return GetName();
        }

        async void IModule.ProcessUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var uid = update.Message.From.Id;

            if (_lock)
            {
                if (!_accessManager.CheckPermission(update.Message.From, Cmd(), botClient)) return;
            }
            else
            {
                if (!_accessManager.CheckPermission(uid, Cmd()))
                {
                    _accessManager.GrantPermission(uid, Cmd());
                    await botClient.SendTextMessageAsync(
                    chatId: _accessManager.AdminId,
                    text: $"ACM: { update.Message.From.Id }\nL'utente { update.Message.From.FirstName } { update.Message.From.LastName } @{ update.Message.From.Username }\nHa iniziato a usare il bot."
                    );
                }
            }

            //if (!scores.Keys.Contains(uid))
            //{
            //    await botClient.SendTextMessageAsync(
            //    chatId: _acm.AdminId,
            //    text: $"ACM: { update.Message.From.Id }\nL'utente { update.Message.From.FirstName } { update.Message.From.LastName } @{ update.Message.From.Username }\nHa iniziato a usare il bot."
            //    );
            //}

            if (update.Type != UpdateType.Message)
                return;
            if (update.Message!.Type != MessageType.Text)
                return;

            if (update.Message.Text.StartsWith("/qsc"))
            {
                int number = 1;
                if (update.Message.Text.Length > 4) int.TryParse(update.Message.Text.Substring(5), out number);

                var mostCorrect = _questionStats.GroupBy(e => e.Value.Correct).ToDictionary(e => e.Key, t => t.Select(r => r.Key).ToArray());

                var c = 0;
                foreach (var item in mostCorrect.Keys.OrderByDescending(i => i))
                {
                    if (c == number) break;
                    var msg = mostCorrect[item].Select(q => q.Quest.Substring(0, 30)).Aggregate((a, b) => a + "\n\n" + b);

                    await botClient.SendTextMessageAsync(
                    chatId: uid,
                    text: $"✅ Risposte indovinate {item} volte:\n{msg}");

                    c++;
                }

                return;
            }

            if (update.Message.Text.StartsWith("/qsw"))
            {
                int number = 1;
                if (update.Message.Text.Length > 4) int.TryParse(update.Message.Text.Substring(5), out number);

                var mostWrong = _questionStats.GroupBy(e => e.Value.Wrong).ToDictionary(e => e.Key, t => t.Select(r => r.Key).ToArray());

                var c = 0;
                foreach (var item in mostWrong.Keys.OrderByDescending(i => i))
                {
                    if (c == number) break;
                    var msg = mostWrong[item].Select(q => q.Quest.Substring(0, 30)).Aggregate((a, b) => a + "\n\n" + b);

                    await botClient.SendTextMessageAsync(
                    chatId: uid,
                    text: $"❌ Risposte sbagliate {item} volte:\n{msg}");

                    c++;
                }

                return;
            }

            if (update.Message.Text.StartsWith("/qsb"))
            {
                int number = 1;
                if (update.Message.Text.Length > 4) int.TryParse(update.Message.Text.Substring(5), out number);

                var mostBlank = _questionStats.GroupBy(e => e.Value.Blank).ToDictionary(e => e.Key, t => t.Select(r => r.Key).ToArray());

                var c = 0;
                foreach (var item in mostBlank.Keys.OrderByDescending(i => i))
                {
                    if (c == number) break;
                    var msg = mostBlank[item].Select(q => q.Quest.Substring(0, 30)).Aggregate((a, b) => a + "\n\n" + b);

                    await botClient.SendTextMessageAsync(
                    chatId: uid,
                    text: $"🟡 Risposte non date {item} volte:\n{msg}");

                    c++;
                }
                return;
            }

            if (update.Message.Text.Equals("/rsp"))
            {
                if (!_playedQuestions.ContainsKey(uid))
                {
                    await botClient.SendTextMessageAsync(
                        chatId: uid,
                        text: "❌ Non c'è niente da eliminare!");
                    return;
                }
                _playedQuestions[uid].Clear();
                await botClient.SendTextMessageAsync(
                    chatId: uid,
                    text: "✅ Memoria eliminata!");
                return;
            }

            if ((!_playingQuestions.ContainsKey(uid)) || update.Message.Text == GetName().ToLower() || update.Message.Text == "/" + GetName().ToLower() || update.Message.Text == "/reset" || update.Message.Text == "/restart")
            {
                if (_scores.ContainsKey(update.Message.From.Id)) _scores[update.Message.From.Id] = new OttoScore();
                else _scores.Add(update.Message.From.Id, new OttoScore());

                SendRandomQuestion(update.Message.From.Id, botClient, cancellationToken);
                return;
            }

            var cur = _playingQuestions[uid];
            var wrongMsg = "🟡 La risposta corretta era la " + (cur.Correct + 1) + " ☹️";

            if (update.Message.Text == "n" || update.Message.Text == "Passa")
            {
                _scores[uid].Blank += 1;
                _questionStats[cur].Blank += 1;

                await botClient.SendTextMessageAsync(
                    chatId: uid,
                    text: wrongMsg
                );

                SendRandomQuestion(uid, botClient, cancellationToken);
                return;
            }

            //todo try parse

            var pick = -1;
            if (!int.TryParse(update.Message.Text, out pick))
            {
                await botClient.SendTextMessageAsync(
                    chatId: uid,
                    text: "❓ scusa, non ho capito 😭");
                await botClient.SendTextMessageAsync(
                    chatId: uid,
                    text: "⭕️ per uscire da 8linux, scrivi /leave");
                return;
            }
            pick -= 1;


            if (pick == _playingQuestions[uid].Correct)
            {
                await botClient.SendTextMessageAsync(
                    chatId: uid,
                    text: "✅ Risposta esatta!");

                _scores[uid].Correct += 1;
                _questionStats[cur].Correct += 1;
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: uid,
                    text: "❌ Risposta errata!");
                await botClient.SendTextMessageAsync(
                    chatId: uid,
                    text: wrongMsg);

                _scores[uid].Wrong += 1;
                _questionStats[cur].Wrong += 1;
            }
            SendStats(uid, botClient, cancellationToken);
            await Task.Delay(400);
            SendRandomQuestion(uid, botClient, cancellationToken);
        }

        private async void SendRandomQuestion(long uid, ITelegramBotClient botClient, CancellationToken cancellationToken)
        {
            var qst = PickRandomQuestion(uid, botClient);

            try
            {
                if (qst.Quest.Length <= 40)
                {
                    Console.WriteLine("Sto inviando la domanda " + qst.Quest + " a " + uid);
                }
                else
                {
                    Console.WriteLine("Sto inviando la domanda " + qst.Quest.Substring(0, 40) + " a " + uid);
                }
            }
            catch(Exception e)
            {
                botClient.SendTextMessageAsync(
                    chatId: _accessManager.AdminId,
                    text: $"Question is malformed -> {qst.Quest} \n {e.Message}"
                );
                return;
            }

            if (!_questionStats.ContainsKey(qst)) _questionStats.Add(qst, new OttoScore());

            if (_playingQuestions.ContainsKey(uid)) _playingQuestions[uid] = qst;
            else _playingQuestions.Add(uid, qst);

            string answers = "";
            List<KeyboardButton> kbs = new List<KeyboardButton>();

            bool splitAns = false;
            foreach (var ans in qst.Answers)
                if ((ans.Contains("\n") && ans.Substring(ans.IndexOf("\n")).Length > 1 ) || ans.Contains("img=")) splitAns = true;

            var corry = qst.Answers[qst.Correct];

            for (int i = 0; i < qst.Answers.Count; i++)
            {
                if (!splitAns) answers += (i + 1) + ". " + qst.Answers[i] + "\n\n";
                kbs.Add((i + 1).ToString());
            }

            KeyboardButton[] fr = kbs.ToArray();

            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                fr,
                new KeyboardButton[] { "Passa" },
            })
            {
                ResizeKeyboard = true
            };

            string quest = qst.Quest;
            if (qst.Quest.StartsWith("img="))
            {
                try
                {
                    await botClient.SendPhotoAsync(
                    chatId: uid,
                    photo: quest.Substring(4).Split('\n')[0]);
                }
                catch(Exception e)
                {
                    CatchParsingError(botClient, quest, uid, e);
                }

                quest = quest.Substring(quest.IndexOf('\n') + 1);
            }
            try
            {
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: uid,
                    text: "📎 " + PrepareHtml(quest) + "\n\n" + PrepareHtml(answers),
                    replyMarkup: replyKeyboardMarkup,
                    parseMode: ParseMode.Html
                );
            }
            catch (Exception e) { CatchParsingError(botClient, quest, uid, e); }

            if (splitAns)
            {
                for (int i = 0; i < qst.Answers.Count; i++)
                {
                    await Task.Delay(350);
                    if (qst.Answers[i].StartsWith("img="))
                    {
                        try
                        {
                            await botClient.SendPhotoAsync(
                            chatId: uid,
                            photo: qst.Answers[i].Split('\n')[0].Substring(4),
                            caption: "✏️ Risposta " + (i + 1) + ": " + qst.Answers[i].Substring(qst.Answers[i].Split('\n')[0].Length)
                        );
                        }
                        catch(Exception e)
                        {
                            CatchParsingError(botClient, "[R] " + qst.Answers[i].Substring(0, 20) + " -> " + quest, uid, e);
                        }
                    }
                    else
                    {
                        Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: uid,
                        text: "✏️ Risposta " + PrepareHtml((i + 1) + ":\n" + qst.Answers[i]),
                        replyMarkup: replyKeyboardMarkup,
                        parseMode: ParseMode.Html
                    );
                    }
                }
            }
        }

        private async void CatchParsingError(ITelegramBotClient botClient, string quest, long uid, Exception e)
        {
            await botClient.SendTextMessageAsync(
                    chatId: uid,
                    text: "❌ Cercavo di inviarti una domanda ma si è verificato un errore anomalo nel parsing. Per favore, invia /restart per resettarmi.\nL'errore verrà automaticamente segnalato (visto che tecnologia avanzatissima?)"
                );
            await botClient.SendTextMessageAsync(
                chatId: uid,
                text: $"❌ La domanda {quest.Substring(0, 60)} è rotta.\nSi è verificato {e.Message}"
            );
        }

        private static string PrepareHtml(string s)
        {
            return s.Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("&lt;code&gt;", "<code>")
                .Replace("&lt;/code&gt;", "</code>")
                .Replace("&lt;pre&gt;", "<pre>")
                .Replace("&lt;/pre&gt;", "</pre>")
                .Replace("&lt;b&gt;", "<b>")
                .Replace("&lt;/b&gt;", "</b>");
        }

        private async void SendStats(long uid, ITelegramBotClient botClient, CancellationToken cancellationToken)
        {
            var stats = _scores[uid];

            var total = stats.Correct + stats.Wrong + stats.Blank;

            await botClient.SendTextMessageAsync(
                chatId: uid,
                text: stats.Correct + " corrette (" + ((float)stats.Correct / (float)total) * 100f + "%)\n" + stats.Wrong + " errate (" + ((float)stats.Wrong / (float)total) * 100f + "%)\n" + stats.Blank + " non date (" + ((float)stats.Blank / (float)total) * 100f + "%)\n");
        }

        public string GetName()
        {
            return _name;
        }

        public List<Question> GetQuestions()
        {
            return _questions;
        }
    }
}