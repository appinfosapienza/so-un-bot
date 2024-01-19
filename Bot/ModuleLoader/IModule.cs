using Telegram.Bot;
using Telegram.Bot.Types;

namespace SoUnBot.ModuleLoader
{
    public interface IModule
    {
        public string Cmd();
        public string GetName();
        public void ProcessUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
    }
}
