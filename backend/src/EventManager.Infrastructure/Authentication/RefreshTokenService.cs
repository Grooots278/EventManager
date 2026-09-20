using System.Security.Cryptography;
using System.Text;
using EventManager.Application.Abstractions.Authentication;

namespace EventManager.Infrastructure.Authentication;

public sealed class RefreshTokenService
    : IRefreshTokenService
{
    public string Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes);
    }

    public string Hash(string refreshToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            refreshToken);

        var hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(refreshToken));

        return Convert.ToHexString(hash);
    }
}