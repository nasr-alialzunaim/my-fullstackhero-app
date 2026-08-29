using FSH.Modules.Catalog.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Catalog;

[DbContext(typeof(CatalogDbContext))]
[Migration("20260829190001_RemoveCatalogTenantIsolation")]
public sealed class RemoveCatalogTenantIsolation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
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
            """);

        migrationBuilder.DropColumn("TenantId", "catalog", "Brands");
        migrationBuilder.DropColumn("TenantId", "catalog", "Categories");
        migrationBuilder.DropColumn("TenantId", "catalog", "Products");
        migrationBuilder.DropColumn("TenantId", "catalog", "ProductImages");

        migrationBuilder.CreateIndex("IX_Brands_Slug", "catalog", "Brands", "Slug", unique: true, filter: ""IsDeleted" = FALSE");
        migrationBuilder.CreateIndex("IX_Categories_Slug", "catalog", "Categories", "Slug", unique: true, filter: ""IsDeleted" = FALSE");
        migrationBuilder.CreateIndex("IX_Products_Sku", "catalog", "Products", "Sku", unique: true, filter: ""IsDeleted" = FALSE");
        migrationBuilder.CreateIndex("IX_Products_Slug", "catalog", "Products", "Slug", unique: true, filter: ""IsDeleted" = FALSE");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex("IX_Brands_Slug", "catalog", "Brands");
        migrationBuilder.DropIndex("IX_Categories_Slug", "catalog", "Categories");
        migrationBuilder.DropIndex("IX_Products_Sku", "catalog", "Products");
        migrationBuilder.DropIndex("IX_Products_Slug", "catalog", "Products");

        migrationBuilder.AddColumn<string>("TenantId", "catalog", "Brands", "text", nullable: false, defaultValue: "root");
        migrationBuilder.AddColumn<string>("TenantId", "catalog", "Categories", "text", nullable: false, defaultValue: "root");
        migrationBuilder.AddColumn<string>("TenantId", "catalog", "Products", "text", nullable: false, defaultValue: "root");
        migrationBuilder.AddColumn<string>("TenantId", "catalog", "ProductImages", "text", nullable: false, defaultValue: "root");

        migrationBuilder.CreateIndex("IX_Brands_Slug", "catalog", "Brands", new[] { "Slug", "TenantId" }, unique: true, filter: ""IsDeleted" = FALSE");
        migrationBuilder.CreateIndex("IX_Categories_Slug", "catalog", "Categories", new[] { "Slug", "TenantId" }, unique: true, filter: ""IsDeleted" = FALSE");
        migrationBuilder.CreateIndex("IX_Products_Sku", "catalog", "Products", new[] { "Sku", "TenantId" }, unique: true, filter: ""IsDeleted" = FALSE");
        migrationBuilder.CreateIndex("IX_Products_Slug", "catalog", "Products", new[] { "Slug", "TenantId" }, unique: true, filter: ""IsDeleted" = FALSE");
    }
}
