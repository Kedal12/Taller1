using Taller.Shared.Entities;

namespace Taller.Backend.Data;

public class SeedDb
{
    private readonly DataContext _context;

    public SeedDb(DataContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckEmployeesAsync();
    }

    private async Task CheckEmployeesAsync()
    {
        if (!_context.Employees.Any())
        {
            var today = DateTime.UtcNow;

            _context.Employees.Add(new Employee { FirstName = "Carlos", LastName = "Ramirez", IsActive = true, HireDate = today.AddYears(-3), Salary = 2500000 });
            _context.Employees.Add(new Employee { FirstName = "Ana", LastName = "Gomez", IsActive = true, HireDate = today.AddYears(-2), Salary = 2800000 });
            _context.Employees.Add(new Employee { FirstName = "Luis", LastName = "Martinez", IsActive = true, HireDate = today.AddYears(-4), Salary = 3000000 });
            _context.Employees.Add(new Employee { FirstName = "Maria", LastName = "Fernandez", IsActive = true, HireDate = today.AddYears(-1), Salary = 2600000 });
            _context.Employees.Add(new Employee { FirstName = "Pedro", LastName = "Lopez", IsActive = false, HireDate = today.AddYears(-5), Salary = 3100000 });
            _context.Employees.Add(new Employee { FirstName = "Sofia", LastName = "Hernandez", IsActive = true, HireDate = today.AddMonths(-18), Salary = 2700000 });
            _context.Employees.Add(new Employee { FirstName = "Andres", LastName = "Gutierrez", IsActive = true, HireDate = today.AddYears(-6), Salary = 2900000 });
            _context.Employees.Add(new Employee { FirstName = "Paola", LastName = "Vargas", IsActive = true, HireDate = today.AddYears(-2), Salary = 2500000 });
            _context.Employees.Add(new Employee { FirstName = "Javier", LastName = "Mendoza", IsActive = true, HireDate = today.AddYears(-3), Salary = 3300000 });
            _context.Employees.Add(new Employee { FirstName = "Claudia", LastName = "Cortez", IsActive = false, HireDate = today.AddYears(-7), Salary = 2400000 });
            _context.Employees.Add(new Employee { FirstName = "Ricardo", LastName = "Salazar", IsActive = true, HireDate = today.AddMonths(-30), Salary = 3100000 });
            _context.Employees.Add(new Employee { FirstName = "Laura", LastName = "Morales", IsActive = true, HireDate = today.AddYears(-1), Salary = 2800000 });
            _context.Employees.Add(new Employee { FirstName = "Fernando", LastName = "Castro", IsActive = true, HireDate = today.AddYears(-8), Salary = 2600000 });
            _context.Employees.Add(new Employee { FirstName = "Patricia", LastName = "Rojas", IsActive = true, HireDate = today.AddMonths(-15), Salary = 2900000 });
            _context.Employees.Add(new Employee { FirstName = "Diego", LastName = "Suarez", IsActive = true, HireDate = today.AddYears(-4), Salary = 3000000 });
        }

        await _context.SaveChangesAsync();
    }
}