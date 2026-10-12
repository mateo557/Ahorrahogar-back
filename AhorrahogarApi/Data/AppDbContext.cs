using Microsoft.EntityFrameworkCore;
using AhorrahogarApi.Models;

namespace AhorrahogarApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Movemnts> movimientos => Set<Movemnts>();

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<Movemnts>(e =>
    //    {
    //        e.HasKey(p => p.Id);
    //        e.Property(p => p.Name).IsRequired().HasMaxLength(100);
    //        e.Property(p => p.Description).HasMaxLength(500);
    //        e.Property(p => p.Price).HasPrecision(18, 2);
    //    });
    //}
}
