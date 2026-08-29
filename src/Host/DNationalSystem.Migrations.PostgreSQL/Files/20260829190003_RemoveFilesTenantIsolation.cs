using FSH.Modules.Files.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Files;

[DbContext(typeof(FilesDbContext))]
[Migration("20260829190003_RemoveFilesTenantIsolation")]
public sealed class RemoveFilesTenantIsolation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DO $$
            BEGIN
                IF EXISTS (SELECT 1 FROM files."FileAssets" WHERE "TenantId" NOT IN ('', 'root')) THEN
                    RAISE EXCEPTION 'Single-tenant migration refused: files contains non-root tenant data.';
                END IF;
            END $$;
            """);

        migrationBuilder.DropColumn("TenantId", "files", "FileAssets");
        migrationBuilder.CreateIndex("UX_FileAsset_StorageKey", "files", "FileAssets", "StorageKey", unique: true, filter: "\\\"IsDeleted\\\" = FALSE");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex("UX_FileAsset_StorageKey", "files", "FileAssets");
        migrationBuilder.AddColumn<string>("TenantId", "files", "FileAssets", "text", nullable: false, defaultValue: "root");
        migrationBuilder.CreateIndex("UX_FileAsset_StorageKey", "files", "FileAssets", new[] { "StorageKey", "TenantId" }, unique: true, filter: "\\\"IsDeleted\\\" = FALSE");
    }
}
