using EventManager.API.IntegrationTests.Infrastructure;
using EventManager.Application.Authentication.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace EventManager.API.IntegrationTests.Authentication;

[Collection("Auth API")]
public sealed class LoginTests
{
    private readonly ApiFixture _fixture;

    public LoginTests(
        ApiFixture fixture)
    => _fixture = fixture;

    [Fact]
    public async Task Login_ShouldReturnTokens()
    {
        var suffix =
            Guid.NewGuid().ToString("N");

        var login =
            $"login_{suffix[..8]}";

        var email =
            $"{suffix}@test.local";

        await _fixture.Client.RegisterAsync(
            login,
            email);

        var response =
            await _fixture.Client
            .PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    login,
                    password =
                        "VeryStringPassword123!"
                },
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.OK, 
            response.StatusCode);

        var result =
            await response.Content
                .ReadFromJsonAsync<
                    AuthenticationResponse>(
                TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.NotEmpty(result!.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
    }

    [Fact]
    public async Task Login_ShouldRejectInvalidPassword()
    {
        var suffix =
            Guid.NewGuid().ToString("N");

        var login =
            $"login_{suffix[..8]}";

        var email =
            $"{suffix}@test.local";

        await _fixture.Client.RegisterAsync(
            login,
            email);

        var response =
            await _fixture.Client
                .PostAsJsonAsync(
                    "/api/auth/login",
                    new
                    {
                        login,
                        password =
                            "WrongPassword123!"
                    },
                    TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldRejectUnlnowUser()
    {
        var response =
            await _fixture.Client
                .PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    login =
                        "does-not-exist",
                    password =
                        "VeryStrongPassword123!"
                },
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }
}
