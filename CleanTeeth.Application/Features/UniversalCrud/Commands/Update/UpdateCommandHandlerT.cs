using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Contracts.Persistence;
using CleanTeeth.Application.Features.UniversalCrud;
using CleanTeeth.Application.Utilities;
using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Application.Features.UniversalCrud.Commands.Update
{
    public class UpdateCommandHandler<TEntity> : IRequestHandler<UpdateCommand<TEntity>> where TEntity : class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepositoryLongKey<Log> _logRepository;

        public UpdateCommandHandler(IServiceProvider serviceProvider, IUnitOfWork unitOfWork, IRepositoryLongKey<Log> logRepository)
        {
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork;
            _logRepository = logRepository;
        }

        public async Task Handle(UpdateCommand<TEntity> request)
        {
            var entityType = typeof(TEntity);
            var repo = GetRepository(entityType);
            var repoType = repo.GetType();
            var updateMethod = GetRepositoryInterface(repoType)?.GetMethod(nameof(IRepository<object>.Update));
            if (updateMethod == null)
                throw new InvalidOperationException($"Repository for {entityType.Name} does not support Update.");

            try
            {
                var task = (Task)updateMethod.Invoke(repo, new object[] { request.Entity })!;
                await task.ConfigureAwait(false);
                var idObject = LogHelper.GetEntityId(request.Entity);
                await _logRepository.Add(new Log(idObject, oldValue: null, newValue: $"Update:{entityType.Name}"));
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
