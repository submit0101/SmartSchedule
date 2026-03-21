using AutoMapper;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Models.DTO.TimeSlotDTO;

namespace SmartSchedule.Core.Mapper;
/// <summary>
/// Профиль маппера Временных слотов
/// </summary>
public class TimeSlotProfile : Profile
{
    /// <summary>
    /// Создание нового экземпляра класса
    /// </summary>
    public TimeSlotProfile()
    {
        CreateMap<TimeSlot, ResponseTimeSlotDto>();
        CreateMap<CreateTimeSlotDto, TimeSlot>();
        CreateMap<UpdateTimeSlotDto, TimeSlot>();
    }
}