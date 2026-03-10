using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace CleanTeeth.Application.Features.UniversalCrud
{
    internal static class LogHelper
    {
        private const int MaxLogValueLength = 2000;

        private static readonly JsonSerializerOptions LogJsonOptions = new()
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        /// <summary>Получить Id сущности в виде строки (Guid или long).</summary>
        public static string GetEntityId(object entity)
        {
            var idProp = entity.GetType().GetProperty("Id");
            var id = idProp?.GetValue(entity);
            return id?.ToString() ?? string.Empty;
        }

        /// <summary>Сериализовать сущность в JSON для записи в OldValue/NewValue лога (кириллица сохраняется).</summary>
        public static string? ToLogJson(object? entity)
        {
            if (entity == null)
                return null;
            try
            {
                var json = JsonSerializer.Serialize(entity, entity.GetType(), LogJsonOptions);
                return TruncateIfNeeded(json);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>Построить два словаря: только старые значения изменённых полей и только новые (одинаковые свойства игнорируются).</summary>
        public static (Dictionary<string, object?> oldValues, Dictionary<string, object?> newValues) BuildEntityDiff(object? oldEntity, object? newEntity, Type entityType)
        {
            var oldValues = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            var newValues = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            if (oldEntity == null || newEntity == null)
                return (oldValues, newValues);

            foreach (var prop in entityType.GetProperties())
            {
                if (prop.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) || !prop.CanRead)
                    continue;

                var oldVal = prop.GetValue(oldEntity);
                var newVal = prop.GetValue(newEntity);

                if (Equals(oldVal, newVal))
                    continue;

                oldValues[prop.Name] = oldVal;
                newValues[prop.Name] = newVal;
            }

            return (oldValues, newValues);
        }

        /// <summary>Сериализовать словарь в JSON для лога (кириллица сохраняется).</summary>
        public static string? ToLogDictJson(Dictionary<string, object?>? dict)
        {
            if (dict == null || dict.Count == 0)
                return null;
            try
            {
                var json = JsonSerializer.Serialize(dict, LogJsonOptions);
                return TruncateIfNeeded(json);
            }
            catch
            {
                return null;
            }
        }

        private static string TruncateIfNeeded(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= MaxLogValueLength)
                return value;
            return value[..(MaxLogValueLength - 3)] + "...";
        }
    }
}
