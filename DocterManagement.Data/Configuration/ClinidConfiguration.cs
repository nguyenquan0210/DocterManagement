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
    public class ClinidConfiguration : IEntityTypeConfiguration<Clinics>
    {
        public void Configure(EntityTypeBuilder<Clinics> builder)
        {
            builder.ToTable("Clinics");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
            builder.Property(x => x.No).IsRequired().HasMaxLength(10);
            builder.Property(x => x.Address).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(int.MaxValue);
            builder.Property(x => x.ImgLogo).IsRequired().HasMaxLength(100);

            builder.HasOne(x => x.Locations).WithMany(x => x.Clinics).HasForeignKey(x => x.LocationId);
        }
    }
}
