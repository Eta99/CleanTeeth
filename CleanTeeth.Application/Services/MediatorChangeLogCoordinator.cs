using CleanTeeth.Application.Contracts.Persistence;
using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Features.UniversalCrud;
using CleanTeeth.Application.Features.UniversalCrud.Commands.Create;
using CleanTeeth.Application.Utilities;
using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Application.Services
{
    /// <summary>
    /// Универсальная подготовка и запись в Logs для запросов <see cref="ILoggable"/> после успешного handler.
    /// Для подготовки diff/снимка удаления тип запроса определяется по имени класса (подстроки Delete / Update), а данные читаются через рефлексию (как у универсальных команд).
    /// </summary>
    internal sealed class MediatorChangeLogCoordinator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ChangeLogSession _session;
        private readonly IRepositoryLongKey<Log> _logRepository;
        private readonly IAppActionRepository _actionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MediatorChangeLogCoordinator(
            IServiceProvider serviceProvider,
            ChangeLogSession session,
            IRepositoryLongKey<Log> logRepository,
            IAppActionRepository actionRepository,
            IUnitOfWork unitOfWork)
        {
            _serviceProvider = serviceProvider;
            _session = session;
            _logRepository = logRepository;
            _actionRepository = actionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task PrepareAsync(object request, CancellationToken cancellationToken = default)
        {
            _session.Clear();

            var rt = request.GetType();
            // Delete раньше Update: имя вроде "UpdateThenDeleteCommand" отнесём к удалению.
            if (TypeNameSuggestsDelete(rt))
                await PrepareDeleteAsync(request, rt, cancellationToken).ConfigureAwait(false);
            else if (TypeNameSuggestsUpdate(rt))
                await PrepareUpdateAsync(request, rt, cancellationToken).ConfigureAwait(false);
        }

        public void DiscardPrepared() => _session.Clear();

        public async Task PersistAfterSuccessAsync(object request, object? response,
            CancellationToken cancellationToken = default)
        {
            // Legacy no-op: audit logging moved to SaveChangesInterceptor.
            await Task.CompletedTask.ConfigureAwait(false);
            _session.Clear();
        }

        private static bool TypeNameSuggestsDelete(Type type) =>
            type.Name.Contains("Delete", StringComparison.OrdinalIgnoreCase);

        private static bool TypeNameSuggestsUpdate(Type type) =>
            type.Name.Contains("Update", StringComparison.OrdinalIgnoreCase);

        private async Task AddCreateLogAsync(object response)
        {
            var idObject = LogHelper.GetEntityId(response);
            var newValueJson = LogHelper.ToLogJson(response);
            await _logRepository.Add(new Log(idObject, oldValue: null, newValue: newValueJson)).ConfigureAwait(false);
        }

        private async Task PrepareDeleteAsync(object request, Type requestType, CancellationToken cancellationToken)
        {
            if (TryGetTypedDeleteShape(requestType, out var entityTypeArg))
            {
                var id = requestType.GetProperty("Id")?.GetValue(request);
                if (id == null)
                    return;
                var entity = await UniversalCrudRepositoryReflection
                    .GetEntityByIdAsync(_serviceProvider, entityTypeArg, id, cancellationToken).ConfigureAwait(false);
                _session.EntityForDelete = entity;
                return;
            }

            var entityTypeProp = requestType.GetProperty("EntityType");
            var idProp = requestType.GetProperty("Id");
            if (entityTypeProp?.GetValue(request) is not Type entityType || idProp?.GetValue(request) is not { } idObj)
                return;

            var idKey = ParseId(entityType, idObj.ToString()!);
            var entityNg = await UniversalCrudRepositoryReflection
                .GetEntityByIdAsync(_serviceProvider, entityType, idKey, cancellationToken).ConfigureAwait(false);
            _session.EntityForDelete = entityNg;
        }

        private async Task PrepareUpdateAsync(object request, Type requestType, CancellationToken cancellationToken)
        {
            if (TryGetTypedUpdateShape(requestType, out var entityTypeArg))
            {
                var id = requestType.GetProperty("Id")?.GetValue(request);
                var entity = requestType.GetProperty("Entity")?.GetValue(request);
                if (id == null || entity == null)
                    return;
                var old = await UniversalCrudRepositoryReflection
                    .GetEntityByIdAsync(_serviceProvider, entityTypeArg, id, cancellationToken).ConfigureAwait(false);
                if (old == null)
                    return;
                _session.EntityForTypedUpdate = old;
                var (o, n) = LogHelper.BuildEntityDiff(old, entity, entityTypeArg);
                _session.UpdateDiffOld = o;
                _session.UpdateDiffNew = n;
                _session.UpdateEntityId = LogHelper.GetEntityId(entity);
                return;
            }

            var entityTypeProp = requestType.GetProperty("EntityType");
            var entityProp = requestType.GetProperty("Entity");
            if (entityTypeProp?.GetValue(request) is not Type entityType || entityProp?.GetValue(request) is not { } newEntity)
                return;

            var idString = LogHelper.GetEntityId(newEntity);
            var idKey = ParseId(entityType, idString);
            var oldNg = await UniversalCrudRepositoryReflection
                .GetEntityByIdAsync(_serviceProvider, entityType, idKey, cancellationToken).ConfigureAwait(false);
            if (oldNg == null)
                return;
            var (oldVals, newVals) = LogHelper.BuildEntityDiff(oldNg, newEntity, entityType);
            _session.UpdateDiffOld = oldVals;
            _session.UpdateDiffNew = newVals;
            _session.UpdateEntityId = idString;
        }

        /// <summary>Форма DeleteCommand&lt;T&gt;: один generic-аргумент сущности и свойство Id.</summary>
        private static bool TryGetTypedDeleteShape(Type requestType, out Type entityType)
        {
            entityType = null!;
            if (!requestType.IsGenericType || requestType.GetGenericArguments().Length != 1)
                return false;
            if (requestType.GetProperty("Id") == null)
                return false;
            entityType = requestType.GetGenericArguments()[0];
            return true;
        }

        /// <summary>Форма UpdateCommand&lt;T&gt;: один generic-аргумент, свойства Id и Entity.</summary>
        private static bool TryGetTypedUpdateShape(Type requestType, out Type entityType)
        {
            entityType = null!;
            if (!requestType.IsGenericType || requestType.GetGenericArguments().Length != 1)
                return false;
            if (requestType.GetProperty("Id") == null || requestType.GetProperty("Entity") == null)
                return false;
            entityType = requestType.GetGenericArguments()[0];
            return true;
        }

        private async Task PersistUpdateFromSessionAsync()
        {
            var idObject = _session.UpdateEntityId;
            if (string.IsNullOrEmpty(idObject))
                return;
            var oldValueJson = LogHelper.ToLogDictJson(_session.UpdateDiffOld);
            var newValueJson = LogHelper.ToLogDictJson(_session.UpdateDiffNew);
            await _logRepository.Add(new Log(idObject, oldValue: oldValueJson, newValue: newValueJson))
                .ConfigureAwait(false);
        }

        private async Task PersistDeleteUsingSessionAsync(object request, Type requestType)
        {
            var entity = _session.EntityForDelete;
            if (entity == null)
                return;
            var id = requestType.GetProperty("Id")?.GetValue(request);
            var idObject = id?.ToString() ?? LogHelper.GetEntityId(entity);
            var oldValueJson = LogHelper.ToLogJson(entity);
            await _logRepository.Add(new Log(idObject, oldValue: oldValueJson, newValue: null)).ConfigureAwait(false);
        }

        private static object ParseId(Type entityType, string idString)
        {
            var idProp = entityType.GetProperty("Id")
                ?? throw new InvalidOperationException($"Type {entityType.Name} has no Id property.");
            if (idProp.PropertyType == typeof(Guid))
                return Guid.Parse(idString);
            if (idProp.PropertyType == typeof(long))
                return long.Parse(idString);
            if (idProp.PropertyType == typeof(int))
                return int.Parse(idString);
            return Convert.ChangeType(idString, idProp.PropertyType)
                ?? throw new InvalidOperationException($"Unsupported Id type {idProp.PropertyType.Name}.");
        }
    }
}
