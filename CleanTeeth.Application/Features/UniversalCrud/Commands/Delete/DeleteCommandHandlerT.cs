using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Contracts.Persistence;
using CleanTeeth.Application.Exceptions;
using CleanTeeth.Application.Features.UniversalCrud;
using CleanTeeth.Application.Utilities;
using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Application.Features.UniversalCrud.Commands.Delete
{
    public class DeleteCommandHandler<TEntity> : IRequestHandler<DeleteCommand<TEntity>> where TEntity : class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepositoryLongKey<Log> _logRepository;
        private readonly IAppActionRepository _actionRepository;

        public DeleteCommandHandler(IServiceProvider serviceProvider, IUnitOfWork unitOfWork, IRepositoryLongKey<Log> logRepository, IAppActionRepository actionRepository)
        {
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork;
            _logRepository = logRepository;
            _actionRepository = actionRepository;
        }

        public async Task Handle(DeleteCommand<TEntity> request)
        {
            var entityType = typeof(TEntity);
            var repo = GetRepository(entityType);
            var repoType = repo.GetType();
            var repoInterface = typeof(IRepository<>).MakeGenericType(entityType);
            var isGuidKey = GetRepositoryInterface(repoType)?.GetGenericTypeDefinition() == typeof(IRepository<>);

            object? entity;
            if (isGuidKey)
            {
                var id = request.Id is Guid g ? g : Guid.Parse(request.Id.ToString()!);
                var getById = repoInterface.GetMethod("GetById")!;
                var task = (Task)getById.Invoke(repo, new object[] { id })!;
                await task.ConfigureAwait(false);
                entity = task.GetType().GetProperty("Result")!.GetValue(task);
            }
            else
            {
                var repoLongInterface = typeof(IRepositoryLongKey<>).MakeGenericType(entityType);
                var idLong = request.Id is long l ? l : Convert.ToInt64(request.Id);
                var getById = repoLongInterface.GetMethod("GetById")!;
                var task = (Task)getById.Invoke(repo, new object[] { idLong })!;
                await task.ConfigureAwait(false);
                entity = task.GetType().GetProperty("Result")!.GetValue(task);
            }

            if (entity == null)
                throw new NotFoundException();

            var deleteMethod = GetRepositoryInterface(repoType)?.GetMethod(nameof(IRepository<object>.Delete));
            if (deleteMethod == null)
                throw new InvalidOperationException($"Repository for {entityType.Name} does not support Delete.");

            try
            {
                var idObject = request.Id?.ToString() ?? LogHelper.GetEntityId(entity);
                var action = await _actionRepository.GetByNameAsync(request.RequiredActionName);
                if (action?.IsLoggable == true)
                {
                    var oldValueJson = LogHelper.ToLogJson(entity);
                    await _logRepository.Add(new Log(idObject, oldValue: oldValueJson, newValue: null));
                }
                var deleteTask = (Task)deleteMethod.Invoke(repo, new[] { entity })!;
                await deleteTask.ConfigureAwait(false);
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
            var repo = _serviceProvider.GetService(typeof(IRepository<>).MakeGenericType(entityType));
            if (repo != null) return repo;
            repo = _serviceProvider.GetService(typeof(IRepositoryLongKey<>).MakeGenericType(entityType));
            if (repo == null)
                throw new InvalidOperationException($"Repository for entity type {entityType.Name} is not registered.");
            return repo;
        }

        private static Type? GetRepositoryInterface(Type repoType)
        {
            var repoOpen = typeof(IRepository<>);
            var repoLongOpen = typeof(IRepositoryLongKey<>);
            foreach (var i in repoType.GetInterfaces())
                if (i.IsGenericType && (i.GetGenericTypeDefinition() == repoOpen || i.GetGenericTypeDefinition() == repoLongOpen))
                    return i;
            return null;
        }
    }
}
