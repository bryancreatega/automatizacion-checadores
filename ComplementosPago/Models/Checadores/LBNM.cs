using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelContext.Models
{
	public class LBNM
	{
		public int nom_keynom { get; set; }
		[StringLength(40)] 
		public string nom_destip { get; set; }
	}
}
