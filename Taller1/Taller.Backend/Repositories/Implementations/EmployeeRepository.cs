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

    public async Task<ActionResponse<Employee>> UpdateAsync(Employee entity)
    {
        var existing = await _entity.FirstOrDefaultAsync(e => e.Id == entity.Id);
        if (existing == null)
        {
            return new ActionResponse<Employee> { Message = "Registro no encontrado" };
        }

        // Opción A: copiar campo a campo (explícito)
        existing.FirstName = entity.FirstName;
        existing.LastName = entity.LastName;
        existing.IsActive = entity.IsActive;   // 👈 aseguras el bool
        existing.HireDate = entity.HireDate;
        existing.Salary = entity.Salary;

        // (SaveChanges lo llamas en CommitAsync desde el UoW/Controller)
        return new ActionResponse<Employee> { WasSuccess = true, Result = existing };
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

    public async Task<int> GetTotalRecordsAsync(string? filter)
    {
        IQueryable<Employee> query = _entity.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var pattern = $"%{filter}%";
            query = query.Where(e =>
                EF.Functions.Like(e.FirstName, pattern) ||
                EF.Functions.Like(e.LastName, pattern));
        }

        return await query.CountAsync();
    }

    public async Task<IEnumerable<Employee>> GetPaginatedAsync(int page, int recordsNumber, string? filter)
    {
        if (page <= 0) page = 1;
        if (recordsNumber <= 0) recordsNumber = 10;

        IQueryable<Employee> query = _entity.AsNoTracking().OrderBy(e => e.LastName).ThenBy(e => e.FirstName);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var pattern = $"%{filter}%";
            query = query.Where(e =>
                EF.Functions.Like(e.FirstName, pattern) ||
                EF.Functions.Like(e.LastName, pattern));
        }

        return await query
            .Skip((page - 1) * recordsNumber)
            .Take(recordsNumber)
            .ToListAsync();
    }
}