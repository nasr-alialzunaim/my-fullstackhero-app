using FSH.Modules.Tickets.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Tickets;

[DbContext(typeof(TicketsDbContext))]
[Migration("20260829190006_RemoveTicketsTenantIsolation")]
public sealed class RemoveTicketsTenantIsolation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        DO $$
        BEGIN
            IF EXISTS (
                SELECT 1 FROM tickets."Tickets" WHERE "TenantId" NOT IN ('', 'root')
                UNION ALL SELECT 1 FROM tickets."TicketComments" WHERE "TenantId" NOT IN ('', 'root')
            ) THEN
                RAISE EXCEPTION 'Single-tenant migration refused: tickets contains non-root tenant data.';
            END IF;
        END $$;

        ALTER TABLE tickets."Tickets" DROP COLUMN "TenantId";
        ALTER TABLE tickets."TicketComments" DROP COLUMN "TenantId";
        CREATE UNIQUE INDEX "IX_Tickets_Number" ON tickets."Tickets" ("Number") WHERE "IsDeleted" = FALSE;
        """);

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        DROP INDEX IF EXISTS tickets."IX_Tickets_Number";
        ALTER TABLE tickets."Tickets" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE tickets."TicketComments" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        CREATE UNIQUE INDEX "IX_Tickets_Number" ON tickets."Tickets" ("Number", "TenantId") WHERE "IsDeleted" = FALSE;
        """);
}
