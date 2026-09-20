namespace EventManager.Application.Abstractions.Authentication;

public interface ICurrentUser
{
    Guid UserId { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}