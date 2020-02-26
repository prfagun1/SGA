using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SGA.Models;
using System;

namespace SGA.Repositories.Configuration
{
    public class UserHRConfiguration : IEntityTypeConfiguration<UserHR>
    {
        public void Configure(EntityTypeBuilder<UserHR> builder)
        {
            builder.HasKey(t => t.Id);
            builder
                .Property(r => r.MetadadosStatus)
                .HasConversion(new BoolToZeroOneConverter<Int16>());
        }
    }
}
