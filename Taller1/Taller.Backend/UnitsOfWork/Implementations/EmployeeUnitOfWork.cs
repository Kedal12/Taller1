using Taller.Backend.Repositories.Interfaces;
using Taller.Backend.UnitsOfWork.Interfaces;
using Taller.Shared.Entities;
using Taller.Shared.Responses;

namespace Taller.Backend.UnitsOfWork.Implementations;

public class EmployeeUnitOfWork : IEmployeeUnitOfWork
{
    private readonly IEmployeeRepository _repository;

    public EmployeeUnitOfWork(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public virtual async Task<ActionResponse<Employee>> AddAsync(Employee entity) =>
        await _repository.AddAsync(entity);

    public virtual async Task<ActionResponse<Employee>> DeleteAsync(int id) =>
        await _repository.DeleteAsync(id);

    public virtual async Task<ActionResponse<Employee>> GetAsync(int id) =>
        await _repository.GetAsync(id);

    public virtual async Task<ActionResponse<IEnumerable<Employee>>> GetAsync() =>
        await _repository.GetAsync();

    public virtual async Task<ActionResponse<Employee>> UpdateAsync(Employee entity) =>
        await _repository.UpdateAsync(entity);

    public virtual async Task<ActionResponse<IEnumerable<Employee>>> SearchByLetterAsync(string letter) =>
        await _repository.SearchByLetterAsync(letter);
}