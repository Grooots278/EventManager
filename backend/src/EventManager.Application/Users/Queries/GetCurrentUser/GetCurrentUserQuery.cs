using EventManager.Application.Common.CQRS;
using EventManager.Application.Users.DTO;

namespace EventManager.Application.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery
    : IQuery<CurrentUserResponse>;