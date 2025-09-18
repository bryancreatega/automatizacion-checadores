using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelContext.Models
{
    public class BIT
    {
        public int id { get; set; }
        public int lectorId { get; set; }
        public int procesoId { get; set; }
        public DateTime fechaEnvio { get; set; }
        public string descripcion { get; set; }

    }
}
