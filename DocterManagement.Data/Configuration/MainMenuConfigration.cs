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
    public class MainMenuConfigration : IEntityTypeConfiguration<MainMenus>
    {
        public void Configure(EntityTypeBuilder<MainMenus> builder)
        {
            builder.ToTable("MainMenus");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Type).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Image).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Action).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Controller).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Description).HasMaxLength(50);
            builder.Property(x => x.Title).HasMaxLength(50);
        }
    }
}
