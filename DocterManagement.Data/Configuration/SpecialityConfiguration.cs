using DoctorManagement.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.Configuration
{
    public class SpecialityConfiguration : IEntityTypeConfiguration<Specialities>
    {
        public void Configure(EntityTypeBuilder<Specialities> builder)
        {
            builder.ToTable("Specialities");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Img).IsRequired().HasMaxLength(255);
            builder.Property(x => x.No).IsRequired().HasMaxLength(10);
            builder.Property(x => x.Description).HasMaxLength(255);
        }
    }
}

