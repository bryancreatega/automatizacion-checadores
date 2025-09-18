using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplementosPago.Models
{
    public class LogEjecucion
    {
        public int Id { get; set; }
        public string Carpeta { get; set; }
        public DateTime Fecha { get; set; }
        public int Archivos { get; set; }
    }
}
