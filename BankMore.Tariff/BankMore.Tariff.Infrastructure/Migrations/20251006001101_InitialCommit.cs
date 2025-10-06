using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMore.Tariff.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TARIFA",
                columns: table => new
                {
                    IdTarifa = table.Column<string>(type: "NVARCHAR2(37)", nullable: false),
                    IdContaCorrente = table.Column<string>(type: "NVARCHAR2(37)", nullable: false),
                    Valor = table.Column<decimal>(type: "NUMBER(12,2)", nullable: false),
                    DataTarifacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TARIFA", x => x.IdTarifa);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TARIFA");
        }
    }
}
