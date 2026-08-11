using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(p => p.Description)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(p => p.SKU)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Price)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.Stock)
                .IsRequired();

            builder.Property(p => p.CategoryId)
                .IsRequired(false);

            // BASE ENTITY
            builder.Property(o => o.IsDeleted).IsRequired();
            builder.Property(o => o.IsActive).IsRequired();
            builder.Property(o => o.ModifiedById).IsRequired(false);
            builder.Property(o => o.CreateById).IsRequired(false);
            builder.Property(o => o.CreateDate).IsRequired(false);
            builder.Property(o => o.ModifiedDate).IsRequired(false);

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.orderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
