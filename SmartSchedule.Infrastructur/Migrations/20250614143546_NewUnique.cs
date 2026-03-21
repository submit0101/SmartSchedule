using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartSchedule.Infrastructur.Migrations
{
    /// <inheritdoc />
    public partial class NewUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);
            migrationBuilder.CreateIndex(
                name: "IX_UniTeatcher_Postiton",
                table: "Teachers",
                columns: ["first_name", "middle_name", "last_name", "position_id"],
                unique: true,
                filter: "[middle_name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UniTitile",
                table: "Subjects",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UniName",
                table: "Groups",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cabinets_Number_BuildingId",
                table: "Cabinets",
                columns: ["number", "building_id"],
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UniName",
                table: "Buildings",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);
            migrationBuilder.DropIndex(
                name: "IX_UniTeatcher_Postiton",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_UniTitile",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_UniName",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Cabinets_Number_BuildingId",
                table: "Cabinets");

            migrationBuilder.DropIndex(
                name: "IX_UniName",
                table: "Buildings");
        }
    }
}
