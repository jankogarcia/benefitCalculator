namespace Api.Dtos.Employee;

public class GetPaycheckYearDto
{
    public decimal YearlySalary { get; set; }
    public Dictionary<int, GetPaycheckDto>? Paychecks { get; set; }
}
