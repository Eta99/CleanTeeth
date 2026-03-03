using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.UniversalCrud.Commands.Create
{
    /// <summary>
    /// Универсальная команда создания сущности.
    /// Тип репозитория задаётся снаружи через EntityType; команда принимает уже собранную сущность.
    /// </summary>
    public class CreateCommand : IRequest<object>
    {
        /// <summary>Тип сущности (тип репозитория).</summary>
        public required Type EntityType { get; set; }

        /// <summary>Сущность для добавления.</summary>
        public required object Entity { get; set; }

        /// <summary>Типизированный вызов: тип репозитория задаётся снаружи через generic.</summary>
        public static CreateCommand For<TEntity>(TEntity entity) where TEntity : class =>
            new() { EntityType = typeof(TEntity), Entity = entity };
    }
}
