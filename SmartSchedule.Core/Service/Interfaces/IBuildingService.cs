using SmartSchedule.Application.DTOs.Building;
using SmartSchedule.Core.Models.DTO.BuildingDTO;
using SmartSchedule.Core.Models.DTOs.Building;

namespace SmartSchedule.Application.Services.Interfaces
{
    /// <summary>
    /// Сервис для работы с зданиями.
    /// </summary>
    public interface IBuildingService
    {
        /// <summary>
        /// Получить список всех зданий в короткой форме
        /// </summary>
        /// <param name="ct">Токен отмены операции</param>
        /// <returns></returns>
        Task<List<ShortBuildingDto>> GetAllShorts(CancellationToken ct);
        /// <summary>
        /// Получить список всех зданий.
        /// </summary>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Список зданий</returns>
        Task<List<ResponseBuildingDto>> GetAllAsync(CancellationToken ct);

        /// <summary>
        /// Получить здание по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор здания</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Данные о здании</returns>
        Task<ResponseBuildingDto> GetByIdAsync(int id, CancellationToken ct);

        /// <summary>
        /// Создать новое здание.
        /// </summary>
        /// <param name="dto">Данные для создания</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Созданное здание</returns>
        Task<ResponseBuildingDto> CreateAsync(CreateBuildingDto dto, CancellationToken ct);

        /// <summary>
        /// Обновить информацию о здании.
        /// </summary>
        /// <param name="id">Идентификатор здания</param>
        /// <param name="dto">Данные для обновления</param>
        /// <param name="ct">Токен отмены</param>
        Task UpdateAsync(int id, UpdateBuildingDto dto, CancellationToken ct);

        /// <summary>
        /// Удалить здание по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор здания</param>
        /// <param name="ct">Токен отмены</param>
        Task DeleteAsync(int id, CancellationToken ct);
    }
}