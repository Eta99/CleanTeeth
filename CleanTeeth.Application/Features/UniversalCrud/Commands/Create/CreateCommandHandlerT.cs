using CleanTeeth.Application.Contracts.Persistence;
using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.UniversalCrud.Commands.Create
{
    public class CreateCommandHandler<TEntity> : IRequestHandler<CreateCommand<TEntity>, TEntity> where TEntity : class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCommandHandler(IServiceProvider serviceProvider, IUnitOfWork unitOfWork)
        {
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<TEntity> Handle(CreateCommand<TEntity> request)
        {
            var entityType = typeof(TEntity);
            var repo = GetRepository(entityType);
            var repoType = repo.GetType();
            var addMethod = GetRepositoryInterface(repoType)?.GetMethod(nameof(IRepository<object>.Add));
            if (addMethod == null)
                throw new InvalidOperationException($"Repository for {entityType.Name} does not support Add.");

            try
            {
                var task = (Task)addMethod.Invoke(repo, new object[] { request.Entity })!;
                await task.ConfigureAwait(false);
                var result = (TEntity)task.GetType().GetProperty("Result")!.GetValue(task)!;
                await _unitOfWork.Commit().ConfigureAwait(false);
                return result;
            }
            catch
            {
                await _unitOfWork.Rollback().ConfigureAwait(false);
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
