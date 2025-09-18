using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplementosPago.Models
{
    public class ComplementoEnvio
    {
        public int Id { get; set; }
        public string Empresa { get; set; }
        public string NombreArchivo { get; set; }
        public string FechaRegistro { get; set; }
        public bool Procesado { get; set; }
        public string? ErrorSap { get; set; }
        public string? FechaEnvio { get; set; }
        public bool Encontrado { get; set; }
    }
}
