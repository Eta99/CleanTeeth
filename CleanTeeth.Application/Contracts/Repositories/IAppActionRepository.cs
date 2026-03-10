using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Application.Contracts.Repositories
{
    public interface IAppActionRepository : IRepositoryLongKey<AppAction>
    {
        Task<AppAction?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
