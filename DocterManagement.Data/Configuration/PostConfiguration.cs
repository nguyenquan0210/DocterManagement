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
    public class PostConfiguration : IEntityTypeConfiguration<Posts>
    {
        public void Configure(EntityTypeBuilder<Posts> builder)
        {
            builder.ToTable("Posts");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Image).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Content).IsRequired().HasMaxLength(int.MaxValue);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(int.MaxValue);

            builder.HasOne(x => x.Doctors).WithMany(x => x.Posts).HasForeignKey(x => x.DoctorId);
            builder.HasOne(x => x.MainMenus).WithMany(x => x.Posts).HasForeignKey(x => x.TopicId);

        }
    }
}
