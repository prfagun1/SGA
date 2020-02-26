using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGA.Models;

namespace SGA.Repositories.Configuration
{
    public class ApplicationSQLConfiguration : IEntityTypeConfiguration<ApplicationSQL>
    {
        public void Configure(EntityTypeBuilder<ApplicationSQL> builder)
        {
            builder.HasKey(t => t.Id);
        }
    }
}
