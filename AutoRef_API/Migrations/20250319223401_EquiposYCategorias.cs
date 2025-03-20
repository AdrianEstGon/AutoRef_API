using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoRef_API.Migrations
{
    /// <inheritdoc />
    public partial class EquiposYCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Equipos");

            migrationBuilder.AlterColumn<Guid>(
                name: "ClubId",
                table: "Equipos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoriaId",
                table: "Equipos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Anotador",
                table: "Categorias",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MinArbitros",
                table: "Categorias",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "PrimerArbitro",
                table: "Categorias",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SegundoArbitro",
                table: "Categorias",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_CategoriaId",
                table: "Equipos",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipos_Categorias_CategoriaId",
                table: "Equipos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipos_Categorias_CategoriaId",
                table: "Equipos");

            migrationBuilder.DropIndex(
                name: "IX_Equipos_CategoriaId",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "Anotador",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "MinArbitros",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "PrimerArbitro",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "SegundoArbitro",
                table: "Categorias");

            migrationBuilder.AlterColumn<Guid>(
                name: "ClubId",
                table: "Equipos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Equipos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
