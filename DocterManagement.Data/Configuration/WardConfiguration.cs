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
    public class WardConfiguration : IEntityTypeConfiguration<Wards>
    {
        public void Configure(EntityTypeBuilder<Wards> builder)
        {
            builder.ToTable("Wards");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);

            builder.HasOne(x => x.Districs).WithMany(x => x.Wards).HasForeignKey(x => x.DisticId);

        }
    }
}
