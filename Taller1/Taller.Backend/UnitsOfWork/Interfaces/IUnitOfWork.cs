using Taller.Backend.Repositories.Interfaces;
using Taller.Shared.Entities;

namespace Taller.Backend.UnitsOfWork.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Repos genéricos por tipo:
    IGenericRepository<T> Repository<T>() where T : class;

    // Repos especializados (si aplica):
    IEmployeeRepository EmployeesSearch { get; }

    // Commit de la transacción
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
