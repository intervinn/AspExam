using AspExam.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspExam.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public DbSet<Link> Links { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (optionsBuilder.IsConfigured) return;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>()
            .HasMany(e => e.Links)
            .WithOne(e => e.Owner)
            .HasForeignKey(e => e.OwnerId)
            .HasPrincipalKey(e => e.Id);

        modelBuilder.Entity<Link>();
    }
}