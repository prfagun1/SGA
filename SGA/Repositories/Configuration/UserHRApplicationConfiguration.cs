using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGA.Models;

namespace SGA.Repositories.Configuration
{
    public class UserHRApplicationConfiguration : IEntityTypeConfiguration<UserHRApplication>
    {
        public void Configure(EntityTypeBuilder<UserHRApplication> builder)
        {
            builder.HasKey(t => new { t.ApplicationId, t.UserHRId});


        }
    }
}