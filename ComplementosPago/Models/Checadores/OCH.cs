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
    public class OCH
    {
        [Key]//[Key, Column(Order = 0)]
        public int och_keyoch { get; set; }
        //public int stf_keystf { get; set; }
        [StringLength(20)]
        public string stf_numstf { get; set; }
        [Column(TypeName = "Date")]
        public DateTime och_datoch { get; set; }
        public TimeSpan och_horoch { get; set; }
        [Column(TypeName = "Date")]
        public DateTime och_dtpoch { get; set; }
        public TimeSpan och_hrpoch { get; set; }
        public string och_stzoch { get; set; }
        public int och_fstoch { get; set; }
        public int och_typoch { get; set; }//OnDemand o Automatico
        [ForeignKey("FPR")]
        public int fpr_keyfpr { get; set; }
        public int fpr_numfpr { get; set; }
        [DefaultValue(0)]
        public int och_staoch { get; set; }
        [ForeignKey("ODT")]
        public int odt_keyodt { get; set; }
        [ForeignKey("fpr_keyfpr")] 
        public virtual FPR FPR { get; set; }
        [ForeignKey("odt_keyodt")]
        public virtual ODT ODT { get; set; }
    }
}
