using FSH.Modules.Notifications.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Notifications;

[DbContext(typeof(NotificationsDbContext))]
[Migration("20260829190005_RemoveNotificationsTenantIsolation")]
public sealed class RemoveNotificationsTenantIsolation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DO $$
            BEGIN
                IF EXISTS (SELECT 1 FROM notifications."Notifications" WHERE "TenantId" NOT IN ('', 'root')) THEN
                    RAISE EXCEPTION 'Single-tenant migration refused: notifications contains non-root tenant data.';
                END IF;
            END $$;
            """);
        migrationBuilder.DropColumn("TenantId", "notifications", "Notifications");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>("TenantId", "notifications", "Notifications", "text", nullable: false, defaultValue: "root");
    }
}
