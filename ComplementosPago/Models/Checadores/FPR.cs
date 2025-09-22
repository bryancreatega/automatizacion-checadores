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
    public class FPR
    {
        [Key]
        public int fpr_keyfpr { get; set; }
        public int fpr_numfpr { get; set; }
        [StringLength(100)]
        public string fpr_namfpr { get; set; }
        [StringLength(100)]
        public string fpr_ipafpr { get; set; }//IP
        [StringLength(100)]
        public string? fpr_macfpr { get; set; }//MAC
        [StringLength(100)]
        public string fpr_prtfpr { get; set; }//Puerto
        [StringLength(100)]
        public string? fpr_frmfpr { get; set; }//FrimWare Version
        [StringLength(100)]
        public string? fpr_cdgfpr { get; set; }//Producto Code//Device Name--nuevo
        [StringLength(100)]
        public string? fpr_plffpr { get; set; }//Plataform
        [StringLength(100)]
        public string? fpr_srnfpr { get; set; }//Serial Numner
        [StringLength(100)]
        public string? fpr_sdkfpr { get; set; }//Sdk Version
        [StringLength(100)]
        public string? fpr_thrfpr { get; set; }//Manufature Time
        public int fpr_fpafpr { get; set; }//Algoritmo Huella--nuevo
        public int fpr_fcafpr { get; set; }//Algoritmo Rostro--nuevo
        public int fpr_usrfpr { get; set; }//User Count--nuevo
        public int fpr_admfpr { get; set; }//Admin Count--nuevo
        public int fpr_pwdfpr { get; set; }//Pwd Count--nuevo
        public int fpr_oplfpr { get; set; }//Oplog Count--nuevo
        public int fpr_attfpr { get; set; }//AttLog Count--nuevo
        public int fpr_facfpr { get; set; }//Face Count--nuevo
        public int fpr_onlfpr { get; set; }
        public int fpr_pngfpr { get; set; }//Ultimo Ping
        public int fpr_fpnfpr { get; set; }//FP Count
        public int fpr_opefpr { get; set; }//Ultimo proceso realizado en el lector
        public DateTime fpr_datfpr { get; set; }
        public TimeSpan fpr_horfpr { get; set; }
        public int fpr_sttfpr { get; set; }//Verifica de que se realizo el respaldo
        [StringLength(100)]
        public string? fpr_blqfpr { get; set; }//Estaus del lector en nombre
        public int fpr_typfpr { get; set; }//Tipo de Lector, huella, rostro o ambos
        [ForeignKey("ZON")] 
        public int zon_keyzon { get; set; }
        [DefaultValue(1)]
        public int fpr_stafpr { get; set; }//Estatus general del lector
        [ForeignKey("zon_keyzon")]
        public bool fpr_comedor { get; set; }
        public virtual ZON ZON { get; set; }
        public ICollection<OBK> OBK { get; set; }
        public ICollection<OCH> OCH { get; set; }
        public ICollection<ODT> ODT { get; set; }
        public ICollection<OGT> OGT { get; set; }
        public ICollection<OST> OST { get; set; }
        public ICollection<SIF> SIF { get; set; }
        public ICollection<USF> USF { get; set; }
    }
}
