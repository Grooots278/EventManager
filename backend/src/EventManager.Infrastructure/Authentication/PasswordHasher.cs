using EventManager.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace EventManager.Infrastructure.Authentication;

public sealed class PasswordHasher
    : IPasswordHasher
{
    private readonly Microsoft.AspNetCore.Identity.PasswordHasher<object>
        _hasher = new();

    public string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException(
                "Password cannot be empty.");

        return _hasher.HashPassword(
            new object(),
            password);
    }

    public bool Verify(
        string password,
        string passwordHash)
    {
        var result = _hasher.VerifyHashedPassword(
            new object(),
            passwordHash,
            password);

        return result ==
            PasswordVerificationResult.Success ||
            result ==
            PasswordVerificationResult.SuccessRehashNeeded;
    }
}
