using Microsoft.EntityFrameworkCore;
using System.Text;
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
        await SeedEmployeesFromSqlAsync();
    }

    private async Task SeedEmployeesFromSqlAsync()
    {
        // ¿Ya hay empleados? No hacemos nada
        if (await _context.Employees.AnyAsync())
            return;

        // Ruta del archivo .sql dentro de la salida (bin/...)
        var sqlPath = Path.Combine(AppContext.BaseDirectory, "Sql", "EmployeesSeed.sql");

        if (!File.Exists(sqlPath))
            throw new FileNotFoundException($"No se encontró el archivo de seed: {sqlPath}");

        var sqlScript = await File.ReadAllTextAsync(sqlPath, Encoding.UTF8);

        // Ejecuta el script completo
        await _context.Database.ExecuteSqlRawAsync(sqlScript);
    }
}