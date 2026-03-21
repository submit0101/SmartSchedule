using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSchedule.Core.Entities;

namespace SmartSchedule.Infrastructure.Configurations;

/// <summary>
/// Конфигурация сущности WeekType.
/// Определяет настройки таблицы, ключи и связи.
/// </summary>
public class WeekTypeConfiguration : IEntityTypeConfiguration<WeekType>
{
    /// <summary>
    /// Метод конфигурации для сущности WeekType.
    /// </summary>
    /// <param name="builder">Построитель сущности</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если параметр builder равен null</exception>
    public void Configure(EntityTypeBuilder<WeekType> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("WeekTypes");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(20);
    }
}
