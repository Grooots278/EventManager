using EventManager.Application.Cities.Queries.GetCities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.API.Controllers;

[ApiController]
[Route("api/cities")]
public sealed class CitiesController : ControllerBase
{
    private readonly ISender _sender;

    public CitiesController(ISender sender)
        => _sender = sender;


    [HttpGet]
    public async Task<IActionResult> Get(
        CancellationToken cancellationToken)
    {
        var result = 
            await _sender.Send(
                new GetCitiesQuery(),
                cancellationToken);

        return Ok(result);
    }
}