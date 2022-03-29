using DocterManagement.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocterManagement.Data.Configuration
{
    public class SchedulesConfiguration : IEntityTypeConfiguration<Schedules>
    {
        public void Configure(EntityTypeBuilder<Schedules> builder)
        {
            builder.ToTable("Schedules");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Doctors).WithMany(x => x.Schedules).HasForeignKey(x => x.DoctorId);

        }
    }
}