using Functions;
using Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using ModelContext.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Reflection;

namespace ComplementosPago.Controllers
{
    public class RespaldoLectores
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<RespaldoLectores> _logger;
        private readonly libFprZkx _libFprZkx;

        #region variables ping
        Ping pngFpr = new Ping();
        byte[] bffFpr = new byte[32];
        PingOptions pngOpc = new PingOptions(64, true);
        PingReply pngRpl = null;
        #endregion


        public RespaldoLectores(
            ILogger<RespaldoLectores> logger,
            IServiceProvider services
            )
        {
            _logger = logger;
            _services = services;
            _libFprZkx = new libFprZkx();
        }

        public async Task<bool> realizarRespaldoLector(FPR lector, FingerPrintsContext db)
        {
            try
            {
                _logger.LogInformation("Extrayendo información del lector {nombre}", lector.fpr_namfpr);

                #region variables de salida
                string fpr_macfpr, fpr_frmfpr, fpr_cdgfpr, fpr_plffpr, fpr_srnfpr, fpr_sdkfpr, fpr_thrfpr;
                int fpr_fpafpr, fpr_fcafpr, fpr_usrfpr, fpr_admfpr, fpr_pwdfpr, fpr_oplfpr, fpr_attfpr, fpr_facfpr, fpr_fpnfpr;
                #endregion

                int result = _libFprZkx.zktInfo(lector.fpr_numfpr, true, out fpr_macfpr, out fpr_frmfpr, out fpr_cdgfpr,
                    out fpr_plffpr, out fpr_srnfpr, out fpr_sdkfpr, out fpr_thrfpr, out fpr_fpafpr,
                    out fpr_fcafpr, out fpr_usrfpr, out fpr_admfpr, out fpr_pwdfpr, out fpr_oplfpr,
                    out fpr_attfpr, out fpr_facfpr, out fpr_fpnfpr);

                if (result == 0)
                {
                    _logger.LogError("Error al extraer información del lector {nombre}. Código error: {result}",
                        lector.fpr_namfpr, result);
                    return false;
                }

                var lectorActualizado = await db.FPRS
                    .Where(x => x.fpr_keyfpr == lector.fpr_keyfpr)
                    .FirstOrDefaultAsync();

                if (lectorActualizado != null)
                {
                    lectorActualizado.fpr_macfpr = fpr_macfpr;
                    lectorActualizado.fpr_frmfpr = fpr_frmfpr;
                    lectorActualizado.fpr_cdgfpr = fpr_cdgfpr;
                    lectorActualizado.fpr_plffpr = fpr_plffpr;
                    lectorActualizado.fpr_srnfpr = fpr_srnfpr;
                    lectorActualizado.fpr_sdkfpr = fpr_sdkfpr;
                    lectorActualizado.fpr_thrfpr = fpr_thrfpr;
                    lectorActualizado.fpr_fpafpr = fpr_fpafpr;
                    lectorActualizado.fpr_fcafpr = fpr_fcafpr;
                    lectorActualizado.fpr_usrfpr = fpr_usrfpr;
                    lectorActualizado.fpr_admfpr = fpr_admfpr;
                    lectorActualizado.fpr_pwdfpr = fpr_pwdfpr;
                    lectorActualizado.fpr_oplfpr = fpr_oplfpr;
                    lectorActualizado.fpr_attfpr = fpr_attfpr;
                    lectorActualizado.fpr_facfpr = fpr_facfpr;
                    lectorActualizado.fpr_fpnfpr = fpr_fpnfpr;
                    lectorActualizado.fpr_typfpr = fpr_fpafpr == 9 ? 1 : 2;

                    db.FPRS.Update(lectorActualizado);
                    await db.SaveChangesAsync();

                    _logger.LogInformation("Información del lector {nombre} actualizada correctamente",
                        lector.fpr_namfpr);

                    await realizarRespaldoHuellas(lector, db);

                    return true;
                }
                else
                {
                    _logger.LogWarning("No se encontró el lector {nombre} en la base de datos para actualizar",
                        lector.fpr_namfpr);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la información del lector {nombre}", lector.fpr_namfpr);
                return false;
            }
        }


        private async Task<bool> realizarRespaldoHuellas(FPR lector, FingerPrintsContext db)
        {
            try
            {
                
                OPR opr = new OPR();
                opr.opr_keyopr = 0;
                opr.opr_desopr = "Resplado de huellas OnDemand";
                opr.opr_opropr = 1;
                opr.opr_datopr = DateTime.Now;
                opr.opr_horopr = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                opr.opr_timopr = 0;
                opr.usu_keyusu = 4;
                opr.opr_staopr = 0;
                db.OPRS.Add(opr);
                db.SaveChanges();
                db.Entry(opr).GetDatabaseValues();

                int odt_linodt = 0;
                var insFprs = new ODT
                {
                    odt_linodt = odt_linodt++,
                    odt_datodt = DateTime.Now,
                    odt_horodt = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                    odt_ndtodt = DateTime.Now,
                    odt_nhrodt = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second),
                    odt_msgodt = "",
                    odt_staodt = 0,
                    odt_typodt = 1,
                    opr_keyopr = opr.opr_keyopr,
                    fpr_keyfpr = lector.fpr_keyfpr,
                    fpr_namfpr = lector.fpr_namfpr,
                    fpr_numfpr = lector.fpr_numfpr
                };

                await db.ODTS.AddAsync(insFprs);
                await db.SaveChangesAsync();

                var proceso = await procesoDeRespaldo(lector, db, opr.opr_keyopr);

                if (proceso)
                {
                    return true;
                }
                else
                {
                    return false;
                }

                
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio el siguiente error al inciar el proceso del respaldo: \n" + ex.ToString());
                return false;
            }
        }

