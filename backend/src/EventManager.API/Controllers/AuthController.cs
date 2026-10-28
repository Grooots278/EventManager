using EventManager.API.Contracts.Authentication;
using EventManager.Application.Authentication.Commands.Login;
using EventManager.Application.Authentication.Commands.Logout;
using EventManager.Application.Authentication.Commands.Refresh;
using EventManager.Application.Authentication.Commands.Register;
using EventManager.Application.Authentication.DTOs;
using EventManager.Application.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private const string RefreshCookieName =
        "eventmanager.refresh";
    
    private const string RefreshCookiePath =
        "/api/auth";

    private readonly ISender _sender;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public AuthController(ISender sender,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        _sender = sender;  
        _configuration = configuration;
        _environment = environment;
    } 

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(
        typeof(AuthTokenResponse),
        StatusCodes.Status200OK
    )]
    public async Task<ActionResult<AuthTokenResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new RegisterCommand(
            request.Login,
            request.Password,
            request.FirstName,
            request.LastName,
            request.Email,
            request.BirthDay,
            request.CityId
        );

        var result = 
            await _sender.Send(
                command,
                cancellationToken
            );

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(ToPublicResponse(result));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(
        typeof(AuthTokenResponse),
        StatusCodes.Status200OK
    )]
    public async Task<ActionResult<AuthTokenResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = 
            new LoginCommand(
                request.Login,
                request.Password);

        var result = 
            await _sender.Send(
                command,
                cancellationToken);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(ToPublicResponse(result));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(
        typeof(AuthTokenResponse),
        StatusCodes.Status200OK
    )]
    public async Task<ActionResult<AuthTokenResponse>> Refresh(
        CancellationToken cancellationToken)
    {
        var refreshToken = 
            Request.Cookies[RefreshCookieName];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new UnauthorizedException(
                "Refresh token is missing.");
        }

        var result = await _sender.Send(
            new RefreshCommand(refreshToken),
            cancellationToken);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(ToPublicResponse(result));
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(
        CancellationToken cancellationToken)
    {
        var refreshToken = 
            Request.Cookies[RefreshCookieName];

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await _sender.Send(
                new LogoutCommand(refreshToken),
                cancellationToken);
        }

        ClearRefreshTokenCookie();
        
        return NoContent();
    }

    private static AuthTokenResponse ToPublicResponse(
        AuthenticationResponse result)
    {
        return new AuthTokenResponse(
            result.AccessToken,
            result.AccessTokenExpiresAtUts);
    }

    private void SetRefreshTokenCookie(
        string refreshToken)
    {
        var refreshTokenDays = 
            _configuration.GetValue<int>(
                "JWT:RefreshTokenDays", 30);

        if (refreshTokenDays <= 0)
        {
            throw new InvalidOperationException(
                "Jwt: RefreshTokenDays must be positive.");
        }

        Response.Cookies.Append(
            RefreshCookieName,
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = _configuration.GetValue<bool?>(
                    "Authentication:RefreshCookieSecure"
                ) ?? _environment.IsProduction(),
                SameSite = SameSiteMode.Lax,
                Path = RefreshCookiePath,
                MaxAge = TimeSpan.FromDays(
                    refreshTokenDays),
                IsEssential = true
            });
    }

    private void ClearRefreshTokenCookie()
    {
        Response.Cookies.Delete(
            RefreshCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = _configuration.GetValue<bool?>(
                    "Authentication:RefreshCookieSecure")
                    ?? _environment.IsProduction(),
                SameSite = SameSiteMode.Lax,
                Path = RefreshCookiePath
            });
    }
}