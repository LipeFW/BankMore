using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMore.Transfer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TRANSFERENCIA",
                columns: table => new
                {
                    IdTransferencia = table.Column<string>(type: "NVARCHAR2(37)", nullable: false),
                    IdContaCorrenteOrigem = table.Column<string>(type: "NVARCHAR2(37)", nullable: false),
                    IdContaCorrenteDestino = table.Column<string>(type: "NVARCHAR2(37)", nullable: false),
                    DataMovimento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Valor = table.Column<decimal>(type: "NUMBER(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRANSFERENCIA", x => x.IdTransferencia);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TRANSFERENCIA");
        }
    }
}
