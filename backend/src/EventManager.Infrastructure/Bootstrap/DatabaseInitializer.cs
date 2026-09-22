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

        var adminExsists = 
            await _db.UserProfiles
                .AnyAsync(
                    x => x.Email.Value == 
                        _options.Email.ToLower(),
                        cancellationToken);

        if (adminExsists)
        {
            return;
        }

        var login = 
            Login.Create(_options.Login);

        var email =
            Email.Create(_options.Email);

        var existinglogin = 
            await _db.Users
                .AnyAsync(
                    x => x.Login == _options.Login,
                    cancellationToken);

        if (existinglogin)
        {
            return;
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
                "Bootrstrap admin login is missing.");
        }

        if (string.IsNullOrWhiteSpace(_options.Password))
        {
            throw new InvalidOperationException(
                "Bootrstrap admin password is missing.");
        }

        if (string.IsNullOrWhiteSpace(_options.Email))
        {
            throw new InvalidOperationException(
                "Bootrstrap admin email is missing.");
        }

        if (_options.Password.Length < 12)
        {
            throw new InvalidOperationException(
                "Bootrstrap admin password must contain at least 12 characters.");
        }
    }
}