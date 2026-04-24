using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriveShare.API.Migrations
{
    /// <inheritdoc />
    public partial class CompletePlatformUpgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Cars_CarId",
                table: "Ratings");

            migrationBuilder.DropTable(
                name: "RentalRequests");

            migrationBuilder.RenameColumn(
                name: "RatingValue",
                table: "Ratings",
                newName: "Score");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "Ratings",
                newName: "Feedback");

            migrationBuilder.RenameColumn(
                name: "CarId",
                table: "Ratings",
                newName: "CarPostId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_CarId",
                table: "Ratings",
                newName: "IX_Ratings_CarPostId");

            migrationBuilder.RenameColumn(
                name: "BodyType",
                table: "Cars",
                newName: "Type");

            migrationBuilder.AddColumn<int>(
                name: "AccountStatus",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "Ratings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarPostId = table.Column<int>(type: "int", nullable: false),
                    RenterId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Cars_CarPostId",
                        column: x => x.CarPostId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_RenterId",
                        column: x => x.RenterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DriverLicenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LicenseImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverLicenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverLicenses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AccountStatus", "PasswordHash" },
                values: new object[] { 1, "AQAAAAIAAYagAAAAEAGnaoR/h35yYak78lmyN4eA1oO4J9n4OkmXiIqX6/Y97Ex4ubPH40G+aFWKZ7GgNA==" });

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_BookingId",
                table: "Ratings",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CarPostId",
                table: "Bookings",
                column: "CarPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RenterId",
                table: "Bookings",
                column: "RenterId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverLicenses_UserId",
                table: "DriverLicenses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Bookings_BookingId",
                table: "Ratings",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Cars_CarPostId",
                table: "Ratings",
                column: "CarPostId",
                principalTable: "Cars",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Bookings_BookingId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Cars_CarPostId",
                table: "Ratings");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "DriverLicenses");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_BookingId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "AccountStatus",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Cars");

            migrationBuilder.RenameColumn(
                name: "Score",
                table: "Ratings",
                newName: "RatingValue");

            migrationBuilder.RenameColumn(
                name: "Feedback",
                table: "Ratings",
                newName: "Comment");

            migrationBuilder.RenameColumn(
                name: "CarPostId",
                table: "Ratings",
                newName: "CarId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_CarPostId",
                table: "Ratings",
                newName: "IX_Ratings_CarId");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Cars",
                newName: "BodyType");

            migrationBuilder.CreateTable(
                name: "RentalRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    RenterId = table.Column<int>(type: "int", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RentalRequests_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalRequests_Users_RenterId",
                        column: x => x.RenterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEGwuBtjxvAi6059PSnkZ20QYO9Ai1J5rezZ8N2owDR6CIuD7e5VW7IUNBxihVskhhw==");

            migrationBuilder.CreateIndex(
                name: "IX_RentalRequests_CarId",
                table: "RentalRequests",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalRequests_RenterId",
                table: "RentalRequests",
                column: "RenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Cars_CarId",
                table: "Ratings",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
