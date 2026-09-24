using EventManager.Application.Common.Interfaces;
using EventManager.Domain.Users;
using EventManager.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EventManager.Infrastructure.Bootstrap;

public sealed class DatabaseInitializer
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly BootrstrapAdminOptions _options;

    public DatabaseInitializer(
        IApplicationDbContext db,
        IPasswordHasher passwordHasher,
        IOptions<BootrstrapAdminOptions> options)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _options = options.Value;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            return;
        }

        ValidateOptions();

        var email = 
            Email.Create(
                _options.Email);

        var login = 
            Login.Create(_options.Login);

        var adminAlreadyExists = 
            await _db.UserProfiles
                .AnyAsync(
                    x => x.Email == email,
                    cancellationToken);

        if (adminAlreadyExists)
        {
            return;
        }

        var existinglogin = 
            await _db.Users
                .AnyAsync(
                    x => x.Login == _options.Login,
                    cancellationToken);

        if (existinglogin)
        {
            throw new InvalidOperationException(
                "Bootstrap admin login is already used by another user.");
        }

        var passwordHash = 
            _passwordHasher.Hash(_options.Password);

        var admin = 
            User.CreateAdmin(
                login,
                PasswordHash.FromHash(passwordHash),
                FirstName.Create(_options.FirstName),
                LastName.Create (_options.LastName),
                email,
                _options.BirthDate,
                null
            );

        await _db.Users.AddAsync(admin, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
    }

    private void ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(_options.Login))
        {
            throw new InvalidOperationException(
                "BootrstrapAdmin: login is required.");
        }

        if (string.IsNullOrWhiteSpace(_options.Password))
        {
            throw new InvalidOperationException(
                "BootrstrapAdmin: password is required.");
        }

        if (string.IsNullOrWhiteSpace(_options.Email))
        {
            throw new InvalidOperationException(
                "BootrstrapAdmin: email is required.");
        }

        if (_options.Password.Length < 12)
        {
            throw new InvalidOperationException(
                "BootrstrapAdmin: password must contain at least 12 characters.");
        }

        if (string.IsNullOrWhiteSpace(_options.FirstName))
        {
            throw new InvalidOperationException(
                "BootstrapAdmin: FirstName is required.");
        }

        if (string.IsNullOrWhiteSpace(_options.LastName))
        {
            throw new InvalidOperationException(
                "BootstrapAdmin: LastName is required.");
        }

        if (string.IsNullOrWhiteSpace(_options.Email))
        {
            throw new InvalidOperationException(
                "BootstrapAdmin: Email is required.");
        }
    }
}