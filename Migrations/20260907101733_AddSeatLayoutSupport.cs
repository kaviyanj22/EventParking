using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Event_parking.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatLayoutSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PositionX",
                table: "Seats",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PositionY",
                table: "Seats",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeatSectionId",
                table: "Seats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeatingLayoutImageUrl",
                table: "Events",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SeatSections",
                columns: table => new
                {
                    SeatSectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LayoutImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatSections", x => x.SeatSectionId);
                    table.ForeignKey(
                        name: "FK_SeatSections_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Seats_SeatSectionId",
                table: "Seats",
                column: "SeatSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_SeatSections_EventId_SectionName",
                table: "SeatSections",
                columns: new[] { "EventId", "SectionName" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_SeatSections_SeatSectionId",
                table: "Seats",
                column: "SeatSectionId",
                principalTable: "SeatSections",
                principalColumn: "SeatSectionId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_SeatSections_SeatSectionId",
                table: "Seats");

            migrationBuilder.DropTable(
                name: "SeatSections");

            migrationBuilder.DropIndex(
                name: "IX_Seats_SeatSectionId",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "PositionX",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "PositionY",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "SeatSectionId",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "SeatingLayoutImageUrl",
                table: "Events");
        }
    }
}
