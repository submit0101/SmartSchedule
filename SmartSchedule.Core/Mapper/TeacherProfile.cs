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
        CreateMap<Teacher, ResponseTeacherDto>().ForMember(dest => dest.PositionName, opt => opt.MapFrom(prt => prt.Position.Name)).ForMember(
                dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.LastName} {src.FirstName} {src.MiddleName}")
            );
        CreateMap<CreateTeacherDto, Teacher>();
        CreateMap<UpdateTeacherDto, Teacher>();
    }
}

