using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGData.Data.Migrations
{
    /// <inheritdoc />
    public partial class CorrigeUsuarioId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys usando os nomes reais existentes
            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacao_Jogo_JogoId",
                table: "Avaliacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacao_Usuarios_UsuarioId",
                table: "Avaliacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Estatistica_Jogo_JogoID",
                table: "Estatistica");

            // Renomeia colunas na tabela Usuarios e Avaliacao
            migrationBuilder.RenameColumn(
                name: "tipoUsuario",
                table: "Usuarios",
                newName: "TipoUsuario");

            migrationBuilder.RenameColumn(
                name: "JogoID",
                table: "Avaliacao",
                newName: "JogoId");

            // REMOVIDO: tentativa de renomear coluna inexistente UsuariosID

            // Renomeia índices associados às colunas renomeadas
            migrationBuilder.RenameIndex(
                name: "IX_Avaliacao_JogoID",
                table: "Avaliacao",
                newName: "IX_Avaliacao_JogoId");

            // Também removido o rename do índice do UsuarioId para evitar erro
            // migrationBuilder.RenameIndex(
            //    name: "IX_Avaliacao_UsuariosID",
            //    table: "Avaliacao",
            //    newName: "IX_Avaliacao_UsuarioId");

            // Adiciona novas colunas na tabela Usuarios (exceto UserName, que já existe)
            migrationBuilder.AddColumn<string>(
                name: "DescricaoProfissional",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instituicao",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteProfissional",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            // Alteração de colunas na tabela Jogo para ter limitações de tamanho
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

            // Alterações nas colunas da tabela Avaliacao
            migrationBuilder.AlterColumn<string>(
                name: "TipoUsuario",
                table: "Avaliacao",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Comentarios",
                table: "Avaliacao",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Recria as foreign keys com os nomes corretos e colunas renomeadas
            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacao_Jogo_JogoId",
                table: "Avaliacao",
                column: "JogoId",
                principalTable: "Jogo",
                principalColumn: "JogoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacao_Usuarios_UsuarioId",
                table: "Avaliacao",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Estatistica_Jogo_JogoID",
                table: "Estatistica",
                column: "JogoID",
                principalTable: "Jogo",
                principalColumn: "JogoId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacao_Jogo_JogoId",
                table: "Avaliacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacao_Usuarios_UsuarioId",
                table: "Avaliacao");

            migrationBuilder.DropForeignKey(
                name: "FK_Estatistica_Jogo_JogoID",
                table: "Estatistica");

            migrationBuilder.DropColumn(
                name: "DescricaoProfissional",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Instituicao",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "WebsiteProfissional",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "TipoUsuario",
                table: "Usuarios",
                newName: "tipoUsuario");

            migrationBuilder.RenameColumn(
                name: "JogoId",
                table: "Avaliacao",
                newName: "JogoID");

            // Sem renomear coluna UsuarioId porque não foi renomeada no Up

            migrationBuilder.RenameIndex(
                name: "IX_Avaliacao_JogoId",
                table: "Avaliacao",
                newName: "IX_Avaliacao_JogoID");

            // Sem renomear índice IX_Avaliacao_UsuarioId pois não foi renomeado

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

            migrationBuilder.AlterColumn<string>(
                name: "TipoUsuario",
                table: "Avaliacao",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Comentarios",
                table: "Avaliacao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacao_Jogo_JogoID",
                table: "Avaliacao",
                column: "JogoID",
                principalTable: "Jogo",
                principalColumn: "JogoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacao_Usuarios_UsuariosID",
                table: "Avaliacao",
                column: "UsuariosID",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
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
