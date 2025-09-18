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
	public class OCD
	{
        [Key]//[Key, Column(Order = 0)]
        public int ocd_keyocd { get; set; }
		public int och_keyoch { get; set; }
        [StringLength(20)]
        public string stf_numstf { get; set; }
        [Column(TypeName = "Date")]
        public DateTime ocd_datocd { get; set; }
        public TimeSpan ocd_horocd { get; set; }
        [Column(TypeName = "Date")]
        public DateTime ocd_dtpocd { get; set; }
        public TimeSpan ocd_hrpocd { get; set; }
        public string ocd_stzocd { get; set; }
        public int ocd_fstocd { get; set; }
        public int ocd_typocd { get; set; }//OnDemand o Automatico
        [ForeignKey("FPR")]
        public int fpr_keyfpr { get; set; }
        public int fpr_numfpr { get; set; }
        [DefaultValue(0)]
        public int ocd_staocd { get; set; }
		public string per_keyper { get; set; }
		[ForeignKey("fpr_keyfpr")]
        public bool ocd_error { get; set; }
        public virtual FPR FPR { get; set; }
    }
}
