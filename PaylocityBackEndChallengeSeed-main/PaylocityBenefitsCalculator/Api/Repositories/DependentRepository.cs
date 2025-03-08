using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class DependentRepository : IDependentRepository
    {
        private readonly BenefitsDbContext _context;
        public DependentRepository(BenefitsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dependent>> GetAllAsync()
            => await _context.Dependents.ToListAsync();

        public async Task<Dependent?> GetDependentByIdAsync(int id)
            => await _context.Dependents.FirstOrDefaultAsync(x => x.Id == id);
    }
}
