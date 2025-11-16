using System.Data;
using DirectoryService.Application.Abstractions;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure;

public class DirectoryServiceDbContext : DbContext, IReadDbContext, IDbConnectionFactory
{
    private readonly string _connectionString;

    public DirectoryServiceDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("ltree");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DirectoryServiceDbContext).Assembly);
    }

    public DbSet<Department> Departments => Set<Department>();
    
    public DbSet<Location> Locations => Set<Location>();
    
    public DbSet<Position> Positions => Set<Position>();

    public DbSet<DepartmentLocation> DepartmentLocations => Set<DepartmentLocation>();

    public DbSet<DepartmentPosition> DepartmentPositions => Set<DepartmentPosition>();
    
    public IQueryable<Department> DepartmentsRead => Set<Department>().AsNoTracking();
    
    public IQueryable<Location> LocationsRead => Set<Location>().AsNoTracking();
    
    public IQueryable<Position> PositionsRead => Set<Position>().AsNoTracking();
    
    public IQueryable<DepartmentLocation> DepartmentLocationsRead => Set<DepartmentLocation>().AsNoTracking();
    
    public IQueryable<DepartmentPosition> DepartmentPositionsRead => Set<DepartmentPosition>().AsNoTracking();
    
    public IDbConnection GetDbConnection() => Database.GetDbConnection();
}