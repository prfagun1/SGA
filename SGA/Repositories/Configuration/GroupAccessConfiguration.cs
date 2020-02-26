using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGA.Models;


namespace SGA.Repositories.Configuration
{
    public class GroupAccessConfiguration : IEntityTypeConfiguration<GroupAccess>
    {
        public void Configure(EntityTypeBuilder<GroupAccess> builder)
        {
            builder.HasKey(t => new { t.GroupDetailsId, t.Permission });

            
                

        }
    }
}
