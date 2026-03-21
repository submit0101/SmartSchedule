using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSchedule.Core.Entities;

namespace SmartSchedule.Infrastructure.Configurations;

/// <summary>
/// Конфигурация сущности Position.
/// Определяет настройки таблицы, ключи и связи.
/// </summary>
public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    /// <summary>
    /// Метод конфигурации для сущности Position.
    /// </summary>
    /// <param name="builder">Построитель сущности</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если параметр builder равен null</exception>
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("Positions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Description)
            .HasMaxLength(255);


        builder.HasMany(e => e.Teachers)
            .WithOne(e => e.Position)
            .HasForeignKey(e => e.PositionId)
            .HasPrincipalKey(e => e.Id)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(e => e.Name).IsUnique().HasDatabaseName("IX_UniName");
    }
}