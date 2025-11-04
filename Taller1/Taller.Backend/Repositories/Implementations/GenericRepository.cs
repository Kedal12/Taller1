using Microsoft.EntityFrameworkCore;
using Taller.Shared.Responses;
using Taller.Shared.DTOs;
using Taller.Backend.Data;
using Taller.Backend.Repositories.Interfaces;

namespace Taller.Backend.Repositories.Implementations;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly DataContext _context;
    protected readonly DbSet<T> _entity;

    public GenericRepository(DataContext context)
    {
        _context = context;
        _entity = _context.Set<T>();
    }

    public virtual async Task<ActionResponse<T>> AddAsync(T entity)
    {
        _entity.Add(entity);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<T> { WasSuccess = true, Result = entity };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<T> { Message = "Ya existe el registro." };
        }
        catch (Exception ex)
        {
            return new ActionResponse<T> { Message = ex.Message };
        }
    }

    public virtual async Task<ActionResponse<T>> DeleteAsync(int id)
    {
        var row = await _entity.FindAsync(id);
        if (row == null) return new ActionResponse<T> { Message = "Registro no encontrado" };

        _entity.Remove(row);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<T> { WasSuccess = true };
        }
        catch
        {
            return new ActionResponse<T> { Message = "No se puede eliminar el registro porque tiene relaciones." };
        }
    }

    public virtual async Task<ActionResponse<T>> GetAsync(int id)
    {
        var row = await _entity.FindAsync(id);
        return row == null
            ? new ActionResponse<T> { Message = "Registro no encontrado" }
            : new ActionResponse<T> { WasSuccess = true, Result = row };
    }

    public virtual async Task<ActionResponse<IEnumerable<T>>> GetAsync() =>
        new ActionResponse<IEnumerable<T>> { WasSuccess = true, Result = await _entity.ToListAsync() };


    public virtual async Task<ActionResponse<IEnumerable<T>>> GetAsync(PaginationDTO pagination)
    {
        var query = _entity.AsQueryable();

        var skip = (pagination.Page - 1) * pagination.RecordsNumber;
        var results = await query
            .Skip(skip)
            .Take(pagination.RecordsNumber)
            .ToListAsync();

        return new ActionResponse<IEnumerable<T>>
        {
            WasSuccess = true,
            Result = results
        };
    }

    public virtual async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var query = _entity.AsQueryable();

        var count = await query.CountAsync();

        return new ActionResponse<int>
        {
            WasSuccess = true,
            Result = count
        };
    }

    public virtual async Task<ActionResponse<T>> UpdateAsync(T entity)
    {
        _entity.Update(entity);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<T> { WasSuccess = true, Result = entity };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<T> { Message = "Conflicto al actualizar el registro." };
        }
        catch (Exception ex)
        {
            return new ActionResponse<T> { Message = ex.Message };
        }
    }
}
