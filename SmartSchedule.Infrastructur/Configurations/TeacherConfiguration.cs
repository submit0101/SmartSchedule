using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSchedule.Core.Entities;

namespace SmartSchedule.Infrastructure.Configurations;

/// <summary>
/// Конфигурация сущности Teacher.
/// Определяет настройки таблицы, ключи и связи.
/// </summary>
public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    /// <summary>
    /// Метод конфигурации для сущности Teacher.
    /// </summary>
    /// <param name="builder">Построитель сущности</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если параметр builder равен null</exception>
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Teachers");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.MiddleName)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasMany(e => e.Lessons)
            .WithOne(e => e.Teacher)
            .HasForeignKey(e => e.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.FirstName, e.MiddleName, e.LastName })
              .IsUnique()
              .HasDatabaseName("IX_UniqueTeacher_FullName");
    }
}