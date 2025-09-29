using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ModelContext.Models;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ComplementosPago.Services
{
    public interface IEmailService
    {
        Task<bool> EnviarResumenGeneralProcesos(List<ResumenProceso> resumenes, List<PRO> procesos, List<ESTPR> estados);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> EnviarResumenGeneralProcesos(List<ResumenProceso> resumenes, List<PRO> procesos, List<ESTPR> estados)
        {
            try
            {
                var smtpConfig = _configuration.GetSection("SmtpSettings");
                var destinatarios = _configuration.GetSection("EmailDestinatarios").Get<List<string>>();

                if (destinatarios == null || !destinatarios.Any())
                {
                    _logger.LogWarning("No hay destinatarios configurados para el correo de resumen");
                    return false;
                }

                using (var client = new SmtpClient(smtpConfig["Host"], int.Parse(smtpConfig["Port"])))
                {
                    client.EnableSsl = bool.Parse(smtpConfig["EnableSsl"]);
                    client.Credentials = new NetworkCredential(smtpConfig["UserName"], smtpConfig["Password"]);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(smtpConfig["FromEmail"], smtpConfig["FromName"]),
                        Subject = $"Resumen de Procesos Automatizados - {DateTime.Now:dd/MM/yyyy}",
                        Body = GenerarCuerpoCorreoGeneral(resumenes, procesos, estados),
                        IsBodyHtml = true
                    };

                    foreach (var destinatario in destinatarios)
                    {
                        mailMessage.To.Add(destinatario);
                    }

                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation("Correo de resumen general enviado exitosamente");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo de resumen general");
                return false;
            }
        }

        private string GenerarCuerpoCorreoGeneral(List<ResumenProceso> resumenes, List<PRO> procesos, List<ESTPR> estados)
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body { font-family: Arial, sans-serif; }
                    .container { max-width: 1200px; margin: 0 auto; }
                    .header { background-color: #f8f9fa; padding: 20px; text-align: center; }
                    .table { width: 100%; border-collapse: collapse; margin: 20px 0; }
                    .table th, .table td { border: 1px solid #ddd; padding: 12px; text-align: left; }
                    .table th { background-color: #4CAF50; color: white; }
                    .status-completed { background-color: #d4edda; color: #155724; }
                    .status-error { background-color: #f8d7da; color: #721c24; }
                    .status-processing { background-color: #fff3cd; color: #856404; }
                    .status-created { background-color: #e2e3e5; color: #383d41; }
                    .lector-header { background-color: #e9ecef; font-weight: bold; }
                    .resumen-general { background-color: #f8f9fa; padding: 15px; margin: 10px 0; }
                    .proceso-header { background-color: #dee2e6; }
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Resumen de Proceso Automatizado de Checadores</h1>
                        <h3>Fecha: " + DateTime.Now.ToString("dd/MM/yyyy") + @"</h3>
                    </div>");

            sb.AppendLine($"<p><strong>Estimado usuario:</strong></p>");
            sb.AppendLine($"<p>Se envía el estatus de la ejecución del día de hoy.</p>");

            // Resumen general
            var totalLectores = resumenes.Select(r => r.LectorId).Distinct().Count();
            var totalProcesosCompletados = resumenes.Count(r =>
                estados.FirstOrDefault(e => e.Id == r.EstadoId)?.Nombre == "Terminado");
            var totalProcesos = resumenes.Count;

            sb.AppendLine($@"
                <div class='resumen-general'>
                    <h3>Resumen General</h3>
                    <p><strong>Total de lectores procesados:</strong> {totalLectores}</p>
                    <p><strong>Total de procesos ejecutados:</strong> {totalProcesos}</p>
                    <p><strong>Procesos completados exitosamente:</strong> {totalProcesosCompletados}</p>
                </div>");

            // Tabla de resumen por lector y proceso
            sb.AppendLine(@"<table class='table'>
                <thead>
                    <tr>
                        <th>Fecha</th>
                        <th>Checador</th>");

            // Encabezados de procesos
            foreach (var proceso in procesos.OrderBy(p => p.Orden))
            {
                sb.AppendLine($"<th>{proceso.Descripcion}</th>");
            }

            sb.AppendLine(@"</tr>
                </thead>
                <tbody>");

            // Agrupar por lector
            var lectoresAgrupados = resumenes.GroupBy(r => r.LectorId);

            foreach (var grupoLector in lectoresAgrupados)
            {
                var lector = grupoLector.First().Lector;
                var registrosLector = grupoLector.ToList();

                sb.AppendLine($@"
                    <tr class='lector-header'>
                        <td>{DateTime.Now:dd/MM/yyyy}</td>
                        <td>{lector.fpr_namfpr} ({lector.fpr_ipafpr})</td>");

                // Para cada proceso, mostrar el estado
                foreach (var proceso in procesos.OrderBy(p => p.Orden))
                {
                    var registro = registrosLector.FirstOrDefault(r => r.ProcesoId == proceso.Id);
                    var estado = registro != null ?
                        estados.FirstOrDefault(e => e.Id == registro.EstadoId)?.Nombre ?? "No ejecutado" :
                        "No ejecutado";

                    string estadoClass = estado.ToLower() switch
                    {
                        "terminado" => "status-completed",
                        "error" => "status-error",
                        "iniciado" => "status-processing",
                        "creado" => "status-created",
                        _ => ""
                    };

                    sb.AppendLine($"<td class='{estadoClass}'>{estado}</td>");
                }

                sb.AppendLine("</tr>");
            }

            sb.AppendLine(@"</tbody>
                        </table>");

            // Tabla detallada
            sb.AppendLine(@"<h3>Detalle de Ejecución</h3>
                        <table class='table'>
                            <thead>
                                <tr>
                                    <th>Checador</th>
                                    <th>Proceso</th>
                                    <th>Estado</th>
                                    <th>Fecha/Hora</th>
                                    <th>Error</th>

                                </tr>
                            </thead>
                            <tbody>");

            foreach (var resumen in resumenes)
            {
                var proceso = procesos.FirstOrDefault(p => p.Id == resumen.ProcesoId);
                var estado = estados.FirstOrDefault(e => e.Id == resumen.EstadoId);

                string estadoClass = estado?.Nombre?.ToLower() switch
                {
                    "terminado" => "status-completed",
                    "error" => "status-error",
                    "iniciado" => "status-processing",
                    "creado" => "status-created",
                    _ => ""
                };

                sb.AppendLine($@"
                    <tr>
                        <td>{resumen.Lector.fpr_namfpr}</td>
                        <td>{proceso?.Descripcion ?? "N/A"}</td>
                        <td class='{estadoClass}'>{estado?.Nombre ?? "N/A"}</td>
                        <td>{resumen.Fecha:dd/MM/yyyy HH:mm}</td>                       
                        <td>{resumen.Error ?? ""}</td>

                    </tr>");
            }

            sb.AppendLine(@"</tbody>
                        </table>");

            sb.AppendLine($@"
                <div style='margin-top: 20px;'>
                    <p><strong>Saludos,</strong><br>Sistema Automatizado de Checadores</p>
                </div>");

            sb.AppendLine(@"</div></body></html>");

            return sb.ToString();
        }
    }

    public class ResumenProceso
    {
        public int LectorId { get; set; }
        public FPR Lector { get; set; }
        public int ProcesoId { get; set; }
        public int EstadoId { get; set; }
        public DateTime Fecha { get; set; }
        public string Error { get; set; }
    }
}