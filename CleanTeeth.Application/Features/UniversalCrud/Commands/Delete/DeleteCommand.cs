using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.UniversalCrud.Commands.Delete
{
    /// <summary>
    /// Универсальная команда удаления сущности по Id.
    /// Тип репозитория задаётся снаружи через EntityType.
    /// </summary>
    public class DeleteCommand : IRequest, IRequireAction
    {
        /// <summary>Тип сущности (тип репозитория).</summary>
        public required Type EntityType { get; set; }

        /// <summary>Идентификатор (Guid или long).</summary>
        public required object Id { get; set; }

        /// <summary>Имя действия для проверки прав (например, "Delete:User").</summary>
        public required string RequiredActionName { get; set; }

        /// <summary>Типизированный вызов: тип репозитория задаётся снаружи через generic.</summary>
        public static DeleteCommand For<TEntity>(object id, string requiredActionName) where TEntity : class =>
            new() { EntityType = typeof(TEntity), Id = id, RequiredActionName = requiredActionName };
    }
}
