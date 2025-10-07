using Microsoft.EntityFrameworkCore;
using Taller.Backend.Data;
using Taller.Backend.Repositories.Interfaces;
using Taller.Shared.Entities;
using Taller.Shared.Responses;

namespace Taller.Backend.Repositories.Implementations;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly DataContext _context;
    private readonly DbSet<Employee> _entity;

    public EmployeeRepository(DataContext context)
    {
        _context = context;
        _entity = context.Set<Employee>();
    }

    public Task<ActionResponse<Employee>> AddAsync(Employee entity)
    {
        _entity.Add(entity);
        return Task.FromResult(new ActionResponse<Employee>
        {
            WasSuccess = true,
            Result = entity
        });
    }

    public async Task<ActionResponse<Employee>> DeleteAsync(int id)
    {
        var row = await _entity.FindAsync(id);
        if (row == null)
        {
            return new ActionResponse<Employee>
            {
                Message = "Registro no encontrado"
            };
        }

        _entity.Remove(row);
        return new ActionResponse<Employee> { WasSuccess = true };
    }

    public async Task<ActionResponse<Employee>> GetAsync(int id)
    {
        var row = await _entity.FindAsync(id);
        return row == null
            ? new ActionResponse<Employee> { Message = "Registro no encontrado" }
            : new ActionResponse<Employee> { WasSuccess = true, Result = row };
    }

    public async Task<ActionResponse<IEnumerable<Employee>>> GetAsync() =>
        new ActionResponse<IEnumerable<Employee>>
        {
            WasSuccess = true,
            Result = await _entity.ToListAsync()
        };

    public Task<ActionResponse<Employee>> UpdateAsync(Employee entity)
    {
        _entity.Update(entity);
        return Task.FromResult(new ActionResponse<Employee>
        {
            WasSuccess = true,
            Result = entity
        });
    }

    public async Task<ActionResponse<IEnumerable<Employee>>> SearchByLetterAsync(string letter)
    {
        if (string.IsNullOrWhiteSpace(letter))
        {
            return new ActionResponse<IEnumerable<Employee>>
            {
                WasSuccess = true,
                Result = Enumerable.Empty<Employee>()
            };
        }

        var pattern = $"%{letter}%";
        var result = await _entity
            .Where(e =>
                EF.Functions.Like(e.FirstName, pattern) ||
                EF.Functions.Like(e.LastName, pattern))
            .ToListAsync();

        return new ActionResponse<IEnumerable<Employee>>
        {
            WasSuccess = true,
            Result = result
        };
    }
}
