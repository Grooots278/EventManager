using EventManager.Domain.Common;

namespace EventManager.Domain.Authentication;

public sealed class RefreshSession : Entity
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public bool IsRevoked => RevokedAtUtc.HasValue;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

    private RefreshSession() { }

    private RefreshSession(
        Guid userId,
        string tokenHash,
        DateTime expiresAtUtc)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static RefreshSession Create(
        Guid userId,
        string tokenHash,
        DateTime expiresAtUtc)
    {

        if (userId == Guid.Empty)
            throw new DomainException(
                "User id cannot be empty.");

        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new DomainException(
                "Refresh token hash is required.");

        if (expiresAtUtc <= DateTime.UtcNow)
            throw new DomainException(
                "Refresh token expiration must be in the future.");

        return new RefreshSession(
            userId,
            tokenHash,
            expiresAtUtc);
    }

    public void Revoke()
    {
        if (RevokedAtUtc.HasValue)
            return;

        RevokedAtUtc = DateTime.UtcNow;
    }
}
