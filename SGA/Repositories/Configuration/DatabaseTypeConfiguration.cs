using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGA.Models;

namespace SGA.Repositories.Configuration
{
    public class DatabaseTypeConfiguration : IEntityTypeConfiguration<DatabaseType>
    {
        public void Configure(EntityTypeBuilder<DatabaseType> builder)
        {
            builder.HasKey(t => t.Id);
        }
    }
}