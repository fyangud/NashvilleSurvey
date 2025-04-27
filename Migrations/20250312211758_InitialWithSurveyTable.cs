using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransitSurveyAzure.Migrations
{
    /// <inheritdoc />
    public partial class InitialWithSurveyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Metro_Resident_Survey_Responses_result");
        }
    }
}
