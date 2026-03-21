using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartSchedule.Infrastructur.Migrations
{
    /// <inheritdoc />
    public partial class CreateNewEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);
            migrationBuilder.DropColumn(
                name: "building",
                table: "Cabinets");

            migrationBuilder.AddColumn<int>(
                name: "building_id",
                table: "Cabinets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cabinets_building_id",
                table: "Cabinets",
                column: "building_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cabinets_Buildings_building_id",
                table: "Cabinets",
                column: "building_id",
                principalTable: "Buildings",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);
            migrationBuilder.DropForeignKey(
                name: "FK_Cabinets_Buildings_building_id",
                table: "Cabinets");

            migrationBuilder.DropTable(
                name: "Buildings");

            migrationBuilder.DropIndex(
                name: "IX_Cabinets_building_id",
                table: "Cabinets");

            migrationBuilder.DropColumn(
                name: "building_id",
                table: "Cabinets");

            migrationBuilder.AddColumn<string>(
                name: "building",
                table: "Cabinets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
