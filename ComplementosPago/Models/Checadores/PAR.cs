using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelContext.Models
{
    public class PAR
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Descripcion { get; set; }
        public bool Mostrar { get; set; }
        public bool Activo { get; set; }
    }
}
