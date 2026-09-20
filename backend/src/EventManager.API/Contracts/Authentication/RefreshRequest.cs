namespace EventManager.API.Contracts.Authentication;

public sealed record RefreshRequest(
    string RefreshToken);