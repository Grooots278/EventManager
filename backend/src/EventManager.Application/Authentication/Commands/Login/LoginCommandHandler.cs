using EventManager.Application.Authentication.DTOs;
using EventManager.Application.Common.CQRS;
using EventManager.Application.Common.Exceptions;
using EventManager.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Application.Authentication.Commands.Login;

public sealed class LoginCommandHandler : 
    ICommandHandler<LoginCommand, AuthenticationResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJWTTokenService _jwtTokenService;

    public LoginCommandHandler(IApplicationDbContext db, IPasswordHasher passwordHasher, IJWTTokenService jwtTokenService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthenticationResponse> Handle(
        LoginCommand command, 
        CancellationToken cancellationToken
    )
    {
        var login = Domain.Users.ValueObjects.Login.Create(command.Login);

        var user = await _db.Users
            .Include(x => x.Profile)
            .SingleOrDefaultAsync(
                x => x.Login == login,
                cancellationToken
            );

        if (user is null)
            throw new UnauthorizedException(
                "Invalid credentials.");

        if (!user.Profile.IsActive)
            throw new UnauthorizedException(
                "Invalid credentials.");

        var passwordValid = 
            _passwordHasher.Verify(
                command.Password,
                user.PasswordHash.Value);

        if (!passwordValid)
            throw new UnauthorizedException(
                "Invalid credentials.");

        return await _jwtTokenService.CreateAuthenticationResponseAsync(
            user,
            cancellationToken);
    }
}