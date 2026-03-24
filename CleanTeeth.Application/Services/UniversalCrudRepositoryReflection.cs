using CleanTeeth.Application.Contracts.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CleanTeeth.Application.Services
{
    internal static class UniversalCrudRepositoryReflection
    {
        public static object GetRepository(IServiceProvider serviceProvider, Type entityType)
        {
            var repo = serviceProvider.GetService(typeof(IRepository<>).MakeGenericType(entityType));
            if (repo != null)
                return repo;

            repo = serviceProvider.GetService(typeof(IRepositoryLongKey<>).MakeGenericType(entityType))
                ?? throw new InvalidOperationException($"Repository for entity type {entityType.Name} is not registered.");
            return repo;
        }

        public static Type? GetRepositoryInterface(Type repoType, Type entityType)
        {
            var repoOpen = typeof(IRepository<>);
            var repoLongOpen = typeof(IRepositoryLongKey<>);
            foreach (var i in repoType.GetInterfaces())
            {
                if (i.IsGenericType &&
                    (i.GetGenericTypeDefinition() == repoOpen || i.GetGenericTypeDefinition() == repoLongOpen))
                    return i;
            }

            return null;
        }

        public static async Task<object?> GetEntityByIdAsync(IServiceProvider sp, Type entityType, object id,
            CancellationToken cancellationToken = default)
        {
            var repo = GetRepository(sp, entityType);
            var repoType = repo.GetType();
            var repoIface = GetRepositoryInterface(repoType, entityType);
            if (repoIface == null)
                return null;

            var isGuidKey = repoIface.GetGenericTypeDefinition() == typeof(IRepository<>);
            if (isGuidKey)
            {
                var gid = id is Guid g ? g : Guid.Parse(id.ToString()!);
                var repoInterface = typeof(IRepository<>).MakeGenericType(entityType);
                var getById = repoInterface.GetMethod("GetById")!;
                var task = (Task)getById.Invoke(repo, new object[] { gid })!;
                await task.ConfigureAwait(false);
                return task.GetType().GetProperty("Result")!.GetValue(task);
            }

            var lid = id is long l ? l : Convert.ToInt64(id);
            var repoLongInterface = typeof(IRepositoryLongKey<>).MakeGenericType(entityType);
            var getByLong = repoLongInterface.GetMethod("GetById")!;
            var taskLong = (Task)getByLong.Invoke(repo, new object[] { lid })!;
            await taskLong.ConfigureAwait(false);
            return taskLong.GetType().GetProperty("Result")!.GetValue(taskLong);
        }
    }
}
