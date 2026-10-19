using EventManager.API.IntegrationTests.Infrastructure;
using EventManager.Application.Authentication.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace EventManager.API.IntegrationTests.Authorization;

[Collection("Auth API")]
public sealed class AdminAuthorizationTests
{
    private readonly ApiFixture _fixture;

    public AdminAuthorizationTests(
        ApiFixture fixture) => 
        _fixture = fixture;

    [Fact]
    public async Task Admin_ShouldAccessAdminEndpoint()
    {
        var response =
            await _fixture.Client
            .PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    login = "admin",
                    password =
                        "AdminPassword123456!"
                },
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var authentication =
            await response.Content
            .ReadFromJsonAsync<
                AuthenticationResponse>(
                TestContext.Current.CancellationToken);

        Assert.NotNull(authentication);

        using var client =
            _fixture.Factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                authentication!.AccessToken);

        var adminResponse =
            await client.GetAsync(
                "/api/admin/me",
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.OK,
            adminResponse.StatusCode);
    }

    [Fact]
    public async Task RegularUser_ShouldNotAccessAdminEndpoint()
    {
        var suffix =
            Guid.NewGuid().ToString("N");

        var authentication =
            await _fixture.Client.RegisterAsync(
                $"user_{suffix[..8]}",
                $"{suffix}@test.local");

        using var client = 
            _fixture.Factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                authentication.AccessToken);

        var response = 
            await client.GetAsync(
                "/api/admin/me",
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}
