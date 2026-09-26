using EventManager.Application.Abstractions.Authentication;
using EventManager.Application.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(
    Policy = 
        AuthorizationPolicies.AdminOnly)]
public sealed class AdminController : ControllerBase
{
    private readonly ICurrentUser _currentUser;

    public AdminController(ICurrentUser currentUser) 
        => _currentUser = currentUser;

    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(
            new
            {
                UserId = _currentUser.UserId,
                Role = _currentUser.Role
            });
    }
}