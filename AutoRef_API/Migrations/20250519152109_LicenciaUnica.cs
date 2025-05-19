using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoRef_API.Migrations
{
    /// <inheritdoc />
    public partial class LicenciaUnica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Comentarios",
                table: "Disponibilidades",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Licencia",
                table: "Usuarios",
                column: "Licencia",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Licencia",
                table: "Usuarios");

            migrationBuilder.AlterColumn<string>(
                name: "Comentarios",
                table: "Disponibilidades",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
