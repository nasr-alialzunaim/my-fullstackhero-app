using FSH.Modules.Chat.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Chat;

[DbContext(typeof(ChatDbContext))]
[Migration("20260829190002_RemoveChatTenantIsolation")]
public sealed class RemoveChatTenantIsolation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        DO $$
        BEGIN
            IF EXISTS (
                SELECT 1 FROM chat."ChannelMembers" WHERE "TenantId" NOT IN ('', 'root')
                UNION ALL SELECT 1 FROM chat."Channels" WHERE "TenantId" NOT IN ('', 'root')
                UNION ALL SELECT 1 FROM chat."Messages" WHERE "TenantId" NOT IN ('', 'root')
                UNION ALL SELECT 1 FROM chat."MessageAttachments" WHERE "TenantId" NOT IN ('', 'root')
                UNION ALL SELECT 1 FROM chat."MessageMentions" WHERE "TenantId" NOT IN ('', 'root')
                UNION ALL SELECT 1 FROM chat."MessageReactions" WHERE "TenantId" NOT IN ('', 'root')
            ) THEN
                RAISE EXCEPTION 'Single-tenant migration refused: chat contains non-root tenant data.';
            END IF;
        END $$;

        ALTER TABLE chat."ChannelMembers" DROP COLUMN "TenantId";
        ALTER TABLE chat."Channels" DROP COLUMN "TenantId";
        ALTER TABLE chat."Messages" DROP COLUMN "TenantId";
        ALTER TABLE chat."MessageAttachments" DROP COLUMN "TenantId";
        ALTER TABLE chat."MessageMentions" DROP COLUMN "TenantId";
        ALTER TABLE chat."MessageReactions" DROP COLUMN "TenantId";

        CREATE UNIQUE INDEX "IX_ChannelMembers_UserId_ChannelId" ON chat."ChannelMembers" ("UserId", "ChannelId");
        CREATE UNIQUE INDEX "IX_Channels_DirectKey" ON chat."Channels" ("DirectKey") WHERE "Type" = 0 AND "IsDeleted" = FALSE;
        CREATE UNIQUE INDEX "IX_Channels_Slug" ON chat."Channels" ("Slug") WHERE "Slug" IS NOT NULL AND "IsDeleted" = FALSE;
        CREATE UNIQUE INDEX "UX_MessageReactions_Message_User_Emoji" ON chat."MessageReactions" ("MessageId", "UserId", "Emoji");
        """);

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.Sql("""
        DROP INDEX IF EXISTS chat."IX_ChannelMembers_UserId_ChannelId";
        DROP INDEX IF EXISTS chat."IX_Channels_DirectKey";
        DROP INDEX IF EXISTS chat."IX_Channels_Slug";
        DROP INDEX IF EXISTS chat."UX_MessageReactions_Message_User_Emoji";

        ALTER TABLE chat."ChannelMembers" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE chat."Channels" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE chat."Messages" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE chat."MessageAttachments" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE chat."MessageMentions" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';
        ALTER TABLE chat."MessageReactions" ADD COLUMN "TenantId" text NOT NULL DEFAULT 'root';

        CREATE UNIQUE INDEX "IX_ChannelMembers_UserId_ChannelId" ON chat."ChannelMembers" ("UserId", "ChannelId", "TenantId");
        CREATE UNIQUE INDEX "IX_Channels_DirectKey" ON chat."Channels" ("DirectKey", "TenantId") WHERE "Type" = 0 AND "IsDeleted" = FALSE;
        CREATE UNIQUE INDEX "IX_Channels_Slug" ON chat."Channels" ("Slug", "TenantId") WHERE "Slug" IS NOT NULL AND "IsDeleted" = FALSE;
        CREATE UNIQUE INDEX "UX_MessageReactions_Message_User_Emoji" ON chat."MessageReactions" ("MessageId", "UserId", "Emoji", "TenantId");
        """);
}
