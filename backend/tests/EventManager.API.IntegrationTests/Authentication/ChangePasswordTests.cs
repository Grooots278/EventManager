using EventManager.API.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace EventManager.API.IntegrationTests.Authentication;

[Collection("Auth API")]
public sealed class ChangePasswordTests
{
    private readonly ApiFixture _fixture;

    public ChangePasswordTests(
        ApiFixture fixture) =>
        _fixture = fixture;

    [Fact]
    public async Task ChangePassword_ShouldAllowLoginWithNewPassword()
    {
        var suffix = 
            Guid.NewGuid().ToString("N");

        var login =
            $"password_{suffix[..8]}";

        var email =
            $"{suffix}@test.local";

        var authentication =
            await _fixture.Client.RegisterAsync(
                login,
                email);

        using var authenticatedClient =
            _fixture.Factory.CreateClient();

        authenticatedClient
            .DefaultRequestHeaders
            .Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                authentication.AccessToken);

        var changeResponse =
            await authenticatedClient
                .PutAsJsonAsync(
                "/api/users/me/password",
                new
                {
                    currentPassword =
                        "VeryStrongPassword123!",
                    newPassword =
                        "EventStrongerPassword456!"
                },
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.NoContent,
            changeResponse.StatusCode);

        var oldPasswordLogin =
            await _fixture.Client
            .PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    login,
                    password =
                        "VeryStrongPassword123!"
                },
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            oldPasswordLogin.StatusCode);

        var newPasswordLogin =
            await _fixture.Client
                .PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    login,
                    password =
                        "EventStrongerPassword456!"
                },
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.OK,
            newPasswordLogin.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_ShouldInvalidateOldRefreshToken()
    {
        var suffix =
            Guid.NewGuid().ToString("N");

        var authentication =
            await _fixture.Client.RegisterAsync(
                $"revoke_{suffix[..8]}",
                $"{suffix}@test.local");

        using var client =
            _fixture.Factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                authentication.AccessToken);

        var changeResponse =
            await client.PutAsJsonAsync(
                "/api/users/me/password",
                new
                {
                    currentPassword =
                        "VeryStrongPassword123!",
                    newPassword =
                        "EventStrongerPassword456!"
                },
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.NoContent,
            changeResponse.StatusCode);

        var refreshResponse =
            await _fixture.Client
            .PostAsJsonAsync(
                "/api/auth/refresh",
                new
                {
                    refreshToken =
                        authentication.RefreshToken
                },
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            refreshResponse.StatusCode);
    }
}
