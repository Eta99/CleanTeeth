using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.UniversalCrud.Queries.GetAll
{
    /// <summary>
    /// Универсальный запрос получения всех сущностей (тип передаётся через EntityType).
    /// Для вызова без передачи типа используйте <see cref="GetAllQuery{TEntity}"/> — тип берётся из шаблона &lt;TEntity&gt;.
    /// </summary>
    public class GetAllQuery : IRequest<IEnumerable<object>>
    {
        /// <summary>Тип сущности (тип репозитория).</summary>
        public required Type EntityType { get; set; }

        /// <summary>Типизированный вызов: тип задаётся через generic.</summary>
        public static GetAllQuery For<TEntity>() where TEntity : class =>
            new() { EntityType = typeof(TEntity) };
    }
}
