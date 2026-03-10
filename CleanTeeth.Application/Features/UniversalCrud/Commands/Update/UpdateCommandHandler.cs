using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Contracts.Persistence;
using CleanTeeth.Application.Features.UniversalCrud;
using CleanTeeth.Application.Utilities;
using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Application.Features.UniversalCrud.Commands.Update
{
    public class UpdateCommandHandler : IRequestHandler<UpdateCommand>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepositoryLongKey<Log> _logRepository;
        private readonly IAppActionRepository _actionRepository;

        public UpdateCommandHandler(IServiceProvider serviceProvider, IUnitOfWork unitOfWork, IRepositoryLongKey<Log> logRepository, IAppActionRepository actionRepository)
        {
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork;
            _logRepository = logRepository;
            _actionRepository = actionRepository;
        }

        public async Task Handle(UpdateCommand request)
        {
            if (request.EntityType == null)
                throw new ArgumentException("EntityType is required.", nameof(request));
            if (request.Entity == null)
                throw new ArgumentException("Entity is required.", nameof(request));

            var repo = GetRepository(request.EntityType);
            var repoType = repo.GetType();
            var updateMethod = GetRepositoryInterface(repoType, request.EntityType)?.GetMethod(nameof(IRepository<object>.Update));
            if (updateMethod == null)
                throw new InvalidOperationException($"Repository for {request.EntityType.Name} does not support Update.");

            object? oldEntity = null;
            var isGuidKey = GetRepositoryInterface(repoType, request.EntityType)?.GetGenericTypeDefinition() == typeof(IRepository<>);
            var idObject = LogHelper.GetEntityId(request.Entity);
            if (isGuidKey && Guid.TryParse(idObject, out var guidId))
            {
                var repoInterface = typeof(IRepository<>).MakeGenericType(request.EntityType);
                var getById = repoInterface.GetMethod("GetById")!;
                var task = (Task)getById.Invoke(repo, new object[] { guidId })!;
                await task.ConfigureAwait(false);
                oldEntity = task.GetType().GetProperty("Result")!.GetValue(task);
            }
            else if (long.TryParse(idObject, out var longId))
            {
                var repoLongInterface = typeof(IRepositoryLongKey<>).MakeGenericType(request.EntityType);
                var getById = repoLongInterface.GetMethod("GetById")!;
                var task = (Task)getById.Invoke(repo, new object[] { longId })!;
                await task.ConfigureAwait(false);
                oldEntity = task.GetType().GetProperty("Result")!.GetValue(task);
            }

            try
            {
                var task = (Task)updateMethod.Invoke(repo, new[] { request.Entity })!;
                await task.ConfigureAwait(false);
                var action = await _actionRepository.GetByNameAsync(request.RequiredActionName);
                if (action?.IsLoggable == true)
                {
                    var (oldValues, newValues) = LogHelper.BuildEntityDiff(oldEntity, request.Entity, request.EntityType);
                    var oldValueJson = LogHelper.ToLogDictJson(oldValues);
                    var newValueJson = LogHelper.ToLogDictJson(newValues);
                    await _logRepository.Add(new Log(idObject, oldValue: oldValueJson, newValue: newValueJson));
                }
                await _unitOfWork.Commit();
            }
            catch
            {
                await _unitOfWork.Rollback();
                throw;
            }
        }

        private object GetRepository(Type entityType)
        {
            var repoInterface = typeof(IRepository<>).MakeGenericType(entityType);
            var repo = _serviceProvider.GetService(repoInterface);
            if (repo != null) return repo;

            var repoLongInterface = typeof(IRepositoryLongKey<>).MakeGenericType(entityType);
            repo = _serviceProvider.GetService(repoLongInterface);
            if (repo == null)
                throw new InvalidOperationException($"Repository for entity type {entityType.Name} is not registered.");
            return repo;
        }

        private static Type? GetRepositoryInterface(Type repoType, Type entityType)
        {
            var ifaces = repoType.GetInterfaces();
            var repoOpen = typeof(IRepository<>);
            var repoLongOpen = typeof(IRepositoryLongKey<>);
            foreach (var i in ifaces)
                if (i.IsGenericType && (i.GetGenericTypeDefinition() == repoOpen || i.GetGenericTypeDefinition() == repoLongOpen))
                    return i;
            return null;
        }
    }
}
