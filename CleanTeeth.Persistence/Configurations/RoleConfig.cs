using CleanTeeth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanTeeth.Persistence.Configurations
{
    internal class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).ValueGeneratedOnAdd();
            builder.Property(r => r.Title).HasMaxLength(256).IsRequired();
            builder.HasMany(r => r.Actions)
                .WithMany(a => a.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RoleAction",
                    j => j.HasOne<AppAction>().WithMany().HasForeignKey("ActionId"),
                    j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId"));
        }
    }
}
