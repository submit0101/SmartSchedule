namespace SmartSchedule.Core.Entities.Abstractions;
/// <summary>
/// Базовый интерфейс для сущности с типизированным идентификатором.
/// </summary>
/// <typeparam name="TId"></typeparam>
public interface IEntity<TId> : IEntity
{
    /// <summary>
    /// Типизированный идентификатор
    /// </summary>
    TId Id { get; set; }
}
/// <summary>
///  Базовый интерфейс для сущности без указания типа данных
/// </summary>
public interface IEntity
{

}