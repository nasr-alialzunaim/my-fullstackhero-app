using FSH.Modules.Samples.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Samples;

[DbContext(typeof(SamplesDbContext))]
[Migration("20260906001200_ReframeSampleAsEvidence")]
public sealed class ReframeSampleAsEvidence : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>("SampleCode", "samples", "BiologicalSamples", type: "character varying(128)", maxLength: 128, nullable: true);
        migrationBuilder.AddColumn<string>("SampleContext", "samples", "BiologicalSamples", type: "character varying(32)", maxLength: 32, nullable: true);
        migrationBuilder.AddColumn<Guid>("CaseId", "samples", "BiologicalSamples", type: "uuid", nullable: true);
        migrationBuilder.AddColumn<Guid>("SubjectId", "samples", "BiologicalSamples", type: "uuid", nullable: true);
        migrationBuilder.AddColumn<string>("SampleType", "samples", "BiologicalSamples", type: "character varying(64)", maxLength: 64, nullable: true);
        migrationBuilder.AddColumn<string>("Matrix", "samples", "BiologicalSamples", type: "character varying(64)", maxLength: 64, nullable: true);
        migrationBuilder.AddColumn<string>("CollectionLocation", "samples", "BiologicalSamples", type: "text", nullable: true);
        migrationBuilder.AddColumn<string>("ContainerCode", "samples", "BiologicalSamples", type: "character varying(128)", maxLength: 128, nullable: true);
        migrationBuilder.AddColumn<string>("SealNumber", "samples", "BiologicalSamples", type: "character varying(128)", maxLength: 128, nullable: true);
        migrationBuilder.AddColumn<string>("Status", "samples", "BiologicalSamples", type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "Registered");
        migrationBuilder.AddColumn<Guid>("CreatedByUserId", "samples", "BiologicalSamples", type: "uuid", nullable: false, defaultValue: Guid.Empty);
        migrationBuilder.AddColumn<DateTime>("UpdatedAtUtc", "samples", "BiologicalSamples", type: "timestamp with time zone", nullable: true);

        migrationBuilder.Sql("UPDATE samples.\"BiologicalSamples\" s SET \"SampleCode\" = 'LEGACY-' || replace(s.\"Id\"::text, '-', ''), \"SampleContext\" = 'CaseSample', \"CaseId\" = e.\"CaseId\" FROM evidence.\"EvidenceItems\" e WHERE s.\"EvidenceItemId\" = e.\"Id\";");
        migrationBuilder.Sql("UPDATE samples.\"BiologicalSamples\" SET \"SampleCode\" = COALESCE(\"SampleCode\", 'LEGACY-' || replace(\"Id\"::text, '-', '')), \"SampleContext\" = COALESCE(\"SampleContext\", 'Unknown') WHERE \"SampleCode\" IS NULL OR \"SampleContext\" IS NULL;");

        migrationBuilder.AlterColumn<string>("SampleCode", "samples", "BiologicalSamples", type: "character varying(128)", maxLength: 128, nullable: false, oldClrType: typeof(string), oldType: "character varying(128)", oldMaxLength: 128, oldNullable: true);
        migrationBuilder.AlterColumn<string>("SampleContext", "samples", "BiologicalSamples", type: "character varying(32)", maxLength: 32, nullable: false, oldClrType: typeof(string), oldType: "character varying(32)", oldMaxLength: 32, oldNullable: true);

        migrationBuilder.DropIndex("IX_BiologicalSamples_EvidenceItemId", "samples", "BiologicalSamples");
        migrationBuilder.DropColumn("EvidenceItemId", "samples", "BiologicalSamples");
        migrationBuilder.AddCheckConstraint("CK_BiologicalSamples_Context", "samples", "BiologicalSamples", "(\"SampleContext\" = 'CaseSample' AND \"CaseId\" IS NOT NULL AND \"SubjectId\" IS NULL) OR (\"SampleContext\" = 'KnownReference' AND \"CaseId\" IS NULL AND \"SubjectId\" IS NOT NULL) OR (\"SampleContext\" = 'Unknown' AND \"CaseId\" IS NULL AND \"SubjectId\" IS NULL)");
        migrationBuilder.AddForeignKey("FK_BiologicalSamples_BiologicalSamples_ParentSampleId", "samples", "BiologicalSamples", "ParentSampleId", "samples", "BiologicalSamples", principalColumn: "Id", onDelete: ReferentialAction.Restrict);

        migrationBuilder.CreateTable(name: "SampleCustodyEvents", schema: "samples", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false), SampleId = table.Column<Guid>(type: "uuid", nullable: false), EventType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false), FromCustodianUserId = table.Column<Guid>(type: "uuid", nullable: true), ToCustodianUserId = table.Column<Guid>(type: "uuid", nullable: true), FromLocation = table.Column<string>(type: "text", nullable: true), ToLocation = table.Column<string>(type: "text", nullable: true), ContainerCode = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true), SealNumber = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true), Reason = table.Column<string>(type: "text", nullable: true), Notes = table.Column<string>(type: "text", nullable: true), PerformedByUserId = table.Column<Guid>(type: "uuid", nullable: false), OccurredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false), PreviousEventHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true), EventHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
        }, constraints: table => { table.PrimaryKey("PK_SampleCustodyEvents", x => x.Id); table.ForeignKey("FK_SampleCustodyEvents_BiologicalSamples_SampleId", x => x.SampleId, "samples", "BiologicalSamples", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "SampleProcessingEvents", schema: "samples", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false), SampleId = table.Column<Guid>(type: "uuid", nullable: false), EventType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false), Method = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true), KitId = table.Column<Guid>(type: "uuid", nullable: true), BatchCode = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true), PerformedByUserId = table.Column<Guid>(type: "uuid", nullable: false), StartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true), CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true), ResultSummary = table.Column<string>(type: "text", nullable: true), ResultJson = table.Column<string>(type: "jsonb", nullable: true),
        }, constraints: table => { table.PrimaryKey("PK_SampleProcessingEvents", x => x.Id); table.ForeignKey("FK_SampleProcessingEvents_BiologicalSamples_SampleId", x => x.SampleId, "samples", "BiologicalSamples", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "SampleAttachments", schema: "samples", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false), SampleId = table.Column<Guid>(type: "uuid", nullable: false), FileAssetId = table.Column<Guid>(type: "uuid", nullable: false), AttachmentType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false), Description = table.Column<string>(type: "text", nullable: true), CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false), CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
        }, constraints: table => { table.PrimaryKey("PK_SampleAttachments", x => x.Id); table.ForeignKey("FK_SampleAttachments_BiologicalSamples_SampleId", x => x.SampleId, "samples", "BiologicalSamples", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateIndex("IX_BiologicalSamples_SampleCode", "samples", "BiologicalSamples", "SampleCode", unique: true); migrationBuilder.CreateIndex("IX_BiologicalSamples_SampleContext", "samples", "BiologicalSamples", "SampleContext"); migrationBuilder.CreateIndex("IX_BiologicalSamples_CaseId", "samples", "BiologicalSamples", "CaseId"); migrationBuilder.CreateIndex("IX_BiologicalSamples_SubjectId", "samples", "BiologicalSamples", "SubjectId"); migrationBuilder.CreateIndex("IX_BiologicalSamples_Status", "samples", "BiologicalSamples", "Status"); migrationBuilder.CreateIndex("IX_SampleCustodyEvents_SampleId_OccurredAtUtc", "samples", "SampleCustodyEvents", new[] { "SampleId", "OccurredAtUtc" }); migrationBuilder.CreateIndex("IX_SampleProcessingEvents_SampleId_StartedAtUtc", "samples", "SampleProcessingEvents", new[] { "SampleId", "StartedAtUtc" }); migrationBuilder.CreateIndex("IX_SampleProcessingEvents_KitId", "samples", "SampleProcessingEvents", "KitId"); migrationBuilder.CreateIndex("IX_SampleAttachments_SampleId", "samples", "SampleAttachments", "SampleId"); migrationBuilder.CreateIndex("IX_SampleAttachments_FileAssetId", "samples", "SampleAttachments", "FileAssetId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("SampleCustodyEvents", "samples"); migrationBuilder.DropTable("SampleProcessingEvents", "samples"); migrationBuilder.DropTable("SampleAttachments", "samples"); migrationBuilder.DropForeignKey("FK_BiologicalSamples_BiologicalSamples_ParentSampleId", "samples", "BiologicalSamples"); migrationBuilder.DropCheckConstraint("CK_BiologicalSamples_Context", "samples", "BiologicalSamples"); migrationBuilder.DropIndex("IX_BiologicalSamples_SampleCode", "samples", "BiologicalSamples"); migrationBuilder.DropIndex("IX_BiologicalSamples_SampleContext", "samples", "BiologicalSamples"); migrationBuilder.DropIndex("IX_BiologicalSamples_CaseId", "samples", "BiologicalSamples"); migrationBuilder.DropIndex("IX_BiologicalSamples_SubjectId", "samples", "BiologicalSamples"); migrationBuilder.DropIndex("IX_BiologicalSamples_Status", "samples", "BiologicalSamples"); migrationBuilder.AddColumn<Guid>("EvidenceItemId", "samples", "BiologicalSamples", type: "uuid", nullable: false, defaultValue: Guid.Empty); migrationBuilder.CreateIndex("IX_BiologicalSamples_EvidenceItemId", "samples", "BiologicalSamples", "EvidenceItemId"); migrationBuilder.DropColumn("SampleCode", "samples", "BiologicalSamples"); migrationBuilder.DropColumn("SampleContext", "samples", "BiologicalSamples"); migrationBuilder.DropColumn("CaseId", "samples", "BiologicalSamples"); migrationBuilder.DropColumn("SubjectId", "samples", "BiologicalSamples"); migrationBuilder.DropColumn("SampleType", "samples", "BiologicalSamples"); migrationBuilder.DropColumn("Matrix", "samples", "BiologicalSamples"); migrationBuilder.DropColumn("CollectionLocation", "samples", "BiologicalSamples"); migrationBuilder.DropColumn("ContainerCode", "samples", "BiologicalSamples"); migrationBuilder.DropColumn("SealNumber", "samples", "BiologicalSamples"); migrationBuilder.DropColumn("Status", "samples", "BiologicalSamples"); migrationBuilder.DropColumn("CreatedByUserId", "samples", "BiologicalSamples"); migrationBuilder.DropColumn("UpdatedAtUtc", "samples", "BiologicalSamples");
    }
}
