using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taller.Backend.Controllers;                
using Taller.Backend.UnitOfWork.Interfaces;      
using Taller.Backend.UnitsOfWork.Interfaces;
using Taller.Shared.Entities;

namespace Taller.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class EmployeesController : GenericController<Employee>
{
    private readonly IEmployeeUnitOfWork _employeeUnitOfWork;

    public EmployeesController(
        IGenericUnitOfWork<Employee> genericUnitOfWork,
        IEmployeeUnitOfWork employeeUnitOfWork)
        : base(genericUnitOfWork)
    {
        _employeeUnitOfWork = employeeUnitOfWork;
    }


    [HttpGet("search")]
    public async Task<IActionResult> SearchByLetterAsync([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Debe ingresar una cadena para buscar.");

        var action = await _employeeUnitOfWork.SearchByLetterAsync(q);
        if (action.WasSuccess)
            return Ok(action.Result);

        return NotFound(action.Message);
    }

    [HttpGet("totalRecordsByFilter")]
    public async Task<IActionResult> GetTotalRecordsByFilterAsync([FromQuery] string? filter = null)
    {
        var total = await _employeeUnitOfWork.GetTotalRecordsAsync(filter);
        return Ok(total);
    }

    [HttpGet("paginatedByFilter")]
    public async Task<IActionResult> GetPaginatedByFilterAsync(
        [FromQuery] int page = 1,
        [FromQuery(Name = "recordsnumber")] int recordsNumber = 10,
        [FromQuery] string? filter = null)
    {
        var list = await _employeeUnitOfWork.GetPaginatedAsync(page, recordsNumber, filter);
        return Ok(list);
    }
}
