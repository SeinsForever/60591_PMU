using Telegram.Bot;
using Telegram.Bot.Types;
using HtmlAgilityPack;
using System.Net;
using Newtonsoft.Json.Linq;

namespace TGbot
{
    class Program
    {
        static void Main(string[] args)
        {
            //var html = @"http://html-agility-pack.net/";
            //HtmlWeb web = new HtmlWeb();
            //var htmlDoc = web.Load(html);
            //HtmlNode node = htmlDoc.DocumentNode.SelectSingleNode("//head/title");
            //Console.WriteLine($"Node Name: {node.Name}, {node.InnerText}");

            var request = new GetRequest("https://aws.random.cat/meow");
            request.Run();
            var response = request.Response;
            var json = JObject.Parse(response);
            var link = json["file"];


            var client = new TelegramBotClient("5754678282:AAEHip5p4fgBqFlz4HKcS5D-cdMYlDXJD7k");
            client.StartReceiving(Update, Error);

            Console.ReadLine();
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;

            if(message.Text != null)
            {
                //Вывод сообщения в консоль
                Console.WriteLine($"{message.Chat.FirstName}  |   {message.Text}");

                //Обработка сообщений пользователя
                if(message.Text.ToLower().Contains("привет"))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Приветик!\nТы можешь спросить у меня:\nСколько времени?\nПокажешь котиков?");
                    return;
                }

                if (message.Text.ToLower().Contains("сколько времени?"))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Хм, сейчас {message.Date.AddHours(5)}");
                    return;
                }

                if (message.Text.ToLower().Contains("покажешь котиков?"))
                {
                    var request = new GetRequest("https://aws.random.cat/meow");
                    request.Run();
                    var response = request.Response;
                    var json = JObject.Parse(response);
                    var link = json["file"];
                    if(link != null)
                    {
                        await botClient.SendPhotoAsync(message.Chat.Id, (string)link);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Что-то пошло не так...");
                    }
                    return;
                }
            }

            if (message.Photo != null)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Фото бомба, но лучше отправь документом");
                return;
            }
        }

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }


        public class GetRequest
        {
            HttpWebRequest _request;
            string _address;

            public string Response { get; set; }

            public GetRequest(string address)
            {
                _address = address;
            }

            public void Run()
            {
                _request = (HttpWebRequest)WebRequest.Create(_address);
                _request.Method = "GET";

                try
                {
                    HttpWebResponse response = (HttpWebResponse)_request.GetResponse();
                    var stream = response.GetResponseStream();
                    if(stream != null)
                    {
                        Response = new StreamReader(stream).ReadToEnd();
                    }
                }
                catch (Exception ex)
                {

                }

                
            }
        }
    }
}

