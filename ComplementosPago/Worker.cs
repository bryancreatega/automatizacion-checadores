using ComplementosPago.Controllers;
using ComplementosPago.Models;
using ComplementosPago.Services;
using ComplementosPago.ViewModels;
using Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelContext.Models;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml.Linq;


namespace ComplementosPago
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _services;
        private readonly LectoresController _lectoresController;
        private readonly RespaldoLectores _respaldoLectores;
        private readonly ExtraccionChecadas _extraccionChecadas;
        private readonly EnvioLabora _envioLabora;

        private readonly libFprZkx _libFprZkx;
        List<FPR> lstFgprs = new List<FPR>();

        public Worker(
            ILogger<Worker> logger, 
            IServiceProvider services,
            LectoresController lectoresController,
            RespaldoLectores respaldoLectores,
            ExtraccionChecadas extraccionChecadas,
            EnvioLabora envioLabora
            )
        {
            _logger = logger;
            _services = services;
            _lectoresController = lectoresController;
            _extraccionChecadas = extraccionChecadas;
            _respaldoLectores = respaldoLectores;
            _envioLabora = envioLabora;
            _libFprZkx = new libFprZkx();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                var ahora = DateTime.Now;
                var dia = ahora.ToString("dddd", new CultureInfo("es-MX")).ToLower();
                var horaMinuto = ahora.ToString("HH:mm");

                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<FingerPrintsContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var ejecuciones = await db.ATMS
                        .Where(t => t.dia.ToLower() == dia && t.hora == horaMinuto)
                        .ToListAsync();
                    

                    if (ejecuciones.Any())
                    {
                        _logger.LogInformation("Ejecutando tarea programada para {dia} a las {hora}", dia, horaMinuto);

                        var procesosAEjecutar = new List<PRO>();
                        var todosLosResumenes = new List<ResumenProceso>();

                        foreach (var ejecucion in ejecuciones)
                        {
                            if (ejecucion.ProcesoId == null)
                            {
                                var procesos = await db.PRO
                                    .Where(p => p.Activo == true)
                                    .OrderBy(p => p.Orden)
                                    .ToListAsync();

                                procesosAEjecutar.AddRange(procesos);
                            }
                            else
                            {
                                var proceso = await db.PRO
                                    .Where(p => p.Id == ejecucion.ProcesoId && p.Activo == true)
                                    .FirstOrDefaultAsync();

                                if (proceso != null)
                                {
                                    procesosAEjecutar.Add(proceso);
                                }
                            }
                        }

                        procesosAEjecutar = procesosAEjecutar

                            .GroupBy(p => p.Id)
                            .Select(g => g.First())
                            .OrderBy(p => p.Orden)
                            .ToList();

                        var estatusProcesos = await db.ESTPR.ToListAsync();
                        var bitacora = new BIT();
                        var automatizacionLog = new LOAT();


                        var lectores = await db.FPRS.Where(e => e.fpr_sttfpr == 1).ToListAsync();

                        if (procesosAEjecutar.Any() && lectores.Any())
                        {
                            _logger.LogInformation("Ejecutando {cantidad} procesos para {cantidadLectores} lectores", procesosAEjecutar.Count, lectores.Count);

                            foreach (var lector in lectores)
                            {
                                _logger.LogInformation("Procesando lector: {ip} - {numero}", lector.fpr_ipafpr, lector.fpr_numfpr);
                                List<OCD> lstOcd = new List<OCD>();

                                var resumenesLector = new List<ResumenProceso>();

                                foreach( var proceso in procesosAEjecutar)
                                {
                                    try
                                    {
                                        var existeRegistro = await db.LOAT
                                            .AnyAsync(x => x.procesoId == proceso.Id &&
                                                           x.checadorId == lector.fpr_keyfpr &&
                                                           x.fecha.Date == DateTime.Now.Date);

                                        if (!existeRegistro)
                                        {
                                            automatizacionLog = new LOAT
                                            {
                                                id = 0,
                                                procesoId = proceso.Id,
                                                checadorId = lector.fpr_keyfpr,
                                                fecha = DateTime.Now,
                                                estadoId = estatusProcesos.FirstOrDefault(e => e.Nombre == "Creado").Id,
                                                totalesPrevios = 0,
                                                totalesPosterior = 0,
                                                fechaRegistro = DateTime.Now
                                            };

                                            await db.LOAT.AddAsync(automatizacionLog);
                                            await db.SaveChangesAsync();
                                        }
                                        else
                                        {
                                            _logger.LogInformation("El registro ya existe, no se creará duplicado");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogInformation("Error al crear registro en LOAT");
                                    }
                                }

                                // Ejecutar procesos para este lector
                                foreach (var proceso in procesosAEjecutar)
                                {
                                    try
                                    {

                                        _logger.LogInformation("Ejecutando proceso: {nombre} (ID: {id}) para lector {ip}",
                                            proceso.Nombre, proceso.Id, lector.fpr_ipafpr);

                                        bool conexionExitosa = await _lectoresController.IntentarConexionLector(lector, 3, db);

                                        if (!conexionExitosa)
                                        {
                                            _logger.LogWarning("No se pudo establecer conexión con el lector {ip} después de 3 intentos. Continuando con el siguiente lector.",
                                                lector.fpr_namfpr);

                                            bitacora = new BIT
                                            {
                                                id = 0,
                                                procesoId = proceso.Id,
                                                lectorId = lector.fpr_keyfpr,
                                                descripcion = "Error de comunicación",
                                                fechaEnvio = DateTime.Now
                                            };

                                            await db.BITA.AddAsync(bitacora);
                                            await db.SaveChangesAsync();

                                            continue;
                                        }

                                        _logger.LogInformation("Conexión exitosa con el lector {ip}. Ejecutando procesos...",
                                            lector.fpr_namfpr);

                                        bitacora = new BIT
                                        {
                                            id = 0,
                                            procesoId = proceso.Id,
                                            lectorId = lector.fpr_keyfpr,
                                            descripcion = "Inicio de proceso",
                                            fechaEnvio = DateTime.Now
                                        };

                                        await db.BITA.AddAsync(bitacora);
                                        await db.SaveChangesAsync();


                                        var autoLog = await db.LOAT
                                            .Where(x => x.procesoId == proceso.Id &&
                                                           x.checadorId == lector.fpr_keyfpr &&
                                                           x.fecha.Date == DateTime.Now.Date).FirstOrDefaultAsync();

                                        autoLog.estadoId = estatusProcesos.FirstOrDefault(e => e.Nombre == "Iniciado").Id;

                                        db.LOAT.Update(autoLog);
                                        await db.SaveChangesAsync();

                                        bool resultado = false;
                                        

                                        switch (proceso.Nombre)
                                        {
                                            case "EXDI":
                                                //resultado = await _respaldoLectores.realizarRespaldoLector(lector, db , _libFprZkx, proceso.Id);
                                                resultado = true;
                                                break;
                                            case "RELE":
                                                //resultado = await _respaldoLectores.realizarRespaldoHuellas(lector, db , _libFprZkx, proceso.Id);
                                                resultado = true;
                                                break;
                                            case "EXCH":
                                                //resultado = await _extraccionChecadas.realizarExtracciones(lector, proceso.Id);
                                                resultado = false;
                                                break;
                                            case "ENLA":
                                                //resultado = await _envioLabora.realizarEnvioLabora(lector, db, proceso.Id);
                                                resultado = true;
                                                break;
                                            case "ELCH":
                                                resultado = true;
                                                break;
                                            default:
                                                _logger.LogWarning("Proceso {nombre} no existe", proceso.Nombre);
                                                resultado = false;
                                                break;
                                        }

                                        if (!resultado)
                                        {
                                            var bitacoraError = await db.BITA
                                                .Where(x => x.procesoId == proceso.Id &&
                                                       x.lectorId == lector.fpr_keyfpr &&
                                                       x.fechaEnvio.Date == DateTime.Now.Date)
                                                .OrderBy(x => x.fechaEnvio) 
                                                .LastOrDefaultAsync(); 


                                            _logger.LogError("El proceso {nombre} (Orden: {orden}) falló para el lector {nombre}.",
                                                proceso.Nombre, proceso.Orden, lector.fpr_namfpr);

                                            var procesosPendientes = procesosAEjecutar
                                                .Where(p => p.Orden > proceso.Orden)
                                                .Select(p => p.Nombre)
                                                .ToList();

                                            if (procesosPendientes.Any())
                                            {
                                                _logger.LogWarning("Procesos que no se ejecutarán para el lector {ip}: {procesosPendientes}",
                                                    lector.fpr_namfpr, string.Join(", ", procesosPendientes));
                                            }

                                            var registroLoatError = await db.LOAT
                                               .FirstOrDefaultAsync(x => x.procesoId == proceso.Id &&
                                                   x.checadorId == lector.fpr_keyfpr &&
                                                   x.fecha.Date == DateTime.Now.Date);

                                            if (registroLoatError != null)
                                            {
                                                string descripcionError = bitacoraError?.descripcion ?? $"El proceso {proceso.Nombre} (Orden: {proceso.Orden}) falló para el lector {lector.fpr_namfpr}.";

                                                resumenesLector.Add(new ResumenProceso
                                                {
                                                    LectorId = lector.fpr_keyfpr,
                                                    Lector = lector,
                                                    ProcesoId = proceso.Id,
                                                    EstadoId = registroLoatError.estadoId,
                                                    Fecha = registroLoatError.fecha,
                                                    Error = descripcionError
                                                });
                                            }

                                            break;
                                        }
                                        else
                                        {
                                            _logger.LogInformation("Proceso {nombre} (Orden: {orden}) ejecutado correctamente para el lector {ip}",
                                                proceso.Nombre, proceso.Orden, lector.fpr_namfpr);
                                            await _lectoresController.RegistrarErrorBitacora(proceso.Id, lector.fpr_keyfpr, $"Proceso {proceso.Nombre} (Orden: {proceso.Orden}) ejecutado correctamente para el lector");

                                            var registroLoat = await db.LOAT
                                                .FirstOrDefaultAsync(x => x.procesoId == proceso.Id &&
                                                    x.checadorId == lector.fpr_keyfpr &&
                                                    x.fecha.Date == DateTime.Now.Date);

                                            if (registroLoat != null)
                                            {
                                                registroLoat.estadoId = estatusProcesos.FirstOrDefault(e => e.Nombre == "Terminado").Id;

                                                db.LOAT.Update(registroLoat);
                                                await db.SaveChangesAsync();
                                            }

                                            var registroLoatGet = await db.LOAT
                                                .FirstOrDefaultAsync(x => x.procesoId == proceso.Id &&
                                                    x.checadorId == lector.fpr_keyfpr &&
                                                    x.fecha.Date == DateTime.Now.Date);

                                            if (registroLoatGet != null)
                                            {
                                                resumenesLector.Add(new ResumenProceso
                                                {
                                                    LectorId = lector.fpr_keyfpr,
                                                    Lector = lector,
                                                    ProcesoId = proceso.Id,
                                                    EstadoId = registroLoatGet.estadoId,
                                                    Fecha = registroLoatGet.fecha
                                                });
                                            }

                                        }

                                        
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, "Error al ejecutar el proceso {nombre} (ID: {id}) para el lector {ip}",
                                            proceso.Nombre, proceso.Id, lector.fpr_namfpr);

                                        if (proceso.Reintentar)
                                        {
                                            _logger.LogInformation("Reintentando proceso: {nombre} para lector {ip}",
                                                proceso.Nombre, lector.fpr_namfpr);
                                        }
                                    }
                                }

                                todosLosResumenes.AddRange(resumenesLector);

                            }

                            if (todosLosResumenes.Any())
                            {
                                try
                                {
                                    await emailService.EnviarResumenGeneralProcesos(todosLosResumenes, procesosAEjecutar, estatusProcesos);
                                    _logger.LogInformation("Correo de resumen general enviado exitosamente para {cantidad} lectores", lectores.Count);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error al enviar correo de resumen general");
                                }
                            }
                            else
                            {
                                _logger.LogInformation("No hay resúmenes para enviar por correo");
                            }
                        }
                        else
                        {
                            if (!lectores.Any())
                            {
                                _logger.LogInformation("No hay lectores activos para procesar");
                            }
                            else
                            {
                                _logger.LogInformation("No hay procesos activos para ejecutar");
                            }
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }

    }
}
