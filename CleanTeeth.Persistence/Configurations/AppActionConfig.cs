using CleanTeeth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanTeeth.Persistence.Configurations
{
    internal class AppActionConfig : IEntityTypeConfiguration<AppAction>
    {
        public void Configure(EntityTypeBuilder<AppAction> builder)
        {
            builder.ToTable("Actions");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.TypeId).IsRequired();
            builder.Property(a => a.Name).HasMaxLength(256).IsRequired();
            builder.Property(a => a.Title).HasMaxLength(256).IsRequired();
            builder.Property(a => a.IsLoggable).IsRequired().HasDefaultValue(true);
            builder.HasOne(a => a.Type)
                .WithMany(t => t.Actions)
                .HasForeignKey(a => a.TypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
