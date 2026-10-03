using EventManager.Domain.Enums;
using EventManager.Domain.Users;
using EventManager.Domain.Users.ValueObjects;

namespace EventManager.Domain.UnitTests.Users;

public sealed class UserTests
{
    [Fact]
    public void Create_ShouldCreateRegularUser()
    {
        var login = 
            Login.Create("Dmitry");

        var passwordHash = 
            PasswordHash.FromHash("Hashed-password");

        var firstName = 
            FirstName.Create("Dmitry");

        var lastName =
            LastName.Create("Ivanov");

        var email = 
            Email.Create("dmitry@example.com");

        var birthDate = 
            new DateOnly(1990, 1, 1);

        var user = 
            User.Create(
                login,
                passwordHash,
                firstName,
                lastName,
                email,
                birthDate,
                null
            );

        Assert.NotEqual(
            Guid.Empty,
            user.Id
        );

        Assert.Equal(
            UserRole.User,
            user.Profile.Role
        );

        Assert.True(
            user.Profile.IsActive
        );

        Assert.Equal(
            email,
            user.Profile.Email
        );

        Assert.Equal(
            login,
            user.Login
        );

        Assert.Equal(
            birthDate,
            user.Profile.BirthDate
        );
    }

    [Fact]
    public void CreateAdmin_ShouldCreateAdmin()
    {
        var user =
            User.CreateAdmin(
                Login.Create("admin"),
                PasswordHash.FromHash("Hashed-password"),
                FirstName.Create("System"),
                LastName.Create("Administrator"),
                Email.Create("admin@example.com"),
                new DateOnly(1990, 1, 1),
                null);

        Assert.Equal(
            UserRole.Admin,
            user.Profile.Role
        );

        Assert.True(
            user.Profile.IsActive
        );
    }

    [Fact]
    public void ChangePassword_ShouldChangeHash()
    {
        var user = 
            User.Create(
                Login.Create("Dmitry"),
                PasswordHash.FromHash("old-hash"),
                FirstName.Create("Dmitry"),
                LastName.Create("Ivanov"),
                Email.Create("dmitry@example.com"),
                new DateOnly(1990, 1, 1),
                null
            );

        user.ChangePassword(
            PasswordHash.FromHash("new-hash"));


        Assert.Equal(
            "new-hash",
            user.PasswordHash.Value
        );
    }

    [Fact]
    public void Deactivate_ShouldDeactivateUser()
    {
        var  user = 
            User.Create(
                Login.Create("Dmitry"),
                PasswordHash.FromHash("old-hash"),
                FirstName.Create("Dmitry"),
                LastName.Create("Ivanov"),
                Email.Create("dmitry@example.com"),
                new DateOnly(1990, 1, 1),
                null
            );

        user.Profile.Deactivate();

        Assert.False(
            user.Profile.IsActive
        );
    }

    [Fact]
    public void Activate_ShouldActivateUser()
    {
        var user = 
            User.Create(
                Login.Create("Dmitry"),
                PasswordHash.FromHash("old-hash"),
                FirstName.Create("Dmitry"),
                LastName.Create("Ivanov"),
                Email.Create("dmitry@example.com"),
                new DateOnly(1990, 1, 1),
                null
            );

        user.Profile.Deactivate();
        user.Profile.Activate();

        Assert.True(
            user.Profile.IsActive
        );
    }
}