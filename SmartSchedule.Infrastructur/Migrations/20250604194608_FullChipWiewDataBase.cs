using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartSchedule.Infrastructur.Migrations
{
    /// <inheritdoc />
    public partial class FullChipWiewDataBase : Migration
    {
        private static readonly string[] columns = new[] { "id", "name" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);
            migrationBuilder.RenameColumn(
                name: "day_of_week",
                table: "Lessons",
                newName: "day_of_week_id");

            migrationBuilder.CreateTable(
                name: "WeekDays",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekDays", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "WeekDays",
                columns: columns,
                values: new object[,]
                {
                    { 1, "Понедельник" },
                    { 2, "Вторник" },
                    { 3, "Среда" },
                    { 4, "Четверг" },
                    { 5, "Пятница" },
                    { 6, "Суббота" },
                    { 7, "Воскресенье" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_day_of_week_id",
                table: "Lessons",
                column: "day_of_week_id");

            migrationBuilder.AddForeignKey(
                name: "FK__Lessons__WeekDay__48C3289E",
                table: "Lessons",
                column: "day_of_week_id",
                principalTable: "WeekDays",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);
            migrationBuilder.DropForeignKey(
                name: "FK__Lessons__WeekDay__48C3289E",
                table: "Lessons");

            migrationBuilder.DropTable(
                name: "WeekDays");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_day_of_week_id",
                table: "Lessons");

            migrationBuilder.RenameColumn(
                name: "day_of_week_id",
                table: "Lessons",
                newName: "day_of_week");
        }
    }
}
