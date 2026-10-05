using System.Net.Http.Headers;
using System.Net.Http.Json;
using EventManager.Application.Authentication.DTOs;

namespace EventManager.API.IntegrationTests.Infrastructure;

public static class HttpClientExtensions
{
    public static async Task<
        AuthenticationResponse> RegisterAsync(
            this HttpClient client,
            string login,
            string email)
    {
        var response = 
            await client.PostAsJsonAsync(
                "/api/auth/register",
                new
                {
                    login = login,
                    password = "VeryStrongPassword123!",
                    firstName = "Dmitry",
                    lastName = "Ivanov",
                    email = email,
                    birthDate = 
                        new DateOnly(
                            2000, 5, 20),
                    cityId = 
                        (Guid?)null
                });

        response.EnsureSuccessStatusCode();

        var result = 
            await response.Content.ReadFromJsonAsync<
                AuthenticationResponse>();

        return result
            ?? throw new InvalidOperationException(
                "Authentication response is null.");
    }

    public static async Task<
        HttpClient>
        CreateAuthenticationClientAsync(
            this ApiFixture fixture,
            AuthenticationResponse authentication)
    {
        var client = 
            fixture.Factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue(
                "Bearer", 
                authentication.AccessToken);

        return client;
    }
}