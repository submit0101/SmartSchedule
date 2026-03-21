using AutoMapper;
using SmartSchedule.Application.DTOs.Building;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Models.DTO.BuildingDTO;
using SmartSchedule.Core.Models.DTOs.Building;
using System.Globalization;
using System.Text.RegularExpressions;
namespace SmartSchedule.Core.Mapper;
/// <summary>
/// Профиль маппера Корпуса
/// </summary>
public class BuildingProfile : Profile
{
    /// <summary>
    /// Создание нового экземпляра класса
    /// </summary>
    public BuildingProfile()
    {
        // Маппинг для чтения
        CreateMap<Building, ResponseBuildingDto>();
        CreateMap<Building, ShortBuildingDto>();
        CreateMap<CreateBuildingDto, Building>()
            .ForMember(dest => dest.Name,
                      opt => opt.MapFrom(src => NormalizeBuildingName(src.Name)));


        CreateMap<UpdateBuildingDto, Building>()
            .ForMember(dest => dest.Name,
                      opt => opt.MapFrom(src => NormalizeBuildingName(src.Name)));
    }
    private static string NormalizeBuildingName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return name;

        // Удаляем все спецсимволы кроме пробелов и букв
        name = Regex.Replace(name, @"[^\w\s]", "");

        // Стандартизируем варианты написания с учетом культуры и регистра
        name = name.Replace("корпус", "корп.", StringComparison.CurrentCultureIgnoreCase)
                  .Replace("Корпус", "корп.", StringComparison.CurrentCultureIgnoreCase);

        // Приводим к единому регистру с явным указанием культуры
        name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
            name.ToLower(CultureInfo.CurrentCulture));

        // Удаляем двойные пробелы
        return Regex.Replace(name.Trim(), @"\s+", " ");
    }
}