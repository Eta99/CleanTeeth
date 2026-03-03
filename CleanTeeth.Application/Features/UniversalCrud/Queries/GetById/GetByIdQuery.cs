using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.UniversalCrud.Queries.GetById
{
    /// <summary>
    /// Универсальный запрос получения сущности по Id (тип передаётся через EntityType).
    /// Для вызова без передачи типа используйте <see cref="GetByIdQuery{TEntity}"/> — тип берётся из шаблона &lt;TEntity&gt;.
    /// </summary>
    public class GetByIdQuery : IRequest<object?>
    {
        /// <summary>Тип сущности (тип репозитория).</summary>
        public required Type EntityType { get; set; }

        /// <summary>Идентификатор (Guid или long в зависимости от репозитория).</summary>
        public required object Id { get; set; }

        /// <summary>Типизированный вызов: тип задаётся через generic, передавать имя типа не нужно.</summary>
        public static GetByIdQuery For<TEntity>(object id) where TEntity : class =>
            new() { EntityType = typeof(TEntity), Id = id };
    }
}
