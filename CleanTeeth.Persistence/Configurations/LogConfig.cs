using CleanTeeth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanTeeth.Persistence.Configurations
{
    internal class LogConfig : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.ToTable("Logs");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id).ValueGeneratedOnAdd();
            builder.Property(l => l.IdObject).HasMaxLength(64).IsRequired();
            builder.Property(l => l.OldValue).HasMaxLength(2000);
            builder.Property(l => l.NewValue).HasMaxLength(2000);
        }
    }
}
