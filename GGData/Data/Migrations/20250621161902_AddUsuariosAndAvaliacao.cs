using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGData.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuariosAndAvaliacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacao_Usuarios_UsuariosID",
                table: "Avaliacao");

            migrationBuilder.DropIndex(
                name: "IX_Avaliacao_UsuariosID",
                table: "Avaliacao");

            migrationBuilder.AlterColumn<string>(
                name: "Plataforma",
                table: "Jogo",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Jogo",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Genero",
                table: "Jogo",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UsuariosID",
                table: "Avaliacao",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Avaliacao",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacao_UsuarioId",
                table: "Avaliacao",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacao_Usuarios_UsuarioId",
                table: "Avaliacao",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacao_Usuarios_UsuarioId",
                table: "Avaliacao");

            migrationBuilder.DropIndex(
                name: "IX_Avaliacao_UsuarioId",
                table: "Avaliacao");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Avaliacao");

            migrationBuilder.AlterColumn<string>(
                name: "Plataforma",
                table: "Jogo",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Jogo",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Genero",
                table: "Jogo",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "UsuariosID",
                table: "Avaliacao",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacao_UsuariosID",
                table: "Avaliacao",
                column: "UsuariosID");

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacao_Usuarios_UsuariosID",
                table: "Avaliacao",
                column: "UsuariosID",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
