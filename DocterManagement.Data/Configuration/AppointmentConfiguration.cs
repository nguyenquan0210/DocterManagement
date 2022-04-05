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
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointments>
    {
        public void Configure(EntityTypeBuilder<Appointments> builder)
        {
            builder.ToTable("Appointments");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.No).IsRequired().HasMaxLength(10);

            builder.HasOne(x => x.SchedulesDetails).WithOne(x => x.Appointments).HasForeignKey<Appointments>(x => x.SchedulesDetailId).OnDelete(DeleteBehavior.ClientCascade);
            builder.HasOne(x => x.Patients).WithMany(x => x.Appointments).HasForeignKey(x => x.PatientId);
        }
    }
}
