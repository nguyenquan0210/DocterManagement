using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorManagement.Data.Migrations
{
    public partial class version21 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rates_Doctors_UserId",
                table: "Rates");

            migrationBuilder.DropIndex(
                name: "IX_Rates_UserId",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "CheckComentId",
                table: "CommentsPost");

            migrationBuilder.RenameColumn(
                name: "CheckLevel",
                table: "CommentsPost",
                newName: "Type");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Rates",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Rates",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "CommentsPost",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("2dd4ec71-5669-42d7-9cf9-bb17220c64c7"),
                column: "ConcurrencyStamp",
                value: "26831096-3e48-4ab8-928e-4b3c2d2fd9e3");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("50fe257e-6475-41f0-93f7-f530d622362b"),
                column: "ConcurrencyStamp",
                value: "56e80529-0a09-4b68-b1ab-0decf04a7f72");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "665d79c0-9fd4-4af7-8303-1995c052f6bc");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "8f7507ea-67e4-4eb3-852b-553ff2107f19", "AQAAAAEAACcQAAAAEFRWrSku4kuKqAUYM3nmFfPPkZo6JgPb6AENOchxFT7vlaN6dp/YpqnlKcIQa+hxZw==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "CommentsPost");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "CommentsPost",
                newName: "CheckLevel");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Rates",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Rates",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Rates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CheckComentId",
                table: "CommentsPost",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("2dd4ec71-5669-42d7-9cf9-bb17220c64c7"),
                column: "ConcurrencyStamp",
                value: "3b79aaf0-bb41-447f-8887-58cb21bd6cf1");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("50fe257e-6475-41f0-93f7-f530d622362b"),
                column: "ConcurrencyStamp",
                value: "0f2927cf-c05c-4f2c-b3da-0488a0fb13b5");

            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "7482e3cd-e6fc-4a30-88f1-eb897a975385");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3b727a63-206a-458c-8577-daa166bbaba6", "AQAAAAEAACcQAAAAED2MG3BsWYXS2ygPPvKpg3RiDnjnXRMfz5WW/B7mQUjuK/cM7L5BHAjZQ9vy+qjF/A==" });

            migrationBuilder.CreateIndex(
                name: "IX_Rates_UserId",
                table: "Rates",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rates_Doctors_UserId",
                table: "Rates",
                column: "UserId",
                principalTable: "Doctors",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
