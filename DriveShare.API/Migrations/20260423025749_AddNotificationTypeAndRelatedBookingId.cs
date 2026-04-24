using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriveShare.API.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationTypeAndRelatedBookingId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RelatedBookingId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEKNKNCkPI18Y5z+hHjM/w99sX7UdXbO3Y1W8XuLn3TKAXChhK6JcVCajD+sBm0So8A==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelatedBookingId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Notifications");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAGnaoR/h35yYak78lmyN4eA1oO4J9n4OkmXiIqX6/Y97Ex4ubPH40G+aFWKZ7GgNA==");
        }
    }
}
