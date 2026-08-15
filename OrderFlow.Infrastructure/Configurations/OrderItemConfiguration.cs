using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");
            builder.HasKey(i => i.Id);

            builder.Property(i => i.ProductId).IsRequired();
            builder.Property(i => i.UnitPrice).HasPrecision(18, 2);
            builder.Property(i => i.Total).HasPrecision(18, 2);
            builder.Property(i => i.Quantity).IsRequired();
            builder.Property(i => i.OrderId).IsRequired();

            // BASE ENTITY
            builder.Property(o => o.ModifiedById).IsRequired(false);
            builder.Property(o => o.CreateById).IsRequired(false);
            builder.Property(o => o.CreateDate).IsRequired(false);
            builder.Property(o => o.ModifiedDate).IsRequired(false);

            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Product)
                .WithMany(p => p.orderItems)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
