using DocterManagement.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocterManagement.Data.Configuration
{
    public class SpecialitiConfiguration : IEntityTypeConfiguration<Specialities>
    {
        public void Configure(EntityTypeBuilder<Specialities> builder)
        {
            builder.ToTable("Specialities");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Description).HasMaxLength(255);
        }
    }
}

