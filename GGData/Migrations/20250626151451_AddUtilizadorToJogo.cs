using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGData.Migrations
{
    /// <inheritdoc />
    public partial class AddUtilizadorToJogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UtilizadorId",
                table: "Jogos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cb039fc6-c875-42c1-8435-babfe42830f2", "AQAAAAIAAYagAAAAEN9xMO7hEx6pdgGRJgC+UAN2mCQviNxrP3xUEOP0oOEUg6pxPnbK0kIpY6f1ZrAkKw==", "96e40a62-81ff-423c-992c-9bd5f83566d5" });

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_UtilizadorId",
                table: "Jogos",
                column: "UtilizadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jogos_AspNetUsers_UtilizadorId",
                table: "Jogos",
                column: "UtilizadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_AspNetUsers_UtilizadorId",
                table: "Jogos");

            migrationBuilder.DropIndex(
                name: "IX_Jogos_UtilizadorId",
                table: "Jogos");

            migrationBuilder.DropColumn(
                name: "UtilizadorId",
                table: "Jogos");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f152000d-a22c-40cc-baf3-1da753da3c9a", "AQAAAAIAAYagAAAAECx3FYOVfqPphlNVCuDd4AHOHpAwZEH37NHgKfBW3ZFH6g4OAy1t6HEXE19L+huoog==", "06a0c3f5-238b-45e8-97b7-a580df417e60" });
        }
    }
}
