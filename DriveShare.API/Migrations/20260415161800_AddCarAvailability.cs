using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriveShare.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCarAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableFrom",
                table: "Cars",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableTo",
                table: "Cars",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEEU/vx+HfPYoIKuf8nTuGLVyl+4Uy362zC/gwtK7U3RUnRYLe7rPilZpdR8aXnWOUg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableFrom",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "AvailableTo",
                table: "Cars");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEClb4pe0MmfURcQF6ayFY3hyACseMXfyzRMaox141WnZZCOkpaecL18NttDs9UL0Kg==");
        }
    }
}
