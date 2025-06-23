using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGData.Data.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirFKsParaNovosNomes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacao_Jogo_JogoID",
                table: "Avaliacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Estatistica_Jogo_JogoID",
                table: "Estatistica");

            migrationBuilder.DropIndex(
                name: "IX_Estatistica_JogoID",
                table: "Estatistica");

            migrationBuilder.DropColumn(
                name: "UsuariosID",
                table: "Avaliacao");

            migrationBuilder.RenameColumn(
                name: "tipoUsuario",
                table: "Usuarios",
                newName: "TipoUsuario");

            migrationBuilder.RenameColumn(
                name: "JogoID",
                table: "Estatistica",
                newName: "JogoId");

            migrationBuilder.RenameColumn(
                name: "JogoID",
                table: "Avaliacao",
                newName: "JogoId");

            migrationBuilder.RenameIndex(
                name: "IX_Avaliacao_JogoID",
                table: "Avaliacao",
                newName: "IX_Avaliacao_JogoId");

            migrationBuilder.AlterColumn<string>(
                name: "TipoUsuario",
                table: "Usuarios",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Senha",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Usuarios",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Conquistas",
                table: "Estatistica",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estatistica_JogoId",
                table: "Estatistica",
                column: "JogoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacao_Jogo_JogoId",
                table: "Avaliacao",
                column: "JogoId",
                principalTable: "Jogo",
                principalColumn: "JogoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Estatistica_Jogo_JogoId",
                table: "Estatistica",
                column: "JogoId",
                principalTable: "Jogo",
                principalColumn: "JogoId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacao_Jogo_JogoId",
                table: "Avaliacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Estatistica_Jogo_JogoId",
                table: "Estatistica");

            migrationBuilder.DropIndex(
                name: "IX_Estatistica_JogoId",
                table: "Estatistica");

            migrationBuilder.RenameColumn(
                name: "TipoUsuario",
                table: "Usuarios",
                newName: "tipoUsuario");

            migrationBuilder.RenameColumn(
                name: "JogoId",
                table: "Estatistica",
                newName: "JogoID");

            migrationBuilder.RenameColumn(
                name: "JogoId",
                table: "Avaliacao",
                newName: "JogoID");

            migrationBuilder.RenameIndex(
                name: "IX_Avaliacao_JogoId",
                table: "Avaliacao",
                newName: "IX_Avaliacao_JogoID");

            migrationBuilder.AlterColumn<string>(
                name: "tipoUsuario",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Senha",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Conquistas",
                table: "Estatistica",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "UsuariosID",
                table: "Avaliacao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Estatistica_JogoID",
                table: "Estatistica",
                column: "JogoID");

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacao_Jogo_JogoID",
                table: "Avaliacao",
                column: "JogoID",
                principalTable: "Jogo",
                principalColumn: "JogoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Estatistica_Jogo_JogoID",
                table: "Estatistica",
                column: "JogoID",
                principalTable: "Jogo",
                principalColumn: "JogoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
