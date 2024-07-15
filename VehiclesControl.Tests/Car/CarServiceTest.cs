using FakeItEasy;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Interfaces;

namespace VehiclesControl.Tests.Car
{
    public class CarServiceTest
    {
        private readonly ICarRepo _carRepo;

        public CarServiceTest()
        {
            _carRepo = A.Fake<ICarRepo>();
        }
        private static CarRequest CreateFakeCar() => A.Fake<CarRequest>();

        [Fact]
        public void CarService_CreateCar_ReturnCreated()
        {
            var carRequest = CreateFakeCar();

            A.CallTo(
                () => _carRepo.Add(carRequest)
                ).Returns(0);
        }
    }
}
