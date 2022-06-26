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
    public class DistricConfiguration : IEntityTypeConfiguration<Districs>
    {
        public void Configure(EntityTypeBuilder<Districs> builder)
        {
            builder.ToTable("Districs");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
        }
    }
}

