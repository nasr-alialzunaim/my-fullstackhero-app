using FSH.Modules.Webhooks.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Webhooks;

[DbContext(typeof(WebhookDbContext))]
[Migration("20260829190007_RemoveWebhooksTenantIsolation")]
public sealed class RemoveWebhooksTenantIsolation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        DO $$
        BEGIN
            IF EXISTS (
                SELECT 1 FROM webhooks."Deliveries" WHERE "TenantId" NOT IN ('', 'root')
                UNION ALL SELECT 1 FROM webhooks."Subscriptions" WHERE "TenantId" NOT IN ('', 'root')
            ) THEN
                RAISE EXCEPTION 'Single-tenant migration refused: webhooks contains non-root tenant data.';
            END IF;
        END $$;

        ALTER TABLE webhooks."Deliveries" DROP COLUMN "TenantId";
        ALTER TABLE webhooks."Subscriptions" DROP COLUMN "TenantId";
        """);

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        ALTER TABLE webhooks."Deliveries" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE webhooks."Subscriptions" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        """);
}
