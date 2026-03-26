using CleanTeeth.Application.Contracts.Persistence;
using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Exceptions;
using CleanTeeth.Application.Utilities;

namespace CleanTeeth.Application.Features.UniversalCrud.Commands.Delete
{
    public class DeleteCommandHandler : IRequestHandler<DeleteCommand>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCommandHandler(IServiceProvider serviceProvider, IUnitOfWork unitOfWork)
        {
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteCommand request)
        {
            if (request.EntityType == null)
                throw new ArgumentException("EntityType is required.", nameof(request));

            var repo = GetRepository(request.EntityType);
            object? entity = null;

            var repoType = repo.GetType();
            var repoInterface = typeof(IRepository<>).MakeGenericType(request.EntityType);
            var isGuidKey = GetRepositoryInterface(repoType, request.EntityType)?.GetGenericTypeDefinition() == typeof(IRepository<>);

            if (entity == null)
            {
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
                    var repoLongInterface = typeof(IRepositoryLongKey<>).MakeGenericType(request.EntityType);
                    var idLong = request.Id is long l ? l : Convert.ToInt64(request.Id);
                    var getById = repoLongInterface.GetMethod("GetById")!;
                    var task = (Task)getById.Invoke(repo, new object[] { idLong })!;
                    await task.ConfigureAwait(false);
                    entity = task.GetType().GetProperty("Result")!.GetValue(task);
                }
            }

            if (entity == null)
                throw new NotFoundException();

            var deleteMethod = GetRepositoryInterface(repoType, request.EntityType)?.GetMethod(nameof(IRepository<object>.Delete));
            if (deleteMethod == null)
                throw new InvalidOperationException($"Repository for {request.EntityType.Name} does not support Delete.");

            try
            {
                var deleteTask = (Task)deleteMethod.Invoke(repo, new[] { entity })!;
                await deleteTask.ConfigureAwait(false);
                await _unitOfWork.Commit().ConfigureAwait(false);
            }
            catch
            {
                await _unitOfWork.Rollback().ConfigureAwait(false);
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
