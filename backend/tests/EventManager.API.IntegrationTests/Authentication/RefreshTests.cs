using EventManager.API.IntegrationTests.Infrastructure;
using EventManager.Application.Authentication.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace EventManager.API.IntegrationTests.Authentication;

[Collection("Auth API")]
public sealed class RefreshTests
{
    private readonly ApiFixture _fixture;

    public RefreshTests(
        ApiFixture fixture) => 
        _fixture = fixture;

    [Fact]
    public async Task Refresh_ShouldRotateRefreshToken()
    {
        var suffix =
            Guid.NewGuid().ToString("N");

        var authentication =
            await _fixture.Client.RegisterAsync(
                $"refresh_{suffix[..8]}",
                $"{suffix}@test.local");

        var oldRefreshToken =
            authentication.RefreshToken;

        var firstRefreshResponse =
            await _fixture.Client
                .PostAsJsonAsync(
                "/api/auth/refresh",
                new
                {
                    RefreshToken = oldRefreshToken
                },
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.OK,
            firstRefreshResponse.StatusCode);

        var rotated =
            await firstRefreshResponse.Content
            .ReadFromJsonAsync<
                AuthenticationResponse>(
                TestContext.Current.CancellationToken);

        Assert.NotNull(rotated);

        Assert.NotEqual(
            oldRefreshToken,
            rotated!.RefreshToken);

        var oldTokenResponse =
            await _fixture.Client
                .PostAsJsonAsync(
                "/api/auth/refresh",
                new
                {
                    refreshToken =
                        oldRefreshToken
                },
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            oldTokenResponse.StatusCode);

        var newTokenResponse =
            await _fixture.Client
            .PostAsJsonAsync(
                "/api/auth/refresh",
                new
                {
                    refreshToken =
                        rotated.RefreshToken
                },
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.OK,
            newTokenResponse.StatusCode);
    }
}
