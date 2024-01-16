using Telegram.Bot;
using Telegram.Bots.Extensions.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Extensions;
using System.Data.SqlClient;
using System;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Data.SqlClient;
using System.Data;
using Telegram.Bots.Http;
using System.Globalization;
using System.Threading.Tasks;

using static System.Net.Mime.MediaTypeNames;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bots;
using HtmlAgilityPack;
using System.Diagnostics.Eventing.Reader;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;
using System.Threading;

namespace tg1
{


    public class Program
    {

        enum BotState
        {
            Main,
            AddTender,
            SubMenu,
            AddApply,
            listDelete,
            DeleteApply,
            Pars,
            TestHelp,
            Sex,
            WorkList,
            SubMenu2,
            SubMenu3,
        }
        static BotState currentState = BotState.Main;
        static string state;
        private static Timer timer;
        private static int isTimerSet;
        private static Timer timer2;
       private static int interval = 0;
        private static int intervalAdded = 0;

        static void Main(string[] args)
        {

            var bot = new TelegramBotClient("6655981877:AAHYzbmbjF3ZM5kzBQhuYADangqCCDptB04");


            bot.StartReceiving(Update, Error);
            
            Console.ReadLine();


        }

        private static async Task ProcessWithTimer(Message message, ITelegramBotClient bot )
        {
            int interval = await CheckEndsOfApply("в работе", message, bot);
            string count = string.Empty;
            if (interval != 5)
            {


                switch (interval)
                {
                    case 1:
                        interval = 60 * 60 * 1000;
                        count = "1 час";
                        break;
                    case 2:
                        interval = 12 * 60 * 60 * 1000;
                        count = "12 часов";
                        break;
                    case 3:
                        interval = 15 * 60 * 1000;
                        count = "15 минут";
                        break;
                    case 4:
                        interval = 15 * 60 * 1000;
                        count = "15 минут - истечение срока";
                        break;

                    default:

                        break;
                }
                await bot.SendTextMessageAsync(message.Chat.Id, $"сейчас бот работает каждые {count}");
                await Task.Delay(interval);
                await ProcessWithTimer(message, bot);

            }

        }

        


        
        async private static Task Update(ITelegramBotClient bot, Update update, CancellationToken cts)
        {
            var message = update.Message;

            if (update.Type == UpdateType.Message && update?.Message?.Text != null || update.Type == UpdateType.CallbackQuery)
            {
                if (update.Type == UpdateType.Message && update?.Message?.Text != null)
                {
                    await HandleMessage(bot, update.Message);
                }
                if (update.Type == UpdateType.CallbackQuery)
                {
                    await HandleCallbackQuery(bot, update.CallbackQuery);
                }
                
            }

        }
        

            

        async static Task HandleCallbackQuery(ITelegramBotClient bot, CallbackQuery? callbackQuery)
        {
            if (callbackQuery.Data == "item1")
            {
                await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "вы хотите купить товар1");
                return;
            }
            else if (callbackQuery.Data == "soap")
            {
                state = "soap";
                BotOnCallbackQueryReceived(bot, callbackQuery, state);
            }
            else if (callbackQuery.Data == "bback")
            {
                state = "soap";
                BotOnCallbackQueryReceived(bot, callbackQuery, state);
            }
            else if (callbackQuery.Data == "back")
            {
                state = "back";
                BotOnCallbackQueryReceived(bot, callbackQuery, state);
            }

