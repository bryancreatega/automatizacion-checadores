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
    public class OGT
    {
        [Key]
        public int ogt_keyogt { get; set; }
        public DateTime ogt_ndtogt { get; set; }
        public TimeSpan ogt_nhrogt { get; set; }
        [StringLength(100)]
        public string ogt_msgogt { get; set; }
        public int ogt_typogt { get; set; }
        [ForeignKey("ODT")]
        public int odt_keyodt { get; set; }
        [ForeignKey("FPR")] 
        public int fpr_keyfpr { get; set; }
        public int fpr_numfpr { get; set; }
        public string fpr_namfpr { get; set; }
        [DefaultValue(0)]
        public int ogt_staogt { get; set; }
        [ForeignKey("fpr_keyfpr")] 
        public virtual FPR FPR { get; set; }
        [ForeignKey("odt_keyodt")] 
        public virtual ODT ODT { get; set; }
    }
}
