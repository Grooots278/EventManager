namespace EventManager.Application.Authentication.DTOs;

public sealed record AuthenticationResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUts
);