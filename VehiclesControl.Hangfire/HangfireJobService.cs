using Hangfire;

namespace VehiclesControl.Hangfire
{
    public class HangfireJobService
    {
        private readonly IRecurringJobManager _recurringJobManager;

        public HangfireJobService(IRecurringJobManager recurringJobManager)
        {
            _recurringJobManager = recurringJobManager;
        }

        public void ScheduleJobs()
        {
            _recurringJobManager.AddOrUpdate<Job>("busList", y => y.SendBusList(), Cron.Daily);
        }
    }
}
