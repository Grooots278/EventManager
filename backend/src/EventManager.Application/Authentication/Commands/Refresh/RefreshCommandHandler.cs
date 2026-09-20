using EventManager.Application.Abstractions.Authentication;
using EventManager.Application.Authentication.DTOs;
using EventManager.Application.Common.CQRS;
using EventManager.Application.Common.Exceptions;
using EventManager.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Application.Authentication.Commands.Refresh;

public sealed class RefreshCommandHandler
    : ICommandHandler<RefreshCommand, AuthenticationResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJWTTokenService _jwtTokenService;

    public RefreshCommandHandler(
        IApplicationDbContext db,
        IRefreshTokenService refreshTokenService,
        IJWTTokenService jwtTokenService)
    {
        _db = db;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthenticationResponse> Handle(
        RefreshCommand command,
        CancellationToken cancellationToken)
    {
        var tokenHash = 
            _refreshTokenService.Hash(
                command.RefreshToken);

        var session = 
            await _db.RefreshSessions
                .SingleOrDefaultAsync(
                    x => x.TokenHash == tokenHash,
                    cancellationToken);

        if (session is null)
        {
            throw new UnauthorizedException(
                "Invalid refresh token.");
        }

        if (!session.IsActive)
            throw new UnauthorizedException(
                "Invalid refresh token.");

        var user = 
            await _db.Users
                .Include(x => x.Profile)
                .SingleOrDefaultAsync(
                    x => x.Id == session.UserId,
                    cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedException(
                "Invalid refresh token.");
        }

        if (!user.Profile.IsActive)
        {
            throw new UnauthorizedException(
                "Invalid refresh token.");
        }

        session.Revoke();

        var response = 
            await _jwtTokenService.CreateAuthenticationResponseAsync(
                user,
                cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);

        return response;
    }
}