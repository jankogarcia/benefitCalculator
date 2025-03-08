using Api.Models;
using Api.Repositories;
using Api.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Api.Tests.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly EmployeeService _employeeService;
        private readonly Mock<TimeProvider> _timeProviderMock;

        public EmployeeServiceTests()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _timeProviderMock = new Mock<TimeProvider>();
            _employeeService = new EmployeeService(_employeeRepositoryMock.Object, _timeProviderMock.Object);
        }

        public static IEnumerable<object[]> GetEmployeeTestData()
        {
            // only base is 12000 year (12000)
            // it has 2 dependents, each one is 600 per month (600 * 12) * 2 = 14400
            // spouse is older than 50 so it has an additional 200 per month (200 * 12) = 2400
            // salary is higher than 80000 so it has an additional 2% of the salary (90000 * 0.02) = 1800
            yield return new object[]
            {
                new Employee
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Salary = 90000m,
                    DateOfBirth = new DateTime(1980, 1, 1),
                    Dependents = new List<Dependent>
                    {
                        new Dependent { Id = 1, FirstName = "Jane", LastName = "Doe", DateOfBirth = new DateTime(1970, 1, 1), Relationship = Relationship.Spouse },
                        new Dependent { Id = 2, FirstName = "Jake", LastName = "Doe", DateOfBirth = new DateTime(2010, 1, 1), Relationship = Relationship.Child }
                    }
                },
                12000 + 14400 + 2400 + 1800
            };

            // only base is 12000 year (12000)
            // it has 3 dependents, each one is 600 per month (600 * 12) * 3 = 21600
            // spouse is not older than 50 so it has no additional cost (0)
            // salary is lower than 80000 so it has no additional cost (0)
            yield return new object[]
            {
                new Employee
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Salary = 75000m,
                    DateOfBirth = new DateTime(1985, 5, 15),
                    Dependents = new List<Dependent>
                    {
                        new Dependent { Id = 3, FirstName = "John", LastName = "Smith", DateOfBirth = new DateTime(1975, 5, 15), Relationship = Relationship.Spouse },
                        new Dependent { Id = 4, FirstName = "Jill", LastName = "Smith", DateOfBirth = new DateTime(2012, 3, 10), Relationship = Relationship.Child },
                        new Dependent { Id = 5, FirstName = "Jimbo", LastName = "Smith", DateOfBirth = new DateTime(2015, 5, 10), Relationship = Relationship.Child }
                    }
                },
                12000 + 21600
            };
        }

        [Theory]
        [MemberData(nameof(GetEmployeeTestData))]
        public async Task GetPaycheckByIdAsync_ShouldReturnPaycheckYearDto_WhenEmployeeExists(Employee employee, decimal expectedDeductionsYear)
        {
            // Arrange
            _employeeRepositoryMock.Setup(repo => repo.GetEmployeeByIdAsync(employee.Id))
                                   .ReturnsAsync(employee);

            var today = new DateTime(2025, 03, 08);
            _timeProviderMock.Setup(tp => tp.GetUtcNow()).Returns(today);

            // Act
            var result = await _employeeService.GetPaycheckByIdAsync(employee.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(employee.Salary, result?.YearlySalary);
            Assert.Equal(26, result!.Paychecks?.Count);

            foreach (var paycheck in result!.Paychecks!.Values)
            {
                Assert.Equal(employee.Salary / 26, paycheck.GrossPay);
                Assert.True(paycheck.Deductions == expectedDeductionsYear / 26);
                Assert.True(paycheck.NetPay == paycheck.GrossPay - (expectedDeductionsYear / 26));
            }
        }

        [Fact]
        public async Task GetPaycheckByIdAsync_ShouldReturnNull_WhenEmployeeDoesNotExist()
        {
            // Arrange
            var employeeId = 1;
            _employeeRepositoryMock.Setup(repo => repo.GetEmployeeByIdAsync(employeeId))
                                   .ReturnsAsync((Employee?)null);

            // Act
            var result = await _employeeService.GetPaycheckByIdAsync(employeeId);

            // Assert
            Assert.Null(result);
        }
    }
}
