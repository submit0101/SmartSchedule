using AutoMapper;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Services.Interfaces;
using SmartSchedule.Core.Entities;
using SmartSchedule.Core.Models.DTO.TeacherDTO;
using SmartSchedule.Core.Repositories;
using SmartSchedule.Core.Service.Interfaces;

namespace SmartSchedule.Application.Services;

/// <summary>
/// Реализация сервиса для работы с преподавателями.
/// </summary>
public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICachingService _cache;

    /// <summary>
    /// Инициализирует новый экземпляр сервиса.
    /// </summary>
    /// <param name="repository">Репозиторий преподавателей.</param>
    /// <param name="mapper">Маппер объектов.</param>
    /// <param name="cache">Сервис кэширования.</param>
    public TeacherService(ITeacherRepository repository, IMapper mapper, ICachingService cache)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <inheritdoc/>
    public async Task<List<ShortTeacherDto>> GetShortAllAsync(CancellationToken ct)
    {
        return await _cache.GetOrSetAsync(
            "teachers:short",
            async () =>
            {
                var teachers = await _repository.GetAllAsync(ct).ConfigureAwait(false);
                return _mapper.Map<List<ShortTeacherDto>>(teachers);
            },
            TimeSpan.FromHours(12),
            ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<ResponseTeacherDto>> GetAllAsync(CancellationToken ct)
    {
        return await _cache.GetOrSetAsync(
            "teachers:all",
            async () =>
            {
                var teachers = await _repository.GetAllAsync(ct).ConfigureAwait(false);
                return _mapper.Map<List<ResponseTeacherDto>>(teachers);
            },
            TimeSpan.FromHours(12),
            ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<ResponseTeacherDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var teacher = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        return _mapper.Map<ResponseTeacherDto>(teacher);
    }

    /// <inheritdoc/>
    public async Task<ResponseTeacherDto> CreateAsync(CreateTeacherDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var teacher = _mapper.Map<Teacher>(dto);
        await _repository.CreateAsync(teacher, ct).ConfigureAwait(false);

        await _cache.RemoveAsync("teachers:all", ct).ConfigureAwait(false);
        await _cache.RemoveAsync("teachers:short", ct).ConfigureAwait(false);

        return _mapper.Map<ResponseTeacherDto>(teacher);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(int id, UpdateTeacherDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var teacher = await _repository.GetByIdAsync(id, ct).ConfigureAwait(false);
        if (teacher == null)
            throw new ArgumentException("Преподаватель не найден", nameof(id));

        _mapper.Map(dto, teacher);
        await _repository.UpdateAsync(teacher, ct).ConfigureAwait(false);

        await _cache.RemoveAsync("teachers:all", ct).ConfigureAwait(false);
        await _cache.RemoveAsync("teachers:short", ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        await _repository.DeleteByIdAsync(id, ct).ConfigureAwait(false);
        await _cache.RemoveAsync("teachers:all", ct).ConfigureAwait(false);
        await _cache.RemoveAsync("teachers:short", ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<List<ResponseTeacherDto>> SearchTeachersAsync(string? search)
    {
        var teachers = await _repository.SearchAsync(search).ConfigureAwait(false);
        return _mapper.Map<List<ResponseTeacherDto>>(teachers);
    }
    /// <summary>
    /// Импортирует список преподавателей из потока Excel-файла.
    /// </summary>
    /// <param name="excelStream">Поток с данными Excel-файла.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Результат импорта с количеством добавленных записей и списком ошибок.</returns>
   
    public async Task<ImportResultDto> ImportFromExcelAsync(Stream excelStream, CancellationToken ct)
    {
        var result = new ImportResultDto();
        var newTeachers = new List<Teacher>();

        try
        {
            var existingTeachers = await _repository.GetAllAsync(ct).ConfigureAwait(false);
            var existingNames = new HashSet<string>(
                existingTeachers.Select(x => $"{x.LastName} {x.FirstName} {x.MiddleName}".Trim())
            );

            using var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheet(1);
            var rangeUsed = worksheet.RangeUsed();

            if (rangeUsed == null)
                return result;

            var rows = rangeUsed.RowsUsed().Skip(1);
            int rowIndex = 1;

            foreach (var row in rows)
            {
                rowIndex++;
                var lastName = row.Cell(1).GetValue<string>()?.Trim();
                var firstName = row.Cell(2).GetValue<string>()?.Trim();
                var middleName = row.Cell(3).GetValue<string>()?.Trim();

                if (string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(firstName))
                {
                    result.Errors.Add($"Строка {rowIndex}: Фамилия и Имя обязательны.");
                    continue;
                }

                if (lastName.Length > 50 || firstName.Length > 50 || middleName?.Length > 50)
                {
                    result.Errors.Add($"Строка {rowIndex}: Превышена максимальная длина ФИО (50 символов).");
                    continue;
                }

                var fullName = $"{lastName} {firstName} {middleName}".Trim();

                if (existingNames.Contains(fullName))
                {
                    result.Errors.Add($"Строка {rowIndex}: Преподаватель {fullName} уже существует.");
                    continue;
                }

                newTeachers.Add(new Teacher
                {
                    LastName = lastName,
                    FirstName = firstName,
                    MiddleName = string.IsNullOrWhiteSpace(middleName) ? null : middleName
                });

                existingNames.Add(fullName);
            }

            if (newTeachers.Count > 0)
            {
                await _repository.CreateRangeAsync(newTeachers, ct).ConfigureAwait(false);
                await _cache.RemoveAsync("teachers:all", ct).ConfigureAwait(false);
                await _cache.RemoveAsync("teachers:short", ct).ConfigureAwait(false);
            }

            result.AddedCount = newTeachers.Count;
        }
        catch (InvalidDataException)
        {
            result.Errors.Add("Файл поврежден или имеет неверный формат Excel.");
        }
        catch (DbUpdateException)
        {
            result.Errors.Add("Ошибка при сохранении данных в базу. Проверьте корректность данных.");
        }
        catch (Exception)
        {
            throw;
        }

        return result;
    }

}