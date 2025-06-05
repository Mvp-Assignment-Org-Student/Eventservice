using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<EventEntity> Events { get; set; }

    public DbSet<PackageEntity> Packages { get; set; }

    public DbSet<EventPackageEntity> EventsPackages { get; set; }



}
