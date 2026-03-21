using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSchedule.Core.Entities;

namespace SmartSchedule.Infrastructure.Configurations;

/// <summary>
/// Конфигурация сущности Building.
/// Определяет настройки таблицы, ключи и связи.
/// </summary>
public class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    /// <summary>
    /// Метод конфигурации для сущности Building.
    /// </summary>
    /// <param name="builder">Построитель сущности</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если параметр builder равен null</exception>
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("Buildings");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("Name");
        builder.Property(e => e.Address)
            .HasMaxLength(255)
            .HasColumnName("Address");
        builder.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_UniName");
    }
}