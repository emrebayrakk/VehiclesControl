using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using VehiclesControl.API.Controllers;
using VehiclesControl.Application.Car;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Tests.Car
{
    public class CarControllerTest
    {
        private readonly ICarService _carService;
        private readonly CarsController _carController;
        private readonly IMapper Mapper;

        public CarControllerTest()
        {
            Mapper = A.Fake<IMapper>();
            _carService = A.Fake<ICarService>();
            _carController = new CarsController(_carService, Mapper);
        }

        private static CarRequest CreateFakeCar() => A.Fake<CarRequest>();

        [Fact]
        public void UserController_Create_ReturnCreated()
        {
            var carRequest = CreateFakeCar();

            ApiResponse<long> apiResponse = new ApiResponse<long>(true, 200, "Successfull", 0);
            A.CallTo(
                () => _carService.CreateCar(carRequest)
                ).Returns(apiResponse);

            var result = _carController.Create(carRequest);
            result.Code.Should().Be(200);
            result.Should().NotBeNull();
        }
    }
}
