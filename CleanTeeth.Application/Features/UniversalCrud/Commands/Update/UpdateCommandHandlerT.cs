using CleanTeeth.Application.Contracts.Persistence;
using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Exceptions;
using CleanTeeth.Application.Features.UniversalCrud;
using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.UniversalCrud.Commands.Update
{
    public class UpdateCommandHandler<TEntity> : IRequestHandler<UpdateCommand<TEntity>> where TEntity : class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCommandHandler(IServiceProvider serviceProvider, IUnitOfWork unitOfWork)
        {
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateCommand<TEntity> request)
        {
            var entityType = typeof(TEntity);
            var repo = GetRepository(entityType);
            var repoType = repo.GetType();
            var updateMethod = GetRepositoryInterface(repoType)?.GetMethod(nameof(IRepository<object>.Update));
            if (updateMethod == null)
                throw new InvalidOperationException($"Repository for {entityType.Name} does not support Update.");

            var idObject = request.Id?.ToString() ?? LogHelper.GetEntityId(request.Entity);
            var isGuidKey = GetRepositoryInterface(repoType)?.GetGenericTypeDefinition() == typeof(IRepository<>);
            TEntity? oldEntity = null;

            if (oldEntity == null)
            {
                if (isGuidKey && Guid.TryParse(idObject, out var guidId))
                {
                    var repoInterface = typeof(IRepository<>).MakeGenericType(entityType);
                    var getById = repoInterface.GetMethod("GetById")!;
                    var task = (Task)getById.Invoke(repo, new object[] { guidId })!;
                    await task.ConfigureAwait(false);
                    oldEntity = (TEntity?)task.GetType().GetProperty("Result")!.GetValue(task);
                }
                else if (long.TryParse(idObject, out var longId))
                {
                    var repoLongInterface = typeof(IRepositoryLongKey<>).MakeGenericType(entityType);
                    var getById = repoLongInterface.GetMethod("GetById")!;
                    var task = (Task)getById.Invoke(repo, new object[] { longId })!;
                    await task.ConfigureAwait(false);
                    oldEntity = (TEntity?)task.GetType().GetProperty("Result")!.GetValue(task);
                }
            }

            if (oldEntity == null)
                throw new NotFoundException();

            ApplyChanges(oldEntity, request.Entity, entityType);

            try
            {
                var task = (Task)updateMethod.Invoke(repo, new object[] { oldEntity })!;
                await task.ConfigureAwait(false);
                await _unitOfWork.Commit().ConfigureAwait(false);
            }
            catch
            {
                await _unitOfWork.Rollback().ConfigureAwait(false);
                throw;
            }
        }

        private static void ApplyChanges(TEntity target, TEntity source, Type entityType)
        {
            foreach (var prop in entityType.GetProperties())
            {
                if (prop.Name == "Id" || !prop.CanRead)
                    continue;
                var updateMethod = entityType.GetMethod("Update" + prop.Name, new[] { prop.PropertyType });
                if (updateMethod == null)
                    continue;
                var value = prop.GetValue(source);
                if (value == null && prop.PropertyType.IsValueType && Nullable.GetUnderlyingType(prop.PropertyType) == null)
                    continue;
                updateMethod.Invoke(target, new[] { value });
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
