using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Utilities;
using System.Collections;

namespace CleanTeeth.Application.Features.UniversalCrud.Queries.GetAll
{
    public class GetAllQueryHandler<TEntity> : IRequestHandler<GetAllQuery<TEntity>, IEnumerable<TEntity>> where TEntity : class
    {
        private readonly IServiceProvider _serviceProvider;

        public GetAllQueryHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<IEnumerable<TEntity>> Handle(GetAllQuery<TEntity> request)
        {
            var entityType = typeof(TEntity);
            var repoInterface = typeof(IRepository<>).MakeGenericType(entityType);
            var repo = _serviceProvider.GetService(repoInterface);

            if (repo != null)
            {
                var method = repoInterface.GetMethod(nameof(IRepository<object>.GetAll))!;
                var task = (Task)method.Invoke(repo, null)!;
                await task.ConfigureAwait(false);
                var result = task.GetType().GetProperty(nameof(Task<object>.Result))!.GetValue(task);
                return ((IEnumerable)result!).Cast<TEntity>().ToList();
            }

            var repoLongInterface = typeof(IRepositoryLongKey<>).MakeGenericType(entityType);
            var repoLong = _serviceProvider.GetService(repoLongInterface);
            if (repoLong == null)
                throw new InvalidOperationException($"Repository for entity type {entityType.Name} is not registered.");

            var methodLong = repoLongInterface.GetMethod(nameof(IRepositoryLongKey<object>.GetAll))!;
            var taskLong = (Task)methodLong.Invoke(repoLong, null)!;
            await taskLong.ConfigureAwait(false);
            var resultLong = taskLong.GetType().GetProperty(nameof(Task<object>.Result))!.GetValue(taskLong);
            return ((IEnumerable)resultLong!).Cast<TEntity>().ToList();
        }
    }
}
