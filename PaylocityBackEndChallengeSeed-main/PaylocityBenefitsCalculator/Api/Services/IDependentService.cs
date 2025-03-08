using Api.Dtos.Dependent;

namespace Api.Services
{
    public interface IDependentService
    {
        Task<IEnumerable<GetDependentDto>> GetAllAsync();
        Task<GetDependentDto?> GetDependentByIdAsync(int id);
    }
}