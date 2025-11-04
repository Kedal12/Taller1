using Microsoft.EntityFrameworkCore;
using System.Text;
using Taller.Backend.UnitsOfWork.Interfaces;
using Taller.Shared.Entities;
using Taller.Shared.Enums;

namespace Taller.Backend.Data;

public class SeedDb
{
    private readonly DataContext _context;
    private readonly IUsersUnitOfWork _usersUnitOfWork;

    public SeedDb(DataContext context, IUsersUnitOfWork usersUnitOfWork)
    {
        _context = context;
        _usersUnitOfWork = usersUnitOfWork;
    }

    public async Task SeedAsync()
    {
        // Aplica migraciones pendientes
        await _context.Database.MigrateAsync();

        // Seed de datos geográficos (Countries, States, Cities)
        await CheckCountriesFullAsync();
        await CheckCountriesAsync();

        // Seed de categorías
        await CheckCategoriesAsync();

        // Seed de empleados desde SQL
        await SeedEmployeesFromSqlAsync();

        // Seed de roles y usuarios de Identity
        await CheckRolesAsync();
        await CheckUserAsync("1010", "kevin", "londoño", "kevin@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", UserType.Admin);
    }

    #region Identity - Users & Roles

    private async Task<User> CheckUserAsync(string document, string firstName, string lastName, string email, string phone, string address, UserType userType)
    {
        var user = await _usersUnitOfWork.GetUserAsync(email);
        if (user == null)
        {
            var city = await _context.Cities.FirstOrDefaultAsync();

            user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email,
                PhoneNumber = phone,
                Address = address,
                Document = document,
                City = city,
                UserType = userType,
            };

            await _usersUnitOfWork.AddUserAsync(user, "123456");
            await _usersUnitOfWork.AddUserToRoleAsync(user, userType.ToString());
        }

        return user;
    }

    private async Task CheckRolesAsync()
    {
        await _usersUnitOfWork.CheckRoleAsync(UserType.Admin.ToString());
        await _usersUnitOfWork.CheckRoleAsync(UserType.User.ToString());
    }

    #endregion

    #region Geographic Data - Countries, States, Cities

    private async Task CheckCountriesFullAsync()
    {
        if (!_context.Countries.Any())
        {
            var contentRoot = Directory.GetCurrentDirectory();
            var sqlPath = Path.Combine(contentRoot, "Data", "CountriesStatesCities.sql");

            if (File.Exists(sqlPath))
            {
                var countriesSQLScript = await File.ReadAllTextAsync(sqlPath, Encoding.UTF8);
                await _context.Database.ExecuteSqlRawAsync(countriesSQLScript);
            }
        }
    }

    private async Task CheckCountriesAsync()
    {
        if (!_context.Countries.Any())
        {
            _context.Countries.Add(new Country
            {
                Name = "Colombia",
                States = [
                    new State()
                    {
                        Name = "Antioquia",
                        Cities = [
                            new City() { Name = "Medellín" },
                            new City() { Name = "Itagüí" },
                            new City() { Name = "Envigado" },
                            new City() { Name = "Bello" },
                            new City() { Name = "Rionegro" },
                        ]
                    },
                    new State()
                    {
                        Name = "Bogotá",
                        Cities = [
                            new City() { Name = "Usaquen" },
                            new City() { Name = "Champinero" },
                            new City() { Name = "Santa fe" },
                            new City() { Name = "Useme" },
                            new City() { Name = "Bosa" },
                        ]
                    },
                ]
            });

            _context.Countries.Add(new Country
            {
                Name = "Estados Unidos",
                States = [
                    new State()
                    {
                        Name = "Florida",
                        Cities = [
                            new City() { Name = "Orlando" },
                            new City() { Name = "Miami" },
                            new City() { Name = "Tampa" },
                            new City() { Name = "Fort Lauderdale" },
                            new City() { Name = "Key West" },
                        ]
                    },
                    new State()
                    {
                        Name = "Texas",
                        Cities = [
                            new City() { Name = "Houston" },
                            new City() { Name = "San Antonio" },
                            new City() { Name = "Dallas" },
                            new City() { Name = "Austin" },
                            new City() { Name = "El Paso" },
                        ]
                    },
                ]
            });

            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region Categories

    private async Task CheckCategoriesAsync()
    {
        if (!_context.Categories.Any())
        {
            _context.Categories.Add(new Category { Name = "Apple" });
            _context.Categories.Add(new Category { Name = "Autos" });
            _context.Categories.Add(new Category { Name = "Belleza" });
            _context.Categories.Add(new Category { Name = "Calzado" });
            _context.Categories.Add(new Category { Name = "Comida" });
            _context.Categories.Add(new Category { Name = "Cosmeticos" });
            _context.Categories.Add(new Category { Name = "Deportes" });
            _context.Categories.Add(new Category { Name = "Erótica" });
            _context.Categories.Add(new Category { Name = "Ferreteria" });
            _context.Categories.Add(new Category { Name = "Gamer" });
            _context.Categories.Add(new Category { Name = "Hogar" });
            _context.Categories.Add(new Category { Name = "Jardín" });
            _context.Categories.Add(new Category { Name = "Jugetes" });
            _context.Categories.Add(new Category { Name = "Lenceria" });
            _context.Categories.Add(new Category { Name = "Mascotas" });
            _context.Categories.Add(new Category { Name = "Nutrición" });
            _context.Categories.Add(new Category { Name = "Ropa" });
            _context.Categories.Add(new Category { Name = "Tecnología" });
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region Employees

    private async Task SeedEmployeesFromSqlAsync()
    {
        // Si ya hay empleados, no vuelvas a sembrar
        if (await _context.Employees.AnyAsync())
            return;

        var contentRoot = Directory.GetCurrentDirectory();
        var sqlPath = Path.Combine(contentRoot, "Data", "Employee.sql");

        if (!File.Exists(sqlPath))
        {
            // Solo advertir, no lanzar excepción para no romper el seed completo
            Console.WriteLine($"⚠️ Advertencia: No se encontró el archivo {sqlPath}");
            return;
        }

        var sqlScript = await File.ReadAllTextAsync(sqlPath, Encoding.UTF8);
        await _context.Database.ExecuteSqlRawAsync(sqlScript);
    }

    #endregion
}