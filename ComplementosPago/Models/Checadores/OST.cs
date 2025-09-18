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
    public class OST
    {
        [Key]
        public int ost_keyost { get; set; }
        public DateTime ost_ndtost { get; set; }
        public TimeSpan ost_nhrost { get; set; }
        [StringLength(100)]
        public string ost_msgost { get; set; }
        public int ost_typost { get; set; }
        [ForeignKey("ODT")] 
        public int odt_keyodt { get; set; }
        [ForeignKey("FPR")] 
        public int fpr_keyfpr { get; set; }
        public int fpr_numfpr { get; set; }
        [StringLength(100)]
        public string fpr_namfpr { get; set; }
        [DefaultValue(0)]
        public int ost_staost { get; set; }
        [ForeignKey("fpr_keyfpr")] 
        public virtual FPR FPR { get; set; }
        [ForeignKey("odt_keyodt")] 
        public virtual ODT ODT { get; set; }
    }
}
