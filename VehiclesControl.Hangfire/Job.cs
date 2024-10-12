using VehiclesControl.Application.Bus;

namespace VehiclesControl.Hangfire
{
    public class Job
    {
        private readonly IBusService _busService;

        public Job(IBusService busService)
        {
            _busService = busService;
        }

        public void SendBusList()
        {
            var busListResponse = _busService.BusList();
            if (busListResponse.Status)
            {
                Console.WriteLine("Bus list retrieved successfully");
                foreach (var bus in busListResponse.Data)
                {
                    Console.WriteLine($"Bus ID: {bus.Id}, Color: {bus.Color}");
                }
            }
            else
            {
                Console.WriteLine("Failed to retrieve bus list");
            }
        }
    }
}
