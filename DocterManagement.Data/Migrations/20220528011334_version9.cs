using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorManagement.Data.Migrations
{
    public partial class version9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountBankName",
                table: "Informations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Informations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ServiceFee",
                table: "Informations",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "AnnualServiceFees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("2dd4ec71-5669-42d7-9cf9-bb17220c64c7"),
                column: "ConcurrencyStamp",
                value: "043c3a4b-d3f6-4ca4-8185-6bbc36225177");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("50fe257e-6475-41f0-93f7-f530d622362b"),
                column: "ConcurrencyStamp",
                value: "e976b7a9-8c25-45fa-a052-5e2a14637ce7");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "25d32b54-8a5c-4669-856d-cf415bde4404");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0f88fcdb-e5d7-476b-88af-8b8fd454f5aa", "AQAAAAEAACcQAAAAEIDf7C+F98OdJuT+T9uL+2HbcjDDYm11ZqgigeILTYi7KhU6F8sV4QdXu8yXF59ewQ==" });

            migrationBuilder.UpdateData(
                table: "Informations",
                keyColumn: "Id",
                keyValue: new Guid("b0603a9c-a60e-496f-b096-e1b18cad69e0"),
                columns: new[] { "AccountBankName", "Content", "ServiceFee" },
                values: new object[] { "Vietinbank", "Nộp phí sử dụng dịch vụ bác sĩ.", 2400000m });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountBankName",
                table: "Informations");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Informations");

            migrationBuilder.DropColumn(
                name: "ServiceFee",
                table: "Informations");

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "AnnualServiceFees");

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
        }
    }
}
