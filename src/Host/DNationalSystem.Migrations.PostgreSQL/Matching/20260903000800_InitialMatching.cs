using FSH.Modules.Matching.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Matching;

[DbContext(typeof(MatchingDbContext))]
[Migration("20260903000800_InitialMatching")]
public sealed class InitialMatching : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "matching");

        migrationBuilder.CreateTable(
            name: "ProfileCategories", schema: "matching",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Code = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                AnalysisTypeId = table.Column<int>(type: "integer", nullable: false),
                IsReference = table.Column<bool>(type: "boolean", nullable: false),
                Mitochondrial = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            },
            constraints: table => table.PrimaryKey("PK_ProfileCategories", x => x.Id));

        migrationBuilder.CreateTable(
            name: "MatchingRules", schema: "matching",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                SourceCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                CategoryRelated = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                MinimumStringency = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                FailOnMatch = table.Column<bool>(type: "boolean", nullable: false),
                ForwardToUpper = table.Column<bool>(type: "boolean", nullable: false),
                MatchingAlgorithm = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                MinLocusMatch = table.Column<int>(type: "integer", nullable: false),
                MismatchsAllowed = table.Column<int>(type: "integer", nullable: false),
                ConsiderForN = table.Column<bool>(type: "boolean", nullable: false),
                Mitochondrial = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            },
            constraints: table => table.PrimaryKey("PK_MatchingRules", x => x.Id));

        migrationBuilder.CreateTable(
            name: "ProfileConfigurations", schema: "matching",
            columns: table => new
            {
                GeneticProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                Matchable = table.Column<bool>(type: "boolean", nullable: false),
                VictimProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            },
            constraints: table => table.PrimaryKey("PK_ProfileConfigurations", x => x.GeneticProfileId));

        migrationBuilder.CreateTable(
            name: "AutosomalMatchSearches", schema: "matching",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                QueryProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                MatchingRuleId = table.Column<Guid>(type: "uuid", nullable: false),
                AnalysisRunId = table.Column<Guid>(type: "uuid", nullable: false),
                CandidateCount = table.Column<int>(type: "integer", nullable: false),
                QualifiedCount = table.Column<int>(type: "integer", nullable: false),
                Mixture = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            },
            constraints: table => table.PrimaryKey("PK_AutosomalMatchSearches", x => x.Id));

        migrationBuilder.CreateTable(
            name: "AutosomalMatchResults", schema: "matching",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                MatchSearchId = table.Column<Guid>(type: "uuid", nullable: false),
                CandidateProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                Rank = table.Column<int>(type: "integer", nullable: false),
                RawOverall = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                RawMismatches = table.Column<int>(type: "integer", nullable: false),
                SharedMarkers = table.Column<int>(type: "integer", nullable: false),
                LeftPonderation = table.Column<double>(type: "double precision", nullable: false),
                RightPonderation = table.Column<double>(type: "double precision", nullable: false),
                RuleMismatches = table.Column<int>(type: "integer", nullable: false),
                RuleQualifiedLoci = table.Column<int>(type: "integer", nullable: false),
                RuleQualified = table.Column<bool>(type: "boolean", nullable: false),
                DetailedJson = table.Column<string>(type: "text", nullable: false),
            },
            constraints: table => table.PrimaryKey("PK_AutosomalMatchResults", x => x.Id));

        migrationBuilder.CreateTable(
            name: "MatchHits", schema: "matching",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                MatchSearchId = table.Column<Guid>(type: "uuid", nullable: false),
                MatchResultId = table.Column<Guid>(type: "uuid", nullable: false),
                QueryProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                CandidateProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                ReviewNote = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                ReviewedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ReviewedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            },
            constraints: table => table.PrimaryKey("PK_MatchHits", x => x.Id));

        migrationBuilder.CreateIndex("IX_ProfileCategories_Code", "matching", "ProfileCategories", "Code", unique: true);
        migrationBuilder.CreateIndex("IX_MatchingRules_SourceCategoryId", "matching", "MatchingRules", "SourceCategoryId");
        migrationBuilder.CreateIndex("IX_ProfileConfigurations_CategoryId", "matching", "ProfileConfigurations", "CategoryId");
        migrationBuilder.CreateIndex("IX_ProfileConfigurations_Matchable", "matching", "ProfileConfigurations", "Matchable");
        migrationBuilder.CreateIndex("IX_AutosomalMatchSearches_QueryProfileId", "matching", "AutosomalMatchSearches", "QueryProfileId");
        migrationBuilder.CreateIndex("IX_AutosomalMatchSearches_CreatedAtUtc", "matching", "AutosomalMatchSearches", "CreatedAtUtc");
        migrationBuilder.CreateIndex("IX_AutosomalMatchResults_MatchSearchId", "matching", "AutosomalMatchResults", "MatchSearchId");
        migrationBuilder.CreateIndex("IX_AutosomalMatchResults_CandidateProfileId", "matching", "AutosomalMatchResults", "CandidateProfileId");
        migrationBuilder.CreateIndex("IX_MatchHits_MatchSearchId", "matching", "MatchHits", "MatchSearchId");
        migrationBuilder.CreateIndex("IX_MatchHits_QueryProfileId", "matching", "MatchHits", "QueryProfileId");
        migrationBuilder.CreateIndex("IX_MatchHits_CandidateProfileId", "matching", "MatchHits", "CandidateProfileId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("AutosomalMatchResults", "matching");
        migrationBuilder.DropTable("MatchHits", "matching");
        migrationBuilder.DropTable("AutosomalMatchSearches", "matching");
        migrationBuilder.DropTable("ProfileConfigurations", "matching");
        migrationBuilder.DropTable("MatchingRules", "matching");
        migrationBuilder.DropTable("ProfileCategories", "matching");
    }
}
