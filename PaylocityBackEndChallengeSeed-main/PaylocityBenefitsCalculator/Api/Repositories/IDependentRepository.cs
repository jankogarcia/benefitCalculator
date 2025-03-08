using Api.Models;

namespace Api.Repositories
{
    public interface IDependentRepository
    {
        Task<IEnumerable<Dependent>> GetAllAsync();
        Task<Dependent?> GetDependentByIdAsync(int id);
    }
}