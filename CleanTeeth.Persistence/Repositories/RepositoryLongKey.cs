using CleanTeeth.Application.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CleanTeeth.Persistence.Repositories
{
    public class RepositoryLongKey<T> : IRepositoryLongKey<T> where T : class
    {
        protected readonly CleanTeethDbContext context;

        public RepositoryLongKey(CleanTeethDbContext context)
        {
            this.context = context;
        }

        public virtual Task<T> Add(T entity)
        {
            context.Add(entity);
            return Task.FromResult(entity);
        }

        public virtual Task Delete(T entity)
        {
            context.Remove(entity);
            return Task.CompletedTask;
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await context.Set<T>().ToListAsync();
        }

        public virtual async Task<T?> GetById(long id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public virtual async Task<int> GetTotalAmountOfRecords()
        {
            return await context.Set<T>().CountAsync();
        }

        public virtual Task Update(T entity)
        {
            context.Update(entity);
            return Task.CompletedTask;
        }
    }
}
