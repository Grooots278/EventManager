using EventManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
                    x => x.Login.Value == login,
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
}
