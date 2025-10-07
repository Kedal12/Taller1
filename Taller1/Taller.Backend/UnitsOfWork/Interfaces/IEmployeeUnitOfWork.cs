// File: UnitsOfWork/Interfaces/IEmployeeUnitOfWork.cs
using Taller.Shared.Entities;
using Taller.Shared.Responses;

namespace Taller.Backend.UnitsOfWork.Interfaces;

public interface IEmployeeUnitOfWork
{
    Task<ActionResponse<IEnumerable<Employee>>> GetAsync();
    Task<ActionResponse<Employee>> GetAsync(int id);
    Task<ActionResponse<Employee>> AddAsync(Employee entity);
    Task<ActionResponse<Employee>> UpdateAsync(Employee entity);
    Task<ActionResponse<Employee>> DeleteAsync(int id);
    Task<ActionResponse<IEnumerable<Employee>>> SearchByLetterAsync(string letter);
    Task<int> CommitAsync(CancellationToken ct = default);
}
