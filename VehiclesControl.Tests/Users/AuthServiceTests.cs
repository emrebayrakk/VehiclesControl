using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Application.Users;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Enums;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;
using VehiclesControl.Tests.Support;

namespace VehiclesControl.Tests.Users;

public sealed class AuthServiceTests
{
    private readonly TestDb _db = new();
    private readonly PasswordHasher<User> _hasher = new();
    private readonly IJwtProvider _jwt = A.Fake<IJwtProvider>();

    public AuthServiceTests() =>
        A.CallTo(() => _jwt.CreateToken(A<User>._)).Returns(("token", DateTime.UtcNow.AddHours(1)));

    private async Task SeedAsync(bool galleryActive = true, bool userActive = true)
    {
        await using var system = _db.System();
        var gallery = new Gallery { Id = 1, Name = "A", IsActive = galleryActive };
        var user = new User { GalleryId = 1, Gallery = gallery, FirstName = "Ada", LastName = "Lovelace", Email = "ada@galeri.local", Role = UserRole.GalleryOwner, IsActive = userActive };
        user.PasswordHash = _hasher.HashPassword(user, "Secret123");
        system.Users.Add(user);
        await system.SaveChangesAsync();
    }

    private AuthService Service(IAppDbContext db) => new(db, _hasher, _jwt, TimeProvider.System);

    [Fact]
    public async Task Valid_login_returns_token_and_records_last_login()
    {
        await SeedAsync();
        await using var ctx = _db.System();

        var result = await Service(ctx).LoginAsync(new LoginRequest { Email = " ADA@galeri.local ", Password = "Secret123" });

        result.Code.Should().Be(ResultCode.Ok);
        result.Data!.Token.Should().Be("token");
        result.Data.User.GalleryName.Should().Be("A");
        (await ctx.Users.SingleAsync()).LastLoginAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Wrong_password_returns_401()
    {
        await SeedAsync();
        await using var ctx = _db.System();

        (await Service(ctx).LoginAsync(new LoginRequest { Email = "ada@galeri.local", Password = "nope" })).Code.Should().Be(ResultCode.Unauthorized);
    }

    [Fact]
    public async Task Users_of_a_deactivated_gallery_cannot_sign_in()
    {
        await SeedAsync(galleryActive: false);
        await using var ctx = _db.System();

        (await Service(ctx).LoginAsync(new LoginRequest { Email = "ada@galeri.local", Password = "Secret123" })).Code.Should().Be(ResultCode.Forbidden);
    }

    [Fact]
    public async Task Deactivated_users_cannot_sign_in()
    {
        await SeedAsync(userActive: false);
        await using var ctx = _db.System();

        (await Service(ctx).LoginAsync(new LoginRequest { Email = "ada@galeri.local", Password = "Secret123" })).Code.Should().Be(ResultCode.Forbidden);
    }

    [Fact]
    public async Task Changing_password_requires_the_current_one()
    {
        await SeedAsync();
        await using var ctx = _db.System();
        var userId = (await ctx.Users.SingleAsync()).Id;
        var request = new UpdateProfileRequest { FirstName = "Ada", LastName = "L", Email = "ada@galeri.local", NewPassword = "NewSecret1", CurrentPassword = "wrong" };

        (await Service(ctx).UpdateProfileAsync(userId, request)).Code.Should().Be(ResultCode.BadRequest);

        request.CurrentPassword = "Secret123";
        (await Service(ctx).UpdateProfileAsync(userId, request)).Code.Should().Be(ResultCode.Ok);
        (await Service(ctx).LoginAsync(new LoginRequest { Email = "ada@galeri.local", Password = "NewSecret1" })).Status.Should().BeTrue();
    }
}
