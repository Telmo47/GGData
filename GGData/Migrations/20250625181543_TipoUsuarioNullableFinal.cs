using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGData.Migrations
{
    /// <inheritdoc />
    public partial class TipoUsuarioNullableFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TipoUsuario",
                table: "Avaliacao",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f152000d-a22c-40cc-baf3-1da753da3c9a", "AQAAAAIAAYagAAAAECx3FYOVfqPphlNVCuDd4AHOHpAwZEH37NHgKfBW3ZFH6g4OAy1t6HEXE19L+huoog==", "06a0c3f5-238b-45e8-97b7-a580df417e60" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TipoUsuario",
                table: "Avaliacao",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8fef583f-1c2e-47e8-b818-7b54942e3c58", "AQAAAAIAAYagAAAAEGcmyynnKYZTG3sACBK47OWFPWXE0A6wXVCZKfYal5XJwdVmypJj0PACDP3ozOVVpw==", "3f854a05-ab3e-4697-8992-54105f2ec1e5" });
        }
    }
}
