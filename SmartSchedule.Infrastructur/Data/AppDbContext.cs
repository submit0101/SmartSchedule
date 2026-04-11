using ComputerShop.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Core.Entities;
using SmartSchedule.Infrastructure.Configurations;

namespace SmartSchedule.Infrastructure.Data;
/// <summary>
/// Контекст базы данных для приложения "Расписание кабинетов"
/// <seealso cref="DbContext"/>
/// </summary>
public class AppDbContext : IdentityDbContext<IdentityUser>
{
    #region Свойства

    /// <summary>
    /// Таблица аудиторий.
    /// </summary>
    public virtual DbSet<Cabinet> Cabinets { get; set; }

    /// <summary>
    /// Таблица групп.
    /// </summary>
    public virtual DbSet<Group> Groups { get; set; }

    /// <summary>
    /// Таблица занятий.
    /// </summary>
    public virtual DbSet<Lesson> Lessons { get; set; }

    /// <summary>
    /// Таблица предметов.
    /// </summary>
    public virtual DbSet<Subject> Subjects { get; set; }

    /// <summary>
    /// Таблица преподавателей.
    /// </summary>
    public virtual DbSet<Teacher> Teachers { get; set; }

    /// <summary>
    /// Таблица Должностей.
    /// </summary>
    public virtual DbSet<Position> Positions { get; set; }

    /// <summary>
    /// Таблица временных слотов.
    /// </summary>
    public virtual DbSet<TimeSlot> TimeSlots { get; set; }

    /// <summary>
    /// Таблица типов недель.
    /// </summary>
    public virtual DbSet<WeekType> WeekTypes { get; set; }
    /// <summary>
    ///  Таблица Корпусов
    /// </summary>
    public virtual DbSet<Building> Buildings { get; set; }

    #endregion

    #region Конструктор

    /// <summary>
    /// Создает новый экземпляр класса <see cref="AppDbContext"/>.
    /// </summary>
    /// <param name="options">Параметры контекста базы данных</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    #endregion

    #region Методы

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new CabinetConfiguration());
        modelBuilder.ApplyConfiguration(new GroupConfiguration());
        modelBuilder.ApplyConfiguration(new LessonConfiguration());
        modelBuilder.ApplyConfiguration(new SubjectConfiguration());
        modelBuilder.ApplyConfiguration(new TeacherConfiguration());
        modelBuilder.ApplyConfiguration(new TimeSlotConfiguration());
        modelBuilder.ApplyConfiguration(new WeekTypeConfiguration());
        modelBuilder.ApplyConfiguration(new BuildingConfiguration());
        modelBuilder.ApplyConfiguration(new WeekDayConfiguration());

        modelBuilder.ConfigureEntities();
    }
    #endregion
}