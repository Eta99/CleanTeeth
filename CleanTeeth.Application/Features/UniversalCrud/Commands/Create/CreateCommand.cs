using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.UniversalCrud.Commands.Create
{
    /// <summary>
    /// Универсальная команда создания сущности.
    /// Тип репозитория задаётся снаружи через EntityType; команда принимает уже собранную сущность.
    /// </summary>
    public class CreateCommand : IRequest<object>, IRequireAction
    {
        /// <summary>Тип сущности (тип репозитория).</summary>
        public required Type EntityType { get; set; }

        /// <summary>Сущность для добавления.</summary>
        public required object Entity { get; set; }

        /// <summary>Имя действия для проверки прав (например, "Create:User").</summary>
        public required string RequiredActionName { get; set; }

        /// <summary>Типизированный вызов: тип репозитория задаётся снаружи через generic.</summary>
        public static CreateCommand For<TEntity>(TEntity entity, string requiredActionName) where TEntity : class =>
            new() { EntityType = typeof(TEntity), Entity = entity, RequiredActionName = requiredActionName };
    }
}
