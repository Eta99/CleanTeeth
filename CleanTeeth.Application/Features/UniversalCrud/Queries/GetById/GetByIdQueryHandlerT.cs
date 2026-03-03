using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Utilities;
using System.Collections;

namespace CleanTeeth.Application.Features.UniversalCrud.Queries.GetById
{
    public class GetByIdQueryHandler<TEntity> : IRequestHandler<GetByIdQuery<TEntity>, TEntity?> where TEntity : class
    {
        private readonly IServiceProvider _serviceProvider;

        public GetByIdQueryHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TEntity?> Handle(GetByIdQuery<TEntity> request)
        {
            var entityType = typeof(TEntity);
            var repoInterface = typeof(IRepository<>).MakeGenericType(entityType);
            var repo = _serviceProvider.GetService(repoInterface);

            if (repo != null)
            {
                var id = request.Id is Guid g ? g : Guid.Parse(request.Id.ToString()!);
                var method = repoInterface.GetMethod(nameof(IRepository<object>.GetById))!;
                var task = (Task)method.Invoke(repo, new object[] { id })!;
                await task.ConfigureAwait(false);
                return (TEntity?)task.GetType().GetProperty(nameof(Task<object>.Result))!.GetValue(task);
            }

            var repoLongInterface = typeof(IRepositoryLongKey<>).MakeGenericType(entityType);
            var repoLong = _serviceProvider.GetService(repoLongInterface);
            if (repoLong == null)
                throw new InvalidOperationException($"Repository for entity type {entityType.Name} is not registered.");

            var idLong = request.Id is long l ? l : Convert.ToInt64(request.Id);
            var methodLong = repoLongInterface.GetMethod(nameof(IRepositoryLongKey<object>.GetById))!;
            var taskLong = (Task)methodLong.Invoke(repoLong, new object[] { idLong })!;
            await taskLong.ConfigureAwait(false);
            return (TEntity?)taskLong.GetType().GetProperty(nameof(Task<object>.Result))!.GetValue(taskLong);
        }
    }
}
