using Api.Data;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly BenefitsDbContext _context;

        public EmployeeRepository(BenefitsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
            => await _context.Employees.Include(e => e.Dependents).ToListAsync();

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
            => await _context.Employees
                .Include(e => e.Dependents)
                .FirstOrDefaultAsync(x => x.Id == id);
    }
}
