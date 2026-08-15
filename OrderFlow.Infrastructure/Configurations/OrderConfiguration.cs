using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.UserId).IsRequired();
            builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(100);
            builder.Property(o => o.Total).HasPrecision(18, 2);
            builder.Property(o => o.StatusId).IsRequired();
            builder.Property(o => o.ConfirmedAt).IsRequired(false);

            // BASE ENTITY
            builder.Property(o => o.ModifiedById).IsRequired(false);
            builder.Property(o => o.CreateById).IsRequired(false);
            builder.Property(o => o.CreateDate).IsRequired(false);
            builder.Property(o => o.ModifiedDate).IsRequired(false);

            builder.HasMany(o => o.OrderItems)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.User)
                .WithMany(os => os.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.OrderStatus)
                .WithMany(os => os.Orders)
                .HasForeignKey(o => o.StatusId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
