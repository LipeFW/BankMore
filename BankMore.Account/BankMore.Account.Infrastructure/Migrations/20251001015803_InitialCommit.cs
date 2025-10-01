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
                    IdContaCorrente = table.Column<string>(type: "NVARCHAR2(37)", nullable: false),
                    Numero = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Cpf = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Ativo = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Senha = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Salt = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONTACORRENTE", x => x.IdContaCorrente);
                });

            migrationBuilder.CreateTable(
                name: "IDEMPOTENCIA",
                columns: table => new
                {
                    Chave_Idempotencia = table.Column<string>(type: "NVARCHAR2(37)", nullable: false),
                    Requisicao = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    Resultado = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IDEMPOTENCIA", x => x.Chave_Idempotencia);
                });

            migrationBuilder.CreateTable(
                name: "MOVIMENTO",
                columns: table => new
                {
                    IdMovimento = table.Column<string>(type: "NVARCHAR2(37)", nullable: false),
                    IdContaCorrente = table.Column<string>(type: "NVARCHAR2(37)", nullable: false),
                    DataMovimento = table.Column<DateTime>(type: "TIMESTAMP(7)", maxLength: 25, nullable: false),
                    TipoMovimento = table.Column<string>(type: "NVARCHAR2(1)", maxLength: 1, nullable: false),
                    Valor = table.Column<decimal>(type: "NUMBER(12,2)", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_CONTACORRENTE_Numero",
                table: "CONTACORRENTE",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MOVIMENTO_IdContaCorrente",
                table: "MOVIMENTO",
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
                name: "CONTACORRENTE");
        }
    }
}
