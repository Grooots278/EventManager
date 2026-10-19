using EventManager.API.IntegrationTests.Infrastructure;
using EventManager.Application.Authentication.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace EventManager.API.IntegrationTests.Authentication;

[Collection("Auth API")]
public sealed class RegisterTests
{
    private readonly ApiFixture _fixture;

    public RegisterTests(
        ApiFixture fixture)
        => _fixture = fixture;

    [Fact]
    public async Task Register_SholdReturnTokens()
    {
        var suffix =
            Guid.NewGuid().ToString("N");

        var login =
            $"user_{suffix[..8]}";

        var email =
            $"{suffix}@test.local";

        var response = 
            await _fixture.Client.PostAsJsonAsync(
                "/api/auth/register",
                new
                {
                   login,
                   password = "VeryStrongPassword123!",
                   firstName = "Dmitry",
                   lastName = "Test",
                   email,
                   birthDate =
                    new DateOnly(
                        2000, 5, 20),
                    cityId = (Guid?)null
                },
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var result = 
            await response.Content
            .ReadFromJsonAsync<AuthenticationResponse>(
                TestContext.Current.CancellationToken);

        Assert.NotNull(result);

        Assert.False(
            string.IsNullOrWhiteSpace(
                result!.AccessToken));

        Assert.False(
            string.IsNullOrWhiteSpace(
                result.RefreshToken));
    }

    [Fact]
    public async Task Register_ShouldRejectDuplicateLogin()
    {
        var login =
            $"duplicate_{Guid.NewGuid():N}"[..20];

        var first =
            new
            {
                login,
                password =
                    "VeryStrongPassword123!",
                fistName =
                    "First",
                lastName =
                    "User",
                email =
                    $"{Guid.NewGuid():N}@test.local",
                birthDate =
                    new DateOnly(
                        2000, 5, 20),
                cityId = (Guid?)null
            };

        var firstResponse =
            await _fixture.Client
                .PostAsJsonAsync(
                    "/api/auth/register",
                    first,
                    TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.OK,
            firstResponse.StatusCode);

        var second =
            new
            {
                login,
                password =
                    "AnotherStrongPassword123!",
                firstName =
                    "Second",
                lastName =
                    "User",
                email =
                    $"{Guid.NewGuid():N}@test.local",
                birthDate =
                    new DateOnly(
                        2001, 6, 21),
                cityId = (Guid?)null
            };

        var secondResponse = 
            await _fixture.Client
                .PostAsJsonAsync(
                "/api/auth/register",
                second, 
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.Conflict,
            secondResponse.StatusCode);
    }
}
