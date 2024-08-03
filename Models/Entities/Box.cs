using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace MemoryBox_API.Models.Entities;

public class Box
{
    public int BoxId { get; init; }
    public required int OwnerId { get; set; }
    public required string BoxName { get; set; }
    public bool IsPrivate { get; set; }
    public DateTime? CreatedDate { get; init; } = DateTime.Now;
    
    // Navigation properties, Virtual keyword is used to enable lazy loading
    public virtual User? Owner { get; init; }
    public virtual List<Post>? Posts { get; init; }
}

public class BoxTypeConfiguration : IEntityTypeConfiguration<Box>
{
    public void Configure(EntityTypeBuilder<Box> builder)
    {
        builder.HasKey(b => b.BoxId);
        builder.Property(b => b.BoxId).ValueGeneratedOnAdd();
        builder.Property(b => b.BoxName).IsRequired().HasMaxLength(30);
        builder.Property(b => b.IsPrivate).IsRequired();
        builder.Property(b => b.CreatedDate).IsRequired();
        
        builder.HasMany(e => e.Posts)
            .WithOne()
            .HasForeignKey(e => e.BoxId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}