        private async Task<bool> procesoDeRespaldo(FPR lector, FingerPrintsContext db, int oprKey)
        {
            try
            {
                int numLect = 0;
                OPR entPrc;
                List<ODT> lstOdt;

                entPrc = db.OPRS.AsNoTracking().FirstOrDefault(x => x.opr_keyopr == oprKey);
                lstOdt = db.ODTS.AsNoTracking()
                    .Where(x => x.opr_keyopr == oprKey)
                    .OrderBy(x => x.fpr_numfpr)
                    .ToList();

                ODT odtPrc = lstOdt.FirstOrDefault(x => x.fpr_keyfpr == lector.fpr_keyfpr);

                var tarea = Task.Run(() => EjecutarRespaldoAsync(lector, entPrc, odtPrc, db));
                bool completada = tarea.Wait(TimeSpan.FromMinutes(3));

                if (completada)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocurrio el siguiente error al inciar el proceso del respaldo: \n" + ex.ToString());
                return false;
            }

        }

        private async Task EjecutarRespaldoAsync(FPR ent, OPR entPrc, ODT odtPrc, FingerPrintsContext db)
        {
            try
            {
                _logger.LogInformation($"[{DateTime.Now}] INICIO respaldo lector {ent.fpr_numfpr}");


                _logger.LogInformation($"[{DateTime.Now}] Enviando ping al lector {ent.fpr_ipafpr}");

                pngRpl = pngFpr.Send(ent.fpr_ipafpr, 1000, bffFpr, pngOpc);

                if (pngRpl.Status == IPStatus.Success && pngRpl.RoundtripTime < 3000)
                {
                    _logger.LogInformation($"[{DateTime.Now}] Ping exitoso ({pngRpl.RoundtripTime} ms). Conectando...");

                    if (_libFprZkx.zktConx(ent.fpr_ipafpr, ent.fpr_numfpr))
                    {
                        _logger.LogInformation($"[{DateTime.Now}] Conexión establecida con lector {ent.fpr_numfpr}. Descargando huellas...");

                        var lstBck = _libFprZkx.zktDown(
                            ent.fpr_numfpr,
                            true,
                            ent.fpr_keyfpr,
                            ent.fpr_numfpr,
                            ent.fpr_typfpr,
                            odtPrc.odt_keyodt
                        );

                        _logger.LogInformation($"[{DateTime.Now}] Huellas descargadas: {lstBck.Count}");

                        if (lstBck.Count > 0)
                        {
                            var buscando = lstBck.Where(x => x.stf_numstf == "4580").ToList();
                            _logger.LogInformation($"[{DateTime.Now}] Insertando respaldo en base de datos...");
                            insBckp(entPrc, odtPrc, lstBck, db);
                            prcSave(db, odtPrc, ent, 1, "Respaldo Realizado con el Lector ");
                        }
                        else
                        {
                            _logger.LogWarning($"[{DateTime.Now}] No se encontraron huellas.");
                            prcSave(db, odtPrc, ent, 4, "Sin Huellas en el Lector ");
                        }
                    }
                    else
                    {
                        _logger.LogError($"[{DateTime.Now}] Falló la conexión con el lector.");
                        prcSave(db, odtPrc, ent, 2, "Conexión Fallida con el Lector ");
                    }
                }
                else
                {
                    _logger.LogError($"[{DateTime.Now}] Ping fallido o tiempo excedido.");
                    prcSave(db, odtPrc, ent, 3, "Ping Fallido con el Lector ");
                }


                _logger.LogInformation($"[{DateTime.Now}] FIN respaldo lector {ent.fpr_numfpr}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now}] ERROR lector {ent.fpr_numfpr}: {ex.Message}");
            }
        }

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
                fprPrc.fpr_datfpr = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                fprPrc.fpr_horfpr = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
                fprPrc.fpr_sttfpr = 1;
                db.FPRS.Update(fprPrc);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                string error = string.Empty;
                error = ex.ToString();
            }
        }

        private void insBckp(OPR prcPrcs, ODT prcPdtl, List<OBK> lstBcks, FingerPrintsContext dbc)
        {

                dbc.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                try
                {
                    // Cargar datos requeridos
                    var odtKey = prcPdtl.odt_keyodt;
                    var fprKey = lstBcks.FirstOrDefault()?.fpr_keyfpr ?? 0;
                    var infFprs = dbc.OBAK.AsNoTracking().Where(x => x.odt_keyodt == odtKey).ToList();
                    var infStfs = dbc.STFS.ToList();

                    // Eliminar huellas existentes del mismo lector
                    var huellasRegistradas = dbc.OBAK.AsNoTracking().Where(f => f.fpr_keyfpr == fprKey).ToList();
                    dbc.OBAK.RemoveRange(huellasRegistradas);

                    dbc.SaveChanges();

                    

                    foreach (var hu in lstBcks)
                    {
                        var stfkey = dbc.STFS.AsNoTracking().Where(x => x.stf_numstf == hu.stf_numstf);

                        if (stfkey.Any())
                        {
                            hu.stf_keystf = stfkey.FirstOrDefault().stf_keystf;
                        }
                    }

                    dbc.OBAK.AddRange(lstBcks);

                    // Guardar TODO de una sola vez
                    dbc.SaveChanges();

                    updPrcs(prcPrcs, prcPdtl, dbc);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error general en el guardado");
                }
            
        }

        private void updPrcs(OPR prcPrcs, ODT prcPdtl, FingerPrintsContext dbc)
        {

            try
            {
                dbc.ODTS.Update(prcPdtl);
                dbc.SaveChanges();

                var cont = dbc.ODTS.AsNoTracking().Where(x => x.opr_keyopr == prcPrcs.opr_keyopr && x.odt_staodt != 0).Count();

                int prc_runprc = prcPrcs.opr_runopr;
                prcPrcs.opr_staopr = prcPrcs.opr_runopr == 2 ? 2 : 1;
                dbc.OPRS.Update(prcPrcs);
                dbc.SaveChanges();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error general en el guardado");

            }
        }

    }
}
