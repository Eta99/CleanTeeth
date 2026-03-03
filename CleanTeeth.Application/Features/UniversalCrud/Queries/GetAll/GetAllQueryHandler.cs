using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Utilities;
using System.Collections;

namespace CleanTeeth.Application.Features.UniversalCrud.Queries.GetAll
{
    public class GetAllQueryHandler : IRequestHandler<GetAllQuery, IEnumerable<object>>
    {
        private readonly IServiceProvider _serviceProvider;

        public GetAllQueryHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<IEnumerable<object>> Handle(GetAllQuery request)
        {
            if (request.EntityType == null)
                throw new ArgumentException("EntityType is required.", nameof(request));

            var repoInterface = typeof(IRepository<>).MakeGenericType(request.EntityType);
            var repo = _serviceProvider.GetService(repoInterface);

            if (repo != null)
            {
                var method = repoInterface.GetMethod(nameof(IRepository<object>.GetAll))!;
                var task = (Task)method.Invoke(repo, null)!;
                await task.ConfigureAwait(false);
                var result = task.GetType().GetProperty(nameof(Task<object>.Result))!.GetValue(task);
                return ((IEnumerable)result!).Cast<object>().ToList();
            }

            var repoLongInterface = typeof(IRepositoryLongKey<>).MakeGenericType(request.EntityType);
            var repoLong = _serviceProvider.GetService(repoLongInterface);
            if (repoLong == null)
                throw new InvalidOperationException($"Repository for entity type {request.EntityType.Name} is not registered.");

            var methodLong = repoLongInterface.GetMethod(nameof(IRepositoryLongKey<object>.GetAll))!;
            var taskLong = (Task)methodLong.Invoke(repoLong, null)!;
            await taskLong.ConfigureAwait(false);
            var resultLong = taskLong.GetType().GetProperty(nameof(Task<object>.Result))!.GetValue(taskLong);
            return ((IEnumerable)resultLong!).Cast<object>().ToList();
        }
    }
}
