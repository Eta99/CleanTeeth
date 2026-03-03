using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.UniversalCrud.Queries.GetById
{
    /// <summary>
    /// Типизированный запрос получения сущности по Id.
    /// Тип берётся из шаблона &lt;TEntity&gt;, передавать имя типа не нужно.
    /// </summary>
    public class GetByIdQuery<TEntity> : IRequest<TEntity?> where TEntity : class
    {
        public required object Id { get; set; }
    }
}
