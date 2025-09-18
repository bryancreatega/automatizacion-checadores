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
    public class ODT
    {
        [Key]
        public int odt_keyodt { get; set; }
        public int odt_linodt { get; set; }
        public DateTime odt_datodt { get; set; }
        public TimeSpan odt_horodt { get; set; }
        public DateTime odt_ndtodt { get; set; }
        public TimeSpan odt_nhrodt { get; set; }
        [StringLength(100)]
        public string odt_msgodt { get; set; }
        public int odt_typodt { get; set; }
        public int odt_dmkodt { get; set; }
        public DateTime odt_hmkodt { get; set; }
        [ForeignKey("OPR")]
        public int opr_keyopr { get; set; }
        [ForeignKey("FPR")] 
        public int fpr_keyfpr { get; set; }
        public int fpr_numfpr { get; set; }
        [StringLength(100)]
        public string fpr_namfpr { get; set; }
        [DefaultValue(0)] 
        public int odt_staodt { get; set; }
        [ForeignKey("fpr_keyfpr")] 
        public virtual FPR FPR { get; set; }
        [ForeignKey("opr_keyopr")] 
        public virtual OPR OPR { get; set; }
        //public int usu_keyusu { get; set; }
        //public USU USU { get; set; }
        public ICollection<OBK> OBK { get; set; }
        public ICollection<OCH> OCH { get; set; }
        public ICollection<ODL> ODL { get; set; }
        public ICollection<OGT> OGT { get; set; }
        public ICollection<OST> OST { get; set; }
        public ICollection<OTB> OTB { get; set; }
    }
}
