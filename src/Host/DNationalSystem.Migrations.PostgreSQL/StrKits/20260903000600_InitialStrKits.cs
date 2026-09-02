using FSH.Modules.StrKits.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.StrKits;

[DbContext(typeof(StrKitsDbContext))]
[Migration("20260903000600_InitialStrKits")]
public sealed class InitialStrKits : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "str_kits");
        migrationBuilder.CreateTable(
            name: "StrKits", schema: "str_kits",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                KitCode = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                AnalysisTypeId = table.Column<int>(type: "integer", nullable: false),
                RepresentativeParameter = table.Column<int>(type: "integer", nullable: false),
                VersionNumber = table.Column<int>(type: "integer", nullable: false),
                SupersedesKitId = table.Column<Guid>(type: "uuid", nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            },
            constraints: table => table.PrimaryKey("PK_StrKits", x => x.Id));

        migrationBuilder.CreateTable(
            name: "StrKitAliases", schema: "str_kits",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                StrKitId = table.Column<Guid>(type: "uuid", nullable: false),
                Alias = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StrKitAliases", x => x.Id);
                table.ForeignKey("FK_StrKitAliases_StrKits_StrKitId", x => x.StrKitId, "str_kits", "StrKits", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "StrKitLoci", schema: "str_kits",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                StrKitId = table.Column<Guid>(type: "uuid", nullable: false),
                Marker = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Chromosome = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                MinimumAllelesQty = table.Column<int>(type: "integer", nullable: false),
                MaximumAllelesQty = table.Column<int>(type: "integer", nullable: false),
                Fluorophore = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                Order = table.Column<int>(type: "integer", nullable: false),
                Required = table.Column<bool>(type: "boolean", nullable: false),
                AlleleRangeMin = table.Column<double>(type: "double precision", nullable: true),
                AlleleRangeMax = table.Column<double>(type: "double precision", nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_StrKitLoci", x => x.Id);
                table.ForeignKey("FK_StrKitLoci_StrKits_StrKitId", x => x.StrKitId, "str_kits", "StrKits", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex("IX_StrKits_KitCode", "str_kits", "StrKits", "KitCode");
        migrationBuilder.CreateIndex("IX_StrKits_SupersedesKitId", "str_kits", "StrKits", "SupersedesKitId");
        migrationBuilder.CreateIndex("IX_StrKitAliases_StrKitId_Alias", "str_kits", "StrKitAliases", new[] { "StrKitId", "Alias" }, unique: true);
        migrationBuilder.CreateIndex("IX_StrKitLoci_StrKitId_Marker", "str_kits", "StrKitLoci", new[] { "StrKitId", "Marker" }, unique: true);
        migrationBuilder.CreateIndex("IX_StrKitLoci_StrKitId_Order", "str_kits", "StrKitLoci", new[] { "StrKitId", "Order" }, unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("StrKitAliases", "str_kits");
        migrationBuilder.DropTable("StrKitLoci", "str_kits");
        migrationBuilder.DropTable("StrKits", "str_kits");
    }
}
