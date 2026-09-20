using EventManager.API.Contracts.Authentication;
using EventManager.Application.Authentication.Commands.Login;
using EventManager.Application.Authentication.Commands.Logout;
using EventManager.Application.Authentication.Commands.Refresh;
using EventManager.Application.Authentication.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender) => _sender = sender;

    [HttpPost("register")]
    [AllowAnonymous]
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

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult> Login(
        LoginRequest request,
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

        return Ok(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult> Refresh(
        RefreshRequest request,
        CancellationToken cancellationToken)
    {
        var command = 
            new RefreshCommand(request.RefreshToken);

        var result = 
            await _sender.Send(
                command,
                cancellationToken);

        return Ok(result);
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(
        LogoutRequest request,
        CancellationToken cancellationToken)
    {
        var command = 
            new LogoutCommand(
                request.RefreshToken);

        await _sender.Send(
            command,
            cancellationToken);

        return NoContent();
    }
}