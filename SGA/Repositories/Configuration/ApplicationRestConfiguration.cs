using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SGA.Models;
using System;

namespace SGA.Repositories.Configuration
{
    public class ApplicationRestConfiguration : IEntityTypeConfiguration<ApplicationRest>
    {
        public void Configure(EntityTypeBuilder<ApplicationRest> builder)
        {
            builder
                .HasKey(t => t.Id);
            builder
                .Property(r => r.MD5)
                .HasConversion(new BoolToZeroOneConverter<Int16>());
        }
    }
}
