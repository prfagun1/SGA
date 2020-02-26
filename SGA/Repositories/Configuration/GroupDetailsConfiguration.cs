using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGA.Models;

namespace SGA.Repositories.Configuration
{
    public class GroupDetailsConfiguration : IEntityTypeConfiguration<GroupDetails>
    {
        public void Configure(EntityTypeBuilder<GroupDetails> builder)
        {
            builder.HasKey(t => t.Id);


        }
    }
}