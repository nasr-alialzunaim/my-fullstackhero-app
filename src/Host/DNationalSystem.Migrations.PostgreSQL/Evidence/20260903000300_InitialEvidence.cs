using FSH.Modules.Evidence.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Evidence;

[DbContext(typeof(EvidenceDbContext))]
[Migration("20260903000300_InitialEvidence")]
public sealed class InitialEvidence : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "evidence");

        migrationBuilder.CreateTable(
            name: "EvidenceItems",
            schema: "evidence",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CaseId = table.Column<Guid>(type: "uuid", nullable: false),
                ExternalReference = table.Column<string>(
                    type: "character varying(128)",
                    maxLength: 128,
                    nullable: true),
                Description = table.Column<string>(
                    type: "character varying(4096)",
                    maxLength: 4096,
                    nullable: true),
                CreatedAtUtc = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EvidenceItems", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_EvidenceItems_CaseId",
            schema: "evidence",
            table: "EvidenceItems",
            column: "CaseId");

        migrationBuilder.CreateIndex(
            name: "IX_EvidenceItems_ExternalReference",
            schema: "evidence",
            table: "EvidenceItems",
            column: "ExternalReference");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "EvidenceItems", schema: "evidence");
    }
}
