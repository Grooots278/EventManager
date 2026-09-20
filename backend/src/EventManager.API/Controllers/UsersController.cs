using EventManager.API.Contracts.Users;
using EventManager.Application.Users.Commands.ChangePassword;
using EventManager.Application.Users.Commands.UpdateProfile;
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

    [HttpPut("me/profile")]
    public async Task<IActionResult> UpdateProfile(
        UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        var command = 
            new UpdateProfileCommand(
                request.FirstName,
                request.LastName,
                request.BirthDate,
                request.CityId);

        await _sender.Send(
            command,
            cancellationToken);

        return NoContent();
    }

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordRequest request, 
        CancellationToken cancellationToken)
    {
        var command = 
            new ChangePasswordCommand(
                request.CurrentPassword,
                request.NewPassword);

        await _sender.Send(command, 
            cancellationToken);

        return NoContent();
    }
}