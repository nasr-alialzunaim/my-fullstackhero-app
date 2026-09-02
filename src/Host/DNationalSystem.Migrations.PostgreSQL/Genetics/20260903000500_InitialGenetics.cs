using FSH.Modules.Genetics.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Genetics;

[DbContext(typeof(GeneticsDbContext))]
[Migration("20260903000500_InitialGenetics")]
public sealed class InitialGenetics : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "genetics");

        migrationBuilder.CreateTable(
            name: "GeneticProfiles",
            schema: "genetics",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                SampleId = table.Column<Guid>(type: "uuid", nullable: false),
                ExternalProfileCode = table.Column<string>(
                    type: "character varying(128)",
                    maxLength: 128,
                    nullable: true),
                VersionNumber = table.Column<int>(type: "integer", nullable: false),
                SupersedesProfileId = table.Column<Guid>(type: "uuid", nullable: true),
                AnalysisTypeId = table.Column<int>(type: "integer", nullable: true),
                IsReference = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GeneticProfiles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ProfileLoci",
            schema: "genetics",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                GeneticProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                Marker = table.Column<string>(
                    type: "character varying(64)",
                    maxLength: 64,
                    nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProfileLoci", x => x.Id);
                table.ForeignKey(
                    name: "FK_ProfileLoci_GeneticProfiles_GeneticProfileId",
                    column: x => x.GeneticProfileId,
                    principalSchema: "genetics",
                    principalTable: "GeneticProfiles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AlleleCalls",
            schema: "genetics",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ProfileLocusId = table.Column<Guid>(type: "uuid", nullable: false),
                Value = table.Column<string>(
                    type: "character varying(64)",
                    maxLength: 64,
                    nullable: false),
                SortOrder = table.Column<int>(type: "integer", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AlleleCalls", x => x.Id);
                table.ForeignKey(
                    name: "FK_AlleleCalls_ProfileLoci_ProfileLocusId",
                    column: x => x.ProfileLocusId,
                    principalSchema: "genetics",
                    principalTable: "ProfileLoci",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "PeakObservations",
            schema: "genetics",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ProfileLocusId = table.Column<Guid>(type: "uuid", nullable: false),
                AlleleValue = table.Column<string>(
                    type: "character varying(64)",
                    maxLength: 64,
                    nullable: true),
                HeightRfu = table.Column<double>(type: "double precision", nullable: true),
                SizeBp = table.Column<double>(type: "double precision", nullable: true),
                Channel = table.Column<string>(
                    type: "character varying(64)",
                    maxLength: 64,
                    nullable: true),
                SortOrder = table.Column<int>(type: "integer", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PeakObservations", x => x.Id);
                table.ForeignKey(
                    name: "FK_PeakObservations_ProfileLoci_ProfileLocusId",
                    column: x => x.ProfileLocusId,
                    principalSchema: "genetics",
                    principalTable: "ProfileLoci",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_AlleleCalls_ProfileLocusId",
            schema: "genetics",
            table: "AlleleCalls",
            column: "ProfileLocusId");

        migrationBuilder.CreateIndex(
            name: "IX_GeneticProfiles_ExternalProfileCode",
            schema: "genetics",
            table: "GeneticProfiles",
            column: "ExternalProfileCode");

        migrationBuilder.CreateIndex(
            name: "IX_GeneticProfiles_SampleId",
            schema: "genetics",
            table: "GeneticProfiles",
            column: "SampleId");

        migrationBuilder.CreateIndex(
            name: "IX_GeneticProfiles_SampleId_VersionNumber",
            schema: "genetics",
            table: "GeneticProfiles",
            columns: new[] { "SampleId", "VersionNumber" });

        migrationBuilder.CreateIndex(
            name: "IX_GeneticProfiles_SupersedesProfileId",
            schema: "genetics",
            table: "GeneticProfiles",
            column: "SupersedesProfileId");

        migrationBuilder.CreateIndex(
            name: "IX_PeakObservations_ProfileLocusId",
            schema: "genetics",
            table: "PeakObservations",
            column: "ProfileLocusId");

        migrationBuilder.CreateIndex(
            name: "IX_ProfileLoci_GeneticProfileId",
            schema: "genetics",
            table: "ProfileLoci",
            column: "GeneticProfileId");

        migrationBuilder.CreateIndex(
            name: "IX_ProfileLoci_GeneticProfileId_Marker",
            schema: "genetics",
            table: "ProfileLoci",
            columns: new[] { "GeneticProfileId", "Marker" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "AlleleCalls", schema: "genetics");
        migrationBuilder.DropTable(name: "PeakObservations", schema: "genetics");
        migrationBuilder.DropTable(name: "ProfileLoci", schema: "genetics");
        migrationBuilder.DropTable(name: "GeneticProfiles", schema: "genetics");
    }
}
