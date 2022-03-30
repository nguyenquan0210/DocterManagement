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
    public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.ToTable("MedicalRecords");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Diagnose).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Prescription).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Note).IsRequired().HasMaxLength(255);

            builder.HasOne(x => x.Patients).WithMany(x => x.MedicalRecords).HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.ClientCascade);
            builder.HasOne(x => x.Doctors).WithMany(x => x.MedicalRecords).HasForeignKey(x => x.DoctorId).OnDelete(DeleteBehavior.ClientCascade);
            builder.HasOne(x => x.Appointments).WithOne(x => x.MedicalRecords).HasForeignKey<MedicalRecord>(x => x.AppointmentId);

        }
    }
}
