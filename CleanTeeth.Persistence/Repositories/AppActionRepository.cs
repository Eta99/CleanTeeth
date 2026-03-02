using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Persistence.Repositories
{
    public class AppActionRepository : RepositoryLongKey<AppAction>, IAppActionRepository
    {
        public AppActionRepository(CleanTeethDbContext context)
            : base(context)
        {
        }
    }
}
