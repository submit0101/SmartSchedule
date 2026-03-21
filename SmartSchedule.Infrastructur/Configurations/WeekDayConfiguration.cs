using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSchedule.Core.Entities;

namespace SmartSchedule.Infrastructure.Configurations
{
    /// <summary>
    /// Конфигурация сущности <see cref="WeekDay"/>.
    /// Определяет настройки таблицы, ограничения и начальные данные.
    /// </summary>
    public class WeekDayConfiguration : IEntityTypeConfiguration<WeekDay>
    {
        /// <summary>
        /// Конфигурирует таблицу WeekDays, включая первичный ключ, свойства и начальные данные.
        /// </summary>
        /// <param name="builder">Построитель конфигурации для сущности <see cref="WeekDay"/>.</param>
        public void Configure(EntityTypeBuilder<WeekDay> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            builder.ToTable("WeekDays");
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Seed-данные
            builder.HasData(
                new WeekDay { Id = 1, Name = "Понедельник" },
                new WeekDay { Id = 2, Name = "Вторник" },
                new WeekDay { Id = 3, Name = "Среда" },
                new WeekDay { Id = 4, Name = "Четверг" },
                new WeekDay { Id = 5, Name = "Пятница" },
                new WeekDay { Id = 6, Name = "Суббота" },
                new WeekDay { Id = 7, Name = "Воскресенье" }
            );
        }
    }
}
