using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGA.Models;

namespace SGA.Repositories.Configuration
{
    public class CCConfiguration : IEntityTypeConfiguration<CC>
    {
        public void Configure(EntityTypeBuilder<CC> builder)
        {
            builder.HasKey(t => new { t.Id, t.Username });
        }
    }
}
