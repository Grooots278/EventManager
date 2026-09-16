using EventManager.Domain.Abstractions;
using EventManager.Domain.Common;
using EventManager.Domain.Users.ValueObjects;

namespace EventManager.Domain.Users;

public sealed class User : AggregateRoot, IAuditable
{
    public Login Login { get; private set; } = null!;
    public PasswordHash PasswordHash { get; private set; } = null!;
    public UserProfile Profile { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    private User() { }

    private User(
        Login login,
        PasswordHash password,
        UserProfile profile)
    {
        Login = login;
        PasswordHash = password;
        Profile = profile;
    }

    public static User Create(
        Login login,
        PasswordHash passwordHash,
        FirstName firstName,
        LastName lastName,
        Email email,
        DateOnly birthDate,
        Guid? cityId)
    {
        var user = new User(
            login,
            passwordHash,
            null!);

        user.Profile = new UserProfile(
            user.Id,
            firstName,
            lastName,
            email,
            birthDate,
            cityId);

        return user;
    }

    public void ChangePassword(PasswordHash passwordHash)
    {
        if (PasswordHash == passwordHash) return;
        PasswordHash = passwordHash;
    }
}
