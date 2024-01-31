using SoUnBot.AccessControl;
using SoUnBot.ModuleLoader;
using SoUnBot.Modules.OttoLinux;
using SoUnBot.Telegram;
using Telegram.Bot.Types;

string dataPath = Environment.GetEnvironmentVariable("DATA_PATH") ?? "BotData";
string aclPath = Environment.GetEnvironmentVariable("ACL_PATH") ?? "BotData/ACL";
string tgToken = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN") ?? "this-string-is-not-a-token";
string tgAdminId = Environment.GetEnvironmentVariable("TELEGRAM_ADMIN_ID") ?? "000000";
string webBaseURL = Environment.GetEnvironmentVariable("WEB_BASE_URL") ?? "http://localhost:8001";
string imagesPath = dataPath + "/Images";

Console.WriteLine("Welcome to SO un bot!");

long tgAdminLong;
if (!long.TryParse(tgAdminId, out tgAdminLong))
{
    Console.WriteLine("Telegram Admin ID is invalid or unset");
    return;
}

var acl = new AccessManager(aclPath, tgAdminLong);
var moduleLoader = new ModuleLoader();

try
{
    foreach (string f in Directory.GetFiles(dataPath + "/Questions"))
    {
        if (f.EndsWith("txt"))
        {
            Console.WriteLine("Loading module " + Path.GetFileNameWithoutExtension(f));
            moduleLoader.LoadModule(new BotGame(acl, Path.GetFileNameWithoutExtension(f), f, false, imagesPath));
        }
        else if (f.EndsWith("json"))
        {
            Console.WriteLine("Loading module " + Path.GetFileNameWithoutExtension(f));
            moduleLoader.LoadModule(new BotGame(acl, Path.GetFileNameWithoutExtension(f), f, false, imagesPath, 3));
        }
        else
        {
            Console.WriteLine("Skipping " + Path.GetFileName(f) + " as the file extension is not supported");
        }
    }
    foreach (string d in Directory.GetDirectories(dataPath + "/Questions"))
    {
        Console.WriteLine("Loading module " + Path.GetFileName(d));
        moduleLoader.LoadModule(new BotGame(acl, Path.GetFileName(d), d, false, imagesPath, 2));
    }
}
catch (System.Exception ex)
{
    Console.WriteLine("There was an issue loading the module: " + ex.Message);
    return;
}

Console.WriteLine("Starting Telegram bot listener...");
new TelegramBot(tgToken, acl, dataPath + "/motd.txt", moduleLoader.Modules);

// worst way ever to keep the main thread running, I know
while (true)
    Thread.Sleep(10000);