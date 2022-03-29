﻿// <auto-generated />
using System;
using DocterManagement.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DocterManagement.Data.Migrations
{
    [DbContext(typeof(DoctorManageDbContext))]
    partial class DoctorManageDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("DocterManagement.Data.Entities.Appointments", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("PatientId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SchedulesDetailId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PatientId");

                    b.HasIndex("SchedulesDetailId")
                        .IsUnique();

                    b.ToTable("Appointments", (string)null);
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.AppRoles", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AppRoles", (string)null);
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.AppUsers", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Dob")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("Roles")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AppUsers", (string)null);
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Clinics", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2147483647)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImgLogo")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<Guid>("WardId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("WardId");

                    b.ToTable("Clinics", (string)null);
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.CommentsPost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CheckComentId")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("CheckLevel")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("CommentsPost", (string)null);
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Districs", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("SortOrder")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Districs", (string)null);
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Doctors", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<Guid>("ClinicId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2147483647)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Img")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("SpecialitiId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId");

                    b.HasIndex("ClinicId");

                    b.HasIndex("SpecialitiId");

                    b.ToTable("Doctors", (string)null);
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.ImageClinics", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ClinicId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Img")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("SortOrder")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ClinicId");

                    b.ToTable("ImageClinics", (string)null);
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.ImagePost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Img")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("SortOrder")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("ImagePost", (string)null);
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.MedicalRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AppointmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Diagnose")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<Guid>("DoctorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("PatientId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Prescription")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("StatusIllness")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AppointmentId")
                        .IsUnique();

                    b.HasIndex("DoctorId");

                    b.HasIndex("PatientId");

                    b.ToTable("MedicalRecords", (string)null);
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Patients", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Img")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("UserId");

                    b.ToTable("Patients", (string)null);
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Posts", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("DoctorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("Id");

                    b.HasIndex("DoctorId");

                    b.ToTable("Posts", (string)null);
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Rates", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AppointmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("AppointmentId")
                        .IsUnique();

                    b.ToTable("Rates", (string)null);
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Schedules", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CheckInDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("DoctorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<TimeSpan>("FromTime")
                        .HasColumnType("time");

                    b.Property<int>("Qty")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("ToTime")
                        .HasColumnType("time");

                    b.HasKey("Id");

                    b.HasIndex("DoctorId");

                    b.ToTable("Schedules", (string)null);
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.SchedulesDetails", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<TimeSpan>("FromTime")
                        .HasColumnType("time");

                    b.Property<Guid>("ScheduleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("ToTime")
                        .HasColumnType("time");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleId");

                    b.ToTable("SchedulesDetails", (string)null);
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Specialities", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SortOrder")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Specialities");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Wards", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DisticId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<int>("SortOrder")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DisticId");

                    b.ToTable("Wards", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("AppRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("AppUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("AppUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.ToTable("AppUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("AppUserTokens", (string)null);
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Appointments", b =>
                {
                    b.HasOne("DocterManagement.Data.Entities.Patients", "Patients")
                        .WithMany("Appointments")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DocterManagement.Data.Entities.SchedulesDetails", "SchedulesDetails")
                        .WithOne("Appointments")
                        .HasForeignKey("DocterManagement.Data.Entities.Appointments", "SchedulesDetailId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Patients");

                    b.Navigation("SchedulesDetails");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.AppUsers", b =>
                {
                    b.HasOne("DocterManagement.Data.Entities.AppRoles", "AppRoles")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppRoles");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Clinics", b =>
                {
                    b.HasOne("DocterManagement.Data.Entities.Wards", "Wards")
                        .WithMany("Clinics")
                        .HasForeignKey("WardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Wards");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.CommentsPost", b =>
                {
                    b.HasOne("DocterManagement.Data.Entities.Posts", "Posts")
                        .WithMany("CommentsPosts")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("DocterManagement.Data.Entities.AppUsers", "AppUsers")
                        .WithMany("CommentsPosts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppUsers");

                    b.Navigation("Posts");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Doctors", b =>
                {
                    b.HasOne("DocterManagement.Data.Entities.Clinics", "Clinics")
                        .WithMany("Doctors")
                        .HasForeignKey("ClinicId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DocterManagement.Data.Entities.Specialities", "Specialities")
                        .WithMany("Doctors")
                        .HasForeignKey("SpecialitiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DocterManagement.Data.Entities.AppUsers", "AppUsers")
                        .WithOne("Doctors")
                        .HasForeignKey("DocterManagement.Data.Entities.Doctors", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppUsers");

                    b.Navigation("Clinics");

                    b.Navigation("Specialities");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.ImageClinics", b =>
                {
                    b.HasOne("DocterManagement.Data.Entities.Clinics", "Clinics")
                        .WithMany("ImageClinics")
                        .HasForeignKey("ClinicId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Clinics");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.ImagePost", b =>
                {
                    b.HasOne("DocterManagement.Data.Entities.Posts", "Posts")
                        .WithMany("ImagePosts")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Posts");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.MedicalRecord", b =>
                {
                    b.HasOne("DocterManagement.Data.Entities.Appointments", "Appointments")
                        .WithOne("MedicalRecords")
                        .HasForeignKey("DocterManagement.Data.Entities.MedicalRecord", "AppointmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DocterManagement.Data.Entities.Doctors", "Doctors")
                        .WithMany("MedicalRecords")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("DocterManagement.Data.Entities.Patients", "Patients")
                        .WithMany("MedicalRecords")
                        .HasForeignKey("PatientId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Appointments");

                    b.Navigation("Doctors");

                    b.Navigation("Patients");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Patients", b =>
                {
                    b.HasOne("DocterManagement.Data.Entities.AppUsers", "AppUsers")
                        .WithOne("Patients")
                        .HasForeignKey("DocterManagement.Data.Entities.Patients", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppUsers");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Posts", b =>
                {
                    b.HasOne("DocterManagement.Data.Entities.Doctors", "Doctors")
                        .WithMany("Posts")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Doctors");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Rates", b =>
                {
                    b.HasOne("DocterManagement.Data.Entities.Appointments", "Appointments")
                        .WithOne("Rates")
                        .HasForeignKey("DocterManagement.Data.Entities.Rates", "AppointmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Appointments");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Schedules", b =>
                {
                    b.HasOne("DocterManagement.Data.Entities.Doctors", "Doctors")
                        .WithMany("Schedules")
                        .HasForeignKey("DoctorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Doctors");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.SchedulesDetails", b =>
                {
                    b.HasOne("DocterManagement.Data.Entities.Schedules", "Schedules")
                        .WithMany("SchedulesDetails")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Wards", b =>
                {
                    b.HasOne("DocterManagement.Data.Entities.Districs", "Districs")
                        .WithMany("Wards")
                        .HasForeignKey("DisticId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Districs");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Appointments", b =>
                {
                    b.Navigation("MedicalRecords");

                    b.Navigation("Rates");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.AppRoles", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.AppUsers", b =>
                {
                    b.Navigation("CommentsPosts");

                    b.Navigation("Doctors");

                    b.Navigation("Patients");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Clinics", b =>
                {
                    b.Navigation("Doctors");

                    b.Navigation("ImageClinics");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Districs", b =>
                {
                    b.Navigation("Wards");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Doctors", b =>
                {
                    b.Navigation("MedicalRecords");

                    b.Navigation("Posts");

                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Patients", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("MedicalRecords");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Posts", b =>
                {
                    b.Navigation("CommentsPosts");

                    b.Navigation("ImagePosts");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Schedules", b =>
                {
                    b.Navigation("SchedulesDetails");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.SchedulesDetails", b =>
                {
                    b.Navigation("Appointments");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Specialities", b =>
                {
                    b.Navigation("Doctors");
                });

            modelBuilder.Entity("DocterManagement.Data.Entities.Wards", b =>
                {
                    b.Navigation("Clinics");
                });
#pragma warning restore 612, 618
        }
    }
}
