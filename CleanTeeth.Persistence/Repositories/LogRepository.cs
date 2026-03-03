using CleanTeeth.Application.Contracts.Repositories;
using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Persistence.Repositories
{
    public class LogRepository : RepositoryLongKey<Log>, ILogRepository
    {
        public LogRepository(CleanTeethDbContext context)
            : base(context)
        {
        }
    }
}
