using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorManagement.Data.Migrations
{
    public partial class version13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecords_Doctors_DoctorId",
                table: "MedicalRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecords_Patients_PatientId",
                table: "MedicalRecords");

            migrationBuilder.DropIndex(
                name: "IX_MedicalRecords_DoctorId",
                table: "MedicalRecords");

            migrationBuilder.DropIndex(
                name: "IX_MedicalRecords_PatientId",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "MedicalRecords");

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorsUserId",
                table: "MedicalRecords",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PatientsPatientId",
                table: "MedicalRecords",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "MedicalRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("2dd4ec71-5669-42d7-9cf9-bb17220c64c7"),
                column: "ConcurrencyStamp",
                value: "5d8e7ed8-a006-4905-bc4c-e812dbd3b27c");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("50fe257e-6475-41f0-93f7-f530d622362b"),
                column: "ConcurrencyStamp",
                value: "b4d9a432-4c27-4d01-9f49-764e7db2a537");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "6465dfa0-6e09-471e-b2ac-98051cecbc1f");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "db3ce7d0-f38b-4b35-a0ab-5df6bfa81454", "AQAAAAEAACcQAAAAEAwtKKY4TkLIcvPYxTBWC5k7YUdAc1kvNVFZKQYP40VMwSGggSkkgEVLpz1SIxO91w==" });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_DoctorsUserId",
                table: "MedicalRecords",
                column: "DoctorsUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_PatientsPatientId",
                table: "MedicalRecords",
                column: "PatientsPatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_Doctors_DoctorsUserId",
                table: "MedicalRecords",
                column: "DoctorsUserId",
                principalTable: "Doctors",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_Patients_PatientsPatientId",
                table: "MedicalRecords",
                column: "PatientsPatientId",
                principalTable: "Patients",
                principalColumn: "PatientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecords_Doctors_DoctorsUserId",
                table: "MedicalRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecords_Patients_PatientsPatientId",
                table: "MedicalRecords");

            migrationBuilder.DropIndex(
                name: "IX_MedicalRecords_DoctorsUserId",
                table: "MedicalRecords");

            migrationBuilder.DropIndex(
                name: "IX_MedicalRecords_PatientsPatientId",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "DoctorsUserId",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "PatientsPatientId",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "MedicalRecords");

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "MedicalRecords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PatientId",
                table: "MedicalRecords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("2dd4ec71-5669-42d7-9cf9-bb17220c64c7"),
                column: "ConcurrencyStamp",
                value: "85659927-cb6d-4ca6-ba49-c287dc4b0cd4");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("50fe257e-6475-41f0-93f7-f530d622362b"),
                column: "ConcurrencyStamp",
                value: "355ea7f0-54c4-4024-b2cd-df280f190b7e");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "28264415-fec6-4b05-9dba-ec036570e8b9");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "a0f65014-ffa6-411e-a6c0-dcee2c95de73", "AQAAAAEAACcQAAAAEE5gr2yImNng8s+pYX3nsyGPgDZjfyOqW2pcwFEvJD88IDEDqAQYsf3ligWhITwSxg==" });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_DoctorId",
                table: "MedicalRecords",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_PatientId",
                table: "MedicalRecords",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_Doctors_DoctorId",
                table: "MedicalRecords",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_Patients_PatientId",
                table: "MedicalRecords",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId");
        }
    }
}
