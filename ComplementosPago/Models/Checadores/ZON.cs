using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelContext.Models
{
    public class ZON
    {
        [Key]
        public int zon_keyzon { get; set; }
        public int zon_numzon { get; set; }
        [StringLength(200)]
        public string zon_namzon { get; set; }
        [DefaultValue(1)]
        public int zon_stazon { get; set; }
        [StringLength(100)]
        public string zon_dstzon { get; set; }
        public ICollection<FPR> FPR { get; set; }
    }
}
