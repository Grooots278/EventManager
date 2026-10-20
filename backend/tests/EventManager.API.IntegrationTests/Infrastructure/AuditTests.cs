using EventManager.Application.Authentication.DTOs;
using EventManager.Domain.Users.ValueObjects;
using EventManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace EventManager.API.IntegrationTests.Infrastructure;

[Collection("Auth API")]
public sealed class AuditTests
{
    private readonly ApiFixture _fixture;

    public AuditTests(
        ApiFixture fixture) => 
        _fixture = fixture;

    [Fact]
    public async Task Register_ShouldSetCreatedAt()
    {
        var suffix =
            Guid.NewGuid().ToString("N");

        var login =
            $"audit_{suffix[..8]}";

        var email =
            $"{suffix}@test.local";

        await _fixture.Client
            .PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                login,
                password =
                    "VeryStrongPassword123!",
                firstName =
                    "Audit",
                lastName =
                    "Test",
                email,
                birthDate =
                    new DateOnly(
                        2000, 5, 20),
                cityId = (Guid?)null
            },
            TestContext.Current.CancellationToken);

        await using var scope =
            _fixture.Factory.Services
                .CreateAsyncScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<
                    AppDbContext>();

        var user =
            await db.Users
                .Include(x => x.Profile)
                .SingleAsync(
                    x => x.Login == Login.Create(login),
                    TestContext.Current.CancellationToken);

        Assert.NotEqual(
            default,
            user.CreatedAtUtc);

        Assert.Null(
            user.UpdatedAtUtc);

        Assert.NotEqual(
            default,
            user.Profile.CreatedAtUtc);

        Assert.Null(
            user.Profile.UpdatedAtUtc);
    }

    [Fact]
    public async Task ProfileUpdate_ShouldSetUpdateAt()
    {
        var suffix =
            Guid.NewGuid().ToString("N");

        var login =
            $"audit_update_{suffix[..6]}";

        var email =
            $"{suffix}@test.local";

        await _fixture.Client
            .PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                login,
                password =
                    "VeryStrongPassword123!",
                firstName =
                    "Before",
                lastName =
                    "Test",
                email,
                birthDate =
                    new DateOnly(
                        2000, 5, 20),
                cityId = (Guid?)null
            },
            TestContext.Current.CancellationToken);

        DateTime createdAt;

        await using (
            var scope = 
                _fixture.Factory.Services
                    .CreateAsyncScope())
        {
            var db =
                scope.ServiceProvider
                    .GetRequiredService<
                        AppDbContext>();

            var user =
                await db.Users
                    .Include(x => x.Profile)
                    .SingleAsync(
                        x => x.Login == Login.Create(login),
                    TestContext.Current.CancellationToken);

            createdAt = user.Profile.CreatedAtUtc;

            Assert.Null(
                user.Profile.UpdatedAtUtc);
        }

        using var client =
            _fixture.Factory.CreateClient();

        var loginResponse =
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

        var authentication =
            await loginResponse.Content
                .ReadFromJsonAsync<
                    AuthenticationResponse>(
                TestContext.Current.CancellationToken);

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                authentication!.AccessToken);

        var updateResponse =
            await client.PutAsJsonAsync(
                "/api/users/me/profile",
                new
                {
                    firstName =
                        "After",
                    lastName =
                        "Changed",
                    birthDate =
                        new DateOnly(
                            2000, 5, 20),
                    cityId = (Guid?)null
                },
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.NoContent,
            updateResponse.StatusCode);

        await using (
            var scope =
                _fixture.Factory.Services
                    .CreateAsyncScope())
        {
            var db =
                scope.ServiceProvider
                    .GetRequiredService<
                        AppDbContext>();

            var user =
                await db.Users
                    .Include(x => x.Profile)
                    .SingleAsync(
                        x => x.Login == Login.Create(login),
                        TestContext.Current.CancellationToken);

            Assert.Equal(
                createdAt,
                user.Profile.CreatedAtUtc);

            Assert.NotNull(
                user.Profile.UpdatedAtUtc);
        }
    }
}
