using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Application.Contracts.Repositories
{
    public interface IUserRepository : IRepositoryLongKey<User>
    {
        Task<User?> GetByIdWithRolesAndActions(long id);
    }
}
