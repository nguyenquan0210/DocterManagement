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
    public class AnnualServiceFeeConfigration : IEntityTypeConfiguration<AnnualServiceFees>
    {
        public void Configure(EntityTypeBuilder<AnnualServiceFees> builder)
        {
            builder.ToTable("AnnualServiceFees");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Image).HasMaxLength(100);
            builder.Property(x => x.AccountBank).HasMaxLength(100);
            builder.Property(x => x.BankName).HasMaxLength(100);
            builder.Property(x => x.Type).IsRequired().HasMaxLength(100);
            builder.Property(x => x.No).IsRequired().HasMaxLength(100);
            builder.Property(x => x.TuitionPaidFreeText).HasMaxLength(100);
            builder.Property(x => x.Note).HasMaxLength(100);
            builder.Property(x => x.TransactionCode).HasMaxLength(100);
            builder.Property(x => x.CancelReason).HasMaxLength(100);

            builder.HasOne(x => x.Doctors).WithMany(x => x.AnnualServiceFees).HasForeignKey(x => x.DoctorId);

        }
    }
}
