using EventManager.Domain.Enums;

namespace EventManager.Application.Abstractions.Authentication;

public interface ICurrentUser
{
    Guid UserId { get; }
    UserRole Role { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(UserRole role);
}