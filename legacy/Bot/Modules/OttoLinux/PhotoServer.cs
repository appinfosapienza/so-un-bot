// This feature is not maintained anymore
// it was exposing a webserver returning a json with all the questions and answers.
// It was originally intended for a web version of the bot, but was never completed.

using System.Net;

namespace SoUnBot.Modules.OttoLinux
{
    public class PhotoServer
    {
        public PhotoServer(string baseDir)
        {
            using var listener = new HttpListener();
            listener.Prefixes.Add("http://+:8001/");
            listener.Start();
            Console.WriteLine("PhotoServer is listening on port 8001...");

            while (true)
            {
                HttpListenerContext ctx = listener.GetContext();
                using HttpListenerResponse resp = ctx.Response;

                resp.StatusCode = (int)HttpStatusCode.OK;
                resp.StatusDescription = "Status OK";
                resp.AddHeader("Access-Control-Allow-Origin", "*");
                resp.AddHeader("Access-Control-Allow-Headers", "*");
                resp.AddHeader("Access-Control-Allow-Methods", "GET");

                if (ctx.Request.Url == null)
                {
                    resp.StatusCode = (int)HttpStatusCode.BadRequest;
                    continue;
                }

                if (!File.Exists(baseDir + "/" + ctx.Request.Url.AbsolutePath))
                {
                    resp.StatusCode = (int)HttpStatusCode.NotFound;
                    continue;
                }

                if (ctx.Request.Url.AbsolutePath.EndsWith("png"))
                {
                    resp.AddHeader("Content-Type", "image/png");
                }
                else if (ctx.Request.Url.AbsolutePath.EndsWith("jpg"))
                {
                    resp.AddHeader("Content-Type", "image/jpeg");
                }
                else
                {
                    resp.StatusCode = (int)HttpStatusCode.BadRequest;
                    continue;
                }

                byte[] buffer = File.ReadAllBytes(baseDir + "/" + ctx.Request.Url.AbsolutePath);
                resp.ContentLength64 = buffer.Length;

                using Stream ros = resp.OutputStream;
                try
                {
                    ros.Write(buffer, 0, buffer.Length);
                } catch { }
            }
        }
    }
}
