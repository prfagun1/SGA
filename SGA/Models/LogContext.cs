using Microsoft.EntityFrameworkCore;
using SGA.Repositories.Configuration;

namespace SGA.Models
{
    public class LogContext : DbContext
    {
        public LogContext(DbContextOptions<LogContext> options) : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LogConfiguration());
        }

    }

}
