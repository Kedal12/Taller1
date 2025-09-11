using Microsoft.AspNetCore.Mvc;
using Taller.Backend.UnitsOfWork.Interfaces;
using Taller.Shared.Entities;

namespace Taller.Backend.Controllers
{
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
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var action = await _unitOfWork.GetAsync(id);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return NotFound(action.Message);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Employee model)
        {
            var action = await _unitOfWork.AddAsync(model);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync(Employee model)
        {
            var action = await _unitOfWork.UpdateAsync(model);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest(action.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var action = await _unitOfWork.DeleteAsync(id);
            if (action.WasSuccess)
            {
                return NoContent();
            }
            return BadRequest(action.Message);
        }

        // 🔎 Endpoint específico: búsqueda por letra en nombre o apellido
        [HttpGet("search-by-letter/{letter}")]
        public async Task<IActionResult> SearchByLetterAsync(string letter)
        {
            var action = await _unitOfWork.SearchByLetterAsync(letter);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return NotFound(action.Message);
        }
    }
}