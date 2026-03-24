using CleanTeeth.Application.Contracts.Persistence;
using CleanTeeth.Application.Contracts.Services;
using CleanTeeth.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CleanTeeth.Persistence.UnitsOfWork
{
    public class UnitOfWorkEFCore : IUnitOfWork
    {
        private readonly CleanTeethDbContext context;
        private readonly ICurrentUserContext currentUser;

        public UnitOfWorkEFCore(CleanTeethDbContext context, ICurrentUserContext currentUser)
        {
            this.context = context;
            this.currentUser = currentUser;
        }

        public async Task Commit()
        {
            ApplyAuditing();
            await context.SaveChangesAsync();
        }

        private void ApplyAuditing()
        {
            var utcNow = DateTime.UtcNow;
            var actor = currentUser.Login
                ?? (currentUser.UserId.HasValue ? currentUser.UserId.Value.ToString() : null);

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.Entity is not IAuditable auditable)
                    continue;

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditable.CreatedAt = utcNow;
                        auditable.ModifiedAt = utcNow;
                        auditable.CreatedBy = actor;
                        auditable.ModifiedBy = actor;
                        break;
                    case EntityState.Modified:
                        auditable.ModifiedAt = utcNow;
                        auditable.ModifiedBy = actor;
                        entry.Property(nameof(IAuditable.CreatedAt)).IsModified = false;
                        entry.Property(nameof(IAuditable.CreatedBy)).IsModified = false;
                        break;
                }
            }
        }

        public Task Rollback()
        {
            return Task.CompletedTask;
        }
    }
}
