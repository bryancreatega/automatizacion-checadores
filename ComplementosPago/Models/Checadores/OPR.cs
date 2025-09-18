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
    public class OPR
    {
        [Key]
        public int opr_keyopr { get; set; }
        [StringLength(100)]
        public string opr_desopr { get; set; }
        public int opr_opropr { get; set; }
        public DateTime opr_datopr { get; set; }
        public TimeSpan opr_horopr { get; set; }
        public DateTime opr_ndtopr { get; set; }
        public TimeSpan opr_nhropr { get; set; }
        public int opr_timopr { get; set; }
        public int opr_lapopr { get; set; }
        public int opr_runopr { get; set; }
        public int opr_incopr { get; set; }
        public int opr_eliopr { get; set; }
        public int opr_delopr { get; set; }
        public int opr_ndlopr { get; set; }
        public DateTime opr_ddlopr { get; set; }
        public int opr_extopr { get; set; }
        public int opr_typopr { get; set; }
        public int opr_selopr { get; set; }
        public int zon_keyzon { get; set; }
        public string opr_filopr { get; set; }
        [ForeignKey("USU")] 
        public int usu_keyusu { get; set; }
        [DefaultValue(0)]
        public int opr_staopr { get; set; }
        public ICollection<ODT> ODT { get; set; }
        [ForeignKey("usu_keyusu")] 
        public virtual USU USU { get; set; }

    }
}
