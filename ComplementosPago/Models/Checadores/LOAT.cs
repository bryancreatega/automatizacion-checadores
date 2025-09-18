using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelContext.Models
{
    public class LOAT
    {
        public int id { get; set; }
        public int procesoId { get; set; }
        public int checadorId { get; set; }
        public DateTime fecha{ get; set; }
        public int estadoId { get; set; }
        public int totalesPrevios { get; set; }
        public int totalesPosterior { get; set; }
        public DateTime fechaRegistro { get; set; }

    }
}
