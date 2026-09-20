using EventManager.Application.Cities.DTO;
using EventManager.Application.Common.CQRS;

namespace EventManager.Application.Cities.Queries.GetCities;

public sealed record GetCitiesQuery
    : IQuery<IReadOnlyList<CityResponse>>;