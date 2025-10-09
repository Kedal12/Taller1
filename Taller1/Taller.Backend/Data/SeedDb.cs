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
        // Aplica migraciones
        await _context.Database.MigrateAsync();

        // Si ya hay empleados, no vuelvas a sembrar
        if (await _context.Employees.AnyAsync())
            return;

        await SeedEmployeesFromSqlAsync();
    }

    private async Task SeedEmployeesFromSqlAsync()
    {
        // Ruta del archivo .sql dentro del proyecto Backend: /Data/Employee.sql
        var contentRoot = Directory.GetCurrentDirectory();
        var sqlPath = Path.Combine(contentRoot, "Data", "Employee.sql");

        if (!File.Exists(sqlPath))
            throw new FileNotFoundException($"No se encontró el archivo de seed: {sqlPath}");

        var sqlScript = await File.ReadAllTextAsync(sqlPath, Encoding.UTF8);

        // Ejecuta TODO el script (todos los INSERTs)
        await _context.Database.ExecuteSqlRawAsync(sqlScript);
    }
}