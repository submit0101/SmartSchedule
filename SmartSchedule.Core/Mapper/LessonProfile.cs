using AutoMapper;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Models.DTO.LessonDTO;
using SmartSchedule.Core.Models.DTO.ReportDTO;

namespace SmartSchedule.Core.Mapper;
/// <summary>
/// Профиль маппера Занятий
/// </summary>
public class LessonProfile : Profile
{
    /// <summary>
    /// Создание нового экземпляра класса
    /// </summary>
    public LessonProfile()
    {
        CreateMap<Lesson, ResponseLessonDto>().ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src =>
        src.Cabinet != null && src.Cabinet.Building != null && !string.IsNullOrEmpty(src.Cabinet.Building.Name)
            ? src.Cabinet.Building.Name.Substring(0, 1)
            : null
        ))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name))
            .ForMember(dest => dest.SubjectTitle, opt => opt.MapFrom(src => src.Subject.Title))
            .ForMember(dest => dest.CabinetNumber, opt => opt.MapFrom(src => src.Cabinet.Number))
            .ForMember(dest => dest.TimeSlotNumber, opt => opt.MapFrom(src => src.TimeSlot.SlotNumber))
            .ForMember(dest => dest.TimeSlotDisplay, opt => opt.MapFrom(src =>
                $"{src.TimeSlot.StartTime} - {src.TimeSlot.EndTime}"))
            .ForMember(dest => dest.WeekTypeName, opt => opt.MapFrom(src => src.WeekType.Name))
            .ForMember(dest => dest.TeacherFullName, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {
                if (src.Teacher != null)
                {
                    dest.TeacherFullName = FormatName(
                        src.Teacher.LastName,
                        src.Teacher.FirstName,
                        src.Teacher.MiddleName
                    );
                }
            });

        CreateMap<CreateLessonDto, Lesson>();
        CreateMap<UpdateLessonDto, Lesson>();
        CreateMap<Lesson, LessonReportFlatDto>()
            .ForMember(d => d.Teacher, opt => opt.MapFrom(s => s.Teacher != null
                ? $"{s.Teacher.LastName} {s.Teacher.FirstName} {s.Teacher.MiddleName}".Trim()
                : "Не назначено"))
            .ForMember(d => d.Group, opt => opt.MapFrom(s => s.Group != null ? s.Group.Name : "Не назначено"))
            .ForMember(d => d.Subject, opt => opt.MapFrom(s => s.Subject != null ? s.Subject.Title : "Не назначено"))
            .ForMember(d => d.Cabinet, opt => opt.MapFrom(s => s.Cabinet != null ? s.Cabinet.Number : "Не назначено"))
            .ForMember(d => d.Building, opt => opt.MapFrom(s => s.Cabinet != null && s.Cabinet.Building != null
                ? s.Cabinet.Building.Name
                : "Не указано"));
    }

    private static string FormatName(string lastName, string firstName, string middleName)
    {
        if (string.IsNullOrWhiteSpace(lastName)) return "";

        var parts = new List<string> { lastName };

        if (!string.IsNullOrWhiteSpace(firstName))
            parts.Add($"{firstName[0]}."); // "Иван"
        if (!string.IsNullOrWhiteSpace(middleName))
            parts.Add($"{middleName[0]}."); // "Иванович"

        return string.Join(" ", parts);
    }
    
}