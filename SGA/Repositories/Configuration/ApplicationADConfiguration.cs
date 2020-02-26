using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGA.Models;

namespace SGA.Repositories.Configuration
{
    public class ApplicationADConfiguration : IEntityTypeConfiguration<ApplicationAD>
    {
        public void Configure(EntityTypeBuilder<ApplicationAD> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Application);
        }
    }
}
