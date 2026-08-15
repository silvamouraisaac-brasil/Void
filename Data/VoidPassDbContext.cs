using Microsoft.EntityFrameworkCore;
using VoidPass.Models;

namespace VoidPass.Data;

public class VoidPassDbContext : DbContext
{
    public VoidPassDbContext(DbContextOptions<VoidPassDbContext> options)
        : base(options)
    {
    }

    public DbSet<UsedPassword> UsedPasswords => Set<UsedPassword>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UsedPassword>(entity =>
        {
            entity.HasKey(x => x.Hash);

            entity.Property(x => x.Hash)
                .IsRequired();

            entity.Property(x => x.CreatedAt)
                .IsRequired();
        });
    }
}