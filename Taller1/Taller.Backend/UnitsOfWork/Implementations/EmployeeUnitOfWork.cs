using Taller.Backend.Data;
using Taller.Backend.Repositories.Interfaces;
using Taller.Backend.UnitsOfWork.Interfaces;
using Taller.Shared.Entities;
using Taller.Shared.Responses;

namespace Taller.Backend.UnitsOfWork.Implementations;

public class EmployeeUnitOfWork : IEmployeeUnitOfWork
{
    private readonly DataContext _context;
    private readonly IEmployeeRepository _repository;

    public EmployeeUnitOfWork(DataContext context, IEmployeeRepository repository)
    {
        _context = context;
        _repository = repository;
    }

    public Task<ActionResponse<IEnumerable<Employee>>> GetAsync() =>
        _repository.GetAsync();

    public Task<ActionResponse<Employee>> GetAsync(int id) =>
        _repository.GetAsync(id);

    public Task<ActionResponse<Employee>> AddAsync(Employee entity) =>
        _repository.AddAsync(entity);

    public Task<ActionResponse<Employee>> UpdateAsync(Employee entity) =>
        _repository.UpdateAsync(entity);

    public Task<ActionResponse<Employee>> DeleteAsync(int id) =>
        _repository.DeleteAsync(id);

    public Task<ActionResponse<IEnumerable<Employee>>> SearchByLetterAsync(string letter) =>
        _repository.SearchByLetterAsync(letter);

    public Task<int> CommitAsync(CancellationToken ct = default) =>
        _context.SaveChangesAsync(ct);
}
