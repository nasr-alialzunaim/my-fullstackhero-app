using FSH.Modules.Tickets.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Tickets;

[DbContext(typeof(TicketsDbContext))]
[Migration("20260829190006_RemoveTicketsTenantIsolation")]
public sealed class RemoveTicketsTenantIsolation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM tickets."Tickets" WHERE "TenantId" NOT IN ('', 'root')
                    UNION ALL SELECT 1 FROM tickets."TicketComments" WHERE "TenantId" NOT IN ('', 'root')
                ) THEN
                    RAISE EXCEPTION 'Single-tenant migration refused: tickets contains non-root tenant data.';
                END IF;
            END $$;
            """);

        migrationBuilder.DropColumn("TenantId", "tickets", "Tickets");
        migrationBuilder.DropColumn("TenantId", "tickets", "TicketComments");
        migrationBuilder.CreateIndex("IX_Tickets_Number", "tickets", "Tickets", "Number", unique: true, filter: "\\\"IsDeleted\\\" = FALSE");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex("IX_Tickets_Number", "tickets", "Tickets");
        migrationBuilder.AddColumn<string>("TenantId", "tickets", "Tickets", "text", nullable: false, defaultValue: "root");
        migrationBuilder.AddColumn<string>("TenantId", "tickets", "TicketComments", "text", nullable: false, defaultValue: "root");
        migrationBuilder.CreateIndex("IX_Tickets_Number", "tickets", "Tickets", new[] { "Number", "TenantId" }, unique: true, filter: "\\\"IsDeleted\\\" = FALSE");
    }
}
