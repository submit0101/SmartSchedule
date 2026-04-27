using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartSchedule.Infrastructur.Migrations
{
    /// <inheritdoc />
    public partial class DeletePositionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Positions_position_id",
                table: "Teachers");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_position_id",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_UniTeatcher_Postiton",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "position_id",
                table: "Teachers");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueTeacher_FullName",
                table: "Teachers",
                columns: new[] { "first_name", "middle_name", "last_name" },
                unique: true,
                filter: "[middle_name] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UniqueTeacher_FullName",
                table: "Teachers");

            migrationBuilder.AddColumn<int>(
                name: "position_id",
                table: "Teachers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_position_id",
                table: "Teachers",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "IX_UniTeatcher_Postiton",
                table: "Teachers",
                columns: new[] { "first_name", "middle_name", "last_name", "position_id" },
                unique: true,
                filter: "[middle_name] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Positions_position_id",
                table: "Teachers",
                column: "position_id",
                principalTable: "Positions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
