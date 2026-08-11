using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(c => c.Description)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(c => c.ParentId).IsRequired(false);


            // BASE ENTITY
            builder.Property(o => o.IsDeleted).IsRequired();
            builder.Property(o => o.IsActive).IsRequired();
            builder.Property(o => o.ModifiedById).IsRequired(false);
            builder.Property(o => o.CreateById).IsRequired(false);
            builder.Property(o => o.CreateDate).IsRequired(false);
            builder.Property(o => o.ModifiedDate).IsRequired(false);

            // Self reference
            builder.HasMany(c => c.Children)
                .WithOne()
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category -> Product
            builder.HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
