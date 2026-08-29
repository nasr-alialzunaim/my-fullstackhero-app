using FSH.Modules.Chat.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNationalSystem.Migrations.PostgreSQL.Chat;

[DbContext(typeof(ChatDbContext))]
[Migration("20260829190002_RemoveChatTenantIsolation")]
public sealed class RemoveChatTenantIsolation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
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
            """);

        migrationBuilder.DropColumn("TenantId", "chat", "ChannelMembers");
        migrationBuilder.DropColumn("TenantId", "chat", "Channels");
        migrationBuilder.DropColumn("TenantId", "chat", "Messages");
        migrationBuilder.DropColumn("TenantId", "chat", "MessageAttachments");
        migrationBuilder.DropColumn("TenantId", "chat", "MessageMentions");
        migrationBuilder.DropColumn("TenantId", "chat", "MessageReactions");

        migrationBuilder.CreateIndex("IX_ChannelMembers_UserId_ChannelId", "chat", "ChannelMembers", new[] { "UserId", "ChannelId" }, unique: true);
        migrationBuilder.CreateIndex("IX_Channels_DirectKey", "chat", "Channels", "DirectKey", unique: true, filter: "\"Type\" = 0 AND \"IsDeleted\" = FALSE");
        migrationBuilder.CreateIndex("IX_Channels_Slug", "chat", "Channels", "Slug", unique: true, filter: "\"Slug\" IS NOT NULL AND \"IsDeleted\" = FALSE");
        migrationBuilder.CreateIndex("UX_MessageReactions_Message_User_Emoji", "chat", "MessageReactions", new[] { "MessageId", "UserId", "Emoji" }, unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex("IX_ChannelMembers_UserId_ChannelId", "chat", "ChannelMembers");
        migrationBuilder.DropIndex("IX_Channels_DirectKey", "chat", "Channels");
        migrationBuilder.DropIndex("IX_Channels_Slug", "chat", "Channels");
        migrationBuilder.DropIndex("UX_MessageReactions_Message_User_Emoji", "chat", "MessageReactions");

        foreach (var table in new[] { "ChannelMembers", "Channels", "Messages", "MessageAttachments", "MessageMentions", "MessageReactions" })
        {
            migrationBuilder.AddColumn<string>("TenantId", "chat", table, "text", nullable: false, defaultValue: "root");
        }

        migrationBuilder.CreateIndex("IX_ChannelMembers_UserId_ChannelId", "chat", "ChannelMembers", new[] { "UserId", "ChannelId", "TenantId" }, unique: true);
        migrationBuilder.CreateIndex("IX_Channels_DirectKey", "chat", "Channels", new[] { "DirectKey", "TenantId" }, unique: true, filter: "\"Type\" = 0 AND \"IsDeleted\" = FALSE");
        migrationBuilder.CreateIndex("IX_Channels_Slug", "chat", "Channels", new[] { "Slug", "TenantId" }, unique: true, filter: "\"Slug\" IS NOT NULL AND \"IsDeleted\" = FALSE");
        migrationBuilder.CreateIndex("UX_MessageReactions_Message_User_Emoji", "chat", "MessageReactions", new[] { "MessageId", "UserId", "Emoji", "TenantId" }, unique: true);
    }
}
