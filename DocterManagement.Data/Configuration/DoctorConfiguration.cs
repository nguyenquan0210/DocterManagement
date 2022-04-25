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
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctors>
    {
        public void Configure(EntityTypeBuilder<Doctors> builder)
        {
            builder.ToTable("Doctors");

            builder.HasKey(x => x.UserId);
            builder.Property(x => x.Address).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(int.MaxValue);
            builder.Property(x => x.No).IsRequired().HasMaxLength(10);
            builder.Property(x => x.Img).IsRequired().HasMaxLength(100);

            builder.HasOne(x => x.AppUsers).WithOne(x => x.Doctors).HasForeignKey<Doctors>(x => x.UserId);
            builder.HasOne(x => x.Specialities).WithMany(x => x.Doctors).HasForeignKey(x => x.SpecialityId);
            builder.HasOne(x => x.Clinics).WithMany(x => x.Doctors).HasForeignKey(x => x.ClinicId);
            builder.HasOne(x => x.Clinics).WithMany(x => x.Doctors).HasForeignKey(x => x.ClinicId);

        }
    }
}
