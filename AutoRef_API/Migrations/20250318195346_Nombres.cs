using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoRef_API.Migrations
{
    /// <inheritdoc />
    public partial class Nombres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipo_Club_ClubId",
                table: "Equipo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Equipo",
                table: "Equipo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Club",
                table: "Club");

            migrationBuilder.RenameTable(
                name: "Equipo",
                newName: "Equipos");
            migrationBuilder.RenameTable(
                name: "Usuario",
                newName: "Usuarios");

            migrationBuilder.RenameTable(
                name: "Club",
                newName: "Clubs");

            migrationBuilder.RenameIndex(
                name: "IX_Equipo_ClubId",
                table: "Equipos",
                newName: "IX_Equipos_ClubId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Equipos",
                table: "Equipos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clubs",
                table: "Clubs",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Equipos_Clubs_ClubId",
                table: "Equipos",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipos_Clubs_ClubId",
                table: "Equipos");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Equipos",
                table: "Equipos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clubs",
                table: "Clubs");

            migrationBuilder.RenameTable(
                name: "Equipos",
                newName: "Equipo");

            migrationBuilder.RenameTable(
                name: "Clubs",
                newName: "Club");

            migrationBuilder.RenameIndex(
                name: "IX_Equipos_ClubId",
                table: "Equipo",
                newName: "IX_Equipo_ClubId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Equipo",
                table: "Equipo",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Club",
                table: "Club",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipo_Club_ClubId",
                table: "Equipo",
                column: "ClubId",
                principalTable: "Club",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
