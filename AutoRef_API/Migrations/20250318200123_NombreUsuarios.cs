using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoRef_API.Migrations
{
    /// <inheritdoc />
    public partial class NombreUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Disponibilidades_Usuario_UsuarioId",
                table: "Disponibilidades");

            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Usuario_AnotadorId",
                table: "Partidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Usuario_Arbitro1Id",
                table: "Partidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Usuario_Arbitro2Id",
                table: "Partidos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuario",
                table: "Usuario");

            migrationBuilder.RenameTable(
                name: "Usuario",
                newName: "Usuarios");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Disponibilidades_Usuarios_UsuarioId",
                table: "Disponibilidades",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Usuarios_AnotadorId",
                table: "Partidos",
                column: "AnotadorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Usuarios_Arbitro1Id",
                table: "Partidos",
                column: "Arbitro1Id",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Usuarios_Arbitro2Id",
                table: "Partidos",
                column: "Arbitro2Id",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Disponibilidades_Usuarios_UsuarioId",
                table: "Disponibilidades");

            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Usuarios_AnotadorId",
                table: "Partidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Usuarios_Arbitro1Id",
                table: "Partidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Usuarios_Arbitro2Id",
                table: "Partidos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "Usuario");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuario",
                table: "Usuario",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Disponibilidades_Usuario_UsuarioId",
                table: "Disponibilidades",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Usuario_AnotadorId",
                table: "Partidos",
                column: "AnotadorId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Usuario_Arbitro1Id",
                table: "Partidos",
                column: "Arbitro1Id",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Usuario_Arbitro2Id",
                table: "Partidos",
                column: "Arbitro2Id",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
