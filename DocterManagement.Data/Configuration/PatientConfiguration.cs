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

            builder.HasKey(x => x.UserId);
            builder.Property(x => x.Address).HasMaxLength(150);
            builder.Property(x => x.Img).HasMaxLength(100);
            builder.Property(x => x.No).IsRequired().HasMaxLength(10);

            builder.HasOne(x => x.AppUsers).WithOne(x => x.Patients).HasForeignKey<Patients>(x => x.UserId);

        }
    }
}
