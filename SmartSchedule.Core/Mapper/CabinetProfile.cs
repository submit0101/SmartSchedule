using AutoMapper;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Models.DTO.CabinetDTO;

namespace SmartSchedule.Core.Mapper;
/// <summary>
/// Профиль маппера Кабинетов
/// </summary>
public class CabinetProfile : Profile
{
    /// <summary>
    /// Создание нового экземпляра класса
    /// </summary>
    public CabinetProfile()
    {
        CreateMap<Cabinet, ShortCabinetDto>().ForMember(dest => dest.BuildingName, scr => scr.MapFrom(buil => buil.Building.Name.Substring(0, 1))); ;
        CreateMap<Cabinet, ResponseCabinetDto>().ForMember(dest => dest.BuldingName, opt => opt.MapFrom(ptr => ptr.Building.Name));
        CreateMap<CreateCabinetDto, Cabinet>();
        CreateMap<UpdateCabinetDto, Cabinet>();
    }
}