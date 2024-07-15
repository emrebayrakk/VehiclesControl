using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using VehiclesControl.API.Controllers;
using VehiclesControl.Application.User;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Tests.Car
{
    public class UserControllerTest
    {
        private readonly IUserService _userService;
        private readonly UsersController _userController;
        private readonly IMapper Mapper;

        public UserControllerTest()
        {
            Mapper = A.Fake<IMapper>();
            _userService = A.Fake<IUserService>();
            _userController = new UsersController(_userService, Mapper);
        }

        private static UserRequest CreateFakeUser() => A.Fake<UserRequest>();

        [Fact]
        public void UserController_Create_ReturnCreated()
        {
            var userRequest = CreateFakeUser();

            ApiResponse<long> apiResponse = new ApiResponse<long>(true, 200, "Successfull", 0);
            A.CallTo(
                () => _userService.Create(userRequest)
                ).Returns(apiResponse);

            var result = _userController.Create(userRequest);
            result.Code.Should().Be(200);
            result.Should().NotBeNull();
        }

    }
}
