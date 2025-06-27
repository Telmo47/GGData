﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGData.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarImagemJogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagemUrl",
                table: "Jogo",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagemUrl",
                table: "Jogo");
        }
    }
}
