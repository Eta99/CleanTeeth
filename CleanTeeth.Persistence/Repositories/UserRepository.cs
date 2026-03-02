using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanTeeth.Persistence.Repositories
{
    public class UserRepository : RepositoryLongKey<User>, IUserRepository
    {
        public UserRepository(CleanTeethDbContext context)
            : base(context)
        {
        }

        public async Task<User?> GetByIdWithRolesAndActions(long id)
        {
            return await context.Users
                .Include(u => u.Roles)
                .ThenInclude(r => r.Actions)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
