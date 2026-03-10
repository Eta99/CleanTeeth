using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Application.Contracts.Persistence;
using CleanTeeth.Application.Features.UniversalCrud;
using CleanTeeth.Application.Utilities;
using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Application.Features.UniversalCrud.Commands.Create
{
    public class CreateCommandHandler : IRequestHandler<CreateCommand, object>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepositoryLongKey<Log> _logRepository;
        private readonly IAppActionRepository _actionRepository;

        public CreateCommandHandler(IServiceProvider serviceProvider, IUnitOfWork unitOfWork, IRepositoryLongKey<Log> logRepository, IAppActionRepository actionRepository)
        {
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork;
            _logRepository = logRepository;
            _actionRepository = actionRepository;
        }

        public async Task<object> Handle(CreateCommand request)
        {
            if (request.EntityType == null)
                throw new ArgumentException("EntityType is required.", nameof(request));
            if (request.Entity == null)
                throw new ArgumentException("Entity is required.", nameof(request));

            var repo = GetRepository(request.EntityType);
            var repoType = repo.GetType();
            var addMethod = GetRepositoryInterface(repoType, request.EntityType)?.GetMethod(nameof(IRepository<object>.Add));
            if (addMethod == null)
                throw new InvalidOperationException($"Repository for {request.EntityType.Name} does not support Add.");

            try
            {
                var task = (Task)addMethod.Invoke(repo, new[] { request.Entity })!;
                await task.ConfigureAwait(false);
                var result = task.GetType().GetProperty(nameof(Task<object>.Result))!.GetValue(task);
                if (result != null)
                {
                    var action = await _actionRepository.GetByNameAsync(request.RequiredActionName);
                    if (action?.IsLoggable == true)
                    {
                        var idObject = LogHelper.GetEntityId(result);
                        var newValueJson = LogHelper.ToLogJson(result);
                        await _logRepository.Add(new Log(idObject, oldValue: null, newValue: newValueJson));
                    }
                }
                await _unitOfWork.Commit();
                return result!;
            }
            catch
            {
                await _unitOfWork.Rollback();
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
