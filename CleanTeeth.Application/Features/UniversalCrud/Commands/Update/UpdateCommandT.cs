using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.UniversalCrud.Commands.Update
{
    /// <summary>
    /// Типизированная команда обновления сущности.
    /// Тип берётся из шаблона &lt;TEntity&gt;, передавать имя типа не нужно.
    /// </summary>
    public class UpdateCommand<TEntity> : IRequest where TEntity : class
    {
        public required TEntity Entity { get; set; }
    }
}
