using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplementosPago.Models
{
    public class DetalleLecturaComplemento
    {
        public int Id { get; set; }
        public LecturaComplemento LecturaComplemento { get; set; }
        public int LecturaComplementoId { get; set; }
        public string? Archivo { get; set; }
        public string? UUID { get; set; }
        public string? VerificacionSat { get; set; }
        public string? Fecha { get; set; }
        public string? Hora { get; set; }
        public string? FechaPago { get; set; }
        public string? HoraPago { get; set; }
        public string? Serie { get; set; }
        public string? Folio { get; set; }
        public string? RfcRecepetor { get; set; }
        public string? NombreReceptor { get; set; }
        public string? Moneda { get; set; }
        public string? Monto { get; set; }
        public string? FormaPago { get; set; }
        public string? UuidDctoRel { get; set; }
        public string? SerieRel { get; set; }
        public string? FolioRel { get; set; }
        public string? MonedaRel { get; set; }
        public string? NumeroParcialidad { get; set; }
        public string? ImporteSaldoAnt { get; set; }
        public string? ImportePagado { get; set; }
        public string? ImporteSaldoInsoluto { get; set; }
        public string? MetodoPagoRel { get; set; }
        public string? RfcEmisor { get; set; }
        public string? NombreEmisor { get; set; }
        public string? Complementos { get; set; }
        public bool Procesado { get; set; }
        public string? MotivoNoProcesado { get; set; }
        public string Empresa { get; set; }
    }
}
