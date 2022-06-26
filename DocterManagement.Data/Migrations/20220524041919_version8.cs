using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorManagement.Data.Migrations
{
    public partial class version8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Informations",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce4-969a-435d-bba4-df3f325983dc"));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MainMenus",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "MainMenus",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountBank",
                table: "Informations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Contacts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AnnualServiceFees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NeedToPay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TuitionPaidFreeNumBer = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TuitionPaidFreeText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Contingency = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccountBank = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Image = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransactionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualServiceFees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnualServiceFees_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoryActives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    User = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryActives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Detail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistoryActiveDetailts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MethodName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    HistoryActiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Parameters = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ExecutionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionDuration = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryActiveDetailts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoryActiveDetailts_HistoryActives_HistoryActiveId",
                        column: x => x.HistoryActiveId,
                        principalTable: "HistoryActives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("2dd4ec71-5669-42d7-9cf9-bb17220c64c7"),
                column: "ConcurrencyStamp",
                value: "bdea9330-a7b3-4d27-aba7-438d991d5a9c");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("50fe257e-6475-41f0-93f7-f530d622362b"),
                column: "ConcurrencyStamp",
                value: "015e28ff-cea7-4dbb-9c4d-c042f5c65aee");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "b013a1d3-7bf3-4ef6-bb77-47c7a030e88d");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "cb92de42-7abc-482a-946b-f98a323331cd", "AQAAAAEAACcQAAAAEOQ8vWq9DLr6i5FDV5zkn2KfGwYYXINl15ONmvhDshDFzoTD327OMmY7Ah0GAy6Ahw==" });

            migrationBuilder.InsertData(
                table: "Informations",
                columns: new[] { "Id", "AccountBank", "Company", "Email", "FullAddress", "Hotline", "Image", "IsDeleted", "TimeWorking" },
                values: new object[] { new Guid("b0603a9c-a60e-496f-b096-e1b18cad69e0"), "34569876567823", "Công ty TNHH DoctorMedio", "nguyenquan52000@gmail.com", "Thôn An lương, Xã Tam Anh Bắc, Huyện Núi Thành, Tỉnh Quảng Nam", "0373951042", "default", false, "7:30-18:00 mỗi tuần" });

            migrationBuilder.UpdateData(
                table: "MainMenus",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce4-969a-435d-bba4-df3f325983dc"),
                columns: new[] { "Description", "Title" },
                values: new object[] { "", "trang chủ" });

            migrationBuilder.UpdateData(
                table: "Specialities",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce4-969a-435d-bba4-df3f325983dc"),
                column: "Img",
                value: "default");

            migrationBuilder.CreateIndex(
                name: "IX_AnnualServiceFees_DoctorId",
                table: "AnnualServiceFees",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryActiveDetailts_HistoryActiveId",
                table: "HistoryActiveDetailts",
                column: "HistoryActiveId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnualServiceFees");

            migrationBuilder.DropTable(
                name: "HistoryActiveDetailts");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "HistoryActives");

            migrationBuilder.DeleteData(
                table: "Informations",
                keyColumn: "Id",
                keyValue: new Guid("b0603a9c-a60e-496f-b096-e1b18cad69e0"));

            migrationBuilder.DropColumn(
                name: "Description",
                table: "MainMenus");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "MainMenus");

            migrationBuilder.DropColumn(
                name: "AccountBank",
                table: "Informations");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Contacts");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("2dd4ec71-5669-42d7-9cf9-bb17220c64c7"),
                column: "ConcurrencyStamp",
                value: "d2610ad0-09c4-4fa4-8870-01f99ca3d4a6");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("50fe257e-6475-41f0-93f7-f530d622362b"),
                column: "ConcurrencyStamp",
                value: "019c3bd6-6d49-4d70-aef3-3464098f3f77");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "61dfe065-6c67-46c9-a569-1ced915eb6f7");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "d69dd3d1-b306-442b-8c08-ef2a62744a58", "AQAAAAEAACcQAAAAEHePJQbZxLLAd+QX/v2wgAO18H0tUzLi+NIQzxZXIRQhZoxE9iHSQkO8NVRc5WLaSg==" });

            migrationBuilder.InsertData(
                table: "Informations",
                columns: new[] { "Id", "Company", "Email", "FullAddress", "Hotline", "Image", "IsDeleted", "TimeWorking" },
                values: new object[] { new Guid("8d04dce4-969a-435d-bba4-df3f325983dc"), "Công ty TNHH DoctorMedio", "nguyenquan52000@gmail.com", "Thôn An lương, Xã Tam Anh Bắc, Huyện Núi Thành, Tỉnh Quảng Nam", "0373951042", "default", false, "7:30-18:00 mỗi tuần" });

            migrationBuilder.UpdateData(
                table: "Specialities",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce4-969a-435d-bba4-df3f325983dc"),
                column: "Img",
                value: null);
        }
    }
}
