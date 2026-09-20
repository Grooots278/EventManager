namespace EventManager.API.Contracts.Authentication;

public sealed record LogoutRequest(
    string RefreshToken);