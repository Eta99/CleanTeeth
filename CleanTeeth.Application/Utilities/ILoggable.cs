namespace CleanTeeth.Application.Utilities
{
    /// <summary>
    /// Запрос даёт запись в Logs, если в БД для действия <see cref="IRequireAction.RequiredActionName"/>
    /// у <see cref="CleanTeeth.Domain.Entities.AppAction"/> включён IsLoggable.
    /// </summary>
    public interface ILoggable : IRequireAction
    {
    }
}
