using FSH.Modules.Subjects.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Subjects;

[DbContext(typeof(SubjectsDbContext))]
[Migration("20260906001000_InitialSubjects")]
public sealed class InitialSubjects : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "subjects");
        migrationBuilder.CreateTable(name: "Subjects", schema: "subjects", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false), SubjectCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false), SubjectType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false), Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false), CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false), UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
        }, constraints: table => table.PrimaryKey("PK_Subjects", x => x.Id));

        migrationBuilder.CreateTable(name: "PersonIdentities", schema: "subjects", columns: table => new
        {
            SubjectId = table.Column<Guid>(type: "uuid", nullable: false), NationalIdProtected = table.Column<string>(type: "text", nullable: true), NationalIdHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true), FirstName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true), MiddleName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true), LastName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true), DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true), Sex = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true), NationalityCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true), IdentityVerified = table.Column<bool>(type: "boolean", nullable: false), VerifiedByUserId = table.Column<Guid>(type: "uuid", nullable: true), VerifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
        }, constraints: table => { table.PrimaryKey("PK_PersonIdentities", x => x.SubjectId); table.ForeignKey("FK_PersonIdentities_Subjects_SubjectId", x => x.SubjectId, "subjects", "Subjects", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "SubjectAliases", schema: "subjects", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false), SubjectId = table.Column<Guid>(type: "uuid", nullable: false), AliasType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false), AliasValue = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false), CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
        }, constraints: table => { table.PrimaryKey("PK_SubjectAliases", x => x.Id); table.ForeignKey("FK_SubjectAliases_Subjects_SubjectId", x => x.SubjectId, "subjects", "Subjects", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "SubjectExternalIdentifiers", schema: "subjects", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false), SubjectId = table.Column<Guid>(type: "uuid", nullable: false), IdentifierType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false), ValueProtected = table.Column<string>(type: "text", nullable: false), ValueHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false), Issuer = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true), IsPrimary = table.Column<bool>(type: "boolean", nullable: false), CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
        }, constraints: table => { table.PrimaryKey("PK_SubjectExternalIdentifiers", x => x.Id); table.ForeignKey("FK_SubjectExternalIdentifiers_Subjects_SubjectId", x => x.SubjectId, "subjects", "Subjects", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateTable(name: "SubjectLegalReferences", schema: "subjects", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false), SubjectId = table.Column<Guid>(type: "uuid", nullable: false), ReferenceType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false), ReferenceNumber = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true), Authority = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true), IssuedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true), ExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true), Description = table.Column<string>(type: "text", nullable: true), FileAssetId = table.Column<Guid>(type: "uuid", nullable: true), CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false), CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
        }, constraints: table => { table.PrimaryKey("PK_SubjectLegalReferences", x => x.Id); table.ForeignKey("FK_SubjectLegalReferences_Subjects_SubjectId", x => x.SubjectId, "subjects", "Subjects", "Id", onDelete: ReferentialAction.Cascade); });

        migrationBuilder.CreateIndex("IX_Subjects_SubjectCode", "subjects", "Subjects", "SubjectCode", unique: true);
        migrationBuilder.CreateIndex("IX_Subjects_SubjectType", "subjects", "Subjects", "SubjectType");
        migrationBuilder.CreateIndex("IX_Subjects_Status", "subjects", "Subjects", "Status");
        migrationBuilder.CreateIndex("IX_PersonIdentities_NationalIdHash", "subjects", "PersonIdentities", "NationalIdHash");
        migrationBuilder.CreateIndex("IX_SubjectAliases_SubjectId_AliasType_AliasValue", "subjects", "SubjectAliases", new[] { "SubjectId", "AliasType", "AliasValue" }, unique: true);
        migrationBuilder.CreateIndex("IX_SubjectExternalIdentifiers_ValueHash", "subjects", "SubjectExternalIdentifiers", "ValueHash");
        migrationBuilder.CreateIndex("IX_SubjectExternalIdentifiers_SubjectId_IdentifierType", "subjects", "SubjectExternalIdentifiers", new[] { "SubjectId", "IdentifierType" });
        migrationBuilder.CreateIndex("IX_SubjectLegalReferences_SubjectId", "subjects", "SubjectLegalReferences", "SubjectId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("PersonIdentities", "subjects"); migrationBuilder.DropTable("SubjectAliases", "subjects"); migrationBuilder.DropTable("SubjectExternalIdentifiers", "subjects"); migrationBuilder.DropTable("SubjectLegalReferences", "subjects"); migrationBuilder.DropTable("Subjects", "subjects");
    }
}
