using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGData.Migrations
{
    /// <inheritdoc />
    public partial class CorrecaoUtilizadores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacao_AspNetUsers_UsuarioId",
                table: "Avaliacao");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Avaliacao",
                newName: "UtilizadorId");

            migrationBuilder.RenameIndex(
                name: "IX_Avaliacao_UsuarioId",
                table: "Avaliacao",
                newName: "IX_Avaliacao_UtilizadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacao_AspNetUsers_UtilizadorId",
                table: "Avaliacao",
                column: "UtilizadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacao_AspNetUsers_UtilizadorId",
                table: "Avaliacao");

            migrationBuilder.RenameColumn(
                name: "UtilizadorId",
                table: "Avaliacao",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_Avaliacao_UtilizadorId",
                table: "Avaliacao",
                newName: "IX_Avaliacao_UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacao_AspNetUsers_UsuarioId",
                table: "Avaliacao",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
