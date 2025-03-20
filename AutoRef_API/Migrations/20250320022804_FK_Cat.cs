using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoRef_API.Migrations
{
    /// <inheritdoc />
    public partial class FK_Cat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Partidos");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoriaId",
                table: "Partidos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_CategoriaId",
                table: "Partidos",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Categorias_CategoriaId",
                table: "Partidos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Categorias_CategoriaId",
                table: "Partidos");

            migrationBuilder.DropIndex(
                name: "IX_Partidos_CategoriaId",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Partidos");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Partidos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
