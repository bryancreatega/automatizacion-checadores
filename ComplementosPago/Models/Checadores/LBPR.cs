using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelContext.Models
{
	public class LBPR
	{
		public short per_keypro { get; set; }
		public short per_keynom { get; set; }
		[StringLength(7)]
		public string per_keyper { get; set; }
		public DateTime per_fecini { get; set; }
		public DateTime per_fecfin { get; set; }
		public short? per_nummes { get; set; }
		public decimal? per_impnom { get; set; }
		public int? per_totemp { get; set; }
		public string per_persel { get; set; }
	}
}
