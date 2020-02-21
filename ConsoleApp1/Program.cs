using System;
using System.Collections.Generic;
using System.Linq;
using MihaZupan;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ConsoleApp1
{
    public class Program
    {    //имя бота @scrSantaBot
         //я хотел перевести бота на вебхук, но не было времени  
        private static TelegramBotClient botClient;
        //список users содержит никнеймы в телеграмме всех участников игры!!!
        private static readonly List<string> users = new List<string>{"user1", "user2"};
        //само существование этого списка вызвано ограниченными возможностями ботов в телеграмме    
        private static readonly Dictionary<string,string> Santas = new Dictionary<string, string>();
        
        public static void Main(string[] args)
        {
            var proxy = new HttpToSocks5Proxy("207.154.233.200", 1080);//прокси стоит обновлять
            botClient = new TelegramBotClient("977242869:AAEOzcihdXnRzzM9qw-aLOmL0pIrp9PLBV0", proxy)
                {Timeout = TimeSpan.FromSeconds(10)};
            botClient.OnMessage += Bot_OnMessage;
            botClient.OnCallbackQuery += BotOnCallbackQueryReceived;
            botClient.StartReceiving(); //
            Console.ReadKey();
            botClient.StopReceiving();
        }

        private static void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            var message = e.CallbackQuery;
            var username = message.From.Username.ToLower();
            var rand = new Random();
            var randomNumber = rand.Next(users.Count);
            while (true)
            {
                if (users[randomNumber] != $"@{username}") break;
                randomNumber = rand.Next(users.Count);
            }
            if (message.Data == null) return;
            if (message.Data != "Next") return;
            if (!Santas.ContainsKey($"@{username}"))
            {
                botClient.SendTextMessageAsync(message.Message.Chat.Id, $"Готовь подарок для {users[randomNumber]}");
                Santas.Add($"@{username}", users[randomNumber]);
                users.Remove(users[randomNumber]);
            }
            else
            {
                foreach (var santa in Santas.Where(santa => santa.Key == $"@{username}"))
                    botClient.SendTextMessageAsync(message.Message.Chat.Id, santa.Value);
            }
        }


        private static void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var text = e?.Message?.Text;
            if (text == null) return;
            if (text != "/start") return;
            Console.WriteLine(e.Message.Chat);
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[] // first row
                {
                    InlineKeyboardButton.WithCallbackData("Начать!", "Next"),
                }
            });
            botClient.SendTextMessageAsync(e.Message.Chat.Id, "Нажми эту кнопку и узнаешь кому дарить подарок", replyMarkup: inlineKeyboard);
        }
    }
}    


