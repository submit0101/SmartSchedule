using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartSchedule.Core.Entities;

namespace SmartSchedule.Infrastructure.Configurations;

/// <summary>
/// Конфигурация сущности Cabinet.
/// Определяет настройки таблицы, ключи и связи с другими сущностями.
/// </summary>
public class CabinetConfiguration : IEntityTypeConfiguration<Cabinet>
{
    /// <summary>
    /// конфигуриет сущность
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Cabinet> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("Cabinets");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Number)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(e => e.Equipment)
            .HasMaxLength(255);

        builder.HasOne(c => c.Building)
            .WithMany(b => b.Cabinets)
            .HasForeignKey(c => c.BuildingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new {c.Number,c.BuildingId}).IsUnique().
        HasDatabaseName("IX_Cabinets_Number_BuildingId");
    }
}