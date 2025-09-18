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
    public class USF
    {
        [Key]
        public int usf_keyusf { get; set; }
        [ForeignKey("STF")]
        public int usu_keyusu { get; set; }//llave de la tabla stfs
        [ForeignKey("FPR")]
        public int fpr_keyfpr { get; set; }//llave de la tabla fprs
        [DefaultValue(1)]
        public int usf_stausf { get; set; }
        [ForeignKey("usu_keyusu")]
        public virtual USU USU { get; set; }
        [ForeignKey("fpr_keyfpr")]
        public virtual FPR FPR { get; set; }
    }
}
