using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace MemoryBox_API.Models.Entities;

public class User
{
    public int UserId { get; init; }
    public required string FullName { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public string? Email { get; set; }
   public string? ProfilePictureUrl { get; set; }
}

public class UserTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.UserId);
        builder.Property(u => u.UserId).ValueGeneratedOnAdd();
        builder.Property(u => u.FullName).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Username).IsRequired().HasMaxLength(20);
        builder.HasIndex(u => u.Username).IsUnique();
        builder.Property(u => u.Password).IsRequired().HasMaxLength(20);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(50);
        builder.Property(u => u.ProfilePictureUrl).HasMaxLength(255);
    }
}
