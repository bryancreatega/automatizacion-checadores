using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelContext.Models
{
	public class LBEM
	{
		public int emp_keyemp { get; set; }
		[StringLength(100)]
		public string emp_nomemp { get; set; }
		[StringLength(20)] 
		public string emp_nomcor { get; set; }
		public int emp_keypro { get; set; }
		public int emp_status { get; set; }
	}
}
