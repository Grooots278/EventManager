using Xunit;

namespace EventManager.API.IntegrationTests.Infrastructure;

[CollectionDefinition("Auth API", 
    DisableParallelization = true)]
public sealed class AuthApiCollection 
    : ICollectionFixture<ApiFixture>
{
}

