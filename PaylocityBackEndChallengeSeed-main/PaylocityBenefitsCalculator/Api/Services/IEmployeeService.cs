using Api.Dtos.Employee;

namespace Api.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<GetEmployeeDto>> GetAllAsync();
        Task<GetEmployeeDto?> GetEmployeeByIdAsync(int id);
        Task<GetPaycheckYearDto?> GetPaycheckByIdAsync(int id);
    }
}