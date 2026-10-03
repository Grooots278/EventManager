using EventManager.Application.Authentication.Commands.Register;
using EventManager.Application.Authentication.DTOs;
using EventManager.Application.Common.Exceptions;
using EventManager.Application.Common.Interfaces;
using EventManager.Domain.Users;
using EventManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Domain.UnitTests.Authentication;

public sealed class RegisterCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateUser()
    {
        await using var db = 
            CreateDbContext();

        var handler = 
            CreateHandler(db);

        var command = 
            CreateCommand();

        var result =await handler.Handle(
            command,
            CancellationToken.None);

        Assert.Equal(
            "access-token",
            result.AccessToken);

        Assert.Equal(
            "refresh-token",
            result.RefreshToken);

        var user = 
            await db.Users
                .Include(x => x.Profile)
                .SingleAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(
            "testuser",
            user.Login.Value);

        Assert.Equal(
            "dmitry@example.com",
            user.Profile.Email.Value);

        Assert.Equal(
            "User",
            user.Profile.Role.ToString());

        Assert.NotEqual(
            command.Password,
            user.PasswordHash.Value);
    }

    [Fact]
    public async Task Handle_ShouldRejectDuplicateLogin()
    {
        await using var db = 
            CreateDbContext();

        var handler = CreateHandler(db);

        await handler.Handle(
            CreateCommand(),
            CancellationToken.None);

        var duplicate = 
            new RegisterCommand(
                "testuser",
                "AnotherVeryStrongPassword123!",
                "Other",
                "User",
                "other@example.com",
                new DateOnly(1990, 1, 1),
                null);
            
            await Assert.ThrowsAsync<ConflictException>(
                () => 
                    handler.Handle(
                        duplicate,
                        CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldRejectUnknownCity()
    {
        await using var db = 
            CreateDbContext();

        var handler = CreateHandler(db);

        var command = 
            CreateCommand(
                cityId: Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(
                () => 
                    handler.Handle(
                        command,
                        CancellationToken.None));
    }

    private static RegisterCommand CreateCommand(
        Guid? cityId = null
    )
    {
        return new RegisterCommand(
            "testuser",
            "VeryStrongPassword123!",
            "Dmitry",
            "Ivanov",
            "dmitry@example.com",
            new DateOnly(2000, 1, 1),
            cityId);
    }

    private static RegisterCommandHandler CreateHandler(
        IApplicationDbContext db)
    {
        return new RegisterCommandHandler(
            db,
            new FakePasswordHasher(),
            new FakeJwtTokenGenerator());
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private sealed class FakePasswordHasher
        : IPasswordHasher
    {
        public string Hash(string password)
        {
            return $"HASH::{password}";
        }

        public bool Verify(string password, string hash)
        {
            return password == $"HASH::{hash}";
        }
    }

    private sealed class FakeJwtTokenGenerator
        : IJWTTokenService
    {
        public Task<AuthenticationResponse> CreateAuthenticationResponseAsync(
            User user,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new AuthenticationResponse(
                "access-token",
                "refresh-token",
                DateTime.UtcNow.AddMinutes(15)));
        }
    }
}