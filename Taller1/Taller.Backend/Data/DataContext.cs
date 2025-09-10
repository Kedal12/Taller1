using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using Taller.Shared.Entities;

namespace Taller.Backend.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
}