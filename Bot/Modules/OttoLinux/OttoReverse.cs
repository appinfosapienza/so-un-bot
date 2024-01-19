// This feature is not maintained anymore
// it has been commented as it depends on the old project structure

/** using HomeBot.ACL;
using HomeBot.ModuleLoader;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace HomeBot.Modules.OttoLinux
{
    public class OttoReverse : IModule
    {
        private List<Question> _questions;
        private string NAME = "8reverse";
        private bool LOCK = false;

        public OttoReverse(string name, List<Question> questions, bool lck)
        {
            _questions = questions;
            LOCK = lck;
            NAME = name;

            Thread thread = new Thread(() => new WebReverse(GetMatch));
            thread.Start();
        }

        public string Cmd()
        {
            return GetName();
        }

        public List<Question> GetMatch(string qst)
        {
            var m = qst
                .Replace("\n", "")
                .Replace("\r", "")
                .Replace("<pre>", "")
                .Replace("<code>", "")
                .Replace("</pre>", "")
                .Replace("</code>", "")
                .Replace("'", "")
                .Replace("à", "a")
                .Replace("è", "e")
                .Replace("é", "e")
                .Replace("ì", "i")
                .Replace("ò", "o")
                .Replace("ù", "u")
                .Replace(" ", "")
                .Replace(" ", "")
                .ToLower();

            var filtered = _questions.FindAll((Question q) =>
            {
                return q.Quest
                .Replace("\n", "")
                .Replace("\r", "")
                .Replace("<pre>", "")
                .Replace("<code>", "")
                .Replace("</pre>", "")
                .Replace("</code>", "")
                .Replace("'", "")
                .Replace("à", "a")
                .Replace("è", "e")
                .Replace("é", "e")
                .Replace("ì", "i")
                .Replace("ò", "o")
                .Replace("ù", "u")
                .Replace(" ", "")
                .Replace(" ", "")
                .ToLower()
                .Contains(m);
            });

            return filtered;
        }

        async void IModule.ProcessUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var uid = update.Message.Chat.Id;

            if (LOCK)
            {
                if (!ACM.CheckPermission(update.Message.From, Cmd(), botClient)) return;
            }
            else
            {
                if (!ACM.CheckPermission(uid, Cmd()))
                {
                    ACM.GrantPermission(uid, Cmd());
                    await botClient.SendTextMessageAsync(
                    chatId: _acm.AdminId,
                    text: $"ACM: {update.Message.From.Id}\nL'utente {update.Message.From.FirstName} {update.Message.From.LastName} @{update.Message.From.Username}\nHa iniziato a usare il bot."
                    );
                }
            }

            if (update.Type != UpdateType.Message)
                return;
            if (update.Message!.Type != MessageType.Text)
                return;

            if (update.Message.Text.Equals("/" + NAME))
            {
                await botClient.SendTextMessageAsync(
                    chatId: uid,
                    text: $"🤫 Hai appena scoperto una funzione nascosta! 🤐🥶"
                );
                return;
            }

            var m = update.Message.Text
                .Replace("\n", "")
                .Replace("\r", "")
                .Replace("<pre>", "")
                .Replace("<code>", "")
                .Replace("</pre>", "")
                .Replace("</code>", "")
                .Replace("'", "")
                .Replace("à", "a")
                .Replace("è", "e")
                .Replace("é", "e")
                .Replace("ì", "i")
                .Replace("ò", "o")
                .Replace("ù", "u")
                .Replace(" ", "")
                .Replace(" ", "")
                .ToLower();

            if (string.IsNullOrEmpty(m)) return;

            var filtered = _questions.FindAll((Question q) =>
            {
                return q.Quest
                .Replace("\n", "")
                .Replace("\r", "")
                .Replace("<pre>", "")
                .Replace("<code>", "")
                .Replace("</pre>", "")
                .Replace("</code>", "")
                .Replace("'", "")
                .Replace("à", "a")
                .Replace("è", "e")
                .Replace("é", "e")
                .Replace("ì", "i")
                .Replace("ò", "o")
                .Replace("ù", "u")
                .Replace(" ", "")
                .Replace(" ", "")
                .ToLower()
                .Contains(m);
            });

            if (!filtered.Any())
                await botClient.SendTextMessageAsync(
                    chatId: uid,
                    text: $"❌ No match :("
                );
            else
                await botClient.SendTextMessageAsync(
                    chatId: uid,
                    text: $"✅ Found {filtered.Count} matches!"
                );

            foreach (var q in filtered)
            {
                await botClient.SendTextMessageAsync(
                    chatId: uid,
                    text: $"❓ Possible match:\n\n{q.Quest}"
                );
                await Task.Delay(250);
                await botClient.SendTextMessageAsync(
                    chatId: uid,
                    text: $"✅ Correct answare for the match:\n\n{q.Answers[q.Correct]}"
                );
                await Task.Delay(500);
            }
        }

        public string GetName()
        {
            return NAME;
        }

        public void ProcessUpdate(ITelegramBotClient botClient, global::Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public List<Question> GetQuestions()
        {
            return _questions;
        }
    }
} **/