using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartSchedule.Infrastructur.Migrations
{
    /// <inheritdoc />
    public partial class RefactorCascadKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Lessons__Cabinet__4316F928",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK__Lessons__GroupId__44FF419A",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK__Lessons__Subject__45F365D3",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK__Lessons__TimeSlo__46E78A0C",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK__Lessons__WeekDay__48C3289E",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK__Lessons__WeekTyp__47DBAE45",
                table: "Lessons");

            migrationBuilder.AddForeignKey(
                name: "FK__Lessons__Cabinet__4316F928",
                table: "Lessons",
                column: "cabinet_id",
                principalTable: "Cabinets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__Lessons__GroupId__44FF419A",
                table: "Lessons",
                column: "group_id",
                principalTable: "Groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__Lessons__Subject__45F365D3",
                table: "Lessons",
                column: "subject_id",
                principalTable: "Subjects",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__Lessons__TimeSlo__46E78A0C",
                table: "Lessons",
                column: "time_slot_id",
                principalTable: "TimeSlots",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__Lessons__WeekDay__48C3289E",
                table: "Lessons",
                column: "day_of_week_id",
                principalTable: "WeekDays",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__Lessons__WeekTyp__47DBAE45",
                table: "Lessons",
                column: "week_type_id",
                principalTable: "WeekTypes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Lessons__Cabinet__4316F928",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK__Lessons__GroupId__44FF419A",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK__Lessons__Subject__45F365D3",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK__Lessons__TimeSlo__46E78A0C",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK__Lessons__WeekDay__48C3289E",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK__Lessons__WeekTyp__47DBAE45",
                table: "Lessons");

            migrationBuilder.AddForeignKey(
                name: "FK__Lessons__Cabinet__4316F928",
                table: "Lessons",
                column: "cabinet_id",
                principalTable: "Cabinets",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK__Lessons__GroupId__44FF419A",
                table: "Lessons",
                column: "group_id",
                principalTable: "Groups",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK__Lessons__Subject__45F365D3",
                table: "Lessons",
                column: "subject_id",
                principalTable: "Subjects",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK__Lessons__TimeSlo__46E78A0C",
                table: "Lessons",
                column: "time_slot_id",
                principalTable: "TimeSlots",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK__Lessons__WeekDay__48C3289E",
                table: "Lessons",
                column: "day_of_week_id",
                principalTable: "WeekDays",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__Lessons__WeekTyp__47DBAE45",
                table: "Lessons",
                column: "week_type_id",
                principalTable: "WeekTypes",
                principalColumn: "id");
        }
    }
}
