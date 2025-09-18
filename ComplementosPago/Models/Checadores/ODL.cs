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
    public class ODL
    {
        [Key]
        public int odl_keyodl { get; set; }
        public int odl_linodl { get; set; }
        [StringLength(100)]
        public string odl_namodl { get; set; }
        [StringLength(100)]
        public string odl_pssodl { get; set; }
        public int odl_priodl { get; set; }
        public int odl_enaodl { get; set; }
        public int odl_finodl { get; set; }
        public string odl_tmpodl { get; set; }
        public int odl_lenodl { get; set; }
        public DateTime odl_datodl { get; set; }
        public TimeSpan odl_horodl { get; set; }
        public int odl_typodl { get; set; }
        [StringLength(100)]
        public string odl_msgodl { get; set; }
        [ForeignKey("ODT")] 
        public int odt_keyodt { get; set; }
        [ForeignKey("FPR")] 
        public int fpr_keyfpr { get; set; }
        public int fpr_numfpr { get; set; }
        public int stf_keystf { get; set; }
        [StringLength(20)]
        public string stf_numstf { get; set; }
        [DefaultValue(0)]
        public int odl_staodl { get; set; }
        [ForeignKey("fpr_keyfpr")]
        public virtual FPR FPR { get; set; }
        [ForeignKey("odt_keyodt")] 
        public virtual ODT ODT { get; set; }
        //public STF STF { get; set; }
    }
}
