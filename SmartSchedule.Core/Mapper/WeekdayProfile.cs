using AutoMapper;
using SmartSchedule.Application.DTOs;
using SmartSchedule.Core.Entities;

namespace SmartSchedule.Core.Mapper;
/// <summary>
/// Профиль маппера Кабинетов
/// </summary>
public class WeekdayProfile : Profile
{
    /// <summary>
    /// Создание нового экземпляра класса
    /// </summary>
    public WeekdayProfile()
    {
        CreateMap<WeekDay, WeekDayShortDto>();
    }
}