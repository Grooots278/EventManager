namespace EventManager.API.Contracts.Authentication;

public sealed record LoginRequest(
    string Login,
    string Password);