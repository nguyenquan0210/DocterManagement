using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorManagement.Data.Migrations
{
    public partial class version7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CratedAt",
                table: "Patients",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CratedAt",
                table: "MainMenus",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Ethnics",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Ethnics",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Ethnics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CratedAt",
                table: "Contacts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

            migrationBuilder.InsertData(
                table: "MainMenus",
                columns: new[] { "Id", "Action", "Controller", "CratedAt", "Image", "IsDeleted", "Name", "ParentId", "SortOrder", "Type" },
                values: new object[] { new Guid("8d04dce4-969a-435d-bba4-df3f325983dc"), "Index", "Home", new DateTime(2000, 10, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "default", true, "Trang Chủ", new Guid("00000000-0000-0000-0000-000000000000"), 1, "MenuHeader" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Informations",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce4-969a-435d-bba4-df3f325983dc"));

            migrationBuilder.DeleteData(
                table: "MainMenus",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce4-969a-435d-bba4-df3f325983dc"));

            migrationBuilder.DropColumn(
                name: "CratedAt",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "CratedAt",
                table: "MainMenus");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Ethnics");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Ethnics");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Ethnics");

            migrationBuilder.DropColumn(
                name: "CratedAt",
                table: "Contacts");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("2dd4ec71-5669-42d7-9cf9-bb17220c64c7"),
                column: "ConcurrencyStamp",
                value: "e6ae8d47-4d1a-4d0c-be52-4073256584ad");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("50fe257e-6475-41f0-93f7-f530d622362b"),
                column: "ConcurrencyStamp",
                value: "c056f50a-edc1-48ae-b634-0886463d85c6");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "ac67c5c1-65e1-4d33-95dd-6614172cfee0");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "115c57f0-ba5a-4f91-bbe6-71b991d3c5d6", "AQAAAAEAACcQAAAAEKwUb2vuKCvnG0rMrytyqmbHDWb/B+68zzRNos6C/6VdZbs5gj/DUNaQsmfDGA4bDg==" });
        }
    }
}
