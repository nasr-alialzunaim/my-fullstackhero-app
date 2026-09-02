using FSH.Modules.Samples.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Samples;

[DbContext(typeof(SamplesDbContext))]
[Migration("20260903000400_InitialSamples")]
public sealed class InitialSamples : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "samples");

        migrationBuilder.CreateTable(
            name: "BiologicalSamples",
            schema: "samples",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                EvidenceItemId = table.Column<Guid>(type: "uuid", nullable: false),
                ParentSampleId = table.Column<Guid>(type: "uuid", nullable: true),
                ExternalSampleCode = table.Column<string>(
                    type: "character varying(128)",
                    maxLength: 128,
                    nullable: true),
                CollectedAtUtc = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: true),
                CollectionNote = table.Column<string>(
                    type: "character varying(4096)",
                    maxLength: 4096,
                    nullable: true),
                CreatedAtUtc = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BiologicalSamples", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_BiologicalSamples_EvidenceItemId",
            schema: "samples",
            table: "BiologicalSamples",
            column: "EvidenceItemId");

        migrationBuilder.CreateIndex(
            name: "IX_BiologicalSamples_ExternalSampleCode",
            schema: "samples",
            table: "BiologicalSamples",
            column: "ExternalSampleCode");

        migrationBuilder.CreateIndex(
            name: "IX_BiologicalSamples_ParentSampleId",
            schema: "samples",
            table: "BiologicalSamples",
            column: "ParentSampleId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "BiologicalSamples", schema: "samples");
    }
}
