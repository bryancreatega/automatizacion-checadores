using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelContext.Models
{
    public class DST
    {
        [Key]
        public int dst_keydst { get; set; }
        //public int dst_lindst { get; set; }
        [StringLength(100)]
        public string dst_pssdst { get; set; }
        public int dst_pridst { get; set; }
        public bool dst_enadst { get; set; }
        public int dst_findst { get; set; }
        public int dst_fladst { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]//text
        [MaxLength()]
        public string dst_tmpdst { get; set; }
        public int dst_lendst { get; set; }
        public int dst_stadst { get; set; }
        public int dst_typdst { get; set; }
        [ForeignKey("STF")] 
        public int stf_keystf { get; set; }
        [ForeignKey("stf_keystf")]
        public virtual STF STF { get; set; }
    }
}
