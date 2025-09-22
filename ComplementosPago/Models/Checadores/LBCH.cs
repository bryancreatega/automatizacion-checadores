using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelContext.Models
{
    public class LBCH
    {
        [Key]
        [StringLength(12)]
        public string? che_keylec { get; set; }
        public int che_keyemp { get; set; }
        public DateTime che_fecche { get; set; }
        [StringLength(8)]
        public string? che_horche { get; set; }
        [StringLength(5)]
        public string? che_status { get; set; }
        [StringLength(2)]
        public string? che_tipche { get; set; }
        [StringLength(7)]
        public string? che_keyper { get; set; }
        //public DateTime che_fecjor { get; set; }
    }
}
