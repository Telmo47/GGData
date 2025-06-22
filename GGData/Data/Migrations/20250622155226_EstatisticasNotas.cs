using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGData.Data.Migrations
{
    /// <inheritdoc />
    public partial class EstatisticasNotas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MediaNotaCriticos",
                table: "Estatistica",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MediaNotaUtilizadores",
                table: "Estatistica",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaNotaCriticos",
                table: "Estatistica");

            migrationBuilder.DropColumn(
                name: "MediaNotaUtilizadores",
                table: "Estatistica");
        }
    }
}
