using Taller.Shared.Entities;
using Taller.Shared.Responses;

namespace Taller.Backend.UnitsOfWork.Interfaces;

public interface IEmployeeUnitOfWork
{
    Task<ActionResponse<IEnumerable<Employee>>> GetAsync();

    Task<ActionResponse<Employee>> AddAsync(Employee model);

    Task<ActionResponse<Employee>> UpdateAsync(Employee model);

    Task<ActionResponse<Employee>> DeleteAsync(int id);

    Task<ActionResponse<Employee>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Employee>>> SearchByLetterAsync(string letter);
}