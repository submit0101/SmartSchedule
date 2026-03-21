using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSchedule.Core.Entities;

namespace SmartSchedule.Infrastructure.Configurations;

/// <summary>
/// Конфигурация сущности Subject.
/// Определяет настройки таблицы, ключи и связи.
/// </summary>
public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    /// <summary>
    /// Метод конфигурации для сущности Subject.
    /// </summary>
    /// <param name="builder">Построитель сущности</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если параметр builder равен null</exception>
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("Subjects");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(e => e.Title).IsUnique().HasDatabaseName("IX_UniTitile");
    }
}
