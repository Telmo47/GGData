using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGData.Data.Migrations
{
    /// <inheritdoc />
    public partial class BD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jogo",
                columns: table => new
                {
                    JogoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Genero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Plataforma = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataLancamento = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jogo", x => x.JogoId);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tipoUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioId);
                });

            migrationBuilder.CreateTable(
                name: "Estatistica",
                columns: table => new
                {
                    EstatisticaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Conquistas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TempoMedioJogo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAvaliacoes = table.Column<int>(type: "int", nullable: false),
                    JogoID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estatistica", x => x.EstatisticaId);
                    table.ForeignKey(
                        name: "FK_Estatistica_Jogo_JogoID",
                        column: x => x.JogoID,
                        principalTable: "Jogo",
                        principalColumn: "JogoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Avaliacao",
                columns: table => new
                {
                    AvaliacaoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nota = table.Column<int>(type: "int", nullable: false),
                    Comentarios = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataReview = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuariosID = table.Column<int>(type: "int", nullable: false),
                    JogoID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliacao", x => x.AvaliacaoId);
                    table.ForeignKey(
                        name: "FK_Avaliacao_Jogo_JogoID",
                        column: x => x.JogoID,
                        principalTable: "Jogo",
                        principalColumn: "JogoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Avaliacao_Usuarios_UsuariosID",
                        column: x => x.UsuariosID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacao_JogoID",
                table: "Avaliacao",
                column: "JogoID");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacao_UsuariosID",
                table: "Avaliacao",
                column: "UsuariosID");

            migrationBuilder.CreateIndex(
                name: "IX_Estatistica_JogoID",
                table: "Estatistica",
                column: "JogoID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Avaliacao");

            migrationBuilder.DropTable(
                name: "Estatistica");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Jogo");
        }
    }
}
