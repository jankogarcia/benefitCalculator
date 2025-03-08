using Api.Models;

namespace Api.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
    }
}