using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KeyExchanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    KeyFingerprint = table.Column<string>(type: "TEXT", nullable: false),
                    KeyPEM = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyExchanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DecryptedRecipientId = table.Column<string>(type: "TEXT", nullable: false),
                    KeyFingerprint = table.Column<string>(type: "TEXT", nullable: false),
                    SenderId = table.Column<byte[]>(type: "BLOB", nullable: false),
                    RecipientId = table.Column<byte[]>(type: "BLOB", nullable: false),
                    MessageText = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Sent = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SenderFingerprint = table.Column<string>(type: "TEXT", nullable: true),
                    SenderText = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Modified = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PreferredName = table.Column<string>(type: "TEXT", nullable: true),
                    ExchangePem = table.Column<string>(type: "TEXT", nullable: false),
                    ExchangeFingerprint = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeyExchanges");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
