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
    public class HistoryActiveDetailtConfigration : IEntityTypeConfiguration<HistoryActiveDetailts>
    {
        public void Configure(EntityTypeBuilder<HistoryActiveDetailts> builder)
        {
            builder.ToTable("HistoryActiveDetailts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.MethodName).HasMaxLength(255);
            builder.Property(x => x.ServiceName).HasMaxLength(255);
            builder.Property(x => x.Parameters).HasMaxLength(int.MaxValue);
            builder.Property(x => x.ExtraProperties).HasMaxLength(255);

            builder.HasOne(x => x.HistoryActives).WithMany(x => x.HistoryActiveDetailts).HasForeignKey(x => x.HistoryActiveId);

        }
    }
}
