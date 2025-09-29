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
        private readonly LectoresController _lectoresController;
        //private readonly libFprZkx _libFprZkx;

        #region variables ping
        Ping pngFpr = new Ping();
        byte[] bffFpr = new byte[32];
        PingOptions pngOpc = new PingOptions(64, true);
        PingReply pngRpl = null;
        #endregion

        int lectorId = 0;
        int procesoId = 0;


        public RespaldoLectores(
            ILogger<RespaldoLectores> logger,
            IServiceProvider services,
            LectoresController lectoresController
            )
        {
            _logger = logger;
            _services = services;
            _lectoresController = lectoresController;
            //_libFprZkx = new libFprZkx();
        }

        public async Task<bool> realizarRespaldoLector(FPR lector, FingerPrintsContext db, libFprZkx _libFprZkx, int procesoId)
        {
            try
            {
                this.lectorId = lector.fpr_keyfpr;
                this.procesoId = procesoId;

                _logger.LogInformation("Extrayendo información del lector {nombre}", lector.fpr_namfpr);

                #region variables de salida
                string fpr_macfpr, fpr_frmfpr, fpr_cdgfpr, fpr_plffpr, fpr_srnfpr, fpr_sdkfpr, fpr_thrfpr;
                int fpr_fpafpr, fpr_fcafpr, fpr_usrfpr, fpr_admfpr, fpr_pwdfpr, fpr_oplfpr, fpr_attfpr, fpr_facfpr, fpr_fpnfpr;
                #endregion

                int result = 0;

                if (_libFprZkx.zktConx(lector.fpr_ipafpr, lector.fpr_numfpr))
                {
                    result = _libFprZkx.zktInfo(lector.fpr_numfpr, true, out fpr_macfpr, out fpr_frmfpr, out fpr_cdgfpr,
                    out fpr_plffpr, out fpr_srnfpr, out fpr_sdkfpr, out fpr_thrfpr, out fpr_fpafpr,
                    out fpr_fcafpr, out fpr_usrfpr, out fpr_admfpr, out fpr_pwdfpr, out fpr_oplfpr,
                    out fpr_attfpr, out fpr_facfpr, out fpr_fpnfpr);






                    if (result == 0)
                    {
                        _logger.LogError("Error al extraer información del lector {nombre}. Código error: {result}",
                            lector.fpr_namfpr, result);
                        await _lectoresController.RegistrarErrorBitacora(this.procesoId, this.lectorId, $"Error al extraer información del lector {lector.fpr_namfpr}. Código error: {result}");
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

                        await realizarRespaldoHuellas(lector, db, _libFprZkx);

                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("No se encontró el lector {nombre} en la base de datos para actualizar",
                            lector.fpr_namfpr);
                        await _lectoresController.RegistrarErrorBitacora(this.procesoId, this.lectorId, $"No se encontró el lector {lector.fpr_namfpr} en la base de datos para actualizar");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la información del lector {nombre}", lector.fpr_namfpr);
                await _lectoresController.RegistrarErrorBitacora(this.procesoId, this.lectorId, $"Error al procesar la información del lector {lector.fpr_namfpr}");

                return false;
            }
        }


        private async Task<bool> realizarRespaldoHuellas(FPR lector, FingerPrintsContext db, libFprZkx _libFprZkx)
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
                opr.usu_keyusu = 1;
                opr.opr_staopr = 0;
                await db.OPRS.AddAsync(opr);
                await db.SaveChangesAsync();

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

                var proceso = await procesoDeRespaldo(lector, db, opr.opr_keyopr, _libFprZkx);

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
                await _lectoresController.RegistrarErrorBitacora(this.procesoId, this.lectorId, $"Ocurrio el siguiente error al inciar el proceso del respaldo: {ex.ToString()}");

                return false;
            }
        }

        private async Task<bool> procesoDeRespaldo(FPR lector, FingerPrintsContext db, int oprKey, libFprZkx _libFprZkx)
        {
            try
            {
                int numLect = 0;
                OPR entPrc;
                List<ODT> lstOdt;

                entPrc = await db.OPRS.FirstOrDefaultAsync(x => x.opr_keyopr == oprKey);
                lstOdt = await db.ODTS
                    .Where(x => x.opr_keyopr == oprKey)
                    .OrderBy(x => x.fpr_numfpr)
                    .ToListAsync();

                ODT odtPrc = lstOdt.FirstOrDefault(x => x.fpr_keyfpr == lector.fpr_keyfpr);


                //var tarea = await EjecutarRespaldoAsync(lector, entPrc, odtPrc, db, _libFprZkx);
                //bool completada =  tarea;
                var tarea = Task.Run(() => EjecutarRespaldoAsync(lector, entPrc, odtPrc, db, _libFprZkx));
                bool completada = tarea.Wait(TimeSpan.FromMinutes(10));

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
                await _lectoresController.RegistrarErrorBitacora(this.procesoId, this.lectorId, $"Ocurrio el siguiente error al inciar el proceso del respaldo: {ex.ToString()}");

                return false;
            }

        }

        private async Task EjecutarRespaldoAsync(FPR ent, OPR entPrc, ODT odtPrc, FingerPrintsContext db, libFprZkx _libFprZkx)
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
                            await insBckp(entPrc, odtPrc, lstBck, db);
                            prcSave(db, odtPrc, ent, 1, "Respaldo Realizado con el Lector ");
                        }
                        else
                        {
                            _logger.LogWarning($"[{DateTime.Now}] No se encontraron huellas.");
                            await _lectoresController.RegistrarErrorBitacora(this.procesoId, this.lectorId, $"Sin Huellas en el Lector");

                            prcSave(db, odtPrc, ent, 4, "Sin Huellas en el Lector ");
                        }
                    }
                    else
                    {
                        _logger.LogError($"[{DateTime.Now}] Falló la conexión con el lector.");
                        await _lectoresController.RegistrarErrorBitacora(this.procesoId, this.lectorId, $"Conexión Fallida con el Lector");

                        prcSave(db, odtPrc, ent, 2, "Conexión Fallida con el Lector ");
                    }
                }
                else
                {
                    _logger.LogError($"[{DateTime.Now}] Ping fallido o tiempo excedido.");
                    await _lectoresController.RegistrarErrorBitacora(this.procesoId, this.lectorId, $"Ping Fallido con el Lector");

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

        private async Task insBckp(OPR prcPrcs, ODT prcPdtl, List<OBK> lstBcks, FingerPrintsContext db)
        {

            try
            {
                using (var scope = _services.CreateScope())
                {
                    var dbc = scope.ServiceProvider.GetRequiredService<FingerPrintsContext>();
                    // Cargar datos requeridos
                    var odtKey = prcPdtl.odt_keyodt;
                    var fprKey = lstBcks.FirstOrDefault()?.fpr_keyfpr ?? 0;

                    // Eliminar huellas existentes del mismo lector
                    var huellasRegistradas = await dbc.OBAK.Where(f => f.fpr_keyfpr == fprKey).ToListAsync();

                    var registroLoat = await db.LOAT
                            .FirstOrDefaultAsync(x => x.procesoId == this.procesoId &&
                                                  x.checadorId == this.lectorId &&
                                                  x.fecha.Date == DateTime.Now.Date);


                    dbc.OBAK.RemoveRange(huellasRegistradas);

                    dbc.SaveChanges();


                    foreach (var hu in lstBcks)
                    {
                        var stf = dbc.STFS
                            .AsNoTracking()
                            .FirstOrDefault(x => x.stf_numstf == hu.stf_numstf);

                        if (stf != null)
                        {
                            hu.stf_keystf = stf.stf_keystf;
                        }
                    }

                    if (registroLoat != null)
                    {
                        registroLoat.totalesPrevios = huellasRegistradas.Count();
                        registroLoat.totalesPosterior = lstBcks.Count();

                        dbc.LOAT.Update(registroLoat);
                    }

                    await dbc.OBAK.AddRangeAsync(lstBcks);

                    // Guardar TODO de una sola vez
                    await dbc.SaveChangesAsync();

                    await updPrcs(prcPrcs, prcPdtl, dbc);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error general en el guardado");
            }

        }

        private async Task updPrcs(OPR prcPrcs, ODT prcPdtl, FingerPrintsContext dbc)
        {

            try
            {
                dbc.ODTS.Update(prcPdtl);
                await dbc.SaveChangesAsync();

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


        public async Task<bool> realizarEliminacionChecadasLector(FPR lector, FingerPrintsContext db, LaboraContext lc, libFprZkx _libFprZkx, int procesoId)
        {
            try
            {
                this.lectorId = lector.fpr_keyfpr;
                this.procesoId = procesoId;

                // Validar día 1
                if (DateTime.Now.Day != 1)
                {
                    _logger.LogInformation("Hoy no es día 1, no se ejecuta la eliminación de checadas para el lector {nombre}", lector.fpr_namfpr);
                    return true;
                }

                _logger.LogInformation("Validando checadas antiguas del lector {nombre}", lector.fpr_namfpr);

                // Calcular fechas
                DateTime fechaFin = DateTime.Now.AddMonths(-2);
                DateTime fechaIni = DateTime.MinValue;

                // Obtener counts
                int countOchd = await db.OCHD
                    .Where(x => x.fpr_numfpr == lector.fpr_numfpr
                             && x.ocd_datocd >= fechaIni
                             && x.ocd_datocd <= fechaFin)
                    .CountAsync();

                int countOche = await db.OCHE
                    .Where(x => x.fpr_numfpr == lector.fpr_numfpr
                             && x.och_datoch >= fechaIni
                             && x.och_datoch <= fechaFin)
                    .CountAsync();

                int countLbch = await lc.molochec
                    .Where(x => x.che_keylec == lector.fpr_numfpr.ToString()
                             && x.che_fecche >= fechaIni
                             && x.che_fecche <= fechaFin)
                    .CountAsync();

                // Validar igualdad de counts
                if (!(countOchd == countOche && countOche == countLbch))
                {
                    _logger.LogWarning("No se eliminaron checadas: los conteos no coinciden para el lector {nombre}. OCHD={ochd}, OCHE={oche}, LBCH={lbch}",
                        lector.fpr_namfpr, countOchd, countOche, countLbch);

                    await _lectoresController.RegistrarErrorBitacora(this.procesoId, this.lectorId,
                        $"No se eliminaron checadas: los conteos no coinciden para el lector {lector.fpr_namfpr}. OCHD={countOchd}, OCHE={countOche}, LBCH={countLbch}");

                    return false;
                }

                // Si counts coinciden -> conectar y ejecutar SDK
                if (_libFprZkx.zktConx(lector.fpr_ipafpr, lector.fpr_numfpr))
                {
                    bool resultado = _libFprZkx.zktDatt(lector.fpr_numfpr, true, fechaIni, fechaFin);

                    if (!resultado)
                    {
                        _logger.LogError("Error al eliminar checadas del lector {nombre} entre {ini} y {fin}",
                            lector.fpr_namfpr, fechaIni, fechaFin);

                        await _lectoresController.RegistrarErrorBitacora(this.procesoId, this.lectorId,
                            $"Error al eliminar checadas del lector {lector.fpr_namfpr} entre {fechaIni} y {fechaFin}");

                        return false;
                    }

                    // Si zktDatt fue exitoso -> eliminar registros de ochd y oche
                    var ochdAEliminar = await db.OCHD
                        .Where(x => x.fpr_numfpr == lector.fpr_numfpr
                                 && x.ocd_datocd >= fechaIni
                                 && x.ocd_datocd <= fechaFin)
                        .ToListAsync();

                    var ocheAEliminar = await db.OCHE
                        .Where(x => x.fpr_numfpr == lector.fpr_numfpr
                                 && x.och_datoch >= fechaIni
                                 && x.och_datoch <= fechaFin)
                        .ToListAsync();

                    db.OCHD.RemoveRange(ochdAEliminar);
                    db.OCHE.RemoveRange(ocheAEliminar);
                    await db.SaveChangesAsync();

                    _logger.LogInformation("Checadas del lector {nombre} eliminadas correctamente y registros de OCHD/OCHE borrados.", lector.fpr_namfpr);
                    return true;
                }
                else
                {
                    _logger.LogWarning("No se pudo conectar al lector {nombre} para eliminar checadas", lector.fpr_namfpr);

                    await _lectoresController.RegistrarErrorBitacora(this.procesoId, this.lectorId,
                        $"No se pudo conectar al lector {lector.fpr_namfpr} para eliminar checadas");

                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la eliminación de checadas en el lector {nombre}", lector.fpr_namfpr);

                await _lectoresController.RegistrarErrorBitacora(this.procesoId, this.lectorId,
                    $"Error al procesar la eliminación de checadas en el lector {lector.fpr_namfpr}");

                return false;
            }
        }



    }



}
