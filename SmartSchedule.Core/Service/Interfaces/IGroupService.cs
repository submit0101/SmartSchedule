
using SmartSchedule.Core.Models.DTO.GroupDTO;

namespace SmartSchedule.Application.Services.Interfaces;

/// <summary>
/// Сервис для работы с учебными группами.
/// </summary>
public interface IGroupService
{

    /// <summary>
    /// Получить список всех групп в полном формате.
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Список групп</returns>
    Task<List<ResponseGroupDto>> GetAllAsync(CancellationToken ct);

    /// <summary>
    /// Получить группу по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор группы</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Подробная информация о группе</returns>
    Task<ResponseGroupDto> GetByIdAsync(int id, CancellationToken ct);

    /// <summary>
    /// Создать новую группу.
    /// </summary>
    /// <param name="dto">Данные для создания группы</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Созданная группа</returns>
    Task<ResponseGroupDto> CreateAsync(CreateGroupDto dto, CancellationToken ct);

    /// <summary>
    /// Обновить информацию о группе.
    /// </summary>
    /// <param name="id">Идентификатор группы</param>
    /// <param name="dto">Данные для обновления</param>
    /// <param name="ct">Токен отмены</param>
    Task UpdateAsync(int id, UpdateGroupDto dto, CancellationToken ct);

    /// <summary>
    /// Удалить группу по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор группы</param>
    /// <param name="ct">Токен отмены</param>
    Task DeleteAsync(int id, CancellationToken ct);
}

