using Api.Dtos.Dependent;
using Api.Models;

namespace Api.Extensions;

public static class DependentExtensions
{
    public static IEnumerable<GetDependentDto> ToGetDependentDtos(this IEnumerable<Dependent> dependents)
        => dependents.Select(ToGetDependentDto);

    public static GetDependentDto ToGetDependentDto(this Dependent dependent)
        => new()
        {
            Id = dependent.Id,
            FirstName = dependent.FirstName,
            LastName = dependent.LastName,
            DateOfBirth = dependent.DateOfBirth,
            Relationship = dependent.Relationship
        };
}
