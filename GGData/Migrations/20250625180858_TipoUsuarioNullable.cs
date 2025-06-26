using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGData.Migrations
{
    /// <inheritdoc />
    public partial class TipoUsuarioNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8fef583f-1c2e-47e8-b818-7b54942e3c58", "AQAAAAIAAYagAAAAEGcmyynnKYZTG3sACBK47OWFPWXE0A6wXVCZKfYal5XJwdVmypJj0PACDP3ozOVVpw==", "3f854a05-ab3e-4697-8992-54105f2ec1e5" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f9dd8ec2-b34d-46d9-b9b9-fbea34a9aa60", "AQAAAAIAAYagAAAAELmVmQtGSnG9F1xL8XQ/UtZFpID+s0xsrVSwe1lbFsTLxZ7c3mAW5+fBLmS35wHBEg==", "4cd36f4d-14a8-4bae-854a-171dd806556b" });
        }
    }
}
