using System;

namespace CleanTeeth.Domain.Entities
{
    /// <summary>
    /// Запись лога изменений: Id объекта и разница полей (старое и новое значение).
    /// </summary>
    public class Log
    {
        public long Id { get; private set; }
        /// <summary>Идентификатор объекта, по которому пишем лог (ID сущности).</summary>
        public string IdObject { get; private set; } = null!;
        /// <summary>Старое значение поля (текст/число и т.д.).</summary>
        public string? OldValue { get; private set; }
        /// <summary>Новое значение поля после изменения.</summary>
        public string? NewValue { get; private set; }

        private Log() { }

        public Log(string idObject, string? oldValue = null, string? newValue = null)
        {
            IdObject = idObject ?? throw new ArgumentNullException(nameof(idObject));
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
