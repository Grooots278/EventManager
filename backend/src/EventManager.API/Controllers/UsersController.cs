using EventManager.Application.Users.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.API.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender) => 
        _sender = sender;

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser(
        CancellationToken cancellationToken)
    {
        var result = 
            await _sender.Send(
                new GetCurrentUserQuery(),
                cancellationToken);

        return Ok(result);
    }
}