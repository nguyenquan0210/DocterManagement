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
    public class ImagePostConfiguration : IEntityTypeConfiguration<ImagePost>
    {
        public void Configure(EntityTypeBuilder<ImagePost> builder)
        {
            builder.ToTable("ImagePost");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Img).IsRequired().HasMaxLength(100);


        }
    }
}
