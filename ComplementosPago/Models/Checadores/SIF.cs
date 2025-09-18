using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelContext.Models
{
    public class SIF
    {
        [Key]
        public int sif_keysif { get; set; }
        [ForeignKey("STF")] 
        public int stf_keystf { get; set; }//llave de la tabla stfs
        [ForeignKey("FPR")] 
        public int fpr_keyfpr { get; set; }//llave de la tabla fprs
        [DefaultValue(1)]
        public int sif_stasif { get; set; }
        [ForeignKey("stf_keystf")] 
        public virtual STF STF { get; set; }
        [ForeignKey("fpr_keyfpr")] 
        public virtual FPR FPR { get; set; }
    }
}
