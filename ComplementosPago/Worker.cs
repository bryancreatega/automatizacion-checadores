using ComplementosPago.Models;
using ComplementosPago.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModelContext.Models;
using System;
using System.Globalization;
using System.Xml.Linq;


namespace ComplementosPago
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _services;

        public Worker(ILogger<Worker> logger, IServiceProvider services)
        {
            _logger = logger;
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //await ProbarOperacionesLabora();
            //await ProbarOperacionesFingerPrintsAsync();

            //await InsertarNuevoChequeo();
            //await ActualizarChequeo();

            while (!stoppingToken.IsCancellationRequested)
            {
                var ahora = DateTime.Now;
                var dia = ahora.ToString("dddd", new CultureInfo("es-MX")).ToLower();
                var horaMinuto = ahora.ToString("HH:mm");

                using (var scope = _services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<FingerPrintsContext>();

                    var ejecuciones = await db.ATMS
                        .Where(t => t.dia.ToLower() == dia && t.hora == horaMinuto)
                        .ToListAsync(stoppingToken);

                    if (ejecuciones.Any())
                    {
                        _logger.LogInformation("Ejecutando tarea programada para {dia} a las {hora}", dia, horaMinuto);
                        await ActualizarChequeo();

                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }

        //public async Task LeerComplementosAsync(ComplementoDbContext db)
        //{
        //    var rutaBase = @"C:\Complementos";
        //    if (!Directory.Exists(rutaBase))
        //        return;

        //    var carpetasEmpresas = Directory.GetDirectories(rutaBase);

        //    foreach (var carpeta in carpetasEmpresas)
        //    {
        //        var empresa = Path.GetFileName(carpeta);
        //        var archivosXml = Directory.GetFiles(carpeta, "*.xml");
        //        int totalProcesados = 0;

        //        foreach (var archivo in archivosXml)
        //        {
        //            var nombreArchivo = Path.GetFileName(archivo);
        //            var yaExiste = await db.LecturaComplementos
        //                .AnyAsync(x => x.NombreArchivo == nombreArchivo);

        //            if (yaExiste)
        //            {
        //                db.LecturaComplementos.Add(new LecturaComplemento
        //                {
        //                    Empresa = empresa,
        //                    NombreArchivo = nombreArchivo,
        //                    Fecha = DateTime.Now,
        //                    Procesado = false,
        //                    MotivoNoProcesado = "Archivo previamente procesado"
        //                });
        //                continue;
        //            }

        //            try
        //            {
        //                var lectura = new LecturaComplemento
        //                {
        //                    Empresa = empresa,
        //                    NombreArchivo = nombreArchivo,
        //                    Fecha = DateTime.Now,
        //                    Procesado = true,
        //                    MotivoNoProcesado = null
        //                };

        //                db.LecturaComplementos.Add(lectura);
        //                await db.SaveChangesAsync(); // Necesario para obtener lectura.Id

        //                var detalles = LeerArchivoXml(archivo, empresa, lectura.Id);
        //                if (detalles != null && detalles.Any())
        //                {
        //                    db.DetalleLecturaComplementos.AddRange(detalles);
        //                }

        //                db.ComplementoEnvios.Add(new ComplementoEnvio
        //                {
        //                    Empresa = empresa,
        //                    NombreArchivo = nombreArchivo,
        //                    FechaRegistro = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        //                    Procesado = false,
        //                    ErrorSap = null,
        //                    FechaEnvio = null,
        //                    Encontrado = false
        //                });

        //                totalProcesados++;
        //            }
        //            catch (Exception ex)
        //            {
        //                db.LecturaComplementos.Add(new LecturaComplemento
        //                {
        //                    Empresa = empresa,
        //                    NombreArchivo = nombreArchivo,
        //                    Fecha = DateTime.Now,
        //                    Procesado = false,
        //                    MotivoNoProcesado = $"Error de lectura: {ex.Message}"
        //                });
        //            }
        //        }

        //        db.LogsEjecucion.Add(new LogEjecucion
        //        {
        //            Carpeta = empresa,
        //            Fecha = DateTime.Now,
        //            Archivos = totalProcesados
        //        });

        //        await db.SaveChangesAsync();
        //    }
        //}



        //public List<DetalleLecturaComplemento> LeerArchivoXml(string rutaArchivo, string empresa, int lecturaId)
        //{
        //    var xdoc = XDocument.Load(rutaArchivo);
        //    XNamespace cfdi = "http://www.sat.gob.mx/cfd/4";
        //    XNamespace pago = "http://www.sat.gob.mx/Pagos20";
        //    XNamespace tfd = "http://www.sat.gob.mx/TimbreFiscalDigital";

        //    var comprobante = xdoc.Root;
        //    var complemento = comprobante.Element(cfdi + "Complemento");
        //    var timbre = complemento.Element(tfd + "TimbreFiscalDigital");
        //    var pagos = complemento.Element(pago + "Pagos");
        //    var pagoNodo = pagos.Element(pago + "Pago");

        //    var emisor = comprobante.Element(cfdi + "Emisor");
        //    var receptor = comprobante.Element(cfdi + "Receptor");

        //    var detalles = new List<DetalleLecturaComplemento>();
        //    var doctosRelacionados = pagoNodo.Elements(pago + "DoctoRelacionado");

        //    foreach (var docto in doctosRelacionados)
        //    {
        //        detalles.Add(new DetalleLecturaComplemento
        //        {
        //            LecturaComplementoId = lecturaId,
        //            Archivo = Path.GetFileName(rutaArchivo),
        //            UUID = timbre?.Attribute("UUID")?.Value,
        //            VerificacionSat = timbre?.Attribute("SelloSAT")?.Value,
        //            Fecha = comprobante.Attribute("Fecha")?.Value?.Substring(0, 10),
        //            Hora = comprobante.Attribute("Fecha")?.Value?.Substring(11),
        //            FechaPago = pagoNodo?.Attribute("FechaPago")?.Value?.Substring(0, 10),
        //            HoraPago = pagoNodo?.Attribute("FechaPago")?.Value?.Substring(11),
        //            Serie = comprobante.Attribute("Serie")?.Value,
        //            Folio = comprobante.Attribute("Folio")?.Value,
        //            RfcRecepetor = receptor?.Attribute("Rfc")?.Value,
        //            NombreReceptor = receptor?.Attribute("Nombre")?.Value,
        //            Moneda = comprobante.Attribute("Moneda")?.Value,
        //            Monto = pagoNodo?.Attribute("Monto")?.Value,
        //            FormaPago = pagoNodo?.Attribute("FormaDePagoP")?.Value,
        //            UuidDctoRel = docto?.Attribute("IdDocumento")?.Value,
        //            SerieRel = docto?.Attribute("Serie")?.Value,
        //            FolioRel = docto?.Attribute("Folio")?.Value,
        //            MonedaRel = docto?.Attribute("MonedaDR")?.Value,
        //            NumeroParcialidad = docto?.Attribute("NumParcialidad")?.Value,
        //            ImporteSaldoAnt = docto?.Attribute("ImpSaldoAnt")?.Value,
        //            ImportePagado = docto?.Attribute("ImpPagado")?.Value,
        //            ImporteSaldoInsoluto = docto?.Attribute("ImpSaldoInsoluto")?.Value,
        //            MetodoPagoRel = docto?.Attribute("ObjetoImpDR")?.Value,
        //            RfcEmisor = emisor?.Attribute("Rfc")?.Value,
        //            NombreEmisor = emisor?.Attribute("Nombre")?.Value,
        //            Complementos = complemento.ToString(),
        //            Empresa = empresa,
        //        });
        //    }

        //    return detalles;
        //}

        private async Task ProbarOperacionesFingerPrintsAsync()
        {
            using (var scope = _services.CreateScope())
            {
                var fingerDb = scope.ServiceProvider.GetRequiredService<FingerPrintsContext>();

                try
                {
                    var bitacora = await fingerDb.BITA.ToListAsync();
                    _logger.LogInformation($"Retrieved {bitacora.Count} bitacora records");

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error con FingerPrintsContext");
                }
            }
        }

        private async Task ProbarOperacionesLabora()
        {
            using (var scope = _services.CreateScope())
            {
                var laboraDb = scope.ServiceProvider.GetRequiredService<LaboraContext>();

                try
                {
                    // Test if database can be connected to
                    if (await laboraDb.Database.CanConnectAsync())
                    {
                        _logger.LogInformation("Successfully connected to database");

                        var molochec = await laboraDb.molochec.ToListAsync();

                        _logger.LogInformation($"Retrieved {molochec.Count} nmcoempl records");
                    }
                    else
                    {
                        _logger.LogError("Cannot connect to database");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error con LaboraContext");

                    // Additional diagnostic info
                    _logger.LogError($"Connection string: {laboraDb.Database.GetConnectionString()}");
                }
            }
        }

        private async Task InsertarNuevoChequeo()
        {
            using (var scope = _services.CreateScope())
            {
                var laboraDb = scope.ServiceProvider.GetRequiredService<LaboraContext>();
                try
                {
                    var nuevoChequeo = new LBCH
                    {
                        che_keylec = "1",
                        che_keyemp = 1001,
                        che_fecche = DateTime.Now.Date,
                        che_horche = DateTime.Now.ToString("HH:mm:ss"),
                        che_status = "A",
                        che_tipche = "E",
                        che_keyper = "2024-01"
                    };

                    laboraDb.molochec.Add(nuevoChequeo);
                    await laboraDb.SaveChangesAsync();

                    _logger.LogInformation($"Nuevo chequeo insertado con éxito. ID: {nuevoChequeo.che_keylec}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al insertar nuevo chequeo");
                }
            }
        }

        private async Task ActualizarChequeo()
        {
            using (var scope = _services.CreateScope())
            {
                var laboraDb = scope.ServiceProvider.GetRequiredService<LaboraContext>();
                try
                {
                    var chequeoExistente = await laboraDb.molochec
                        .FirstOrDefaultAsync(c => c.che_keylec == "1");

                    if (chequeoExistente != null)
                    {
                        chequeoExistente.che_status = "A";
                        chequeoExistente.che_horche = DateTime.Now.ToString("HH:mm:ss");

                        laboraDb.molochec.Update(chequeoExistente);
                        await laboraDb.SaveChangesAsync();

                        _logger.LogInformation($"Chequeo 1 actualizado con éxito");
                    }
                    else
                    {
                        _logger.LogWarning($"No se encontró el chequeo con key: 1");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar chequeo");
                }
            }
        }

    }
}
