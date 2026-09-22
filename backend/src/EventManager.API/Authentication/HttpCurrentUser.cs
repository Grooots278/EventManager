using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EventManager.Application.Abstractions.Authentication;
using EventManager.Application.Common.Exceptions;
using EventManager.Domain.Enums;

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

    public UserRole Role
    {
        get
        {
            var principal = GetPrincipal();

            var roleValue = 
                principal.FindFirstValue(
                    ClaimTypes.Role);

            if(!Enum.TryParse<UserRole>(
                roleValue, 
                out var role))
            {
                throw new UnauthorizedException(
                    "User role is invalid.");
            }

            return role;
        }
    }

    public bool IsInRole(UserRole role)
        => Role == role;

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

    private HttpContext? HttpContext => 
        _httpContextAccessor.HttpContext;

    private ClaimsPrincipal GetPrincipal()
    {
        var context = HttpContext;

        if (context is null)
        {
            throw new UnauthorizedException(
                "HTTP context is unavailable.");
        }

        if (context.User.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedException(
                "User is not authorized.");
        }

        return context.User;
    }
}