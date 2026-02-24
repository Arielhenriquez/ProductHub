using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductHub.Domain.Models;

namespace ProductHub.Infrastructure.Persistence.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index for sorting
        builder.HasIndex(x => new { x.ProductId, x.SortOrder });

        // Unique index for single main image per product (filtered)
        builder.HasIndex(x => new { x.ProductId, x.IsMain })
            .IsUnique()
            .HasFilter("[IsMain] = 1 AND [IsDeleted] = 0");
            
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
