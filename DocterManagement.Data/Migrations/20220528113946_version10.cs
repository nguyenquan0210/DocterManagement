using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorManagement.Data.Migrations
{
    public partial class version10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "No",
                table: "AnnualServiceFees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("2dd4ec71-5669-42d7-9cf9-bb17220c64c7"),
                column: "ConcurrencyStamp",
                value: "fb60f697-36dd-4cda-a080-473e31b2163e");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("50fe257e-6475-41f0-93f7-f530d622362b"),
                column: "ConcurrencyStamp",
                value: "4e642f0f-e57f-42d1-be6a-5035137e90af");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "b00b8e21-16a3-439b-8104-e2efd0e1646b");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "c1dedbbb-149e-461d-904c-939dcc7dc5a1", "AQAAAAEAACcQAAAAEHbyWd9vvdYFow8poUxq4sniXF0zfoQKalzzalTCAbPdtsqdeD4KAuK4z/BaxM1wgA==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "No",
                table: "AnnualServiceFees");

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
        }
    }
}
