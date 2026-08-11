using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(320);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(u => u.DisplayName)
                .HasMaxLength(250)
                .IsRequired(false);

            // BASE ENTITY
            builder.Property(o => o.IsDeleted).IsRequired();
            builder.Property(o => o.IsActive).IsRequired();
            builder.Property(o => o.ModifiedById).IsRequired(false);
            builder.Property(o => o.CreateById).IsRequired(false);
            builder.Property(o => o.CreateDate).IsRequired(false);
            builder.Property(o => o.ModifiedDate).IsRequired(false);

            builder.HasOne(u => u.UserRole)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
