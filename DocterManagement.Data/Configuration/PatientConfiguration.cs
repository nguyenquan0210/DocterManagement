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
    public class PatientConfiguration : IEntityTypeConfiguration<Patients>
    {
        public void Configure(EntityTypeBuilder<Patients> builder)
        {
            builder.ToTable("Patients");

            builder.HasKey(x => x.PatientId);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Address).HasMaxLength(150);
            builder.Property(x => x.RelativeEmail).HasMaxLength(150);
            builder.Property(x => x.Img).HasMaxLength(100);
            builder.Property(x => x.No).IsRequired().HasMaxLength(50);
            builder.Property(x => x.RelativeName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.RelativePhone).IsRequired().HasMaxLength(11);
            builder.Property(x => x.Identitycard).IsRequired().HasMaxLength(50);

            builder.HasOne(x => x.AppUsers).WithMany(x => x.Patients).HasForeignKey(x => x.UserId);
            builder.HasOne(x => x.Ethnics).WithMany(x => x.Patients).HasForeignKey(x => x.EthnicId);
            builder.HasOne(x => x.Locations).WithMany(x => x.Patients).HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.ClientCascade); ;

        }
    }
}
