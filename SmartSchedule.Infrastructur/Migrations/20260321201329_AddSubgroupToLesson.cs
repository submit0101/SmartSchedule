using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace SmartSchedule.Infrastructur.Migrations
{
    /// <inheritdoc />
    public partial class AddSubgroupToLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);
            migrationBuilder.AddColumn<int>(
                name: "subgroup",
                table: "Lessons",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);
            migrationBuilder.DropColumn(
                name: "subgroup",
                table: "Lessons");
        }
    }
}
