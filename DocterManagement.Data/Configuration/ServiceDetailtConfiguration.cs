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
    public class ServiceDetailtConfiguration : IEntityTypeConfiguration<ServiceDetailts>
    {
        public void Configure(EntityTypeBuilder<ServiceDetailts> builder)
        {
            builder.ToTable("ServiceDetailts");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Services).WithMany(x => x.ServiceDetailts).HasForeignKey(x => x.ServicesId);
            builder.HasOne(x => x.MedicalRecord).WithMany(x => x.ServiceDetailts).HasForeignKey(x => x.MedicalRecordId);
        }
    }
}

