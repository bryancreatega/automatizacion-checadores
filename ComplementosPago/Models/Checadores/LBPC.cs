using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelContext.Models
{
	public class LBPC
	{
		public int pro_keypro { get; set; }
		[StringLength(20)] 
		public string pro_despro { get; set; }
	}
}
