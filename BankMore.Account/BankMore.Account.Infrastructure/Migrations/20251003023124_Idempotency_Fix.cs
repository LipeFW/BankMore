using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMore.Account.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Idempotency_Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MOVIMENTO_RequestId",
                table: "MOVIMENTO");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "MOVIMENTO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestId",
                table: "MOVIMENTO",
                type: "NVARCHAR2(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MOVIMENTO_RequestId",
                table: "MOVIMENTO",
                column: "RequestId",
                unique: true);
        }
    }
}
