using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Contracts.Persistence;
using CleanTeeth.Application.Features.UniversalCrud;
using CleanTeeth.Application.Utilities;
using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Application.Features.UniversalCrud.Commands.Create
{
    public class CreateCommandHandler<TEntity> : IRequestHandler<CreateCommand<TEntity>, TEntity> where TEntity : class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepositoryLongKey<Log> _logRepository;

        public CreateCommandHandler(IServiceProvider serviceProvider, IUnitOfWork unitOfWork, IRepositoryLongKey<Log> logRepository)
        {
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork;
            _logRepository = logRepository;
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
                var result = (TEntity)task.GetType().GetProperty(nameof(Task<object>.Result))!.GetValue(task)!;
                var idObject = LogHelper.GetEntityId(result);
                await _logRepository.Add(new Log(idObject, oldValue: null, newValue: $"Create:{entityType.Name}"));
                await _unitOfWork.Commit();
                return result;
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