            else if (callbackQuery.Data == "item2")
            {
                await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "вы хотите купить товар2");
                return;
            }
            else if (callbackQuery.Data == "item3")
            {
                state = "item3";
                BotOnCallbackQueryReceived(bot, callbackQuery, state);
            }
        }
        private static async void BotOnCallbackQueryReceived(ITelegramBotClient bot, CallbackQuery callBackQueary, string state)
        {
            if (state == "soap")
            {
                await bot.EditMessageTextAsync(
                callBackQueary.Message.Chat.Id,
                callBackQueary.Message.MessageId,
                $"Выберите какое именно мыло вы хотите",
                replyMarkup: GetButtons1());
            }
            else if (state == "item3")
            {
                var keyboard = new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithCallbackData(text: "Назад", "bback"));

                await bot.EditMessageTextAsync(
                callBackQueary.Message.Chat.Id,
                callBackQueary.Message.MessageId,
                $"МЫЛО: \n Пиздатое \n Цена: 400 \n Вес: 500 грамм",
                replyMarkup: keyboard);
            }
            else if (state == "back")
            {


                await bot.EditMessageTextAsync(
                callBackQueary.Message.Chat.Id,
                callBackQueary.Message.MessageId,
                $"Выберите категорию: ",
                replyMarkup: GetButtons2());

            }


        }


        async static Task HandleMessage(ITelegramBotClient bot, Message message)
        {
            var mess = message.Text;

            string chatid = message.Chat.Id.ToString();
            if (mess == "/start")
            {
                Baza.RegisterUser("@" + message.Chat.Username, chatid);

                currentState = BotState.Main;
                await bot.SendTextMessageAsync(message.Chat.Id, "Добрйы день еблан выбери че хошь", replyMarkup: GetButtonsReply());
                return;
            }
            else if (mess == "/users")
            {
                string userstr = string.Empty;

                foreach (var user in Baza.GetUSers())
                {
                    userstr += user + Environment.NewLine;
                }
                await bot.SendTextMessageAsync(message.Chat.Id, $"Пользователи: \n {userstr}");
                return;
            }

            else if (mess == "/add_apply")
            {
                currentState = BotState.AddApply;
                await bot.SendTextMessageAsync(message.Chat.Id, "Введите реестровый номер или ссылку на тендер: ");
                return;
            }

            else if (mess == "/add_tender")
            {

                currentState = BotState.AddTender;
                await bot.SendTextMessageAsync(message.Chat.Id, "Введите ссылку на тендер в формате: 'https://zakupki.gov.ru/epz/order/notice/notice223/common-info.html?noticeInfoId=16067108'");
                return;

            }
            else if (mess == "/delete_apply")
            {
                currentState = BotState.DeleteApply;

                await bot.SendTextMessageAsync(
                message.Chat.Id,
                $"Введите реестровый номер или ссылку на тендер:");


            }


            else if (mess == "/list")
            {
                currentState = BotState.Main;

                string status = "подана";
                string Apply = string.Empty;
                string infoSub = string.Empty;
                string urlSub = string.Empty;
                int count = 1;

                foreach (var apply in Baza.AllWork(status, count,message.Chat.Id))
                {
                    int index = apply.IndexOf(' ');
                    urlSub = apply.Substring(0, index);
                    infoSub = apply.Substring(index);



                    if (urlSub != null)
                    {
                        Apply += $"<a href='" + urlSub + $"'>Заявка № </a> \n " + infoSub + Environment.NewLine;
                        count++;
                    }

                    


                }
                if (Apply.Contains("Заявка"))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, $"Список поданных: \n {Apply}", parseMode: ParseMode.Html);


                }
                else if (!Apply.Contains("Заявка"))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "вы не добавляли ни один тендер в поданные ");
                }


                return;

            }

            else if (mess == "/work_list")
            {
                currentState = BotState.Main;



                string Apply = string.Empty;
                string infoSub = string.Empty;
                string urlSub = string.Empty;
                int count = 1;

                foreach (var apply in Baza.AllWork("секс", 0,message.Chat.Id))
                {
                    int index = apply.IndexOf(' ');
                    urlSub = apply.Substring(0, index);
                    infoSub = apply.Substring(index);

                    if (urlSub != null)
                    {
                        Apply += $"<a href='" + urlSub + $"'>Заявка № {count}</a> \n " + infoSub + Environment.NewLine;
                        count++;

                    }

                    
                }
                if (Apply.Contains("Заявка"))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, $"Список в работе : \n {Apply}", parseMode: ParseMode.Html);

                }
                else if (!Apply.Contains("Заявка"))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "вы не добавляли ни один тендер ");
                }


                return;

            }

            else if (mess == "/list_delete")
            {
                currentState = BotState.Main;

                string status = "удалена";
                string Apply = string.Empty;
                string infoSub = string.Empty;
                string urlSub = string.Empty;
                int count = 1;
                int choice = 1;
                foreach (var apply in Baza.AllWork(status, choice, message.Chat.Id))
                {
                    int index = apply.IndexOf(' ');
                    urlSub = apply.Substring(0, index);
                    infoSub = apply.Substring(index);

                    if (urlSub != null)
                    {
                        Apply += $"<a href='" + urlSub + $"'>Заявка № {count}</a> \n " + infoSub + Environment.NewLine;
                        count++;

                    }

                }
                if (Apply.Contains("Заявка"))
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, $"Список удаленных : \n {Apply}", parseMode: ParseMode.Html);

                }
                else if (!Apply.Contains("Заявка") )
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "вы не удаляли не один тендер");

                }

                return;





            }
            else if (mess == "Поддержка")
            {
                currentState = BotState.TestHelp;
                await bot.SendTextMessageAsync(message.Chat.Id, "Вставьте url тендера, Для тестовой смены даты заявки: ");
                return;

            }

            else if (mess == "Секс" && currentState == BotState.Main)
            {
                currentState = BotState.Main;
                await CheckHourlyChanges("в работе", message, bot);
                return;

            }
            else if (currentState == BotState.TestHelp)
            {
                currentState = BotState.Main;
                try
                {
                    if (Baza.UpdateDateBase(mess, "28.01.2024") == true)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Дата успешно изменена");
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, " Нет такого тендера");
                    }


                }
                catch (Exception)
                {

                    await bot.SendTextMessageAsync(message.Chat.Id, "ОШИБКА");
                    return;
                }

            }

            else if (currentState == BotState.DeleteApply)
            {
                currentState = BotState.Main;
                string status = "удалена";

                var url = mess;

                if (Baza.AddApply(url, status,message.Chat.Id) == true)
                {
                    await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                    await bot.EditMessageTextAsync(
                message.Chat.Id,
                message.MessageId - 1,
                $"Данный тендер отсутствует в боте");



                }

                else
                {
                    await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                    await bot.EditMessageTextAsync(
                message.Chat.Id,
                message.MessageId - 1,
                $"Тендер убран в удаленные");


                }

            }

            else if (currentState == BotState.AddTender)
            {
                string status = "в работе";
                HtmlWeb web = new HtmlWeb();

                if (mess.StartsWith("https") && mess.Contains("zakupki.gov.ru"))
                {
                    HtmlDocument document = web.Load(mess);

                    var numberOfApply = document.DocumentNode.SelectSingleNode("(//a[@target='_blank'])[5]").InnerText.Trim();

                    var dateOfApply = string.Empty;


                    var node = document.DocumentNode.SelectSingleNode("(//div[@class='data-block__value'])[3]");

                    var node2 = document.DocumentNode.SelectSingleNode("(//span[@class='cardMainInfo__content'])[5]");

                    var nameOfApply = string.Empty;

                    var nodeName = document.DocumentNode.SelectNodes("(//div[@class='registry-entry__body-value'])[1]");
                    var nodeExists = document.DocumentNode.SelectNodes("(//span[@class='cardMainInfo__content'])[1]");

                    var customer = document.DocumentNode.SelectNodes("(//a[@target='_blank'])[6]");



                    if (nodeName != null && customer != null)
                    {

                        nameOfApply = nodeName.First().InnerText.Trim();
                    }
                    else if (nodeName == null && nodeExists != null && customer != null)
                    {
                        nameOfApply = nodeExists.First().InnerText.Trim();
                    }
                    else if (nodeName == null && nodeExists == null)
                    {
                        nameOfApply = "нет описания";
                    }


                    if (node != null)
                    {
                        dateOfApply = node.InnerText.Trim();
                    }
                    else if (node2 != null)
                    {
                        dateOfApply = node2.InnerText.Trim();
                    }
                    else if (node == null && node2 == null)
                    {
                        dateOfApply = "не найдена дата";
                    }



                    var result = Baza.AddTender(mess, numberOfApply, dateOfApply, nameOfApply, status, message.Chat.Id.ToString());

                    if (result == true)
                    {
                        await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                        await bot.EditMessageTextAsync(
                message.Chat.Id,
                message.MessageId - 1,
                $"Заявка успешно добавлена");

                        if (timer == null)
                        {
                            TimerCallback callback = new TimerCallback(async delegate (object state)
                            {
                                await CheckHourlyChanges("в работе", message, bot);

                            });
                            var hours = 12 * 60 * 60 * 1000;
                            timer = new Timer(callback, null, 100, hours);
                        }
                        
                        if (interval == 0)
                        {
                            
                            ProcessWithTimer(message,bot);
                            
                            interval++;
                        }
                       
                      



                    }
                    else if (result == false)
                    {
                        await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                        await bot.EditMessageTextAsync(
                    message.Chat.Id,
                    message.MessageId - 1,
                    $"Тендер уже есть в базе");

                    }






                    currentState = BotState.Main;


                }

                else
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "Не корректная ссылка. \n Введите новую");
                    return;
                }


            }

            else if (currentState == BotState.AddApply)
            {
                currentState = BotState.Main;

                string status = "подана";

                var url = mess;

                if (Baza.AddApply(url, status,message.Chat.Id) == true)
                {
                    await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                    await bot.EditMessageTextAsync(message.Chat.Id, message.MessageId - 1, "Данный тендер отсутствует в боте");
                    return;
                }

                else
                {
                    await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                    await bot.EditMessageTextAsync(message.Chat.Id, message.MessageId - 1, "Тендер убран из рабочих в поданные заявки");

                    if (intervalAdded == 0)
                    {
                         
                        ProcessWithTimer(message, bot);
                        intervalAdded++;
                    }


                    
                }

            }

            else if (mess.ToLower() == "товары")
            {
                await bot.SendTextMessageAsync(message.Chat.Id, "Выберите категорию:", replyMarkup: GetButtons2());
                return;
            }
            else if (mess.ToLower() == "спарсить")
            {
                currentState = BotState.Pars;
                await bot.SendTextMessageAsync(message.Chat.Id, "Вставьте ссылку на заявку");

                return;
            }


            else
            {
                currentState = BotState.Main;
                await bot.SendTextMessageAsync(message.Chat.Id, "Главное меню 🥶");
                return;
            }

        }

        private static Task Error(ITelegramBotClient pisya, Exception arg2, CancellationToken arg3)
        {
            var ErrorMessage = arg2 switch
            {
                ApiRequestException apiRequestException
                    => $"Ошибка телеграм АПИ:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
                _ => arg2.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;


        }


        private static string CheckTimeApply(DateTime dateapply)
        {

            DateTime utcTime = DateTime.UtcNow;
            DateTimeOffset moscowTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTimeOffset.UtcNow, "Russian Standard Time");

            try
            {
                
                TimeSpan interval = dateapply - moscowTime;

                if (interval.Minutes > 0)
                {
                    
                    if (interval.Days == 3 && interval.Hours <= 2)
                    {
                        return "3 дня";

                    }
                    else if (interval.Days == 2)
                    {
                        return "2 дня";
                    }
                    else if (interval.Days == 1 && interval.Hours <= 2)
                    {
                        return "1 день";

                    }
                    if (interval.Days == 0 && interval.Hours == 2)
                    {
                        return "2 часа";
                    }
                    if (interval.Days == 0 && interval.Hours <= 1 && interval.Minutes >= 1)
                    {
                        return "1 час";
                    }
                    return "нету даты";
                }
                else if (interval.Minutes <= 0 && interval.Minutes !> -15)
                {
                    return "15 минут";
                }
                else if (( interval.Days == 0  || interval.Hours == -1) && interval.Minutes <= -15 && interval.Minutes !>= -40)
                {
                    return "15 минут";
                }

                else
                {
                    return "некорректная дата (закончился срок)";
                }

            }
            catch (Exception)
            {

                return "нету даты";
            }

        }

        public static async Task<int> CheckEndsOfApply(string status, Message message, ITelegramBotClient bot)
        {
            int count = 1;


            
            string urlSub = string.Empty;
            string OlddateSub = string.Empty;
            string chatId = string.Empty;
            string newDateSub = string.Empty;
            string nameApply = string.Empty;
            int dayOfApply = 0;
            int dayz2 = 0;
            int dayz = 0;
            int endOfDate = 0;
            if (message != null)
            {
                foreach (var apply in Baza.AllWork(status, 3,message.Chat.Id))
                {

                    if (apply.Contains("zakupki"))
                    {
                        int index = apply.IndexOf(' ');
                        int index2 = apply.IndexOf('_');
                        int index3 = apply.LastIndexOf(' ');
                        urlSub = apply.Substring(0, index);
                        OlddateSub = apply.Substring(index, index2 - index);
                        nameApply = apply.Substring(index2 + 1, index3 - index2);
                        chatId = apply.Substring(index3);


                        if (!OlddateSub.Contains("найдена"))
                        {


                            DateTime date = DateTime.ParseExact(OlddateSub.Trim(), "dd.MM.yyyy", CultureInfo.InvariantCulture);
                            string remains = CheckTimeApply(date);

                            if (remains != "нету даты" && !remains.Contains("некорректная") && remains != "15 минут")
                            {

                                if (status != "подана")
                                {
                                    await bot.SendTextMessageAsync(message.Chat.Id, $"Внимание - окончание подачи заявок через {remains} \n   <a href='" + urlSub + $"'> ссылка </a> \n  - {nameApply} \n - {OlddateSub}", parseMode: ParseMode.Html);

                                }

                                if (remains == "1 день")
                                {
                                    dayOfApply = 1;
                                    dayz = 1;
                                }
                                else if (remains == "1 час" || remains == "2 часа")
                                {
                                    dayz2 = 1;
                                }



                                else
                                {
                                    dayOfApply = 2;
                                }
                            }
                            else if (remains == "15 минут")
                            {
                                await bot.SendTextMessageAsync(message.Chat.Id, $"Внимание - закончился срок приема заявок. \n   <a href='" + urlSub + $"'> ссылка </a> \n  - {nameApply} \n - {OlddateSub}", parseMode: ParseMode.Html);

                                endOfDate = 1;
                            }
                        }
                        count++;
                    }
                    
                }
            }
            if (dayz == 1)
            {
                return dayz;
            }
            else if (dayz2 == 1)
            {
                return 3;
            }
            else if (endOfDate == 1)
            {
                return 4;
            }
            else if (dayz == 0  && dayz2 == 0 && endOfDate == 0 && dayOfApply == 0)
            {
                return 5;
            }
            else
            {
                return 2;
            }



        }

        public static async Task CheckHourlyChanges(string status, Message message, ITelegramBotClient bot)
        {
            List<string> updatedApply = new List<string>();
            int count = 1;


            HtmlWeb html = new HtmlWeb();
            string urlSub = string.Empty;
            string OlddateSub = string.Empty;
            string chatId = string.Empty;
            string newDateSub = string.Empty;
            string nameApply = string.Empty;
            int dayOfApply = 0;
            int dayz2 = 0;
            int dayz = 0;
            int endOfDate = 0;
            if (message != null)
            {
                foreach (var apply in Baza.AllWork(status, 3, message.Chat.Id))
                {

                    int index = apply.IndexOf(' ');
                    int index2 = apply.IndexOf('_');
                    int index3 = apply.LastIndexOf(' ');
                    urlSub = apply.Substring(0, index);
                    OlddateSub = apply.Substring(index, index2 - index);
                    nameApply = apply.Substring(index2 + 1, index3 - index2);
                    chatId = apply.Substring(index3);


                    HtmlDocument document = html.Load(urlSub);
                    var node = document.DocumentNode.SelectSingleNode("(//div[@class='data-block__value'])[3]");
                    var dateOfApply = string.Empty;
                    var node2 = document.DocumentNode.SelectSingleNode("(//span[@class='cardMainInfo__content'])[5]");

                    if (node != null)
                    {
                        dateOfApply = node.InnerText.Trim();
                    }
                    else if (node2 != null)
                    {
                        dateOfApply = node2.InnerText.Trim();
                    }
                    else if (node == null && node2 == null)
                    {
                        dateOfApply = "не найдена дата";
                    }

                    if (dateOfApply != OlddateSub.TrimStart())
                    {

                        Baza.UpdateDateBase(urlSub, dateOfApply);

                        updatedApply.Add(urlSub);

                        string info = $"Внимание - изменение даты окончания подачи заявок \n   <a href='" + urlSub + $"'> Заявка № {count}</a> \n  - {nameApply} \n - {dateOfApply}";

                        await bot.SendTextMessageAsync(chatId, info, parseMode: ParseMode.Html);

                    }
                    

                       

                    

                    count++;
                }
                await bot.SendTextMessageAsync(chatId, "ТЕСТ - каждые 12 часов проверка работоспособности таймера", parseMode: ParseMode.Html);
            }
           
        }


        private static InlineKeyboardMarkup GetButtons1()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                   InlineKeyboardButton.WithCallbackData(text: "Товар1", "item1"),
                    InlineKeyboardButton.WithCallbackData(text: "Товар2", "item2"),



                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Товар3", "item3"),
                    InlineKeyboardButton.WithCallbackData(text: "Товар4", "item4"),

                },
                new[]
                {
                InlineKeyboardButton.WithCallbackData(text: "Товар5", "item5"),
                    InlineKeyboardButton.WithCallbackData(text: "Товар6", "item6"),
                },

                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Назад", "back"),

                }


            });
        }
        private static ReplyKeyboardMarkup GetButtonsReply()
        {


            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]

            {

                new KeyboardButton[]
                {
                    new KeyboardButton( "Спарсить" ),
                     new KeyboardButton( "Товары" ),


                },
                new KeyboardButton[]
                {
                    new KeyboardButton( "Кабинет" ),
                     new KeyboardButton( "Поддержка" ),


                },
                new KeyboardButton[]
                {
                    new KeyboardButton( "Промокод" ),
                     new KeyboardButton( "Секс" ),

                }

            })
            {
                ResizeKeyboard = true
            };


            return replyKeyboardMarkup;

        }


        private static InlineKeyboardMarkup GetButtons2()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                     InlineKeyboardButton.WithCallbackData(text: "Периферия", "accesories"),
                    InlineKeyboardButton.WithCallbackData(text: "Чист средства", "clean"),

                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Мыло", "soap"),
                    InlineKeyboardButton.WithCallbackData(text: "Бумага", "paper"),


                }

            });
        }

        


        
    }
    
    
}    