using EventManager.API.IntegrationTests.Infrastructure;
using EventManager.Application.Users.DTO;
using System.Net;
using System.Net.Http.Json;

namespace EventManager.API.IntegrationTests.Users;

[Collection("Auth API")]
public sealed class CurrentUserTests
{
    private readonly ApiFixture _fixture;

    public CurrentUserTests(ApiFixture fixture)
        => _fixture = fixture;

    [Fact]
    public async Task GetMe_ShouldReturnAuthenticatedUser()
    {
        var suffix =
            Guid.NewGuid().ToString("N");

        var authentication =
            await _fixture.Client.RegisterAsync(
                $"me_{suffix[..8]}",
                $"{suffix}@test.local");

        using var client =
            _fixture.Factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                authentication.AccessToken);

        var response =
            await client.GetAsync(
                "/api/users/me",
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var result =
            await response.Content
            .ReadFromJsonAsync<
                CurrentUserResponse>(
                TestContext.Current.CancellationToken);

        Assert.NotNull(result);

        Assert.Equal(
            $"me_{suffix[..8]}",
            result!.Login);

        Assert.Equal(
            "User",
            result.Role.ToString());

        Assert.True(
            result.IsActive);
    }
}
