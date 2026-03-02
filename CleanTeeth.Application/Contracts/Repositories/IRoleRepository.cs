using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Application.Contracts.Repositories
{
    public interface IRoleRepository : IRepositoryLongKey<Role>
    {
        Task<Role?> GetByIdWithActions(long id);
    }
}
