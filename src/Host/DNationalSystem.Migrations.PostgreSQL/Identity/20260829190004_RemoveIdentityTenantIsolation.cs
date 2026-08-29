using FSH.Modules.Identity.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Identity;

[DbContext(typeof(IdentityDbContext))]
[Migration("20260829190004_RemoveIdentityTenantIsolation")]
public sealed class RemoveIdentityTenantIsolation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
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

        ALTER TABLE identity."Roles" DROP COLUMN "TenantId";
        ALTER TABLE identity."RoleClaims" DROP COLUMN "TenantId";
        ALTER TABLE identity."Users" DROP COLUMN "TenantId";
        ALTER TABLE identity."Groups" DROP COLUMN "TenantId";
        ALTER TABLE identity."GroupRoles" DROP COLUMN "TenantId";
        ALTER TABLE identity."PasswordHistory" DROP COLUMN "TenantId";
        ALTER TABLE identity."UserGroups" DROP COLUMN "TenantId";
        ALTER TABLE identity."UserSessions" DROP COLUMN "TenantId";
        ALTER TABLE identity."UserClaims" DROP COLUMN "TenantId";
        ALTER TABLE identity."UserLogins" DROP COLUMN "TenantId";
        ALTER TABLE identity."UserRoles" DROP COLUMN "TenantId";
        ALTER TABLE identity."UserTokens" DROP COLUMN "TenantId";

        CREATE UNIQUE INDEX "RoleNameIndex" ON identity."Roles" ("NormalizedName");
        CREATE UNIQUE INDEX "UserNameIndex" ON identity."Users" ("NormalizedUserName");
        """);

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        DROP INDEX IF EXISTS identity."RoleNameIndex";
        DROP INDEX IF EXISTS identity."UserNameIndex";

        ALTER TABLE identity."Roles" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE identity."RoleClaims" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE identity."Users" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE identity."Groups" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE identity."GroupRoles" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE identity."PasswordHistory" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE identity."UserGroups" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE identity."UserSessions" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE identity."UserClaims" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE identity."UserLogins" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE identity."UserRoles" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE identity."UserTokens" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';

        CREATE UNIQUE INDEX "RoleNameIndex" ON identity."Roles" ("NormalizedName", "TenantId");
        CREATE UNIQUE INDEX "UserNameIndex" ON identity."Users" ("NormalizedUserName", "TenantId");
        """);
}
