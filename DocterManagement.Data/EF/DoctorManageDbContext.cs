﻿using DoctorManagement.Data.Configuration;
using DoctorManagement.Data.Entities;
using DoctorManagement.Data.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Data.EF
{
    public class DoctorManageDbContext : IdentityDbContext<AppUsers, AppRoles, Guid>
    {
        public DoctorManageDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AppRoleConfiguration());
            modelBuilder.ApplyConfiguration(new AppUserConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
            modelBuilder.ApplyConfiguration(new ClinidConfiguration());
            modelBuilder.ApplyConfiguration(new CommentsPostConfiguration());
            modelBuilder.ApplyConfiguration(new DistricConfiguration());
            modelBuilder.ApplyConfiguration(new DoctorConfiguration());
            modelBuilder.ApplyConfiguration(new ImageClinicConfiguration());
            modelBuilder.ApplyConfiguration(new ImagePostConfiguration());
            modelBuilder.ApplyConfiguration(new MedicalRecordConfiguration());
            modelBuilder.ApplyConfiguration(new PatientConfiguration());
            modelBuilder.ApplyConfiguration(new PostConfiguration());
            modelBuilder.ApplyConfiguration(new RateConfiguration());
            modelBuilder.ApplyConfiguration(new SchedulesConfiguration());
            modelBuilder.ApplyConfiguration(new SchedulesDetailConfiguration());
            modelBuilder.ApplyConfiguration(new WardConfiguration());

            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("AppUserClaims");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("AppUserRoles").HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("AppUserLogins").HasKey(x => x.UserId);
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("AppRoleClaims");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("AppUserTokens").HasKey(x => x.UserId);

            //Data seeding
            modelBuilder.Seed();


        }
        public DbSet<AppRoles> AppRoles { get; set; }
        public DbSet<AppUsers> AppUsers { get; set; }

        public DbSet<Appointments> Appointments { get; set; }
        public DbSet<Clinics> Clinics { get; set; }
        public DbSet<CommentsPost> CommentsPost { get; set; }
        public DbSet<Districs> Districs { get; set; }
        public DbSet<Doctors> Doctors { get; set; }
        public DbSet<ImageClinics> ImageClinics { get; set; }
        public DbSet<ImagePost> ImagePosts { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Patients> Patients { get; set; }
        public DbSet<Posts> Posts { get; set; }
        public DbSet<Rates> Rates { get; set; }
        public DbSet<Schedules> Schedules { get; set; }
        public DbSet<SchedulesDetails> SchedulesDetails { get; set; }
        public DbSet<Specialities> Specialities { get; set; }
        public DbSet<Wards> Wards { get; set; }
    }
}