using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Application.DTOs.Building;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SmartSchedule.Core.Models.DTOs.Building;
using SmartSchedule.Core.Models.DTO.BuildingDTO;
using Microsoft.AspNetCore.Http;
using SmartSchedul.Core.Exceptions;
using System;
using SmartSchedule.Core.Exceptions;

namespace SmartSchedule.Application.Controllers
{
    /// <summary>
    /// Контроллер для работы со зданиями.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BuildingController : ControllerBase
    {
        #region Поля

        private readonly IBuildingService _buildingService;

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор контроллера зданий.
        /// </summary>
        /// <param name="buildingService">Сервис для работы со зданиями.</param>
        public BuildingController(IBuildingService buildingService)
        {
            _buildingService = buildingService;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Получить список всех зданий.
        /// </summary>
        /// <param name="ct">Токен отмены операции.</param>
        /// <returns>Список зданий.</returns>
        [HttpGet("all")]
        public async Task<ActionResult<List<ResponseBuildingDto>>> GetAll(CancellationToken ct)
        {
            var buildings = await _buildingService.GetAllAsync(ct);
            return Ok(buildings);
        }

        /// <summary>
        /// Получить список всех зданий в кратком варианте нужен для выподашек.
        /// </summary>
        /// <param name="ct">Токен отмены операции.</param>
        /// <returns>Список зданий.</returns>
        [HttpGet("Short")]
        public async Task<ActionResult<List<ShortBuildingDto>>> GetAllShorts(CancellationToken ct)
        {
            var buildings = await _buildingService.GetAllShorts(ct);
            return Ok(buildings);
        }
        /// <summary>
        /// Получить здание по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор здания.</param>
        /// <param name="ct">Токен отмены операции.</param>
        /// <returns>Здание с указанным идентификатором.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseBuildingDto>> GetById(int id, CancellationToken ct)
        {
            var building = await _buildingService.GetByIdAsync(id, ct);
            return building == null ? NotFound() : Ok(building);
        }
        /// <summary>
        /// Создать новое здание.
        /// </summary>
        /// <param name="dto">Данные для создания здания.</param>
        /// <param name="ct">Токен отмены операции.</param>
        /// <returns>Созданное здание.</returns>
        /// <response code="201">Здание успешно создано</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="409">Здание с таким названием уже существует</response>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseBuildingDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ResponseBuildingDto>> Create(
            [FromBody] CreateBuildingDto dto,
            CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var building = await _buildingService.CreateAsync(dto, ct);
                return CreatedAtAction(nameof(GetById), new { id = building.Id }, building);
            }
            catch (UniqueNameConflictException ex)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Конфликт уникальности",
                    Detail = ex.Message,
                    Extensions = { ["name"] = ex.Name },
                    Status = StatusCodes.Status409Conflict
                });
            }
            catch (Exception)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Внутренняя ошибка сервера",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        /// <summary>
        /// Обновить данные здания.
        /// </summary>
        /// <param name="id">Идентификатор здания.</param>
        /// <param name="dto">Данные для обновления здания.</param>
        /// <param name="ct">Токен отмены операции.</param>
        /// <returns>Результат операции.</returns>
        /// <response code="204">Здание успешно обновлено</response>
        /// <response code="400">Некорректные данные</response>
        /// <response code="404">Здание не найдено</response>
        /// <response code="409">Здание с таким названием уже существует</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(
            [FromRoute] int id,
            [FromBody] UpdateBuildingDto dto,
            CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                await _buildingService.UpdateAsync(id, dto, ct);
                return NoContent();
            }
            catch (ObjectNotFoundException ex)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Здание не найдено",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (UniqueNameConflictException ex)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Конфликт уникальности",
                    Detail = ex.Message,
                    Extensions = { ["name"] = ex.Name },
                    Status = StatusCodes.Status409Conflict
                });
            }
            catch (Exception )
            {
               
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Внутренняя ошибка сервера",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }


        /// <summary>
        /// Удалить здание по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор здания.</param>
        /// <param name="ct">Токен отмены операции.</param>
        /// <returns>Результат операции.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _buildingService.DeleteAsync(id, ct);
            return NoContent();
        }

        #endregion
    }
}
