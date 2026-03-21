using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSchedule.Core.Entities;

namespace SmartSchedule.Infrastructure.Configurations;

/// <summary>
/// Конфигурация сущности TimeSlot.
/// Определяет настройки таблицы, ключи и связи.
/// </summary>
public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    /// <summary>
    /// Метод конфигурации для сущности TimeSlot.
    /// </summary>
    /// <param name="builder">Построитель сущности</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если параметр builder равен null</exception>
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("TimeSlots");
        builder.HasKey(e => e.Id);
    }
}