using AutoMapper;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Models.DTO.LessonDTO;

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