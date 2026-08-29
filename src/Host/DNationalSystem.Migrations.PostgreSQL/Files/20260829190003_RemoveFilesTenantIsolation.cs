using FSH.Modules.Files.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Files;

[DbContext(typeof(FilesDbContext))]
[Migration("20260829190003_RemoveFilesTenantIsolation")]
public sealed class RemoveFilesTenantIsolation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        DO $$
        BEGIN
            IF EXISTS (SELECT 1 FROM files."FileAssets" WHERE "TenantId" NOT IN ('', 'root')) THEN
                RAISE EXCEPTION 'Single-tenant migration refused: files contains non-root tenant data.';
            END IF;
        END $$;

        ALTER TABLE files."FileAssets" DROP COLUMN "TenantId";
        CREATE UNIQUE INDEX "UX_FileAsset_StorageKey" ON files."FileAssets" ("StorageKey") WHERE "IsDeleted" = FALSE;
        """);

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        DROP INDEX IF EXISTS files."UX_FileAsset_StorageKey";
        ALTER TABLE files."FileAssets" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        CREATE UNIQUE INDEX "UX_FileAsset_StorageKey" ON files."FileAssets" ("StorageKey", "TenantId") WHERE "IsDeleted" = FALSE;
        """);
}
