using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartSchedule.Core.Extensions;

namespace ComputerShop.Infrastructure.Extensions;
/// <summary>
/// Методы расширения для класса <see cref="ModelBuilder"/>.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Конфигурирует сущности 
    /// </summary>
    /// <param name="modelBuilder">Строитель сущностей</param>
    public static void ConfigureEntities(this ModelBuilder modelBuilder)
    {
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
        var dateTimeTypes = new List<Type> { typeof(DateTime), typeof(DateTime?) };
        ArgumentNullException.ThrowIfNull(modelBuilder);
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var entityTypeBuilder = modelBuilder.Entity(entity.Name);

            foreach (var property in entity.GetProperties())
            {
                entityTypeBuilder.Property(property.Name).HasColumnName(property.Name.ToSnakeCase());
            }
        }
    }
}
