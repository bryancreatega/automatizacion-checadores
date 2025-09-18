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
    public class OBK
    {
        [Key]
        public int obk_keyobk { get; set; }
        public int obk_linobk { get; set; }
        [StringLength(100)]
        public string obk_namobk { get; set; }
        [StringLength(100)]
        public string obk_pssobk { get; set; }
        public int obk_priobk { get; set; }
        public int obk_enaobk { get; set; }
        public int obk_finobk { get; set; }
        public string obk_tmpobk { get; set; }
        public int obk_lenobk { get; set; }
        public DateTime obk_datobk { get; set; }
        public TimeSpan obk_horobk { get; set; }
        public int obk_typobk { get; set; }
        [DefaultValue(0)] 
        public int obk_staobk { get; set; }
        [StringLength(100)]
        public string obk_msgobk { get; set; }
        public string obk_crdobk { get; set; }
        [ForeignKey("ODT")]
        public int odt_keyodt { get; set; }
        [ForeignKey("FPR")] 
        public int fpr_keyfpr { get; set; }
        public int fpr_numfpr { get; set; }
        public int stf_keystf { get; set; }
        [StringLength(20)]
        public string stf_numstf { get; set; }
        [ForeignKey("fpr_keyfpr")]
        public virtual FPR FPR { get; set; }
        [ForeignKey("odt_keyodt")] 
        public virtual ODT ODT { get; set; }
        //public STF STF { get; set; }
    }
}
