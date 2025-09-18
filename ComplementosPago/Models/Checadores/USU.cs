using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelContext.Models
{
    public class USU
    {
        [Key]
        public int usu_keyusu { get; set; }
        [StringLength(100)]
        public string usu_namusu { get; set; }
        [StringLength(100)]
        public string usu_dscusu { get; set; }
        [StringLength(100)]
        public string usu_conusu { get; set; }
        [DefaultValue(0)]
        public int usu_fnpusu { get; set; }
        [DefaultValue(0)]
        public int usu_bckusu { get; set; }
        [DefaultValue(0)]
        public int usu_rplusu { get; set; }
        [DefaultValue(0)]
        public int usu_rmvusu { get; set; }
        [DefaultValue(0)]
        public int usu_synusu { get; set; }
        [DefaultValue(0)]
        public int usu_mrkusu { get; set; }
        [DefaultValue(0)]
        public int usu_rptusu { get; set; }
        [DefaultValue(0)]
        public int usu_sttusu { get; set; }
        [DefaultValue(1)]
        public int usu_stausu { get; set; }
        public int usu_AdminConf { get; set; }
        [DefaultValue(0)]
        public ICollection<OPR> OPR { get; set; }
        //public int pro_keypro { get; set; }
    }
}
