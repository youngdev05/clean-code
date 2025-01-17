using Microsoft.EntityFrameworkCore;
using Core.Models;
using File = Core.Models.File;

namespace Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<FilePermission> FilePermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasDefaultValue("Viewer");

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<FilePermission>()
            .HasIndex(fp => new { fp.FileId, fp.UserId })
            .IsUnique(); // Один юзер - одна роль для одного файла
    }
}