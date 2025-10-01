using Functions;
using Library;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ModelContext.Models;
using System.ComponentModel;
using System.Data;

namespace ComplementosPago.Controllers
{
    public class EnvioLabora
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<EnvioLabora> _logger;
        private readonly libFprZkx _libFprZkx;
        private readonly IConfiguration _configuration;
        private readonly funFprGra _funFprGra;
        private readonly LectoresController _lectoresController;

        List<FPR> lstFpr = new List<FPR>();
        List<OCH> lstOch = new List<OCH>();
        List<OCH> lstChc = new List<OCH>();
        List<OCH> lstCht = new List<OCH>();
        List<LBCH> lstChe = new List<LBCH>(); 
        List<OCD> lstChd = new List<OCD>();

        int lectorId = 0;
        int procesoId = 0;
        private DateTime _fechaProceso;    

        public EnvioLabora(
            ILogger<EnvioLabora> logger,
            IServiceProvider services,
            IConfiguration configuration,
            funFprGra funFprGra,
            LectoresController lectoresController
            )
        {
            _logger = logger;
            _services = services;
            _libFprZkx = new libFprZkx();
            _configuration = configuration;
            _funFprGra = funFprGra;
            _lectoresController = lectoresController;
            _fechaProceso = DateTime.Now.Date;
        }

