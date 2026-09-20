using EventManager.Application.Abstractions.Authentication;
using EventManager.Application.Common.CQRS;
using EventManager.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Application.Authentication.Commands.Logout;

public sealed class LogoutCommandHandler
    : ICommandHandler<LogoutCommand, Unit>
{
    private readonly IApplicationDbContext _db;
    private readonly IRefreshTokenService _refreshTokenService;

    public LogoutCommandHandler(IApplicationDbContext db, IRefreshTokenService refreshTokenService)
    {
        _db = db;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Unit> Handle(
        LogoutCommand command, 
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
            return Unit.Value;

        session.Revoke();

        await _db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}