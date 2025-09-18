using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplementosPago.Models
{
    public class LecturaComplemento
    {
        public int Id { get; set; }
        public string Empresa { get; set; }
        public string NombreArchivo { get; set; }
        public DateTime Fecha { get; set; }
        public bool Procesado { get; set; }
        public string? MotivoNoProcesado { get; set; }
    }
}
