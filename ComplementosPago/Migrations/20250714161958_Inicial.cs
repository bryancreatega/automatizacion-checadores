using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplementosPago.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComplementoEnvios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Empresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Procesado = table.Column<bool>(type: "bit", nullable: false),
                    ErrorSap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaEnvio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Encontrado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplementoEnvios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LecturaComplementos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Empresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Procesado = table.Column<bool>(type: "bit", nullable: false),
                    MotivoNoProcesado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LecturaComplementos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DetalleLecturaComplementos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LecturaComplementoId = table.Column<int>(type: "int", nullable: false),
                    Archivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UUID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VerificacionSat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hora = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoraPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Serie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Folio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RfcRecepetor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreReceptor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Monto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormaPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UuidDctoRel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerieRel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FolioRel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MonedaRel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroParcialidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImporteSaldoAnt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImportePagado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImporteSaldoInsoluto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetodoPagoRel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RfcEmisor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreEmisor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Complementos = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleLecturaComplementos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalleLecturaComplementos_LecturaComplementos_LecturaComplementoId",
                        column: x => x.LecturaComplementoId,
                        principalTable: "LecturaComplementos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetalleLecturaComplementos_LecturaComplementoId",
                table: "DetalleLecturaComplementos",
                column: "LecturaComplementoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComplementoEnvios");

            migrationBuilder.DropTable(
                name: "DetalleLecturaComplementos");

            migrationBuilder.DropTable(
                name: "LecturaComplementos");
        }
    }
}
