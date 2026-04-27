using AutoMapper;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Models.DTO.TeacherDTO;

namespace SmartSchedule.Core.Mapper;
/// <summary>
/// Профиль маппера Преподавателей
/// </summary>
public class TeacherProfile : Profile
{
    /// <summary>
    /// Создание нового экземпляра класса
    /// </summary>
    public TeacherProfile()
    {
        CreateMap<Teacher, ShortTeacherDto>()
            .ForMember(
                dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.LastName} {src.FirstName} {src.MiddleName}")
            );
        CreateMap<Teacher, ResponseTeacherDto>().ForMember(dest => dest.FullName, opt => opt.MapFrom(src => GetFormattedFullName(src)));
        CreateMap<CreateTeacherDto, Teacher>();
        CreateMap<UpdateTeacherDto, Teacher>();

    }
    /// <summary>
    /// Формирует полное имя. Trim() уберет лишний пробел в конце, если Отчество пустое.
    /// </summary>
    private static string GetFormattedFullName(Teacher src)
        => $"{src.LastName} {src.FirstName} {src.MiddleName}".Trim();
}

