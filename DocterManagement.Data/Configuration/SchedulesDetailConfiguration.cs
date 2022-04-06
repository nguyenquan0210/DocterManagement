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
    public class SchedulesDetailConfiguration : IEntityTypeConfiguration<SchedulesDetailts>
    {
        public void Configure(EntityTypeBuilder<SchedulesDetailts> builder)
        {
            builder.ToTable("SchedulesDetails");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Schedules).WithMany(x => x.SchedulesDetails).HasForeignKey(x => x.ScheduleId);

        }
    }
}
