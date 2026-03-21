
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Application.Services;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Core.Service;
using Microsoft.Extensions.DependencyInjection;
using SmartSchedule.Core.Service.Interfaces;
using SmartSchedule.Infrastructure.Repositories;

namespace SmartSchedule.Extensions;

/// <summary>
/// Dependency Injection Extensions
/// </summary>
public static class DependencyExtensions
{
    /// <summary>
    /// Adds repositories to the DI container
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<ICabinetRepository, CabinetRepository>();
        services.AddTransient<IGroupRepository, GroupRepository>();
        services.AddTransient<ILessonRepository, LessonRepository>();
        services.AddTransient<IPositionRepository, PositionRepository>();
        services.AddTransient<ISubjectRepository, SubjectRepository>();
        services.AddTransient<ITeacherRepository, TeacherRepository>();
        services.AddTransient<ITimeSlotRepository, TimeSlotRepository>();
        services.AddTransient<IWeekTypeRepository, WeekTypeRepository>();
        services.AddTransient<IBuildingRepository, BuildingRepository>();
        services.AddTransient<IWeekDayRepository, WeekDayRepository>();
    }

    /// <summary>
    /// Adds services to the DI container
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void AddServices(this IServiceCollection services)
    {
        services.AddTransient<ICabinetService, CabinetService>();
        services.AddTransient<IGroupService, GroupService>();
        services.AddTransient<ILessonService, LessonService>();
        services.AddTransient<IPositionService, PositionService>();
        services.AddTransient<ISubjectService, SubjectService>();
        services.AddTransient<ITeacherService, TeacherService>();
        services.AddTransient<ITimeSlotService, TimeSlotService>();
        services.AddTransient<IWeekTypeService, WeekTypeService>();
        services.AddTransient<IBuildingService, BuildingService>();
        services.AddTransient<IWeekDayService, WeekDayService>();
    }
}