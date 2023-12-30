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
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bots;
using HtmlAgilityPack;
using System.Diagnostics.Eventing.Reader;
using System.Collections.Generic;
using Quartz;
using Quartz.Impl;









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

        

        static void Main(string[] args)
        {
            
            var bot = new TelegramBotClient("6655981877:AAHYzbmbjF3ZM5kzBQhuYADangqCCDptB04");

           

           
            bot.StartReceiving(Update, Error);
            Console.ReadLine();
            
            

            
            

            

           


        }

        async private static Task Update(ITelegramBotClient bot, Update update, CancellationToken cts)
        {
            var message = update.Message;

            

            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandleMessage(bot, update.Message);
                var data = new DailyJobData("в работе", message, bot);

                Scheduler sch = new();
                await sch.StartScheduler(data);
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallbackQuery(bot, update.CallbackQuery);
                
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
            else if (callbackQuery.Data == "item3" )
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

                foreach (var apply in Baza.AllWork(status, count))
                {
                    int index = apply.IndexOf(' ');
                    urlSub = apply.Substring(0, index);
                    infoSub = apply.Substring(index);



                    
                    
                    Apply += $"<a href='" + urlSub + $"'>Заявка № </a> \n " + infoSub + Environment.NewLine;
                    count++;


                }

                await bot.SendTextMessageAsync(message.Chat.Id, $"Список поданных: \n {Apply}", parseMode: ParseMode.Html);

                return;

            }

            else if (mess == "/work_list")
            {
                currentState = BotState.Main;



                string Apply = string.Empty;
                string infoSub = string.Empty;
                string urlSub = string.Empty;
                int count = 1;

                foreach (var apply in Baza.AllWork("секс", 0))
                {
                    int index = apply.IndexOf(' ');
                    urlSub = apply.Substring(0, index);
                    infoSub = apply.Substring(index);



                    Apply += $"<a href='" + urlSub + $"'>Заявка № {count}</a> \n " + infoSub + Environment.NewLine;
                    count++;
                }

                await bot.SendTextMessageAsync(message.Chat.Id, $"Список в работе : \n {Apply}", parseMode: ParseMode.Html);

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
                foreach (var apply in Baza.AllWork(status, choice))
                {
                    int index = apply.IndexOf(' ');
                    urlSub = apply.Substring(0, index);
                    infoSub = apply.Substring(index);



                    Apply += $"<a href='" + urlSub + $"'>Заявка № {count}</a> \n " + infoSub + Environment.NewLine;
                    count++;
                }

                await bot.SendTextMessageAsync(message.Chat.Id, $"Список удаленных : \n {Apply}", parseMode: ParseMode.Html);

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

                if (Baza.AddApply(url, status) == true)
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
                message.MessageId -1,
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



                    if (nodeName != null && customer != null )
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

                    
                   
                      var result =   Baza.AddTender(mess, numberOfApply, dateOfApply, nameOfApply,status, message.Chat.Id.ToString());

                    if (result == true)
                    {
                        await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                        await bot.EditMessageTextAsync(
                message.Chat.Id,
                message.MessageId - 1,
                $"Заявка успешно добавлена");
                        

                        
                    }
                    else if (result == false)
                    {
                        await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                        await bot.EditMessageTextAsync(
                    message.Chat.Id,
                    message.MessageId-1,
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

                if (Baza.AddApply(url, status) == true)
                {
                    await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                    await bot.EditMessageTextAsync(message.Chat.Id,message.MessageId - 1, "Данный тендер отсутствует в боте");
                    return;
                }

                else
                {
                    await bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                    await bot.EditMessageTextAsync(message.Chat.Id,message.MessageId - 1, "Тендер убран из рабочих в поданные заявки");
                    return;
                }
                
            }

            else if(mess.ToLower() == "товары")
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
            DateTime datenow = DateTime.Now;

            try
            {
                TimeSpan interval = dateapply - datenow;

                if (interval.Minutes >= 0) 
                {
                    if (interval.Days == 3)
                    {
                        return "3 дня";

                    }
                    else if (interval.Days == 2)
                    {
                        return "2 дня";
                    }
                    else if (interval.Days == 1)
                    {
                        return "1 день";

                    }
                    if (interval.Days == 0 && interval.Hours == 2)
                    {
                        return "2 часа";
                    }
                    if (interval.Days == 0 && interval.Hours == 1)
                    {
                        return "1 час";
                    }
                    return "нету даты";
                }

                else
                {
                    return "нету даты";
                }



            }
            catch (Exception)
            {

                return "нету даты";
            }
            

            

        }


        public static async Task CheckHourlyChanges(string status, Message message, ITelegramBotClient bot )
        {
            List<string> updatedApply = new List<string>();
            int count = 1;


            HtmlWeb html = new HtmlWeb();
            string urlSub = string.Empty;
            string OlddateSub = string.Empty;
            string chatId = string.Empty;
            string newDateSub = string.Empty;
            string nameApply = string.Empty;

            foreach (var apply in  Baza.AllWork(status, 3))
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
                else if (dateOfApply == OlddateSub.TrimStart())
                {
                    await bot.SendTextMessageAsync(message.Chat.Id, "Все заявки имеют актуальную дату");
                    
                    
                }
                try
                {
                    DateTime date = Convert.ToDateTime(dateOfApply);
                    string remains = CheckTimeApply(date);

                    if (remains != "нету даты")
                    {

                            await bot.SendTextMessageAsync(message.Chat.Id, $"До окончания подачи заявок осталось: {remains} ");
                          
                    }


                       

                    

                }
                catch (FormatException)
                {

                    await bot.SendTextMessageAsync(message.Chat.Id,"ошибка в дате заявки (чек даты))");
                }
                

                count++;
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


            }) ; 
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
    public class Scheduler
    {



        public async Task StartScheduler(DailyJobData data)
        {

            ISchedulerFactory sch = new StdSchedulerFactory();
            IScheduler shcd = await sch.GetScheduler();
            await shcd.Start();

            var jobKey = new JobKey("dailyJob", "group1");
            if (await shcd.CheckExists(jobKey))
            {
                await shcd.DeleteJob(jobKey);
            }


            IJobDetail job = JobBuilder.Create<DailyJob>()
                .UsingJobData(new JobDataMap { {"data",data } })
                .WithIdentity("dailyJob", "group1")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("dailyTrigger", "group1")
                .StartNow()
                .WithSimpleSchedule(x=> x
                .WithRepeatCount(10)
                .WithInterval(TimeSpan.FromSeconds(10)))
                .Build();

            await shcd.ScheduleJob(job, trigger);

        }


    }

    public class DailyJob : IJob
    {
        public readonly DailyJobData data;

        public DailyJob(DailyJobData date)
        {

            data = date;
        }

        public async Task Execute(IJobExecutionContext context)
        {

            await Program.CheckHourlyChanges(data.status,data.message,data.bot);

        }



    }

    public class DailyJobData
    {
        public string status { get; set; }
        public Message message { get; set; }
        public ITelegramBotClient bot { get; set; }

        public DailyJobData(string status, Message message, ITelegramBotClient bot)
        {
            this.message = message;
            this.status = status;
            this.bot = bot;
        }
    }
}    