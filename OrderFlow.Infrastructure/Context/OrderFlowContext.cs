using Microsoft.EntityFrameworkCore;
using OrderFlow.Domain.Entities;
using OrderFlow.Infrastructure.Configurations;

namespace OrderFlow.Infrastructure.Context
{
    public class OrderFlowContext(DbContextOptions<OrderFlowContext> options) : DbContext(options)
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderConfiguration).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
