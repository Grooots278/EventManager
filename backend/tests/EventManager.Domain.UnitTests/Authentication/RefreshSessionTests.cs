using EventManager.Domain.Authentication;
using EventManager.Domain.Common;

namespace EventManager.Domain.UnitTests.Authentication;

public sealed class RefreshSessionTests
{
    [Fact]
    public void Create_ShouldCreateActiveSession()
    {
        var userId = Guid.NewGuid();

        var expiresAt = 
            DateTime.UtcNow.AddDays(30);

        var session = 
            RefreshSession.Create(
                userId,
                "refresh-token-hash",
                expiresAt);

        Assert.Equal(
            userId,
            session.UserId
        );

        Assert.Equal(
            "refresh-token-hash",
            session.TokenHash
        );

        Assert.False(
            session.IsRevoked
        );

        Assert.False(
            session.IsExpired
        );

        Assert.True(
            session.IsActive
        );

        Assert.Null(
            session.RevokedAtUtc
        );
    }

    [Fact]
    public void Revoke_ShouldRevokeSession()
    {
        var session = 
            RefreshSession.Create(
                Guid.NewGuid(),
                "refresh-token-hash",
                DateTime.UtcNow.AddDays(30));

        session.Revoke();

        Assert.True(
            session.IsRevoked
        );

        Assert.False(
            session.IsActive
        );

        Assert.NotNull(
            session.RevokedAtUtc
        );
    }

    [Fact]
    public void Create_ShouldRejectExpiredDate()
    {
        Assert.Throws<DomainException>(
            () => RefreshSession.Create(
                Guid.NewGuid(),
                "refresh-token-hash",
                DateTime.UtcNow.AddMinutes(-1))
        );
    }

    [Fact]
    public void Revoke_ShouldBeIdempotent()
    {
        var session = 
            RefreshSession.Create(
                Guid.NewGuid(),
                "refresh-token-hash",
                DateTime.UtcNow.AddDays(30));

        session.Revoke();

        var firstRevokedAt = 
            session.RevokedAtUtc;

        session.Revoke();

        Assert.Equal(
            firstRevokedAt,
            session.RevokedAtUtc
        );
    }
}