using AutoMapper;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Models.DTO.GroupDTO;

namespace SmartSchedule.Core.Mapper;
/// <summary>
/// Профиль маппера Групп
/// </summary>
public class GroupProfile : Profile
{
    /// <summary>
    /// Создание нового экземпляра класса
    /// </summary>
    public GroupProfile()
    {
        CreateMap<Group, ResponseGroupDto>();
        CreateMap<CreateGroupDto, Group>()
            .ForMember(dest => dest.Name,
                      opt => opt.MapFrom(src => NormalizeGroupName(src.Name)));
        CreateMap<UpdateGroupDto, Group>()
            .ForMember(dest => dest.Name,
                      opt => opt.MapFrom(src => NormalizeGroupName(src.Name)));
    }
    private static string NormalizeGroupName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return name;
        return name.Trim().ToUpperInvariant();
    }
}
