namespace VehiclesControl.Hangfire
{
    public class Job
    {
        public void SendBusList()
        {
            Task.Delay(100000000);
            Console.WriteLine("test");
        }
    }
}
