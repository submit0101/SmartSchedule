using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSchedule.Core.Entities;

namespace SmartSchedule.Infrastructure.Configurations;

/// <summary>
/// Конфигурация сущности Lesson.
/// Определяет настройки таблицы, ключи и связи.
/// </summary>
public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    /// <summary>
    /// Метод конфигурации для сущности Lesson.
    /// </summary>
    /// <param name="builder">Построитель сущности</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если параметр builder равен null</exception>
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("Lessons");

        builder.HasKey(e => e.Id);

        // Связь с кабинетом (при удалении кабинета - запрет, если есть уроки)
        builder.HasOne(d => d.Cabinet)
            .WithMany(p => p.Lessons)
            .HasForeignKey(d => d.CabinetId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK__Lessons__Cabinet__4316F928");

        // Связь с группой (при удалении группы - запрет, если есть уроки)
        builder.HasOne(d => d.Group)
            .WithMany(p => p.Lessons)
            .HasForeignKey(d => d.GroupId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK__Lessons__GroupId__44FF419A");

        // Связь с предметом (при удалении предмета - запрет, если есть уроки)
        builder.HasOne(d => d.Subject)
            .WithMany(p => p.Lessons)
            .HasForeignKey(d => d.SubjectId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK__Lessons__Subject__45F365D3");

        // Связь с учителем (при удалении учителя - запрет, если есть уроки)
        builder.HasOne(d => d.Teacher)
            .WithMany(p => p.Lessons)
            .HasForeignKey(d => d.TeacherId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK__Lessons__Teacher__440B1D61");

        // Связь с тайм-слотом (при удалении времени - запрет, если есть уроки)
        builder.HasOne(d => d.TimeSlot)
            .WithMany(p => p.Lessons)
            .HasForeignKey(d => d.TimeSlotId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK__Lessons__TimeSlo__46E78A0C");

        // Связь с типом недели (четная/нечетная)
        builder.HasOne(d => d.WeekType)
            .WithMany(p => p.Lessons)
            .HasForeignKey(d => d.WeekTypeId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK__Lessons__WeekTyp__47DBAE45");

        // Связь с днем недели
        builder.HasOne(d => d.WeekDay)
            .WithMany(p => p.Lessons)
            .HasForeignKey(d => d.DayOfWeekId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK__Lessons__WeekDay__48C3289E");
    }
}