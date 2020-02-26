using Microsoft.EntityFrameworkCore;
using SGA.Repositories.Configuration;

namespace SGA.Models
{
    public class SGAContext : DbContext
    {
        public SGAContext(DbContextOptions<SGAContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.ApplyConfiguration(new ApplicationConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationADConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationRestConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationSQLConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DatabaseSGAConfiguration());
            modelBuilder.ApplyConfiguration(new DatabaseTypeConfiguration());
            modelBuilder.ApplyConfiguration(new EnvironmentConfiguration());
            modelBuilder.ApplyConfiguration(new GroupAccessConfiguration());
            modelBuilder.ApplyConfiguration(new GroupDetailsConfiguration());
            modelBuilder.ApplyConfiguration(new LdapConfiguration());
            modelBuilder.ApplyConfiguration(new ParameterConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionGroupConfiguration());
            modelBuilder.ApplyConfiguration(new UserAccessConfiguration());
            modelBuilder.ApplyConfiguration(new UserDetailsConfiguration());
            modelBuilder.ApplyConfiguration(new UserHRConfiguration());
            modelBuilder.ApplyConfiguration(new ScheduleConfiguration());
            modelBuilder.ApplyConfiguration(new LogConfiguration());
            modelBuilder.ApplyConfiguration(new UserHRApplicationConfiguration());
            modelBuilder.ApplyConfiguration(new CCConfiguration());
            modelBuilder.ApplyConfiguration(new UserCreateEmployeeConfiguration());

        }

    }

}

/*
https://stackoverflow.com/questions/48368634/how-should-i-inject-a-dbcontext-instance-into-an-ihostedservice/48368934 
https://docs.microsoft.com/en-us/ef/core/miscellaneous/configuring-dbcontext
*/
