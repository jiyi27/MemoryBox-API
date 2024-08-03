using MemoryBox_API.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace MemoryBox_API.Models;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Box> Boxes { get; set; } = null!;
    public DbSet<Post> Posts { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserTypeConfiguration());
        modelBuilder.ApplyConfiguration(new BoxTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PostTypeConfiguration());
    }
}