        public async Task<bool> realizarEnvioLabora(FPR lector, FingerPrintsContext db, int procesoId)
        {
            this.lectorId = lector.fpr_keyfpr;
            this.procesoId = procesoId;
            try
            {
                _logger.LogInformation("Enviando información del lector {nombre} a labora", lector.fpr_namfpr);

                DataTable datLab = null;
                string prd_nomprd = string.Empty;

                using (var scope = _services.CreateScope())
                {
                    var laboraDb = scope.ServiceProvider.GetRequiredService<LaboraContext>();

                    #region llenado de periodos
                    var per = await funFprGra.graLsAs(laboraDb.nmloperi.AsNoTracking().Where(x => x.per_keynom == 1 && x.per_persel == "*").Select(x => new LBPR
                    {
                        per_keypro = x.per_keypro,
                        per_keynom = x.per_keynom,
                        per_keyper = x.per_keyper,
                        per_fecini = x.per_fecini,
                        per_fecfin = x.per_fecfin,
                        per_nummes = x.per_nummes,
                        per_impnom = x.per_impnom == null ? 0 : x.per_impnom,
                        per_totemp = x.per_totemp == null ? 0 : x.per_totemp,
                        per_persel = x.per_persel
                    }));
                    var lstPrd = per.GroupBy(x => x.per_keyper).Select(x => new { per_keyper = x.Key, per_maxper = x.Count() }).OrderByDescending(x => x.per_maxper).FirstOrDefault();
                    prd_nomprd = lstPrd.per_keyper;
                    #endregion

                }



                List<OCD> lstMolc = new List<OCD>();
                List<OCD> lstOcd = new List<OCD>();

                

                lstOcd = await db.OCHD.Where(e => e.fpr_numfpr == lector.fpr_numfpr).ToListAsync();

                var totalPrev = lstOcd.Where(e => e.per_keyper == prd_nomprd && e.ocd_staocd == 1);

                if (lstOcd.Count() > 0)
                {
                    lstChd = lstOcd.Select(x => { x.per_keyper = prd_nomprd; return x; }).ToList();
                    db.OCHD.UpdateRange(lstChd);
                    db.SaveChanges();
                    lstMolc = lstChd;
                }

                using (var scope = _services.CreateScope())
                {
                    var laboraDb = scope.ServiceProvider.GetRequiredService<LaboraContext>();
                    #region llenado de checadas
                    int anio = DateTime.Now.Year - 1;
                    var mol = await funFprGra.graLsAs(laboraDb.molochec.AsNoTracking().Where(x => x.che_fecche.Year >= anio));
                    var lcr = lstMolc.Select(x => new { che_keyemp = Convert.ToInt32(x.stf_numstf), che_fecche = x.ocd_datocd.Date, che_horche = x.ocd_horocd.ToString().Substring(0, 8), che_keylec = x.fpr_numfpr.ToString() })
                                      .GroupBy(x => new { x.che_keyemp, x.che_fecche, x.che_horche, x.che_keylec }).ToList();
                    var lcd = lcr.Select(x => new { x.Key.che_keyemp, x.Key.che_fecche, x.Key.che_horche, x.Key.che_keylec }).ToList();
                    //var lcd = lstMolc.Select(x => new { che_keyemp = Convert.ToInt32(x.stf_numstf), che_fecche = x.ocd_datocd.Date, che_horche = x.ocd_horocd.ToString().Substring(0, 8), che_keylec = x.fpr_numfpr.ToString() }).ToList();
                    var mcl = (from tbl1 in lcd
                               join tbl2 in mol on new { tbl1.che_keyemp, tbl1.che_fecche.Date, tbl1.che_horche } equals new { tbl2.che_keyemp, tbl2.che_fecche.Date, tbl2.che_horche } into tmp
                               from tbl3 in tmp.DefaultIfEmpty()
                               select new LBCH
                               {
                                   che_keylec = tbl3 == null ? tbl1.che_keylec : tbl3.che_keylec,
                                   che_keyemp = tbl3 == null ? tbl1.che_keyemp : tbl3.che_keyemp,
                                   che_fecche = tbl3 == null ? tbl1.che_fecche : tbl3.che_fecche,
                                   che_horche = tbl3 == null ? tbl1.che_horche : tbl3.che_horche,
                                   che_status = tbl3 == null ? "0" : "1", //null,
                                   che_tipche = "", //null,
                                   che_keyper = prd_nomprd //null,
                               }).ToList();
                    #endregion
                    var lstChl = mcl.Where(x => x.che_status == "0").ToList();

                    string tst = string.Empty;
                    lstChe = (from tbl1 in lstChl//lstCht
                                  select new LBCH
                                  {
                                      che_keylec = tbl1.che_keylec,
                                      che_keyemp = tbl1.che_keyemp,
                                      che_fecche = tbl1.che_fecche,
                                      che_horche = tbl1.che_horche,
                                      che_status = "", //null,
                                      che_tipche = "", //null,
                                      che_keyper = prd_nomprd //null,
                                                              //che_fecjor = tbl1.och_dtpoch,
                                  }).ToList();

                    // Eliminar duplicados por keyemp, fecche y horche
                    lstChe = lstChe
                        .GroupBy(x => new { x.che_keyemp, x.che_fecche, x.che_horche })
                        .Select(g => g.First())
                        .ToList();

                    if (lstChe.Count() == 0)
                    {
                        _logger.LogWarning("No hay información del lector {ip} para enviar a labora",
                        lector.fpr_namfpr);
                        await _lectoresController.RegistrarErrorBitacora(this.procesoId, this.lectorId, $"No hay información del lector para enviar a labora");
                        return true;
                    }


                    datLab = funFprGra.graLsdt(lstChe);
                }
                //datLab = funFprGra.graLsdt(lstChl);

                if (funFprGra.graInsb(datLab, "molochec"))
                {
                    int totalPosterior = lstChe.Count();

                    var lstOCHD = lstMolc.Select(x => { x.ocd_staocd = 1; x.per_keyper = prd_nomprd; return x; }).ToList();
                    db.OCHD.UpdateRange(lstOCHD);
                    //db.OCHD.UpdateRange(lstMolc);
                    db.SaveChanges();

                    var registroLoat = await db.LOAT
                       .FirstOrDefaultAsync(x => x.procesoId == this.procesoId &&
                                             x.checadorId == this.lectorId &&
                                             x.fecha.Date == DateTime.Now.Date);

                    if (registroLoat != null)
                    {
                        registroLoat.totalesPrevios = totalPrev.Count();
                        registroLoat.totalesPosterior = totalPrev.Count()+totalPosterior;
                        
                        db.LOAT.Update(registroLoat);
                        await db.SaveChangesAsync(); 
                    }

                    lstCht = new List<OCH>();
                }
                else
                {
                    _logger.LogError("La exportación a Labora a fallado");
                    await _lectoresController.RegistrarErrorBitacora(this.procesoId, this.lectorId, $"La exportación a Labora a fallado");
                    return false;
                }

                _logger.LogInformation("Información del lector {ip} actualizada correctamente",
                    lector.fpr_namfpr);

                return true;
                

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la información del lector {nombre}", lector.fpr_namfpr);
                await _lectoresController.RegistrarErrorBitacora(this.procesoId, this.lectorId, $"Error al procesar la información del lector: {lector.fpr_namfpr}");
                return false;
            }
        }
    }
}
