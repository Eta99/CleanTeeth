using CleanTeeth.Application.Contracts.Infrastructure;
using CleanTeeth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace CleanTeeth.Persistence.Interceptors
{
    internal sealed class AuditSaveChangesInterceptor : SaveChangesInterceptor
    {
        private const int MaxLogValueLength = 2000;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private readonly IAuditScopeAccessor auditScopeAccessor;
        private bool isSaving;

        public AuditSaveChangesInterceptor(IAuditScopeAccessor auditScopeAccessor)
        {
            this.auditScopeAccessor = auditScopeAccessor;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            AddLogs(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            AddLogs(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void AddLogs(DbContext? context)
        {
            if (context is null || isSaving)
                return;

            var scope = auditScopeAccessor.Current;
            if (!scope.Enabled || !scope.IsLoggableAction)
                return;

            try
            {
                isSaving = true;
                var logSet = context.Set<Log>();
                var entries = context.ChangeTracker.Entries()
                    .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                    .Where(e => e.Entity is not Log)
                    .ToArray();

                foreach (var entry in entries)
                {
                    var id = GetEntityId(entry);
                    if (string.IsNullOrWhiteSpace(id))
                        continue;

                    string? oldValue = null;
                    string? newValue = null;

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            newValue = Serialize(CurrentValues(entry));
                            break;
                        case EntityState.Modified:
                            var (oldDict, newDict) = DiffValues(entry);
                            oldValue = Serialize(oldDict);
                            newValue = Serialize(newDict);
                            if (oldValue == null && newValue == null)
                                continue;
                            break;
                        case EntityState.Deleted:
                            oldValue = Serialize(OriginalValues(entry));
                            break;
                    }

                    logSet.Add(new Log(id, oldValue, newValue));
                }
            }
            finally
            {
                isSaving = false;
            }
        }

        private static string GetEntityId(EntityEntry entry)
        {
            var prop = entry.Properties.FirstOrDefault(p => p.Metadata.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            var value = prop?.CurrentValue ?? prop?.OriginalValue;
            return value?.ToString() ?? string.Empty;
        }

        private static Dictionary<string, object?> CurrentValues(EntityEntry entry)
        {
            return entry.CurrentValues.Properties
                .Where(p => !p.IsShadowProperty() && !p.IsPrimaryKey())
                .ToDictionary(p => p.Name, p => entry.CurrentValues[p.Name]);
        }

        private static Dictionary<string, object?> OriginalValues(EntityEntry entry)
        {
            return entry.OriginalValues.Properties
                .Where(p => !p.IsShadowProperty() && !p.IsPrimaryKey())
                .ToDictionary(p => p.Name, p => entry.OriginalValues[p.Name]);
        }

        private static (Dictionary<string, object?> oldDict, Dictionary<string, object?> newDict) DiffValues(EntityEntry entry)
        {
            var oldDict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            var newDict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

            foreach (var property in entry.Properties)
            {
                if (property.Metadata.IsShadowProperty() || property.Metadata.IsPrimaryKey() || !property.IsModified)
                    continue;

                var oldValue = entry.OriginalValues[property.Metadata.Name];
                var newValue = entry.CurrentValues[property.Metadata.Name];
                if (Equals(oldValue, newValue))
                    continue;

                oldDict[property.Metadata.Name] = oldValue;
                newDict[property.Metadata.Name] = newValue;
            }

            return (oldDict, newDict);
        }

        private static string? Serialize(object? value)
        {
            if (value == null)
                return null;

            try
            {
                var serialized = JsonSerializer.Serialize(value, JsonOptions);
                return Truncate(serialized);
            }
            catch
            {
                return null;
            }
        }

        private static string Truncate(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= MaxLogValueLength)
                return value;

            return value[..(MaxLogValueLength - 3)] + "...";
        }
    }
}
