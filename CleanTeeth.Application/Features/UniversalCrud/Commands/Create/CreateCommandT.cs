using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.UniversalCrud.Commands.Create
{
    /// <summary>
    /// Типизированная команда создания сущности.
    /// Тип берётся из шаблона &lt;TEntity&gt;, передавать имя типа не нужно.
    /// </summary>
    public class CreateCommand<TEntity> : IRequest<TEntity> where TEntity : class
    {
        public required TEntity Entity { get; set; }
    }
}
