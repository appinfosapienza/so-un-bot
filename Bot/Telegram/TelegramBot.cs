﻿using SoUnBot.AccessControl;
using SoUnBot.ModuleLoader;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SoUnBot.Telegram
{
    internal class TelegramBot
    {
        private AccessManager _accessManager;
        private string _moth_path;

        private Dictionary<string, IModule> _modules;
        public TelegramBotClient BotClient { get; private set; }
        private Dictionary<long, IModule> _usersContext;

        public TelegramBot(string token, AccessManager accessManager, string motd_path, Dictionary<string, IModule> modules)
        {
            _accessManager = accessManager;
            _moth_path = motd_path;
            _modules = modules;
            _usersContext = new Dictionary<long, IModule>();
            BotClient = new TelegramBotClient(token);

            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // receive all update types
            };
            BotClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);

            GetMe();
        }

        private async void GetMe()
        {
            var me = await BotClient.GetMeAsync();
            Console.WriteLine($"Start listening for @{me.Username}");
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                            long chatId;

            if (update.Type == UpdateType.CallbackQuery)
            {
                chatId = update.CallbackQuery.From.Id;
                if (update.CallbackQuery.Message.Text.StartsWith("ACM: ") && update.CallbackQuery.Data.Contains("Grant"))
                {
                    long uid = int.Parse(update.CallbackQuery.Message.Text.Substring(5).Split('\n')[0]);
                    string perm = update.CallbackQuery.Message.Text.Split("Ha richiesto l'accesso a: ")[1];
                    _accessManager.GrantPermission(uid, perm);

                    await botClient.AnswerCallbackQueryAsync(
                        update.CallbackQuery.Id,
                        "Successo!"
                        );
                    await botClient.SendTextMessageAsync(
                        uid,
                        "✅ Congratulazioni! Hai ottenuto l'accesso a: " + perm
                        );
                    return;
                }
                if (update.CallbackQuery.Message.Text.StartsWith("ACM") && update.CallbackQuery.Data.Contains("🔆 Richiedi accesso"))
                {
                    string perm = update.CallbackQuery.Message.Text.Split('`')[1];
                    _accessManager.AskPermission(update.CallbackQuery.From, perm, botClient);
                    await botClient.AnswerCallbackQueryAsync(
                        update.CallbackQuery.Id,
                        "Richiesta effettuata"
                        );
                    await botClient.EditMessageTextAsync(
                        update.CallbackQuery.From.Id,
                        update.CallbackQuery.Message.MessageId,
                        update.CallbackQuery.Message.Text,
                        replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Richiesto 〽️"))
                        );
                    return;
                }
                if (update.CallbackQuery.Message.Text.StartsWith("ACM")) {
                    return;
                }
            }

            if (update.Type != UpdateType.Message) // this is temp
                return;
            if (update.Message!.Type != MessageType.Text)
                return;

            chatId = update.Message.Chat.Id;

            if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Text && update.Message.Text.StartsWith("/spam"))
            {
                if (!_accessManager.CheckPermission(update.Message.From, "global.spam", botClient)) return;

                await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Invio annuncio in corso...");
                new Thread(() => 
                {
                    Thread.CurrentThread.IsBackground = true; 
                    SendToEveryone(botClient, chatId, update.Message.Text.Substring(6));
                }).Start();
                return;
            }

            if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Text && update.Message.Text == "/leave")
            {
                _usersContext.Remove(chatId);
            }

            if (_usersContext.ContainsKey(chatId))
            {
                _usersContext[chatId].ProcessUpdate(botClient, update, cancellationToken);
                return;
            }

            if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Text)
            {
                var msg = update.Message.Text.StartsWith("/") ? update.Message.Text.Substring(1) : update.Message.Text;
                if (_modules.ContainsKey(msg))
                {
                    _usersContext.Add(chatId, _modules[msg]);
                    _modules[msg].ProcessUpdate(botClient, update, cancellationToken);
                    return;
                }
            }

            string validModules = _modules.Keys.Select(i => "/" + i).Aggregate((a, b) => a + "\n" + b);

            var motd = System.IO.File.ReadAllText(_moth_path);

            // Echo received message text
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: motd);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error handling the update: " + e.Message);
            }
        }

        async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ApiRequestException apiRequestException)
            {
                await botClient.SendTextMessageAsync(_accessManager.AdminId, apiRequestException.ToString());
            }

            // Restart the bot (otherwise it would become an amoeba)
            using var cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };
            BotClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);
        }

        private async void SendToEveryone(ITelegramBotClient botClient, long chatId, string text)
        {
            foreach (long user in _accessManager.Users())
            {
                try
                {
                    Console.WriteLine("Sto spammando a" + user.ToString());
                    await botClient.SendTextMessageAsync(
                        chatId: user,
                        text: text
                    );
                    await Task.Delay(100);
                }
                catch
                {
                    Console.WriteLine("Ho fallito");
                }
            }
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "✅ Annunciato a tutti!");
        }
    }
}
