using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanTeeth.Persistence.Repositories
{
    public class AppActionRepository : RepositoryLongKey<AppAction>, IAppActionRepository
    {
        public AppActionRepository(CleanTeethDbContext context)
            : base(context)
        {
        }

        public Task<AppAction?> GetByNameAsync(string name, CancellationToken cancellationToken = default) =>
            context.Set<AppAction>().FirstOrDefaultAsync(a => a.Name == name, cancellationToken);
    }
}
