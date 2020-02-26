using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGA.Models;


namespace SGA.Repositories.Configuration
{
    public class LdapConfiguration : IEntityTypeConfiguration<Ldap>
    {
        public void Configure(EntityTypeBuilder<Ldap> builder)
        {
            builder.HasKey(t => t.Id);
        }
    }
}
