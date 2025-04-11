using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoRef_API.Migrations
{
    /// <inheritdoc />
    public partial class EstadoArbitros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstadoAnotador",
                table: "Partidos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstadoArbitro1",
                table: "Partidos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstadoArbitro2",
                table: "Partidos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoAnotador",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "EstadoArbitro1",
                table: "Partidos");

            migrationBuilder.DropColumn(
                name: "EstadoArbitro2",
                table: "Partidos");
        }
    }
}
