using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentsAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class v001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pendente"),
                    DataCriacao = table.Column<DateTimeOffset>(type: "datetimeoffset(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentItem_Payment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_DataCriacao",
                table: "Payment",
                column: "DataCriacao");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_Status",
                table: "Payment",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_UsuarioId",
                table: "Payment",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentItem_GameId",
                table: "PaymentItem",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentItem_PaymentId",
                table: "PaymentItem",
                column: "PaymentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentItem");

            migrationBuilder.DropTable(
                name: "Payment");
        }
    }
}
