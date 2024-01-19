using HomeBot.ModuleLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using HomeBot.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using File = Telegram.Bot.Types.File;

namespace HomeBot.ACL
{
    public class ACM
    {
        private string _acl_path;
        private Dictionary<long, HashSet<string>> _acl;
        public long AdminId { get; private set; }

        public ACM(string aclPath, long adminId)
        {
            _acl_path = aclPath;
            AdminId = adminId;
            if (!System.IO.File.Exists(aclPath + "/acl.json"))
            {
                _acl = new Dictionary<long, HashSet<string>>();
                return;
            }
            var json = System.IO.File.ReadAllText(aclPath + "/acl.json");
            _acl = JsonConvert.DeserializeObject<Dictionary<long, HashSet<string>>>(json) ??  new Dictionary<long, HashSet<string>>();;
        }

        public long[] Users()
        {
            return _acl.Keys.ToArray();
        }

        private void SaveToJson()
        {
            var json = JsonConvert.SerializeObject(_acl);
            System.IO.File.WriteAllText(_acl_path + "/acl.json", json);
        }

        public void GrantPermission(long uid, string perm)
        {
            if (_acl.ContainsKey(uid)) _acl[uid].Add(perm);
            else _acl.Add(uid, new HashSet<string> { perm });
            SaveToJson();
        }

        public bool RevokePermission(long uid, string perm)
        {
            if (_acl.ContainsKey(uid)) _acl[uid].Remove(perm);
            else return false;
            SaveToJson();
            return true;
        }

        public bool CheckPermission(long uid, string perm)
        {
            return _acl.ContainsKey(uid) ? _acl[uid].Contains(perm) : false;
        }

        public bool CheckPermission(User user, string perm, ITelegramBotClient client)
        {
            var uid = user.Id;
            var hasPerm = _acl.ContainsKey(uid) ? _acl[uid].Contains(perm) : false;
            if (hasPerm) return true;
            client.SendTextMessageAsync(
                chatId: uid,
                text: $"ACM\nNon hai l'accesso a `{ perm }`. Puoi richiederlo ad un amministratore usando il pulsante qui sotto",
                replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("🔆 Richiedi accesso"))
                );
            return false;
        }
        public void AskPermission(User user, string perm, ITelegramBotClient client)
        {
            /*InlineKeyboardButton[][] ik = new InlineKeyboardButton[][]
            {
                new InlineKeyboardButton[]
                {
                    new InlineKeyboardButton("✅ Grant")
                },
                new InlineKeyboardButton[]
                {
                    new InlineKeyboardButton("❌ Deny")
                }
            };*/
            client.SendTextMessageAsync(
                chatId: AdminId,
                text: $"ACM: { user.Id }\nL'utente { user.FirstName } { user.LastName } @{ user.Username }\nHa richiesto l'accesso a: { perm }",
                replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("✅ Grant"))
                );
        }
    }
}
