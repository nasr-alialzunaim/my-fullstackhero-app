using FSH.Modules.Notifications.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Notifications;

[DbContext(typeof(NotificationsDbContext))]
[Migration("20260829190005_RemoveNotificationsTenantIsolation")]
public sealed class RemoveNotificationsTenantIsolation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        DO $$
        BEGIN
            IF EXISTS (SELECT 1 FROM notifications."Notifications" WHERE "TenantId" NOT IN ('', 'root')) THEN
                RAISE EXCEPTION 'Single-tenant migration refused: notifications contains non-root tenant data.';
            END IF;
        END $$;

        ALTER TABLE notifications."Notifications" DROP COLUMN "TenantId";
        """);

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        ALTER TABLE notifications."Notifications" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        """);
}
