// This feature is not maintained anymore
// it was exposing a webserver returning a json with all the questions and answers.
// It was originally intended for a web version of the bot, but was never completed.

/**using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace HomeBot.Modules.OttoLinux
{
    internal class WebReverse
    {
        public WebReverse(Func<string, List<Question>> match)
        {
            using var listener = new HttpListener();
            listener.Prefixes.Add("http://+:8001/");

            listener.Start();

            Console.WriteLine("Listening on port 8001...");

            string pre = "";

            while (true)
            {
                HttpListenerContext ctx = listener.GetContext();
                using HttpListenerResponse resp = ctx.Response;

                resp.StatusCode = (int)HttpStatusCode.OK;
                resp.StatusDescription = "Status OK";
                resp.AddHeader("Access-Control-Allow-Origin", "*");
                resp.AddHeader("Access-Control-Allow-Headers", "*");
                resp.AddHeader("Access-Control-Allow-Methods", "POST");
                resp.AddHeader("Content-Type", "application/json");

                var body = new StreamReader(ctx.Request.InputStream).ReadToEnd();
                //if (body.Equals("")) body = pre;
                pre = body;

                string data = JsonConvert.SerializeObject(match(pre));
                byte[] buffer = Encoding.UTF8.GetBytes(data);
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
**/