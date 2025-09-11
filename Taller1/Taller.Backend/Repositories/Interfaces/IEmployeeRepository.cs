using Taller.Shared.Entities;
using Taller.Shared.Responses;

namespace Taller.Backend.Repositories.Interfaces;

public interface IEmployeeRepository
{
    Task<ActionResponse<Employee>> GetAsync(int id);

    Task<ActionResponse<IEnumerable<Employee>>> GetAsync();

    Task<ActionResponse<Employee>> AddAsync(Employee entity);

    Task<ActionResponse<Employee>> DeleteAsync(int id);

    Task<ActionResponse<Employee>> UpdateAsync(Employee entity);

    Task<ActionResponse<IEnumerable<Employee>>> SearchByLetterAsync(string letter);
}