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
    public class STF
    {
        [Key]
        public int stf_keystf { get; set; }
        [Key, StringLength(20)]
        public string stf_numstf { get; set; }
        [StringLength(300)]
        public string? stf_namstf { get; set; }
        [StringLength(300)]
        public string? stf_lnastf { get; set; }
        [StringLength(100)]
        public string? stf_nckstf { get; set; }
        public DateTime stf_dsastf { get; set; }
        public DateTime stf_drestf { get; set; }
        public DateTime stf_dbastf { get; set; }
        public DateTime stf_ddestf { get; set; }
        public string? stf_crdstf { get; set; }
        public string? stf_pssstf { get; set; }
        [DefaultValue(1)] 
        public int stf_stastf { get; set; }
        public ICollection<DST> DST { get; set; }
        public ICollection<SIF> SIF { get; set; }
    }
}
