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
    public class ServicesSpecialityConfiguration : IEntityTypeConfiguration<ServicesSpecialities>
    {
        public void Configure(EntityTypeBuilder<ServicesSpecialities> builder)
        {
            builder.ToTable("ServicesSpecialities");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Doctors).WithMany(x => x.ServicesSpecialities).HasForeignKey(x => x.DoctorId);
            builder.HasOne(x => x.Specialities).WithMany(x => x.ServicesSpecialities).HasForeignKey(x => x.SpecialityId);
        }
    }
}
