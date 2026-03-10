using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.UniversalCrud.Commands.Delete
{
    /// <summary>
    /// Типизированная команда удаления сущности по Id.
    /// Тип берётся из шаблона &lt;TEntity&gt;, передавать имя типа не нужно.
    /// </summary>
    public class DeleteCommand<TEntity> : IRequest, IRequireAction where TEntity : class
    {
        public required object Id { get; set; }

        /// <summary>Имя действия для проверки прав (например, "Delete:Patient").</summary>
        public required string RequiredActionName { get; set; }
    }
}
