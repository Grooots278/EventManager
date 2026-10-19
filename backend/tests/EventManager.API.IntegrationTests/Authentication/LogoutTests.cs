using EventManager.API.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace EventManager.API.IntegrationTests.Authentication;

[Collection("Auth API")]
public sealed class LogoutTests
{
    private readonly ApiFixture _fixture;

    public LogoutTests(
        ApiFixture fixture) => 
        _fixture = fixture;

    [Fact]
    public async Task Logout_ShouldRevokeRefreshToken()
    {
        var suffix =
            Guid.NewGuid().ToString("N");

        var authentication =
            await _fixture.Client.RegisterAsync(
                $"logout_{suffix[..8]}",
                $"{suffix}@test.local");

        var refreshToken =
            authentication.RefreshToken;

        var logoutResponse =
            await _fixture.Client
                .PostAsJsonAsync(
                "/api/auth/logout",
                new
                {
                    refreshToken
                },
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.NoContent,
            logoutResponse.StatusCode);

        var refreshResponse =
            await _fixture.Client
                .PostAsJsonAsync(
                "/api/auth/refresh",
                new
                {
                    refreshToken
                },
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            refreshResponse.StatusCode);
    }
}
