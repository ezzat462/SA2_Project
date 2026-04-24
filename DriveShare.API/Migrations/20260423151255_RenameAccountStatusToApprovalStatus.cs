using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriveShare.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameAccountStatusToApprovalStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccountStatus",
                table: "Users",
                newName: "ApprovalStatus");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEATqm4k9E/OslpNkNPt96jMj2H/baNfG6PGFYTApvh1fPAFNY0XVDe53iQJg2rmHtw==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApprovalStatus",
                table: "Users",
                newName: "AccountStatus");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPddidu0ydrVnsbWa9ahZ5y6AZghisB8shUSy3nig7tXwZD7mpQQCpHsSmoq3gAUtQ==");
        }
    }
}
