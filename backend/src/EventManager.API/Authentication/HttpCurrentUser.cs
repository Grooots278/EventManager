using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EventManager.Application.Abstractions.Authentication;
using EventManager.Application.Common.Exceptions;

namespace EventManager.API.Authentication;

public sealed class HttpCurrentUser
    : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUser(
        IHttpContextAccessor httpContextAccessor) => 
        _httpContextAccessor = httpContextAccessor;

    public bool IsAuthenticated
    {
        get
        {
            return _httpContextAccessor
                .HttpContext?
                .User
                .Identity?
                .IsAuthenticated
                ?? false;
        }
    }

    public Guid UserId
    {
        get
        {
            var user = GetUser();

            var value = 
                user.FindFirstValue(
                    JwtRegisteredClaimNames.Sub)
                ?? 
                user.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(value, out var userId))
            {
                throw new UnauthorizedException(
                    "User identity is invalid.");
            }

            return userId;
        }
    }

    public string? Role
    {
        get
        {
            return GetUser()
                .FindFirstValue(
                    ClaimTypes.Role);
        }
    }

    private ClaimsPrincipal GetUser()
    {
        var httpContext = 
            _httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            throw new UnauthorizedException(
                "HTTP context is unavailable.");
        }

        if (httpContext.User.Identity?.IsAuthenticated
            != true)
        {
            throw new UnauthorizedException(
                "User is not authenticated");
        }

        return httpContext.User;
    }
}