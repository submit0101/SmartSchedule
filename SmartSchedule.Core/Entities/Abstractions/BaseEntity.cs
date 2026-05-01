namespace SmartSchedule.Core.Entities.Abstractions;
/// <summary>
///  Базовый класс сущности
/// </summary>
/// <typeparam name="TId"></typeparam>
public abstract class BaseEntity<TId> : IEntity<TId> where TId : notnull
{
    /// <summary>
    ///  индетификатор сущности 
    /// </summary>
    public TId Id { get; set; } = default!;
}