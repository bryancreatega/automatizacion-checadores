using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplementosPago.Migrations
{
    /// <inheritdoc />
    public partial class cambiosProcesar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Empresa",
                table: "DetalleLecturaComplementos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MotivoNoProcesado",
                table: "DetalleLecturaComplementos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Procesado",
                table: "DetalleLecturaComplementos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Empresa",
                table: "DetalleLecturaComplementos");

            migrationBuilder.DropColumn(
                name: "MotivoNoProcesado",
                table: "DetalleLecturaComplementos");

            migrationBuilder.DropColumn(
                name: "Procesado",
                table: "DetalleLecturaComplementos");
        }
    }
}
