using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.UniversalCrud.Commands.Update
{
    /// <summary>
    /// Универсальная команда обновления сущности.
    /// Тип репозитория задаётся снаружи через EntityType.
    /// </summary>
    public class UpdateCommand : IRequest, ILoggable
    {
        /// <summary>Тип сущности (тип репозитория).</summary>
        public required Type EntityType { get; set; }

        /// <summary>Сущность с обновлёнными данными.</summary>
        public required object Entity { get; set; }

        /// <summary>Имя действия для проверки прав (например, "Update:User").</summary>
        public required string RequiredActionName { get; set; }

        /// <summary>Типизированный вызов: тип репозитория задаётся снаружи через generic.</summary>
        public static UpdateCommand For<TEntity>(TEntity entity, string requiredActionName) where TEntity : class =>
            new() { EntityType = typeof(TEntity), Entity = entity, RequiredActionName = requiredActionName };
    }
}
