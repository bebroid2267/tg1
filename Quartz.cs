using System;
using Quartz;
using System.Threading.Tasks;
using Quartz.Impl;
using tg1;
using System.Net.NetworkInformation;
using Telegram.Bot;
using Telegram.Bot.Types;



public class Scheduler
{
    


    public async Task StartScheduler(string status, Message message, ITelegramBotClient bot)
	{

        ISchedulerFactory sch = new StdSchedulerFactory();
		IScheduler shcd = await sch.GetScheduler();
		await shcd.Start();
		

		IJobDetail job = JobBuilder.Create<DailyJob>().Build();

		ITrigger trigger = TriggerBuilder.Create()
			.WithIdentity("dalyTrigger", "group1")
			.StartNow()
			.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(0, 0))
			.Build();

		await shcd.ScheduleJob(job, trigger);

	}

	
}

public class DailyJob : IJob
{
	string status;
	Message message; 
	ITelegramBotClient bot;

	public DailyJob(string status, Message message, ITelegramBotClient bot)
	{
		this.status = status;
		this.message = message;
		this.bot = bot;



	}

	public async Task Execute(IJobExecutionContext context)
    {

       await Program.CheckHourlyChanges(status,  message,  bot);

    }
	
}


