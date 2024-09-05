using FakeItEasy;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Interfaces.EntityFramework;

namespace VehiclesControl.Tests.Bus
{
    public class BusServiceTest
    {
        private readonly IBusRepo _busRepo;

        public BusServiceTest()
        {
            _busRepo = A.Fake<IBusRepo>();
        }
        private static BusRequest CreateFakeBus() => A.Fake<BusRequest>();

        [Fact]
        public void CarService_CreateCar_ReturnCreated()
        {
            var carRequest = CreateFakeBus();

            A.CallTo(
                () => _busRepo.Add(carRequest)
                ).Returns(0);
        }
    }
}
