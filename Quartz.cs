using System;
using Quartz;
using System.Threading.Tasks;
using Quartz.Impl;



public class Scheduler
{
	public async Task StartSchedluer()
	{

		IShcedulerFactory sch = new();
		IScheduler shcd = await sch.GetScheduler();
		await shcd.Start();

		IJobDetail job = JobBuilder.Create<DailyJob>().Build();

		ITrigger trigger = TriggerBuilder.Create()
			.WithIdentity("dalyTrigger", "group1")
			.StartNow()
			.WitghSchedule(CronScheduleBuilder.DailyAtHourAndMinute(0, 0))
			.Build();

		await shcd.ScheduleJob(job, trigger);

	}

	
}

public class DailyJob : IJob
{
	public async Task Execute(IJobExecutionContext context)
	{

       await Program.CheckHourlyChanges();

    }
	
}


