using AutoMapper;
using SmartSchedule.Application.DTOs.Building;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Exceptions;
using SmartSchedule.Core.Models.DTO.BuildingDTO;
using SmartSchedule.Core.Models.DTOs.Building;
using SmartSchedule.Core.Repositories;

namespace SmartSchedule.Core.Service
{
    /// <summary>
    /// Сервис для работы со зданиями.
    /// </summary>
    public class BuildingService : IBuildingService
    {
        #region Поля

        private readonly IBuildingRepository _repository;
        private readonly IMapper _mapper;

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор сервиса зданий.
        /// </summary>
        /// <param name="repository">Репозиторий зданий.</param>
        /// <param name="mapper">Объект маппинга.</param>
        /// <exception cref="ArgumentNullException">Выбрасывается, если параметры равны null.</exception>
        public BuildingService(IBuildingRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #endregion

        #region Методы

        /// <summary>
        /// Получить список всех зданий.
        /// </summary>
        /// <param name="ct">Токен отмены операции.</param>
        /// <returns>Список зданий в виде DTO.</returns>
        public async Task<List<ResponseBuildingDto>> GetAllAsync(CancellationToken ct)
        {
            var buildings = await _repository.GetAllAsync(ct).ConfigureAwait(false);
            return _mapper.Map<List<ResponseBuildingDto>>(buildings);
        }
        /// <summary>
        /// Получить список всех зданий в кратком варианте
        /// </summary>
        /// <param name="ct">токен отмены </param>
        /// <returns></returns>
        public async Task<List<ShortBuildingDto>> GetAllShorts(CancellationToken ct)
        {
            var buildings = await _repository.GetAllAsync(ct).ConfigureAwait(false);
            return _mapper.Map<List<ShortBuildingDto>>(buildings);
        }

        /// <summary>
        /// Получить здание по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор здания.</param>
        /// <param name="ct">Токен отмены операции.</param>
        /// <returns>DTO здания.</returns>
        public async Task<ResponseBuildingDto> GetByIdAsync(int id, CancellationToken ct)
        {
            var building = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
            return _mapper.Map<ResponseBuildingDto>(building);
        }

        /// <summary>
        /// Создать новое здание.
        /// </summary>
        /// <param name="dto">Данные для создания здания.</param>
        /// <param name="ct">Токен отмены операции.</param>
        /// <returns>DTO созданного здания.</returns>
        public async Task<ResponseBuildingDto> CreateAsync(CreateBuildingDto dto, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(dto);
            if (await _repository.ExistsByNameAsync(dto.Name, null, ct).ConfigureAwait(false))
                throw new UniqueNameConflictException(dto.Name);
            var building = _mapper.Map<Building>(dto);
            await _repository.CreateAsync(building, ct).ConfigureAwait(false);
            return _mapper.Map<ResponseBuildingDto>(building);
        }

        /// <summary>
        /// Обновить информацию о здании.
        /// </summary>
        /// <param name="id">Идентификатор здания.</param>
        /// <param name="dto">Данные для обновления.</param>
        /// <param name="ct">Токен отмены операции.</param>
        public async Task UpdateAsync(int id, UpdateBuildingDto dto, CancellationToken ct)
        {
            ArgumentNullException.ThrowIfNull(dto);
            if (await _repository.ExistsByNameAsync(dto.Name, id, ct).ConfigureAwait(false))
                throw new UniqueNameConflictException(dto.Name);
            var building = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
            ArgumentNullException.ThrowIfNull(building);
            _mapper.Map(dto, building);
            await _repository.UpdateAsync(building, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Удалить здание по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор здания.</param>
        /// <param name="ct">Токен отмены операции.</param>
        public async Task DeleteAsync(int id, CancellationToken ct)
        {
            await _repository.DeleteByIdAsync(id, ct).ConfigureAwait(false);
        }

        #endregion
    }
}
