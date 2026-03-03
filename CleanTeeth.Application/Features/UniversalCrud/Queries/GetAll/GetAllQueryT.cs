using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.UniversalCrud.Queries.GetAll
{
    /// <summary>
    /// Типизированный запрос получения всех сущностей.
    /// Тип берётся из шаблона &lt;TEntity&gt;, передавать имя типа не нужно.
    /// </summary>
    public class GetAllQuery<TEntity> : IRequest<IEnumerable<TEntity>> where TEntity : class
    {
    }
}
