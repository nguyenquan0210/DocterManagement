using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorManagement.Data.Migrations
{
    public partial class version15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Services",
                table: "Doctors");

            migrationBuilder.AddColumn<int>(
                name: "Unit",
                table: "Medicines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Afternoon",
                table: "MedicineDetailts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Morning",
                table: "MedicineDetailts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Night",
                table: "MedicineDetailts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Noon",
                table: "MedicineDetailts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Use",
                table: "MedicineDetailts",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ServiceName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Unit = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceDetailts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServicesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDetailts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceDetailts_MedicalRecords_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceDetailts_Services_ServicesId",
                        column: x => x.ServicesId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("2dd4ec71-5669-42d7-9cf9-bb17220c64c7"),
                column: "ConcurrencyStamp",
                value: "03c514db-b8d1-4f94-940c-a4991357f9b1");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("50fe257e-6475-41f0-93f7-f530d622362b"),
                column: "ConcurrencyStamp",
                value: "99f6f946-f8b7-42e6-b4e7-07db0fcd867b");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "4420c9b5-15e5-4153-baf9-79c5ebdeca73");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "c76fcb38-07e3-4b95-b297-0a2742da96b9", "AQAAAAEAACcQAAAAEBx8yrdNYIEbrjVXecVEtbevtPPbcE5us++xOGXv9/6FSrRwC5Xfpvl8kzzs62TGxQ==" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDetailts_MedicalRecordId",
                table: "ServiceDetailts",
                column: "MedicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDetailts_ServicesId",
                table: "ServiceDetailts",
                column: "ServicesId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_DoctorId",
                table: "Services",
                column: "DoctorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceDetailts");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "Afternoon",
                table: "MedicineDetailts");

            migrationBuilder.DropColumn(
                name: "Morning",
                table: "MedicineDetailts");

            migrationBuilder.DropColumn(
                name: "Night",
                table: "MedicineDetailts");

            migrationBuilder.DropColumn(
                name: "Noon",
                table: "MedicineDetailts");

            migrationBuilder.DropColumn(
                name: "Use",
                table: "MedicineDetailts");

            migrationBuilder.AddColumn<string>(
                name: "Services",
                table: "Doctors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("2dd4ec71-5669-42d7-9cf9-bb17220c64c7"),
                column: "ConcurrencyStamp",
                value: "36a1b2e4-4997-4f52-8048-52f9412c1bc6");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("50fe257e-6475-41f0-93f7-f530d622362b"),
                column: "ConcurrencyStamp",
                value: "8bb1ca90-0328-48f6-96a9-79002edf3a8f");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "c19ece31-3589-48a3-9488-4c06a07d5fe2");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "e1fbccd5-18d7-4e33-9837-ef36616cf31e", "AQAAAAEAACcQAAAAECO0S8StV6XU4pnS0wFFYQPqdNfGueDDtayFKVSp7Sk5cfwytSwQjzjtLoqNkV9eKQ==" });
        }
    }
}
