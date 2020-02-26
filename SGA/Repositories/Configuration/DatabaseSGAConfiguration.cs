using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGA.Models;

namespace SGA.Repositories.Configuration
{
    public class DatabaseSGAConfiguration : IEntityTypeConfiguration<DatabaseSGA>
    {
        public void Configure(EntityTypeBuilder<DatabaseSGA> builder)
        {
            builder.HasKey(t => t.Id);
        }
    }
}
