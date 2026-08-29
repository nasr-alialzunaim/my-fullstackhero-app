using FSH.Modules.Catalog.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Catalog;

[DbContext(typeof(CatalogDbContext))]
[Migration("20260829190001_RemoveCatalogTenantIsolation")]
public sealed class RemoveCatalogTenantIsolation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        DO $$
        BEGIN
            IF EXISTS (
                SELECT 1 FROM catalog."Brands" WHERE "TenantId" NOT IN ('', 'root')
                UNION ALL SELECT 1 FROM catalog."Categories" WHERE "TenantId" NOT IN ('', 'root')
                UNION ALL SELECT 1 FROM catalog."Products" WHERE "TenantId" NOT IN ('', 'root')
                UNION ALL SELECT 1 FROM catalog."ProductImages" WHERE "TenantId" NOT IN ('', 'root')
            ) THEN
                RAISE EXCEPTION 'Single-tenant migration refused: catalog contains non-root tenant data.';
            END IF;
        END $$;

        ALTER TABLE catalog."Brands" DROP COLUMN "TenantId";
        ALTER TABLE catalog."Categories" DROP COLUMN "TenantId";
        ALTER TABLE catalog."Products" DROP COLUMN "TenantId";
        ALTER TABLE catalog."ProductImages" DROP COLUMN "TenantId";

        CREATE UNIQUE INDEX "IX_Brands_Slug" ON catalog."Brands" ("Slug") WHERE "IsDeleted" = FALSE;
        CREATE UNIQUE INDEX "IX_Categories_Slug" ON catalog."Categories" ("Slug") WHERE "IsDeleted" = FALSE;
        CREATE UNIQUE INDEX "IX_Products_Sku" ON catalog."Products" ("Sku") WHERE "IsDeleted" = FALSE;
        CREATE UNIQUE INDEX "IX_Products_Slug" ON catalog."Products" ("Slug") WHERE "IsDeleted" = FALSE;
        """);

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        DROP INDEX IF EXISTS catalog."IX_Brands_Slug";
        DROP INDEX IF EXISTS catalog."IX_Categories_Slug";
        DROP INDEX IF EXISTS catalog."IX_Products_Sku";
        DROP INDEX IF EXISTS catalog."IX_Products_Slug";

        ALTER TABLE catalog."Brands" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE catalog."Categories" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE catalog."Products" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE catalog."ProductImages" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';

        CREATE UNIQUE INDEX "IX_Brands_Slug" ON catalog."Brands" ("Slug", "TenantId") WHERE "IsDeleted" = FALSE;
        CREATE UNIQUE INDEX "IX_Categories_Slug" ON catalog."Categories" ("Slug", "TenantId") WHERE "IsDeleted" = FALSE;
        CREATE UNIQUE INDEX "IX_Products_Sku" ON catalog."Products" ("Sku", "TenantId") WHERE "IsDeleted" = FALSE;
        CREATE UNIQUE INDEX "IX_Products_Slug" ON catalog."Products" ("Slug", "TenantId") WHERE "IsDeleted" = FALSE;
        """);
}
