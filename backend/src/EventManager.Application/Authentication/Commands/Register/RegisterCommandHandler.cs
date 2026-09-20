using EventManager.Application.Authentication.DTOs;
using EventManager.Application.Common.CQRS;
using EventManager.Application.Common.Exceptions;
using EventManager.Application.Common.Interfaces;
using EventManager.Domain.Users;
using EventManager.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Application.Authentication.Commands.Register;

public sealed class RegisterCommandHandler
    : ICommandHandler<
        RegisterCommand,
        AuthenticationResponse>
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJWTTokenService _jwtTokenService;

    public RegisterCommandHandler(
        IApplicationDbContext db,
        IPasswordHasher passwordHasher,
        IJWTTokenService jwtTokenService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthenticationResponse> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var login = Domain.Users.ValueObjects.Login.Create(command.Login);
        var email = Email.Create(command.Email);
        var firstName = FirstName.Create(command.FirstName);
        var lastName = LastName.Create(command.LastName);

        var loginExists = await _db.Users
            .AnyAsync(
            x => x.Login == login, cancellationToken);

        if (loginExists)
        {
            throw new ConflictException(
                "Login is already taken.");
        }

        var emailExists = await _db.UserProfiles
            .AnyAsync(x => x.Email == email, cancellationToken);

        if (emailExists)
        {
            throw new ConflictException(
                "Email is already taken.");
        }

        if (command.CityId.HasValue)
        {
            var cityExists = await _db.Cities
                .AnyAsync(x => x.Id == command.CityId.Value, cancellationToken);

            if (!cityExists)
                throw new NotFoundException(
                    "City was not found.");
        }

        var passwordHash =
            _passwordHasher.Hash(command.Password);

        var passwordHashObject =
            PasswordHash.FromHash(passwordHash);

        var user = User.Create(
            login,
            passwordHashObject,
            firstName,
            lastName,
            email,
            command.BirthDate,
            command.CityId);

        await _db.Users.AddAsync(user, cancellationToken);

        var authenticationResponse = 
            await _jwtTokenService.CreateAuthenticationResponseAsync(
                user, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);

        return authenticationResponse; 
    }
}
