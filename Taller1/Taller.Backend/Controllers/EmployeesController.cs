using Microsoft.AspNetCore.Mvc;
using Taller.Backend.UnitsOfWork.Interfaces;
using Taller.Shared.Entities;

namespace Taller.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeUnitOfWork _unitOfWork;

    public EmployeesController(IEmployeeUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var action = await _unitOfWork.GetAsync();
        if (action.WasSuccess)
            return Ok(action.Result);

        return BadRequest(action.Message);
    }

    [HttpGet("{id:int}", Name = "GetEmployeeById")]
    public async Task<IActionResult> GetAsync(int id)
    {
        var action = await _unitOfWork.GetAsync(id);
        if (action.WasSuccess)
            return Ok(action.Result);

        return NotFound(action.Message);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] Employee model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var action = await _unitOfWork.AddAsync(model);
        if (!action.WasSuccess)
            return BadRequest(action.Message);

        await _unitOfWork.CommitAsync(ct);

        var created = action.Result ?? model;
        return CreatedAtRoute("GetEmployeeById", new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutAsync(int id, [FromBody] Employee model, CancellationToken ct = default)
    {
        if (id != model.Id)
            return BadRequest("El ID de la URL no coincide con el del cuerpo.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var action = await _unitOfWork.UpdateAsync(model);
        if (!action.WasSuccess)
            return BadRequest(action.Message);

        await _unitOfWork.CommitAsync(ct);

        return Ok(action.Result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken ct = default)
    {
        var action = await _unitOfWork.DeleteAsync(id);
        if (!action.WasSuccess)
            return BadRequest(action.Message);

        await _unitOfWork.CommitAsync(ct);

        return NoContent();
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByLetterAsync([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Debe ingresar una cadena para buscar.");

        var action = await _unitOfWork.SearchByLetterAsync(q);
        if (action.WasSuccess)
            return Ok(action.Result);

        return NotFound(action.Message);
    }

    [HttpGet("totalRecords")]
    public async Task<IActionResult> GetTotalRecordsAsync([FromQuery] string? filter = null)
    {
        var total = await _unitOfWork.GetTotalRecordsAsync(filter);
        return Ok(total);
    }

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginatedAsync(
        [FromQuery] int page = 1,
        [FromQuery(Name = "recordsnumber")] int recordsNumber = 10,
        [FromQuery] string? filter = null)
    {
        var list = await _unitOfWork.GetPaginatedAsync(page, recordsNumber, filter);
        return Ok(list);
    }
}