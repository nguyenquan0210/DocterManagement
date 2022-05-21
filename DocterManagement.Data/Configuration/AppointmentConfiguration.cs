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
            builder.Property(x => x.No).IsRequired().HasMaxLength(15);
            builder.Property(x => x.Note).HasMaxLength(int.MaxValue);

            builder.HasOne(x => x.SchedulesSlots).WithOne(x => x.Appointments).HasForeignKey<Appointments>(x => x.SchedulesSlotId).OnDelete(DeleteBehavior.ClientCascade);
            builder.HasOne(x => x.Patients).WithMany(x => x.Appointments).HasForeignKey(x => x.PatientId).OnDelete(DeleteBehavior.ClientCascade); ;
            builder.HasOne(x => x.Doctors).WithMany(x => x.Appointments).HasForeignKey(x => x.DoctorId).OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
