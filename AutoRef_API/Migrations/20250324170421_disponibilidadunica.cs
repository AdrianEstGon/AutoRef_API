using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoRef_API.Migrations
{
    /// <inheritdoc />
    public partial class disponibilidadunica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Disponibilidades_UsuarioId",
                table: "Disponibilidades");

            migrationBuilder.CreateIndex(
                name: "IX_Disponibilidades_UsuarioId_Fecha",
                table: "Disponibilidades",
                columns: new[] { "UsuarioId", "Fecha" },
                unique: true,
                filter: "[UsuarioId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Disponibilidades_UsuarioId_Fecha",
                table: "Disponibilidades");

            migrationBuilder.CreateIndex(
                name: "IX_Disponibilidades_UsuarioId",
                table: "Disponibilidades",
                column: "UsuarioId");
        }
    }
}
