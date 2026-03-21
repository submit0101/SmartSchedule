using AutoMapper;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Models.DTO.PosittonDTO;
using System.Globalization;

namespace SmartSchedule.Core.Mapper;
/// <summary>
/// Профиль маппера Должностей
/// </summary>
public class PositionProfile : Profile
{
    /// <summary>
    /// Создание нового экземпляра класса
    /// </summary>
    public PositionProfile()
    {
        CreateMap<Position, ShortPositionDto>();
        CreateMap<Position, ResponsePositionDto>();
        CreateMap<CreatePositionDto, Position>().ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => NormalizeName(src.Name)));
        CreateMap<UpdatePositionDto, Position>().ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => NormalizeName(src.Name)));

    }

    private static string NormalizeName(string name)
        => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name?.Trim().ToLower(CultureInfo.CurrentCulture) ?? "");
}