using FluentAssertions;
using VehiclesControl.Application.Validation;
using VehiclesControl.Domain.Requests;

namespace VehiclesControl.Tests.Validation;

public sealed class ValidatorTests
{
    private static VehicleRequest ValidVehicle() => new()
    {
        Brand = "Renault", Model = "Clio", Year = 2021, Kilometers = 1000, Color = "Beyaz", PurchasePrice = 1, ListPrice = 2,
        PurchaseDate = DateTime.Today,
    };

    [Fact]
    public void Valid_vehicle_passes() => new VehicleRequestValidator().Validate(ValidVehicle()).IsValid.Should().BeTrue();

    [Theory]
    [InlineData("SHORTVIN")]
    [InlineData("WVWZZZ1KZAW00000012")]
    public void Vin_must_be_17_characters(string vin)
    {
        var request = ValidVehicle();
        request.Vin = vin;
        new VehicleRequestValidator().Validate(request).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Deposit_cannot_exceed_sale_price() =>
        new SaleRequestValidator().Validate(new SaleRequest { VehicleId = 1, CustomerId = 1, SalePrice = 100, Deposit = 200 })
            .IsValid.Should().BeFalse();

    [Theory]
    [InlineData("short1A", false)]
    [InlineData("alllowercase1", false)]
    [InlineData("Strong123", true)]
    public void Team_member_password_must_be_strong(string password, bool valid) =>
        new TeamMemberRequestValidator()
            .Validate(new TeamMemberRequest { FirstName = "A", LastName = "B", Email = "a@b.com", Password = password })
            .IsValid.Should().Be(valid);

    [Fact]
    public void New_password_requires_current_password() =>
        new UpdateProfileRequestValidator()
            .Validate(new UpdateProfileRequest { FirstName = "A", LastName = "B", Email = "a@b.com", NewPassword = "Strong123" })
            .IsValid.Should().BeFalse();
}
