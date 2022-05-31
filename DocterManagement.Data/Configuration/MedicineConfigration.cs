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
    public class MedicineConfigration : IEntityTypeConfiguration<Medicines>
    {
        public void Configure(EntityTypeBuilder<Medicines> builder)
        {
            builder.ToTable("Medicines");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Description).HasMaxLength(150);
            builder.Property(x => x.Image).IsRequired().HasMaxLength(50);
        }
    }
}
