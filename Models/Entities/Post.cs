using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace MemoryBox_API.Models.Entities;

public class Post
{
    public int PostId { get; init; }
    public int BoxId { get; set; }
    public string? Title { get; init; }
    public string? Content { get; init; }
    public DateTime CreatedDate { get; init; } = DateTime.Now;

    public virtual List<Image> Images { get; init; } = [];
}

public class Image(int postId, string url)
{
    public int ImageId { get; init; }
    public int PostId { get; init; } = postId;
    [MaxLength(200)]public string Url { get; init; } = url;
}

public class PostTypeConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(p => p.PostId);
        builder.Property(p => p.PostId).ValueGeneratedOnAdd();
        builder.Property(p => p.Title).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Content).HasMaxLength(1000);
        builder.Property(p => p.CreatedDate).IsRequired();
        
        builder.HasMany(p => p.Images)
            .WithOne()
            .HasForeignKey(i => i.PostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
