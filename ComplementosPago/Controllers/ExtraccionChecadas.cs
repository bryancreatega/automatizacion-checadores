using Library;
using Microsoft.EntityFrameworkCore;
using ModelContext.Models;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace ComplementosPago.Controllers
{
    public class ExtraccionChecadas
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<ExtraccionChecadas> _logger;
        private readonly libFprZkx _libFprZkx;

        #region variables ping
        Ping pngFpr = new Ping();
        byte[] bffFpr = new byte[32];
        PingOptions pngOpc = new PingOptions(64, true);
        PingReply pngRpl = null;
        #endregion

        int odt_keyodt = 0;
        ODT odtPrc = new ODT();
        List<OCH> lstOch = new List<OCH>();
        List<OCH> lstChc = new List<OCH>();
        List<OCH> lstCht = new List<OCH>();
        List<OCD> lstOcd = new List<OCD>();
        List<OCD> filteredZerolstOcd = new List<OCD>();

        public ExtraccionChecadas(
            ILogger<ExtraccionChecadas> logger,
            IServiceProvider services
            )
        {
            _logger = logger;
            _services = services;
            _libFprZkx = new libFprZkx();
        }

        public async Task<bool> realizarExtracciones(FPR lector)
        {
            using (var scope = _services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<FingerPrintsContext>();

                try
                {
                    _logger.LogInformation("Iniciando extracción de marcajes para lector: {nombre} ({ip})",
                        lector.fpr_namfpr, lector.fpr_ipafpr);

                    OPR opr = new OPR
                    {
                        opr_keyopr = 0,
                        opr_desopr = "Extracción de Marcajes Automática",
                        opr_opropr = 4,
                        opr_datopr = DateTime.Now,
                        opr_horopr = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                        opr_timopr = 0,
                        usu_keyusu = 4,
                        opr_staopr = 0
                    };

                    db.OPRS.Add(opr);
                    await db.SaveChangesAsync();

                    int opr_keyopr = opr.opr_keyopr;

                    // Verificar si el ID se generó correctamente
                    if (opr_keyopr <= 0)
                    {
                        // Si no se generó el ID, intentar obtener el último registro insertado
                        var ultimoOpr = await db.OPRS
                            .OrderByDescending(x => x.opr_keyopr)
                            .FirstOrDefaultAsync();

                        if (ultimoOpr != null)
                        {
                            opr_keyopr = ultimoOpr.opr_keyopr;
                            opr = ultimoOpr; // Actualizar la referencia
                        }
                        else
                        {
                            _logger.LogError("No se pudo obtener el ID del registro OPR insertado");
                            return false;
                        }
                    }

                    _logger.LogInformation("Registro OPR creado con ID: {id}", opr_keyopr);

                    ODT odt = new ODT
                    {
                        odt_linodt = 0,
                        odt_datodt = DateTime.Now,
                        odt_horodt = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                        odt_ndtodt = DateTime.Now,
                        odt_nhrodt = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                        odt_msgodt = "",
                        odt_staodt = 0,
                        odt_typodt = 1,
                        opr_keyopr = opr_keyopr,
                        fpr_keyfpr = lector.fpr_keyfpr,
                        fpr_namfpr = lector.fpr_namfpr,
                        fpr_numfpr = lector.fpr_numfpr
                    };

                    db.ODTS.Add(odt);
                    await db.SaveChangesAsync();

                    bool extraccionExitosa = await extraerChecadas(lector, db, odt);

                    if (extraccionExitosa)
                    {
                        odt.odt_staodt = 1;
                        odt.odt_msgodt = "Extracción completada exitosamente";
                        opr.opr_staopr = 1;

                        _logger.LogInformation("Extracción completada para lector: {nombre}", lector.fpr_namfpr);
                    }
                    else
                    {
                        odt.odt_staodt = 2;
                        odt.odt_msgodt = "Error en la extracción";
                        opr.opr_staopr = 2;

                        _logger.LogWarning("Extracción falló para lector: {nombre}", lector.fpr_namfpr);
                    }

                    await db.SaveChangesAsync();
                    return extraccionExitosa;

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en InformacionPorLector para lector: {nombre}", lector.fpr_namfpr);
                    return false;
                }
            }
        }

        private async Task<bool> extraerChecadas(FPR lector, FingerPrintsContext db, ODT odt)
        {
            int numLect = 0;
            var mSegundos = false;

            var rSegundos = db.PAR.AsNoTracking().Where(x => x.Tipo == "Checadas" && x.Descripcion == "Segundos");

            if (rSegundos.Any())
            {
                mSegundos = rSegundos.FirstOrDefault().Mostrar;
            }

            Task tskOpr = null;
            lstOch = new List<OCH>();

            #region creación de la tarea
            tskOpr = Task.Factory.StartNew(() =>
            {
                numLect++;

                try
                {
                    pngRpl = pngFpr.Send(lector.fpr_ipafpr, 1000, bffFpr, pngOpc);
                    #region ping establecido
                    if (pngRpl.Status == IPStatus.Success && pngRpl.RoundtripTime < 3000)
                    {
                        #region conexion establecida
                        if (_libFprZkx.zktConx(lector.fpr_ipafpr, lector.fpr_numfpr))
                        {
                            odt_keyodt = odtPrc.odt_keyodt;
                            lstOch = _libFprZkx.zktChec(lector.fpr_numfpr, true, lector.fpr_keyfpr, lector.fpr_numfpr, lector.fpr_typfpr, 1, odtPrc.odt_keyodt, mSegundos);
                            #region con huellas
                            if (lstOch.Count() > 0)
                            {
                                if (odtPrc != null)
                                {
                                    prcSave(db, odt, lector, 1, "Extraccón de Marcajes Realizado con el Lector ");
                                }
                            }
                            #endregion
                            #region sin huellas
                            else
                            {
                                if (odtPrc != null)
                                {
                                    prcSave(db, odt, lector, 4, "Sin Marcajes en el Lector ");
                                }
                            }
                            #endregion
                        }
                        #endregion
                        #region conexion fallida
                        else
                        {
                            if (odtPrc != null)
                            {
                                prcSave(db, odt, lector, 2, "Conexión Fallida con el Lector ");
                            }
                        }
                        #endregion
                    }
                    #endregion
                    #region ping fallido
                    else
                    {
                        if (odtPrc != null)
                        {
                            prcSave(db, odt, lector, 3, "Ping Fallido con el Lector ");
                        }
                    }
                    #endregion
 
                }
                catch (Exception ex)
                {
                    try
                    {
                        _logger.LogError("Ocurrio el siguiente error: " + ex.Message + "; Lector: " + lector.fpr_numfpr.ToString(), "bckError");
                    }
                    catch (Exception exf)
                    {
                        _logger.LogError("Aplicacion", "Ocurrio el siguiente error " + exf.Message);
                    }
                }
            });
            #endregion
            tskOpr.Wait(TimeSpan.FromMinutes(3));
            if (tskOpr.Status == TaskStatus.Canceled || tskOpr.Status == TaskStatus.RanToCompletion)
            {
                var periodo = "";

                using (var scope = _services.CreateScope())
                {
                    var laboraDb = scope.ServiceProvider.GetRequiredService<LaboraContext>();

                    #region llenado de periodos
                    var per = (laboraDb.nmloperi.AsNoTracking().Where(x => x.per_keynom == 1 && x.per_persel == "*").Select(x => new LBPR
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
                    periodo = lstPrd.per_keyper;
                    #endregion
                }


                #region lista a instertar huellas del respaldo
                DateTime dttVali = DateTime.Now;
                var lstMark = db.OCHE.AsNoTracking().Where(x => x.fpr_keyfpr == lector.fpr_keyfpr).ToList();
                // x.och_datoch.Year == dttVali.Year && 
                // && x.och_datoch.Month == dttVali.Month && x.och_datoch.Day == dttVali.Day
                try
                {
                    var insOchk = (from tbl1 in lstOch
                                   join tbl2 in lstMark on new { tbl1.och_datoch.Date, tbl1.och_horoch, tbl1.stf_numstf, tbl1.fpr_keyfpr } equals new { tbl2.och_datoch.Date, tbl2.och_horoch, tbl2.stf_numstf, tbl2.fpr_keyfpr } into tmp
                                   from tbl3 in tmp.DefaultIfEmpty()
                                   select new OCH
                                   {
                                       och_keyoch = tbl3 == null ? 0 : tbl3.och_keyoch,
                                       stf_numstf = tbl3 == null ? tbl1.stf_numstf : tbl3.stf_numstf,
                                       och_datoch = tbl3 == null ? tbl1.och_datoch : tbl3.och_datoch,
                                       och_horoch = tbl3 == null ? tbl1.och_horoch : tbl3.och_horoch,
                                       och_dtpoch = tbl3 == null ? tbl1.och_dtpoch : tbl3.och_dtpoch,
                                       och_hrpoch = tbl3 == null ? tbl1.och_hrpoch : tbl3.och_hrpoch,
                                       och_stzoch = tbl3 == null ? tbl1.och_stzoch : tbl3.och_stzoch,
                                       och_staoch = tbl3 == null ? tbl1.och_staoch : tbl3.och_staoch,
                                       och_fstoch = tbl3 == null ? tbl1.och_fstoch : tbl3.och_fstoch,
                                       och_typoch = tbl3 == null ? tbl1.och_typoch : tbl3.och_typoch,
                                       fpr_keyfpr = tbl3 == null ? tbl1.fpr_keyfpr : tbl3.fpr_keyfpr,
                                       fpr_numfpr = tbl3 == null ? tbl1.fpr_numfpr : tbl3.fpr_numfpr,
                                       odt_keyodt = tbl1.odt_keyodt
                                   }).ToList();

                    lstChc = insOchk.Where(x => x.och_keyoch == 0)
                                    .GroupBy(x => new { x.och_keyoch, x.stf_numstf, x.och_datoch, x.och_horoch, x.fpr_keyfpr, x.fpr_numfpr })//x.och_dtpoch, x.och_hrpoch, 
                                    .Select(x => new OCH
                                    {
                                        och_keyoch = x.Key.och_keyoch,
                                        stf_numstf = x.Key.stf_numstf,
                                        och_datoch = x.Key.och_datoch,
                                        och_horoch = x.Key.och_horoch,
                                        och_dtpoch = DateTime.Now.Date,
                                        och_hrpoch = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0),
                                        och_stzoch = "0",
                                        och_staoch = 1,
                                        och_fstoch = 0,
                                        och_typoch = 1,
                                        fpr_keyfpr = x.Key.fpr_keyfpr,
                                        fpr_numfpr = x.Key.fpr_numfpr,
                                        odt_keyodt = odt.odt_keyodt
                                    }).ToList();
                    lstCht.AddRange(lstChc);
                    db.OCHE.UpdateRange(lstChc.ToList());//insOchk.Where(x => x.och_keyoch == 0).ToList());//(lstOch);//ctx.OBAK.AddRange(insObak);//Where(x => x.och_keyoch == 0).//Go062019.
                    db.SaveChanges();

                    #region llenado de OCHD
                    var nuevosOch = db.OCHE.AsQueryable().Where(x => x.fpr_keyfpr == lector.fpr_keyfpr).ToList();
                    var lstOdw = nuevosOch.Select(x => new OCD
                    {
                        ocd_keyocd = 0,
                        och_keyoch = x.och_keyoch,
                        stf_numstf = x.stf_numstf,
                        ocd_datocd = x.och_datoch,
                        ocd_horocd = x.och_horoch,
                        ocd_dtpocd = x.och_dtpoch,
                        ocd_hrpocd = x.och_hrpoch,
                        ocd_stzocd = x.och_stzoch,
                        ocd_fstocd = x.och_fstoch,
                        ocd_typocd = x.och_typoch,
                        fpr_keyfpr = x.fpr_keyfpr,
                        fpr_numfpr = x.fpr_numfpr,
                        ocd_staocd = x.och_staoch
                    }).ToList();
                    var lstMard = db.OCHD.AsQueryable().Where(x => x.fpr_keyfpr == lector.fpr_keyfpr).ToList();
                    var insOchd = (from tbl1 in lstOdw
                                   join tbl2 in lstMard on new { tbl1.ocd_datocd, tbl1.ocd_horocd, tbl1.stf_numstf, tbl1.fpr_keyfpr } equals new { tbl2.ocd_datocd, tbl2.ocd_horocd, tbl2.stf_numstf, tbl2.fpr_keyfpr } into tmp
                                   from tbl3 in tmp.DefaultIfEmpty()
                                   select new OCD
                                   {
                                       ocd_keyocd = tbl3 == null ? 0 : tbl3.ocd_keyocd,
                                       stf_numstf = tbl3 == null ? tbl1.stf_numstf : tbl3.stf_numstf,
                                       ocd_datocd = tbl3 == null ? tbl1.ocd_datocd : tbl3.ocd_datocd,
                                       ocd_horocd = tbl3 == null ? tbl1.ocd_horocd : tbl3.ocd_horocd,
                                       ocd_dtpocd = tbl3 == null ? tbl1.ocd_dtpocd : tbl3.ocd_dtpocd,
                                       ocd_hrpocd = tbl3 == null ? tbl1.ocd_hrpocd : tbl3.ocd_hrpocd,
                                       ocd_stzocd = tbl3 == null ? tbl1.ocd_stzocd : tbl3.ocd_stzocd,
                                       ocd_fstocd = tbl3 == null ? tbl1.ocd_fstocd : tbl3.ocd_fstocd,
                                       ocd_typocd = tbl3 == null ? tbl1.ocd_typocd : tbl3.ocd_typocd,
                                       fpr_keyfpr = tbl3 == null ? tbl1.fpr_keyfpr : tbl3.fpr_keyfpr,
                                       fpr_numfpr = tbl3 == null ? tbl1.fpr_numfpr : tbl3.fpr_numfpr,
                                       ocd_staocd = tbl3 == null ? tbl1.ocd_staocd : tbl3.ocd_staocd,
                                       och_keyoch = tbl3 == null ? tbl1.och_keyoch : tbl3.och_keyoch,
                                       per_keyper = ""
                                   }).ToList();
                    var lstChd = insOchd.Where(x => x.ocd_keyocd == 0)
                                        .GroupBy(x => new { x.ocd_keyocd, x.och_keyoch, x.stf_numstf, x.ocd_datocd, x.ocd_horocd, x.fpr_keyfpr, x.fpr_numfpr })
                                        .Select(x => new OCD
                                        {
                                            ocd_keyocd = x.Key.ocd_keyocd,
                                            stf_numstf = x.Key.stf_numstf,
                                            ocd_datocd = x.Key.ocd_datocd,
                                            ocd_horocd = x.Key.ocd_horocd,
                                            ocd_dtpocd = DateTime.Now.Date,
                                            ocd_hrpocd = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0),
                                            ocd_stzocd = "0",
                                            ocd_fstocd = 0,
                                            ocd_typocd = 1,
                                            fpr_keyfpr = x.Key.fpr_keyfpr,
                                            fpr_numfpr = x.Key.fpr_numfpr,
                                            ocd_staocd = 0,
                                            och_keyoch = x.Key.och_keyoch,
                                            per_keyper = periodo
                                        }).ToList();
                    db.OCHD.UpdateRange(lstChd);
                    db.SaveChanges();
                    lstOcd.AddRange(lstChd);
                    filteredZerolstOcd = lstChd;
                    #endregion
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error al guardar las checadas ");
                    return false;
                }
                

                

            }
            else
            {
                _logger.LogError("El Lector no responde ");
                return false;
            }
           
        }
        #endregion


        private void prcSave(FingerPrintsContext db, ODT odtPrc, FPR fprPrc, int staPrc = 1, string msgPrc = "Respaldo Realizado con el Lector ")
        {
            try
            {
                odtPrc.odt_staodt = staPrc;
                odtPrc.odt_ndtodt = DateTime.Now;
                odtPrc.odt_nhrodt = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                odtPrc.odt_msgodt = msgPrc + "-" + fprPrc.fpr_numfpr.ToString() + "-" + fprPrc.fpr_namfpr;
                db.ODTS.Update(odtPrc);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                string error = string.Empty;
                error = ex.ToString();
            }
           
        }



    }



}