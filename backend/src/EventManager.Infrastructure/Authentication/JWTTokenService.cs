using EventManager.Application.Abstractions.Authentication;
using EventManager.Application.Authentication.DTOs;
using EventManager.Application.Common.Interfaces;
using EventManager.Domain.Authentication;
using EventManager.Domain.Users;
using EventManager.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventManager.Infrastructure.Authentication;

public sealed class JWTTokenService : IJWTTokenService
{
    private readonly JwtOptions _options;
    private readonly IApplicationDbContext _db;
    private readonly IRefreshTokenService _refreshTokenService;

    public JWTTokenService(
        IOptions<JwtOptions> options,
        IApplicationDbContext db,
        IRefreshTokenService refreshTokenService)
    {
        _options = options.Value;
        _db = db;
        _refreshTokenService = refreshTokenService;

        ValidateOptions();
    }

    public async Task<AuthenticationResponse>
        CreateAuthenticationResponseAsync(
            User user,
            CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var accessTokenExpires =
            now.AddMinutes(_options.AccessTokenMinutes);

        var jti = Guid.NewGuid().ToString();

        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),
            new Claim(
                JwtRegisteredClaimNames.Jti,
                jti),
            new Claim(
                ClaimTypes.Role,
                user.Profile.Role.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.Secret));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: accessTokenExpires,
            signingCredentials: credentials);

        var accessToken =
            new JwtSecurityTokenHandler()
                .WriteToken(token);

        var refreshToken = _refreshTokenService.Generate();

        var refreshTokenHash =
            _refreshTokenService.Hash(refreshToken);

        var refreshSession =
            RefreshSession.Create(
                user.Id,
                refreshTokenHash,
                now.AddDays(
                    _options.RefreshTokenDays));

        await _db.RefreshSessions.AddAsync(
            refreshSession,
            cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);

        return new AuthenticationResponse(
            accessToken,
            refreshToken,
            accessTokenExpires);
    }

    private void ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(_options.Audience))
            throw new InvalidOperationException(
                "JWT Audience is not configured."
            );

        if (string.IsNullOrWhiteSpace(_options.Issuer))
            throw new InvalidOperationException(
                "JWT Issuer is not configured."
            );

        if (string.IsNullOrWhiteSpace(_options.Secret))
            throw new InvalidOperationException(
                "JWT Secret is not configured."
            );

        if (_options.Secret.Length < 32)
            throw new InvalidOperationException(
                "JWT Secret must be contain at least 32 characters."
            );
    }
}
