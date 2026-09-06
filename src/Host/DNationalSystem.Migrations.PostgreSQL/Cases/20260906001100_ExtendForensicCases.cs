using FSH.Modules.Cases.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Cases;

[DbContext(typeof(CasesDbContext))]
[Migration("20260906001100_ExtendForensicCases")]
public sealed class ExtendForensicCases : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(name: "CaseType", schema: "cases", table: "Cases", type: "character varying(32)", maxLength: 32, nullable: true);
        migrationBuilder.AddColumn<string>(name: "Status", schema: "cases", table: "Cases", type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Open");
        migrationBuilder.AddColumn<string>(name: "Priority", schema: "cases", table: "Cases", type: "character varying(32)", maxLength: 32, nullable: true);
        migrationBuilder.AddColumn<string>(name: "JurisdictionCode", schema: "cases", table: "Cases", type: "character varying(64)", maxLength: 64, nullable: true);
        migrationBuilder.AddColumn<DateTime>(name: "IncidentAtUtc", schema: "cases", table: "Cases", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<DateTime>(name: "OpenedAtUtc", schema: "cases", table: "Cases", type: "timestamp with time zone", nullable: true);
        migrationBuilder.AddColumn<DateTime>(name: "ClosedAtUtc", schema: "cases", table: "Cases", type: "timestamp with time zone", nullable: true);
        migrationBuilder.Sql("UPDATE cases.\"Cases\" SET \"OpenedAtUtc\" = \"CreatedAtUtc\" WHERE \"OpenedAtUtc\" IS NULL;");
        migrationBuilder.AlterColumn<DateTime>(name: "OpenedAtUtc", schema: "cases", table: "Cases", type: "timestamp with time zone", nullable: false, oldClrType: typeof(DateTime), oldType: "timestamp with time zone", oldNullable: true);

        migrationBuilder.CreateTable(name: "CaseAssignments", schema: "cases", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false), CaseId = table.Column<Guid>(type: "uuid", nullable: false), UserId = table.Column<Guid>(type: "uuid", nullable: false), AssignmentRole = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false), AssignedByUserId = table.Column<Guid>(type: "uuid", nullable: false), AssignedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false), ReleasedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
        }, constraints: table => { table.PrimaryKey("PK_CaseAssignments", x => x.Id); table.ForeignKey("FK_CaseAssignments_Cases_CaseId", x => x.CaseId, "cases", "Cases", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "CaseStatusHistory", schema: "cases", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false), CaseId = table.Column<Guid>(type: "uuid", nullable: false), FromStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true), ToStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false), Reason = table.Column<string>(type: "text", nullable: true), ChangedByUserId = table.Column<Guid>(type: "uuid", nullable: false), ChangedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
        }, constraints: table => { table.PrimaryKey("PK_CaseStatusHistory", x => x.Id); table.ForeignKey("FK_CaseStatusHistory_Cases_CaseId", x => x.CaseId, "cases", "Cases", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateIndex("IX_Cases_Status", "cases", "Cases", "Status"); migrationBuilder.CreateIndex("IX_Cases_CaseType", "cases", "Cases", "CaseType"); migrationBuilder.CreateIndex("IX_Cases_OpenedAtUtc", "cases", "Cases", "OpenedAtUtc"); migrationBuilder.CreateIndex("IX_CaseAssignments_CaseId_UserId", "cases", "CaseAssignments", new[] { "CaseId", "UserId" }); migrationBuilder.CreateIndex("IX_CaseAssignments_UserId", "cases", "CaseAssignments", "UserId"); migrationBuilder.CreateIndex("IX_CaseStatusHistory_CaseId_ChangedAtUtc", "cases", "CaseStatusHistory", new[] { "CaseId", "ChangedAtUtc" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("CaseAssignments", "cases"); migrationBuilder.DropTable("CaseStatusHistory", "cases"); migrationBuilder.DropIndex("IX_Cases_Status", "cases", "Cases"); migrationBuilder.DropIndex("IX_Cases_CaseType", "cases", "Cases"); migrationBuilder.DropIndex("IX_Cases_OpenedAtUtc", "cases", "Cases"); migrationBuilder.DropColumn("CaseType", "cases", "Cases"); migrationBuilder.DropColumn("Status", "cases", "Cases"); migrationBuilder.DropColumn("Priority", "cases", "Cases"); migrationBuilder.DropColumn("JurisdictionCode", "cases", "Cases"); migrationBuilder.DropColumn("IncidentAtUtc", "cases", "Cases"); migrationBuilder.DropColumn("OpenedAtUtc", "cases", "Cases"); migrationBuilder.DropColumn("ClosedAtUtc", "cases", "Cases");
    }
}
