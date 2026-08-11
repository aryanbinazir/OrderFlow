using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Application.Helper.Enum;
using OrderFlow.Domain.Entities;

namespace OrderFlow.Infrastructure.Configurations;

public abstract class ConfigurationBase
{
    protected void SeedFromEnum<TEntity, TEnum>(EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseLookupEntity<TEnum>, new()
        where TEnum : struct, Enum
    {
        builder.HasKey(x => x.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.FarsiName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(t => t.Name).IsUnique();
        builder.HasIndex(t => t.FarsiName).IsUnique();

        var data = Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Select(value => new TEntity
            {
                Id = value,
                Name = value.ToString(),
                FarsiName = value.GetEnumDescription(),
                IsActive = true
            })
            .ToArray();

        builder.HasData(data);
    }
}
