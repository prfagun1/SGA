using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SGA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Repositories.Configuration
{

    public class UserCreateEmployeeConfiguration : IEntityTypeConfiguration<UserCreateEmployee>
    {
        public void Configure(EntityTypeBuilder<UserCreateEmployee> builder)
        {
            builder.HasKey(t => t.Id);
            builder
                .Property(r => r.MetadadosStatus)
                .HasConversion(new BoolToZeroOneConverter<Int16>());
        }
    }

}
