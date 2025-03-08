using Api.Dtos.Dependent;
using Api.Extensions;
using Api.Repositories;

namespace Api.Services
{
    public class DependentService : IDependentService
    {
        private readonly IDependentRepository _dependentRepository;

        public DependentService(IDependentRepository dependentRepository)
        {
            _dependentRepository = dependentRepository;
        }

        public async Task<IEnumerable<GetDependentDto>> GetAllAsync()
            => (await _dependentRepository.GetAllAsync()).ToGetDependentDtos();

        public async Task<GetDependentDto?> GetDependentByIdAsync(int id)
        {
            var dependent = await _dependentRepository.GetDependentByIdAsync(id);
            return dependent is not null
                ? dependent.ToGetDependentDto()
                : null;
        }
    }
}
