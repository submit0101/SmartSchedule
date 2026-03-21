using AutoMapper;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Models.DTO.WeekTypeDTO;

namespace SmartSchedule.Core.Mapper;
/// <summary>
/// Профиль маппера Типов недели
/// </summary>
public class WeekTypeProfile : Profile
{
    /// <summary>
    /// Создание нового экземпляра класса
    /// </summary>
    public WeekTypeProfile()
    {
        CreateMap<WeekType, ResponseWeekTypeDto>();
        CreateMap<CreateWeekTypeDto, WeekType>();
        CreateMap<UpdateWeekTypeDto, WeekType>();
    }
}
