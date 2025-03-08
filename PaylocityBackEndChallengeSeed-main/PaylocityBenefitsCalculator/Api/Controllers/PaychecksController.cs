using Api.Dtos.Employee;
using Api.Extensions;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PaychecksController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public PaychecksController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [SwaggerOperation(Summary = "Get paycheck by employee id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetPaycheckYearDto>>> Get(int id)
    {
        var paycheck = await _employeeService.GetPaycheckByIdAsync(id);

        return paycheck is null
            ? NotFound()
            : paycheck.ToApiResponse<GetPaycheckYearDto>();
    }
}

