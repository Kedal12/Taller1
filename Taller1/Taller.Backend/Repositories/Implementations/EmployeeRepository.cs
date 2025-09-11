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

    public virtual async Task<ActionResponse<Employee>> AddAsync(Employee entity)
    {
        _context.Add(entity);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Employee>
            {
                WasSuccess = true,
                Result = entity
            };
        }
        catch (DbUpdateException)
        {
            return DbUpdateExceptionActionResponse();
        }
        catch (Exception exception)
        {
            return ExceptionActionResponse(exception);
        }
    }

    public virtual async Task<ActionResponse<Employee>> DeleteAsync(int id)
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
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Employee>
            {
                WasSuccess = true
            };
        }
        catch
        {
            return new ActionResponse<Employee>
            {
                Message = "No se puede eliminar el registro porque tiene relaciones con otros registros."
            };
        }
    }

    public virtual async Task<ActionResponse<Employee>> GetAsync(int id)
    {
        var row = await _entity.FindAsync(id);
        if (row == null)
        {
            return new ActionResponse<Employee>
            {
                Message = "Registro no encontrado"
            };
        }
        return new ActionResponse<Employee>
        {
            WasSuccess = true,
            Result = row
        };
    }

    public virtual async Task<ActionResponse<IEnumerable<Employee>>> GetAsync() =>
        new ActionResponse<IEnumerable<Employee>>
        {
            WasSuccess = true,
            Result = await _entity.ToListAsync()
        };

    public virtual async Task<ActionResponse<Employee>> UpdateAsync(Employee entity)
    {
        _context.Update(entity);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Employee>
            {
                WasSuccess = true,
                Result = entity
            };
        }
        catch (DbUpdateException)
        {
            return DbUpdateExceptionActionResponse();
        }
        catch (Exception exception)
        {
            return ExceptionActionResponse(exception);
        }
    }

    public virtual async Task<ActionResponse<IEnumerable<Employee>>> SearchByLetterAsync(string letter)
    {
        if (string.IsNullOrWhiteSpace(letter))
        {
            return new ActionResponse<IEnumerable<Employee>>
            {
                WasSuccess = true,
                Result = Enumerable.Empty<Employee>()
            };
        }

        var result = await _entity
            .Where(e => e.FirstName.ToLower().Contains(letter.ToLower()) ||
                        e.LastName.ToLower().Contains(letter.ToLower()))
            .ToListAsync();

        return new ActionResponse<IEnumerable<Employee>>
        {
            WasSuccess = true,
            Result = result
        };
    }

    private ActionResponse<Employee> ExceptionActionResponse(Exception exception) => new ActionResponse<Employee>
    {
        Message = exception.Message
    };

    private ActionResponse<Employee> DbUpdateExceptionActionResponse() =>
        new ActionResponse<Employee>
        {
            Message = "Ya existe el registro."
        };
}