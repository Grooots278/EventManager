using EventManager.Application.Abstractions.Authentication;
using EventManager.Application.Common.CQRS;
using EventManager.Application.Common.Exceptions;
using EventManager.Application.Common.Interfaces;
using EventManager.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Application.Users.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler :
    ICommandHandler<ChangePasswordCommand>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IPasswordHasher passwordHasher)
    {
        _db = db;
        _currentUser = currentUser;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(
        ChangePasswordCommand command,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var user = 
            await _db.Users
                .SingleOrDefaultAsync(
                    x => x.Id == userId,
                    cancellationToken);
        
        if (user is null)
        {
            throw new NotFoundException(
                "User was not found.");
        }

        var currentPasswordValid = 
            _passwordHasher.Verify(
                command.CurrentPassword,
                user.PasswordHash.Value);

        if (!currentPasswordValid)
        {
            throw new UnauthorizedException(
                "Current password is invalid.");
        }

        var samePassword = 
            _passwordHasher.Verify(
                command.NewPassword,
                user.PasswordHash.Value);

        if (samePassword)
        {
            throw new ConflictException(
                "New password must differ from current password.");
        }

        var newPasswordHash = 
            _passwordHasher.Hash(
                command.NewPassword);
        
        user.ChangePassword(
            PasswordHash.FromHash(newPasswordHash));

        var refershSession = 
            await _db.RefreshSessions
                .Where(x => x.UserId == userId)
                .ToListAsync(
                    cancellationToken);

        foreach (var session in refershSession)
        {
            session.Revoke();
        }

        await _db.SaveChangesAsync(
            cancellationToken);
    }
}