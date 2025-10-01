using Library;
using Microsoft.EntityFrameworkCore;
using ModelContext.Models;

namespace ComplementosPago.Controllers
{
    public class LectoresController
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<LectoresController> _logger;
        private readonly libFprZkx _libFprZkx;


        public LectoresController(
            ILogger<LectoresController> logger,
            IServiceProvider services
            )
        {
            _logger = logger;
            _services = services;
            _libFprZkx = new libFprZkx();
        }

        public async Task<bool> IntentarConexionLector(FPR lector, int maxIntentos, FingerPrintsContext db)
        {
            for (int intento = 1; intento <= maxIntentos; intento++)
            {
                try
                {
                    _logger.LogInformation("Intento {intento} de conexión con lector {nombre}", intento, lector.fpr_namfpr);

                    if (_libFprZkx.zktConx(lector.fpr_ipafpr, lector.fpr_numfpr))
                    {
                        _logger.LogInformation("Conexión exitosa con lector {nombre} en el intento {intento}",
                            lector.fpr_namfpr, intento);
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("Fallo en el intento {intento} de conexión con lector {nombre}",
                            intento, lector.fpr_namfpr);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en el intento {intento} de conexión con lector {nombre}",
                        intento, lector.fpr_namfpr);
                }

                if (intento < maxIntentos)
                {
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            }

            return false;
        }

        public async Task RegistrarErrorBitacora(int procesoId, int lectorId, string descripcion)
        {
            try
            {
                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<FingerPrintsContext>();

                    var bitacora = new BIT
                    {
                        id = 0,
                        procesoId = procesoId,
                        lectorId = lectorId,
                        descripcion = descripcion,
                        fechaEnvio = DateTime.Now
                    };

                    await db.BITA.AddAsync(bitacora);
                    await db.SaveChangesAsync();

                    _logger.LogError($"Error registrado en bitácora: {descripcion}");
                }
                    
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al registrar en bitácora: {ex.Message}");
            }
        }

        public async Task RegistrarBitacora(int procesoId, int lectorId, string descripcion)
        {
            try
            {
                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<FingerPrintsContext>();

                    var bitacora = new BIT
                    {
                        id = 0,
                        procesoId = procesoId,
                        lectorId = lectorId,
                        descripcion = descripcion,
                        fechaEnvio = DateTime.Now
                    };

                    await db.BITA.AddAsync(bitacora);
                    await db.SaveChangesAsync();

                    _logger.LogInformation($"Registrado en bitácora: {descripcion}");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al registrar en bitácora: {ex.Message}");
            }
        }

    }
}
