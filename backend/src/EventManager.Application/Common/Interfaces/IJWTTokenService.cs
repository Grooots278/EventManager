using EventManager.Application.Authentication.DTOs;
using EventManager.Domain.Users;

namespace EventManager.Application.Common.Interfaces;

public interface IJWTTokenService
{
    Task<AuthenticationResponse>
        CreateAuthenticationResponseAsync(
            User user,
            CancellationToken cancellationToken);
}
