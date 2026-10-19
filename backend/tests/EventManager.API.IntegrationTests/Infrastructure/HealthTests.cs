using System.Net;

namespace EventManager.API.IntegrationTests.Infrastructure;

[Collection("Auth API")]
public sealed class HealthTests
{
    private readonly ApiFixture _fixture;

    public HealthTests(
        ApiFixture fixture) => 
        _fixture = fixture;

    [Fact]
    public async Task Health_ShouldBeHealthy()
    {
        var response =
            await _fixture.Client
                .GetAsync("/health",
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }
}
