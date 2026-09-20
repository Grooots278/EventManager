namespace EventManager.Application.Abstractions.Authentication;

public interface IRefreshTokenService
{
    string Generate();

    string Hash(string refreshToken);
}