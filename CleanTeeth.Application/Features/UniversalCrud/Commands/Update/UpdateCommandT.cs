using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.UniversalCrud.Commands.Update
{
    /// <summary>
    /// Типизированная команда обновления сущности.
    /// Тип берётся из шаблона &lt;TEntity&gt;, передавать имя типа не нужно.
    /// Id нужен для загрузки старой версии и записи лога (до/после).
    /// </summary>
    public class UpdateCommand<TEntity> : IRequest where TEntity : class
    {
        /// <summary>Идентификатор сущности (из маршрута).</summary>
        public required object Id { get; set; }

        /// <summary>Сущность с новыми значениями (из тела запроса).</summary>
        public required TEntity Entity { get; set; }
    }
}
