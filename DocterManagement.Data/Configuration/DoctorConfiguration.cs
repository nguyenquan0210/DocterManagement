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
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Address).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Intro).IsRequired().HasMaxLength(int.MaxValue);
            builder.Property(x => x.Note).HasMaxLength(int.MaxValue);
            builder.Property(x => x.No).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Img).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Educations).HasMaxLength(int.MaxValue);
            builder.Property(x => x.Prizes).HasMaxLength(int.MaxValue);
            builder.Property(x => x.TimeWorking).HasMaxLength(int.MaxValue);
            builder.Property(x => x.Experiences).HasMaxLength(int.MaxValue);
            builder.Property(x => x.Slug).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Prefix).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Services).HasMaxLength(100);
            builder.Property(x => x.MapUrl).IsRequired().HasMaxLength(int.MaxValue);

            builder.HasOne(x => x.AppUsers).WithOne(x => x.Doctors).HasForeignKey<Doctors>(x => x.UserId);
            builder.HasOne(x => x.Locations).WithMany(x => x.Doctors).HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.ClientCascade);
            builder.HasOne(x => x.Clinics).WithMany(x => x.Doctors).HasForeignKey(x => x.ClinicId);

        }
    }
}
