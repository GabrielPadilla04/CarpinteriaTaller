using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TP5_Servicios_API_REST.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposRecuperacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "codigo_recuperacion",
                table: "usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "expiracion_codigo",
                table: "usuarios",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "codigo_recuperacion",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "expiracion_codigo",
                table: "usuarios");
        }
    }
}
