using EventManager.Application.Cities.DTO;
using EventManager.Application.Common.CQRS;
using EventManager.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Application.Cities.Queries.GetCities;

public sealed class GetCitiesQueryHandler
    : IQueryHandler<GetCitiesQuery,
        IReadOnlyList<CityResponse>>
{
    private readonly IApplicationDbContext _db;

    public GetCitiesQueryHandler(IApplicationDbContext db)
        => _db = db;

    public async Task<IReadOnlyList<CityResponse>> Handle(
        GetCitiesQuery query,
        CancellationToken cancellationToken)
    {
        return await _db.Cities
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => 
                new CityResponse(
                    x.Id,
                    x.Name
                ))
            .ToListAsync(cancellationToken);
    }
}