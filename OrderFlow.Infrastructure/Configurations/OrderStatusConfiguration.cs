using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;
using OrderFlow.Domain.Enums;

namespace OrderFlow.Infrastructure.Configurations
{
    public class OrderStatusConfiguration : ConfigurationBase, IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> builder)
        {
            SeedFromEnum<OrderStatus, _OrderStatus>(builder);
        }
    }
}
