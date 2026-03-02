using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanTeeth.Persistence.Repositories
{
    public class RoleRepository : RepositoryLongKey<Role>, IRoleRepository
    {
        public RoleRepository(CleanTeethDbContext context)
            : base(context)
        {
        }

        public async Task<Role?> GetByIdWithActions(long id)
        {
            return await context.Roles
                .Include(r => r.Actions)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
