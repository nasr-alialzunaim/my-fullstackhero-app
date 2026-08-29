using FSH.Modules.Identity.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Identity;

[DbContext(typeof(IdentityDbContext))]
[Migration("20260829190004_RemoveIdentityTenantIsolation")]
public sealed class RemoveIdentityTenantIsolation : Migration
{
    private static readonly string[] TenantTables =
    [
        "Roles", "RoleClaims", "Users", "Groups", "GroupRoles", "PasswordHistory",
        "UserGroups", "UserSessions", "UserClaims", "UserLogins", "UserRoles", "UserTokens"
    ];

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1 FROM identity."Roles" WHERE "TenantId" NOT IN ('', 'root')
                    UNION ALL SELECT 1 FROM identity."RoleClaims" WHERE "TenantId" NOT IN ('', 'root')
                    UNION ALL SELECT 1 FROM identity."Users" WHERE "TenantId" NOT IN ('', 'root')
                    UNION ALL SELECT 1 FROM identity."Groups" WHERE "TenantId" NOT IN ('', 'root')
                    UNION ALL SELECT 1 FROM identity."GroupRoles" WHERE "TenantId" NOT IN ('', 'root')
                    UNION ALL SELECT 1 FROM identity."PasswordHistory" WHERE "TenantId" NOT IN ('', 'root')
                    UNION ALL SELECT 1 FROM identity."UserGroups" WHERE "TenantId" NOT IN ('', 'root')
                    UNION ALL SELECT 1 FROM identity."UserSessions" WHERE "TenantId" NOT IN ('', 'root')
                    UNION ALL SELECT 1 FROM identity."UserClaims" WHERE "TenantId" NOT IN ('', 'root')
                    UNION ALL SELECT 1 FROM identity."UserLogins" WHERE "TenantId" NOT IN ('', 'root')
                    UNION ALL SELECT 1 FROM identity."UserRoles" WHERE "TenantId" NOT IN ('', 'root')
                    UNION ALL SELECT 1 FROM identity."UserTokens" WHERE "TenantId" NOT IN ('', 'root')
                ) THEN
                    RAISE EXCEPTION 'Single-tenant migration refused: identity contains non-root tenant data.';
                END IF;
            END $$;
            """);

        foreach (var table in TenantTables)
        {
            migrationBuilder.DropColumn("TenantId", "identity", table);
        }

        migrationBuilder.CreateIndex("RoleNameIndex", "identity", "Roles", "NormalizedName", unique: true);
        migrationBuilder.CreateIndex("UserNameIndex", "identity", "Users", "NormalizedUserName", unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex("RoleNameIndex", "identity", "Roles");
        migrationBuilder.DropIndex("UserNameIndex", "identity", "Users");

        foreach (var table in TenantTables)
        {
            migrationBuilder.AddColumn<string>("TenantId", "identity", table, "text", nullable: false, defaultValue: "root");
        }

        migrationBuilder.CreateIndex("RoleNameIndex", "identity", "Roles", new[] { "NormalizedName", "TenantId" }, unique: true);
        migrationBuilder.CreateIndex("UserNameIndex", "identity", "Users", new[] { "NormalizedUserName", "TenantId" }, unique: true);
    }
}
