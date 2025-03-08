using Api.Dtos.Employee;
using Api.Models;

namespace Api.Extensions;
public static class EmployeeExtensions
{
    public static IEnumerable<GetEmployeeDto> ToEmployeeDtos(this IEnumerable<Employee> employees)
        => employees.Select(ToEmployeeDto);

    public static GetEmployeeDto ToEmployeeDto(this Employee employee)
        => new()
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            DateOfBirth = employee.DateOfBirth,
            Salary = employee.Salary,
            Dependents = employee.Dependents.ToGetDependentDtos().ToList()
        };

    public static ApiResponse<T> ToApiResponse<T>(this object obj)
        => new()
        {
            Data = (T)obj,
            Success = true
        };
}
