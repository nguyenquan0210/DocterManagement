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
    public class AttachedfileConfiguration : IEntityTypeConfiguration<Attachedfiles>
    {
        public void Configure(EntityTypeBuilder<Attachedfiles> builder)
        {
            builder.ToTable("Attachedfiles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Img).IsRequired().HasMaxLength(100);

            builder.HasOne(x => x.Appointments).WithMany(x => x.Attachedfiles).HasForeignKey(x => x.AppointmentId);

        }
    }
}
