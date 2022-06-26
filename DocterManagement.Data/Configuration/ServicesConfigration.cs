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
    public class ServicesConfigration : IEntityTypeConfiguration<Services>
    {
        public void Configure(EntityTypeBuilder<Services> builder)
        {
            builder.ToTable("Services");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.ServiceName).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Unit).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Description).HasMaxLength(150);
            builder.HasOne(x => x.Doctor).WithMany(x => x.Services).HasForeignKey(x => x.DoctorId);
        }
    }
}
