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
    public class NotificationConfigration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notification");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(100);
            builder.Property(x => x.Body).HasMaxLength(100);
            builder.Property(x => x.Type).HasMaxLength(100);
            builder.Property(x => x.Detail).HasMaxLength(100);
            builder.Property(x => x.ActionType).HasMaxLength(100);

        }
    }
}
