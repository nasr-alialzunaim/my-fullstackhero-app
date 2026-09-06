using FSH.Modules.ScientificAnalysis.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.ScientificAnalysis;

[DbContext(typeof(ScientificAnalysisDbContext))]
[Migration("20260903000200_InitialScientificAnalysis")]
public sealed class InitialScientificAnalysis : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "scientific_analysis");

        migrationBuilder.CreateTable(
            name: "AnalysisRuns",
            schema: "scientific_analysis",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                AlgorithmId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                EngineName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                EngineVersion = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                UpstreamCommit = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                RequestJson = table.Column<string>(type: "text", nullable: false),
                ResponseJson = table.Column<string>(type: "text", nullable: true),
                RequestSha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                ResponseSha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                EngineHttpStatusCode = table.Column<int>(type: "integer", nullable: true),
                Outcome = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                InitiatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AnalysisRuns", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AnalysisRuns_AlgorithmId",
            schema: "scientific_analysis",
            table: "AnalysisRuns",
            column: "AlgorithmId");

        migrationBuilder.CreateIndex(
            name: "IX_AnalysisRuns_InitiatedByUserId",
            schema: "scientific_analysis",
            table: "AnalysisRuns",
            column: "InitiatedByUserId");

        migrationBuilder.CreateIndex(
            name: "IX_AnalysisRuns_StartedAtUtc",
            schema: "scientific_analysis",
            table: "AnalysisRuns",
            column: "StartedAtUtc");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AnalysisRuns",
            schema: "scientific_analysis");
    }
}
