using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplementosPago.Models
{
    public class TiempoEjecucion
    {
        public int Id { get; set; }
        public string Dia { get; set; }
        public string Hora { get; set; }
        public bool Activo { get; set; }
    }
}
