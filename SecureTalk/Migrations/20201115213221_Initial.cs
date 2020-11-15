using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SecureTalk.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ExchangeFingerprint = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DecryptedSenderId = table.Column<string>(type: "TEXT", nullable: true),
                    DecryptedRecipientId = table.Column<string>(type: "TEXT", nullable: true),
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
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    PEMPath = table.Column<string>(type: "TEXT", nullable: true),
                    PreferredName = table.Column<string>(type: "TEXT", nullable: true),
                    ExchangePem = table.Column<string>(type: "TEXT", nullable: false),
                    ExchangeFingerprint = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageKeys",
                columns: table => new
                {
                    Fingerprint = table.Column<string>(type: "TEXT", nullable: false),
                    IsExchange = table.Column<bool>(type: "INTEGER", nullable: false),
                    AssignedContactId = table.Column<string>(type: "TEXT", nullable: true),
                    PrivatePem = table.Column<string>(type: "TEXT", nullable: true),
                    PublicPem = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageKeys", x => x.Fingerprint);
                    table.ForeignKey(
                        name: "FK_MessageKeys_Contacts_AssignedContactId",
                        column: x => x.AssignedContactId,
                        principalTable: "Contacts",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageKeys_AssignedContactId",
                table: "MessageKeys",
                column: "AssignedContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_DecryptedSenderId",
                table: "Messages",
                column: "DecryptedSenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageKeys");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Contacts");
        }
    }
}
