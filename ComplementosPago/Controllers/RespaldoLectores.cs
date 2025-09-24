using Library;
using Microsoft.EntityFrameworkCore;
using ModelContext.Models;

namespace ComplementosPago.Controllers
{
    public class RespaldoLectores
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<RespaldoLectores> _logger;
        private readonly libFprZkx _libFprZkx;


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


    }
}
