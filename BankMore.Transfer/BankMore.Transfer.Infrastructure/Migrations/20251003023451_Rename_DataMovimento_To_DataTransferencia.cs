using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMore.Transfer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Rename_DataMovimento_To_DataTransferencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataMovimento",
                table: "TRANSFERENCIA",
                newName: "DataTransferencia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataTransferencia",
                table: "TRANSFERENCIA",
                newName: "DataMovimento");
        }
    }
}
