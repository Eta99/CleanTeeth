using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Exceptions;
using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.UniversalCrud.Queries.GetById
{
    public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, object?>
    {
        private readonly IServiceProvider _serviceProvider;

        public GetByIdQueryHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<object?> Handle(GetByIdQuery request)
        {
            if (request.EntityType == null)
                throw new ArgumentException("EntityType is required.", nameof(request));

            var repoInterface = typeof(IRepository<>).MakeGenericType(request.EntityType);
            var repo = _serviceProvider.GetService(repoInterface);

            if (repo != null)
            {
                var id = request.Id is Guid g ? g : Guid.Parse(request.Id.ToString()!);
                var method = repoInterface.GetMethod(nameof(IRepository<object>.GetById))!;
                var task = (Task)method.Invoke(repo, new object[] { id })!;
                await task.ConfigureAwait(false);
                return task.GetType().GetProperty(nameof(Task<object>.Result))!.GetValue(task);
            }

            var repoLongInterface = typeof(IRepositoryLongKey<>).MakeGenericType(request.EntityType);
            var repoLong = _serviceProvider.GetService(repoLongInterface);
            if (repoLong == null)
                throw new InvalidOperationException($"Repository for entity type {request.EntityType.Name} is not registered.");

            var idLong = request.Id is long l ? l : Convert.ToInt64(request.Id);
            var methodLong = repoLongInterface.GetMethod(nameof(IRepositoryLongKey<object>.GetById))!;
            var taskLong = (Task)methodLong.Invoke(repoLong, new object[] { idLong })!;
            await taskLong.ConfigureAwait(false);
            return taskLong.GetType().GetProperty(nameof(Task<object>.Result))!.GetValue(taskLong);
        }
    }
}
