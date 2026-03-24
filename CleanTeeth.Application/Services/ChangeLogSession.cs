using System.Collections.Generic;

namespace CleanTeeth.Application.Services
{
    /// <summary>
    /// Данные для записи лога изменений между Prepare и Persist в одном scope запроса.
    /// </summary>
    public sealed class ChangeLogSession
    {
        /// <summary>Загруженная сущность для UpdateCommand&lt;T&gt; (ApplyChanges мутирует её).</summary>
        public object? EntityForTypedUpdate { get; set; }

        public Dictionary<string, object?>? UpdateDiffOld { get; set; }
        public Dictionary<string, object?>? UpdateDiffNew { get; set; }
        public string? UpdateEntityId { get; set; }

        public object? EntityForDelete { get; set; }

        public void Clear()
        {
            EntityForTypedUpdate = null;
            UpdateDiffOld = null;
            UpdateDiffNew = null;
            UpdateEntityId = null;
            EntityForDelete = null;
        }
    }
}
