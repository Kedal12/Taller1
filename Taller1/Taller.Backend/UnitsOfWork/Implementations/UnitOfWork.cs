using System.Collections.Concurrent;
using Taller.Backend.Data;
using Taller.Backend.Repositories.Implementations;
using Taller.Backend.Repositories.Interfaces;
using Taller.Backend.UnitsOfWork.Interfaces;


namespace Taller.Backend.UnitsOfWork.Implementations;
public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;
    private readonly IEmployeeRepository _employeeRepository; // especializado
    private bool _disposed;

    // Cache de repos genéricos
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public UnitOfWork(DataContext context, IEmployeeRepository employeeRepository)
    {
        _context = context;
        _employeeRepository = employeeRepository;
    }

    public IGenericRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        if (_repositories.TryGetValue(type, out var repo))
            return (IGenericRepository<T>)repo;

        var newRepo = new GenericRepository<T>(_context);
        _repositories[type] = newRepo;
        return newRepo;
    }

    public IEmployeeRepository EmployeesSearch => _employeeRepository;

    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
