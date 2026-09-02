using FSH.Modules.FrequencyTables.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.FrequencyTables;

[DbContext(typeof(FrequencyTablesDbContext))]
[Migration("20260903000700_InitialFrequencyTables")]
public sealed class InitialFrequencyTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "frequency_tables");

        migrationBuilder.CreateTable(
            name: "FrequencyTables", schema: "frequency_tables",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Model = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Theta = table.Column<double>(type: "double precision", nullable: false),
                VersionNumber = table.Column<int>(type: "integer", nullable: false),
                SupersedesTableId = table.Column<Guid>(type: "uuid", nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            },
            constraints: table => table.PrimaryKey("PK_FrequencyTables", x => x.Id));

        migrationBuilder.CreateTable(
            name: "FrequencyEntries", schema: "frequency_tables",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                FrequencyTableId = table.Column<Guid>(type: "uuid", nullable: false),
                Marker = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Allele = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Frequency = table.Column<double>(type: "double precision", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FrequencyEntries", x => x.Id);
                table.ForeignKey("FK_FrequencyEntries_FrequencyTables_FrequencyTableId", x => x.FrequencyTableId, "frequency_tables", "FrequencyTables", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex("IX_FrequencyTables_Name", "frequency_tables", "FrequencyTables", "Name");
        migrationBuilder.CreateIndex("IX_FrequencyTables_SupersedesTableId", "frequency_tables", "FrequencyTables", "SupersedesTableId");
        migrationBuilder.CreateIndex("IX_FrequencyTables_IsDefault", "frequency_tables", "FrequencyTables", "IsDefault");
        migrationBuilder.CreateIndex("IX_FrequencyEntries_FrequencyTableId_Marker", "frequency_tables", "FrequencyEntries", new[] { "FrequencyTableId", "Marker" });
        migrationBuilder.CreateIndex("IX_FrequencyEntries_FrequencyTableId_Marker_Allele", "frequency_tables", "FrequencyEntries", new[] { "FrequencyTableId", "Marker", "Allele" }, unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("FrequencyEntries", "frequency_tables");
        migrationBuilder.DropTable("FrequencyTables", "frequency_tables");
    }
}
