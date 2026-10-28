namespace EventManager.API.Contracts.Authentication;

public sealed record AuthTokenResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc);