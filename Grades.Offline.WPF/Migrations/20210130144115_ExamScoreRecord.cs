using Microsoft.EntityFrameworkCore.Migrations;

namespace Grades.Offline.WPF.Migrations
{
    public partial class ExamScoreRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudentScoresEncoded",
                table: "Exams",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentScoresEncoded",
                table: "Exams");
        }
    }
}
