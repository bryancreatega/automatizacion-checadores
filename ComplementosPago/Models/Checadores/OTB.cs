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
    public class OTB
    {
        [Key]
        public int otb_keyotb { get; set; }
        public int otb_linotb { get; set; }
        [StringLength(100)]
        public string otb_namotb { get; set; }
        [StringLength(100)]
        public string otb_pssotb { get; set; }
        public int otb_priotb { get; set; }
        public int otb_enaotb { get; set; }
        public int otb_finotb { get; set; }
        public string otb_tmpotb { get; set; }
        public int otb_lenotb { get; set; }
        public DateTime otb_datotb { get; set; }
        public TimeSpan otb_horotb { get; set; }
        public int otb_typotb { get; set; }
        [StringLength(100)]
        public string otb_msgotb { get; set; }
        public string otb_crdotb { get; set; }
        public int stf_keystf { get; set; }
        [StringLength(20)]
        public string stf_numstf { get; set; }
        [ForeignKey("ODT")] 
        public int odt_keyodt { get; set; }
        [ForeignKey("FPR")] 
        public int fpr_keyfpr { get; set; }
        public int fpr_numfpr { get; set; }
        public int otb_staotb { get; set; }
        [ForeignKey("fpr_keyfpr")] 
        public virtual FPR FPR { get; set; }
        [ForeignKey("odt_keyodt")] 
        public virtual ODT ODT { get; set; }
        //public STF STF { get; set; }
    }
}
