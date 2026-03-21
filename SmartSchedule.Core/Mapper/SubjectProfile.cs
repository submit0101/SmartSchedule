using AutoMapper;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Models.DTO.SubjectDTO;
using System.Globalization;

namespace SmartSchedule.Core.Mapper;
/// <summary>
/// Профиль маппера Предметов
/// </summary>
public class SubjectProfile : Profile
{
    /// <summary>
    /// Создание нового экземпляра класса
    /// </summary>
    public SubjectProfile()
    {
        CreateMap<Subject, ResponseSubjectDto>();
        CreateMap<CreateSubjectDto, Subject>().ForMember(dest => dest.Title,
                      opt => opt.MapFrom(src => NormalizeTitle(src.Title))); ;
        CreateMap<UpdateSubjectDto, Subject>().ForMember(dest => dest.Title,
                      opt => opt.MapFrom(src => NormalizeTitle(src.Title))); ;
    }
    private static string NormalizeTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return title;


        title = title.Trim();
        return char.ToUpper(title[0], CultureInfo.CurrentCulture) +
               title.Substring(1).ToLower(CultureInfo.CurrentCulture);
    }
}
