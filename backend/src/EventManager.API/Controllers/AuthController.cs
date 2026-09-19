using EventManager.API.Contracts.Authentication;
using EventManager.Application.Authentication.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender) => _sender = sender;

    [HttpPost("register")]
    public async Task<ActionResult> Register(
        RegisterRequest request,
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

        return Ok(result);
    }
}