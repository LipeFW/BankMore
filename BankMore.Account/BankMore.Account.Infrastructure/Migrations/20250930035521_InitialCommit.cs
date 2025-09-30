using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMore.Account.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CONTACORRENTE",
                columns: table => new
                {
                    IdContaCorrente = table.Column<byte[]>(type: "RAW(16)", nullable: false),
                    Cpf = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Senha = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Numero = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Ativo = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONTACORRENTE", x => x.IdContaCorrente);
                });

            migrationBuilder.CreateTable(
                name: "IDEMPOTENCIA",
                columns: table => new
                {
                    Chave_Idempotencia = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Requisicao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Resultado = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IDEMPOTENCIA", x => x.Chave_Idempotencia);
                });

            migrationBuilder.CreateTable(
                name: "TRANSFERENCIA",
                columns: table => new
                {
                    IdTransferencia = table.Column<byte[]>(type: "RAW(16)", nullable: false),
                    IdContaCorrenteOrigem = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdContaCorrenteDestino = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DataMovimento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Valor = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRANSFERENCIA", x => x.IdTransferencia);
                });

            migrationBuilder.CreateTable(
                name: "MOVIMENTO",
                columns: table => new
                {
                    IdMovimento = table.Column<byte[]>(type: "RAW(16)", nullable: false),
                    IdContaCorrente = table.Column<byte[]>(type: "RAW(16)", nullable: false),
                    DataMovimento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TipoMovimento = table.Column<string>(type: "NVARCHAR2(1)", nullable: false),
                    Valor = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MOVIMENTO", x => x.IdMovimento);
                    table.ForeignKey(
                        name: "FK_MOVIMENTO_CONTACORRENTE_IdContaCorrente",
                        column: x => x.IdContaCorrente,
                        principalTable: "CONTACORRENTE",
                        principalColumn: "IdContaCorrente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TARIFA",
                columns: table => new
                {
                    IdTarifa = table.Column<byte[]>(type: "RAW(16)", nullable: false),
                    IdContaCorrente = table.Column<byte[]>(type: "RAW(16)", nullable: false),
                    DataMovimento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Valor = table.Column<decimal>(type: "DECIMAL(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TARIFA", x => x.IdTarifa);
                    table.ForeignKey(
                        name: "FK_TARIFA_CONTACORRENTE_IdContaCorrente",
                        column: x => x.IdContaCorrente,
                        principalTable: "CONTACORRENTE",
                        principalColumn: "IdContaCorrente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MOVIMENTO_IdContaCorrente",
                table: "MOVIMENTO",
                column: "IdContaCorrente");

            migrationBuilder.CreateIndex(
                name: "IX_TARIFA_IdContaCorrente",
                table: "TARIFA",
                column: "IdContaCorrente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IDEMPOTENCIA");

            migrationBuilder.DropTable(
                name: "MOVIMENTO");

            migrationBuilder.DropTable(
                name: "TARIFA");

            migrationBuilder.DropTable(
                name: "TRANSFERENCIA");

            migrationBuilder.DropTable(
                name: "CONTACORRENTE");
        }
    }
}
