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
    public class MedicineDetailtConfigration : IEntityTypeConfiguration<MedicineDetailts>
    {
        public void Configure(EntityTypeBuilder<MedicineDetailts> builder)
        {
            builder.ToTable("MedicineDetailts");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Use).HasMaxLength(150);

            builder.HasOne(x => x.Medicine).WithMany(x => x.MedicineDetailts).HasForeignKey(x => x.MedicineId);
            builder.HasOne(x => x.MedicalRecord).WithMany(x => x.MedicineDetailts).HasForeignKey(x => x.MedicalRecordId);
        }
    }
}
