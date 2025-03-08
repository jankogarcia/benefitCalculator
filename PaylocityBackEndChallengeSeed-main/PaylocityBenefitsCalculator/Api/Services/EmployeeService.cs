using Api.Dtos.Employee;
using Api.Extensions;
using Api.Models;
using Api.Repositories;

namespace Api.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly TimeProvider _timeProvider;

        private const int PAYCHECKS_PER_YEAR = 26;
        private const int MONTHS_IN_YEAR = 12;
        private const int YEARS_THRESHOLD = 50;
        private const int SALARY_THRESHOLD = 80000;
        private const decimal BASE_COST_PER_MONTH = 1000m;
        private const decimal DEPENDENT_COST_PER_MONTH = 600m;
        private const decimal ADDITIONAL_COST_PERCENTAGE = 0.02m;
        private const decimal ADDITIONAL_DEPENDENT_COST_PER_MONTH = 200m;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            TimeProvider timeProvider)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public async Task<IEnumerable<GetEmployeeDto>> GetAllAsync()
            => (await _employeeRepository.GetAllAsync()).ToEmployeeDtos();

        public async Task<GetEmployeeDto?> GetEmployeeByIdAsync(int id)
            => (await _employeeRepository.GetEmployeeByIdAsync(id))?.ToEmployeeDto();

        public async Task<GetPaycheckYearDto?> GetPaycheckByIdAsync(int id)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
            return employee is not null
                ? CalculatePaycheck(employee)
                : null;
        }

        private GetPaycheckYearDto CalculatePaycheck(Employee employee)
        {
            // assuming the salary is yearly
            decimal yearlySalary = employee.Salary;

            // employees have a base cost of $1,000 per month(for benefits)
            decimal baseCostPerYear = BASE_COST_PER_MONTH * MONTHS_IN_YEAR;

            // each dependent represents an additional $600 cost per month (for benefits)
            // dependents that are over 50 years old will incur an additional $200 per month
            decimal dependentCostPerYear = employee.Dependents.Sum(d => DEPENDENT_COST_PER_MONTH * MONTHS_IN_YEAR
                + (d.DateOfBirth.AddYears(YEARS_THRESHOLD) <= _timeProvider.GetUtcNow() ? ADDITIONAL_DEPENDENT_COST_PER_MONTH * MONTHS_IN_YEAR : 0));

            // employees that make more than $80,000 per year will incur an additional 2% of their yearly salary in benefits costs
            decimal additionalCost = yearlySalary > SALARY_THRESHOLD ? yearlySalary * ADDITIONAL_COST_PERCENTAGE : 0;

            decimal totalDeductionsPerYear = baseCostPerYear + dependentCostPerYear + additionalCost;
            decimal totalDeductionsPerPaycheck = totalDeductionsPerYear / PAYCHECKS_PER_YEAR;
            decimal grossPayPerPaycheck = yearlySalary / PAYCHECKS_PER_YEAR;
            decimal netPayPerPaycheck = grossPayPerPaycheck - totalDeductionsPerPaycheck;

            var paycheckYearDto = new GetPaycheckYearDto
            {
                YearlySalary = yearlySalary,
                Paychecks = []
            };

            var paycheck = new GetPaycheckDto
            {
                GrossPay = grossPayPerPaycheck,
                Deductions = totalDeductionsPerPaycheck,
                NetPay = netPayPerPaycheck
            };

            // 26 paychecks per year with deductions spread as evenly as possible on each paycheck
            for (int i = 1; i <= PAYCHECKS_PER_YEAR; i++)
            {
                paycheckYearDto.Paychecks[i] = paycheck;
            }

            return paycheckYearDto;
        }
    }
}
