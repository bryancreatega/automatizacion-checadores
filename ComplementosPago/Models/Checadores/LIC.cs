using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ModelContext.Models
{
	public class LIC
	{
        [Key]
        public int lic_keylic { get; set; }
        [StringLength(int.MaxValue)]
        public string lic_vlalic { get; set; }//empresa para su uso
        [StringLength(int.MaxValue)]
        public string lic_vlblic { get; set; }//hardware key de la empresa
        [StringLength(int.MaxValue)]
        public string lic_vlclic { get; set; }//licencia valida
        [StringLength(int.MaxValue)]
        public string lic_vldlic { get; set; }//fecha de inicio y fecha de fin de la licencia
        [StringLength(int.MaxValue)]
        public string lic_vlelic { get; set; }//número de equipos a instalar
        [StringLength(int.MaxValue)]
        public string lic_vlflic { get; set; }//número de equipos a instaldos
        [StringLength(int.MaxValue)]
        public string lic_vlglic { get; set; }//veces ejecucion
        [StringLength(int.MaxValue)]
        public string lic_vlhlic { get; set; }//ejecutado
        [StringLength(int.MaxValue)]
        public string lic_vlilic { get; set; }//es servidor
        [StringLength(int.MaxValue)]
        public string lic_vljlic { get; set; }//servidor
        [StringLength(int.MaxValue)]
        public string lic_vlklic { get; set; }//base de datos fingerprints
        [StringLength(int.MaxValue)]
        public string lic_vlllic { get; set; }//base de datos labora
        [StringLength(int.MaxValue)]
        public string lic_vlmlic { get; set; }//usuario
        [StringLength(int.MaxValue)]
        public string lic_vlnlic { get; set; }//password
        [DefaultValue(1)]
        public int lic_stalic { get; set; }
    }
}
