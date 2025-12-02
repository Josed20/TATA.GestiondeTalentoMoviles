using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MongoDB.Driver;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using Microsoft.Extensions.Logging;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Services
{
    /// <summary>
    /// Implementación del servicio de dashboard de alertas unificado
    /// </summary>
    public class AlertaDashboardService : IAlertaDashboardService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Colaborador> _colaboradores;
        private readonly IMongoCollection<Vacante> _vacantes;
        private readonly IMongoCollection<Evaluacion> _evaluaciones;
        private readonly IMongoCollection<Alerta> _alertas;
        private readonly IMongoCollection<User> _usuarios;
        private readonly ILogger<AlertaDashboardService> _logger;

        public AlertaDashboardService(IMongoDatabase database, ILogger<AlertaDashboardService> logger)
        {
            _database = database;
            _logger = logger;
            _colaboradores = database.GetCollection<Colaborador>("colaboradores");
            _vacantes = database.GetCollection<Vacante>("vacantes");
            _evaluaciones = database.GetCollection<Evaluacion>("evaluaciones");
            _alertas = database.GetCollection<Alerta>("alertas");
            _usuarios = database.GetCollection<User>("users");
        }

        /// <summary>
        /// Obtiene todas las alertas para el administrador (GAPs y Alertas Genéricas pendientes)
        /// </summary>
        public async Task<IEnumerable<AlertaDashboardDto>> ObtenerAlertasAdminAsync()
        {
            var alertasResult = new List<AlertaDashboardDto>();

            // 1. Obtener todas las evaluaciones con GAPs
            var evaluaciones = await _evaluaciones.Find(_ => true).ToListAsync();
            foreach (var evaluacion in evaluaciones)
            {
                if (evaluacion.SkillsEvaluados != null)
                {
                    var skillsConGap = evaluacion.SkillsEvaluados
                        .Where(s => s.NivelActual < s.NivelRecomendado)
                        .ToList();

                    foreach (var skill in skillsConGap)
                    {
                        var colaborador = await _colaboradores.Find(c => c.Id == evaluacion.ColaboradorId).FirstOrDefaultAsync();
                        var nombreColaborador = colaborador != null ? $"{colaborador.Nombres} {colaborador.Apellidos}" : "Desconocido";

                        alertasResult.Add(new AlertaDashboardDto
                        {
                            IdReferencia = evaluacion.Id,
                            Titulo = $"GAP en {skill.Nombre} - {nombreColaborador}",
                            Mensaje = $"Nivel actual: {skill.NivelActual}, Nivel recomendado: {skill.NivelRecomendado}",
                            Fecha = evaluacion.FechaEvaluacion.ToString("yyyy-MM-dd"),
                            TipoOrigen = "SKILL_GAP",
                            Activa = true,
                            ColorPrioridad = "ROJO"
                        });
                    }
                }
            }

            // 2. Obtener alertas genéricas pendientes
            var alertasPendientes = await _alertas.Find(a => a.Estado == "PENDIENTE").ToListAsync();
            foreach (var alerta in alertasPendientes)
            {
                alertasResult.Add(new AlertaDashboardDto
                {
                    IdReferencia = alerta.Id,
                    Titulo = $"Alerta {alerta.Tipo}",
                    Mensaje = alerta.Detalle?.Descripcion ?? "Sin descripción",
                    Fecha = alerta.FechaCreacion.ToString("yyyy-MM-dd"),
                    TipoOrigen = "GENERICA",
                    Activa = true,
                    ColorPrioridad = "AMARILLO"
                });
            }

            return alertasResult.OrderByDescending(a => a.ColorPrioridad == "ROJO" ? 3 : (a.ColorPrioridad == "AMARILLO" ? 2 : 1));
        }

        /// <summary>
        /// Obtiene alertas consolidadas para un colaborador específico
        /// </summary>
        public async Task<IEnumerable<AlertaDashboardDto>> ObtenerAlertasColaboradorAsync(string usuarioId)
        {
            var alertasResult = new List<AlertaDashboardDto>();

            // Obtener el usuario y su colaboradorId
            var usuario = await _usuarios.Find(u => u.Id == usuarioId).FirstOrDefaultAsync();
            if (usuario == null || string.IsNullOrEmpty(usuario.ColaboradorId))
            {
                return alertasResult;
            }

            var colaboradorId = usuario.ColaboradorId;
            var colaborador = await _colaboradores.Find(c => c.Id == colaboradorId).FirstOrDefaultAsync();

            if (colaborador == null)
            {
                return alertasResult;
            }

            // 1. CERTIFICACIONES POR VENCER O VENCIDAS
            if (colaborador.Certificaciones != null)
            {
                var hoy = DateTime.UtcNow;
                foreach (var cert in colaborador.Certificaciones)
                {
                    if (cert.FechaVencimiento.HasValue)
                    {
                        var diasParaVencer = (cert.FechaVencimiento.Value - hoy).Days;

                        if (diasParaVencer < 0)
                        {
                            // Ya venció - ROJO
                            alertasResult.Add(new AlertaDashboardDto
                            {
                                IdReferencia = cert.CertificacionId ?? colaboradorId,
                                Titulo = $"Certificación Vencida: {cert.Nombre}",
                                Mensaje = $"Venció hace {Math.Abs(diasParaVencer)} días",
                                Fecha = cert.FechaVencimiento.Value.ToString("yyyy-MM-dd"),
                                TipoOrigen = "CERTIFICACION",
                                Activa = true,
                                ColorPrioridad = "ROJO"
                            });
                        }
                        else if (diasParaVencer <= 60)
                        {
                            // Por vencer en menos de 60 días - AMARILLO
                            alertasResult.Add(new AlertaDashboardDto
                            {
                                IdReferencia = cert.CertificacionId ?? colaboradorId,
                                Titulo = $"Certificación por Vencer: {cert.Nombre}",
                                Mensaje = $"Vence en {diasParaVencer} días",
                                Fecha = cert.FechaVencimiento.Value.ToString("yyyy-MM-dd"),
                                TipoOrigen = "CERTIFICACION",
                                Activa = true,
                                ColorPrioridad = "AMARILLO"
                            });
                        }
                    }
                }
            }

            // 2. BRECHAS DE SKILLS (GAPs)
            var evaluaciones = await _evaluaciones.Find(e => e.ColaboradorId == colaboradorId).ToListAsync();
            foreach (var evaluacion in evaluaciones)
            {
                if (evaluacion.SkillsEvaluados != null)
                {
                    var skillsConGap = evaluacion.SkillsEvaluados
                        .Where(s => s.NivelActual < s.NivelRecomendado)
                        .ToList();

                    foreach (var skill in skillsConGap)
                    {
                        alertasResult.Add(new AlertaDashboardDto
                        {
                            IdReferencia = evaluacion.Id,
                            Titulo = $"Mejorar Skill: {skill.Nombre}",
                            Mensaje = $"Tu nivel es {skill.NivelActual}, se recomienda {skill.NivelRecomendado}",
                            Fecha = evaluacion.FechaEvaluacion.ToString("yyyy-MM-dd"),
                            TipoOrigen = "SKILL_GAP",
                            Activa = true,
                            ColorPrioridad = "ROJO"
                        });
                    }
                }
            }

            // 3. ALERTAS GENÉRICAS DIRIGIDAS AL USUARIO
            var alertasGenericas = await _alertas.Find(a =>
                a.Estado == "PENDIENTE" &&
                a.Destinatarios != null &&
                a.Destinatarios.Any(d => d.UsuarioId == usuarioId)
            ).ToListAsync();

            foreach (var alerta in alertasGenericas)
            {
                alertasResult.Add(new AlertaDashboardDto
                {
                    IdReferencia = alerta.Id,
                    Titulo = $"Notificación: {alerta.Tipo}",
                    Mensaje = alerta.Detalle?.Descripcion ?? "Tienes una notificación pendiente",
                    Fecha = alerta.FechaCreacion.ToString("yyyy-MM-dd"),
                    TipoOrigen = "GENERICA",
                    Activa = true,
                    ColorPrioridad = "AMARILLO"
                });
            }

            // 4. VACANTES ABIERTAS (DISPONIBLES)
            var vacantesAbiertas = await _vacantes.Find(v => v.EstadoVacante == "ABIERTA").ToListAsync();
            foreach (var vacante in vacantesAbiertas)
            {
                alertasResult.Add(new AlertaDashboardDto
                {
                    IdReferencia = vacante.Id,
                    Titulo = $"Vacante Disponible: {vacante.NombrePerfil}",
                    Mensaje = $"Área: {vacante.Area} - Rol: {vacante.RolLaboral}",
                    Fecha = vacante.FechaInicio.ToString("yyyy-MM-dd"),
                    TipoOrigen = "VACANTE_DISPONIBLE",
                    Activa = true,
                    ColorPrioridad = "VERDE"
                });
            }

            // Ordenar por prioridad: ROJO > AMARILLO > VERDE
            return alertasResult.OrderByDescending(a =>
                a.ColorPrioridad == "ROJO" ? 3 : (a.ColorPrioridad == "AMARILLO" ? 2 : 1)
            );
        }

        /// <summary>
        /// Anuncia una vacante por correo electrónico a todos los colaboradores activos
        /// </summary>
        public async Task<bool> AnunciarVacantePorCorreoAsync(string vacanteId)
        {
            try
            {
                _logger.LogInformation("=== INICIO: Proceso de anuncio de vacante ===");
                _logger.LogInformation($"VacanteId recibido: {vacanteId}");

                // 1. Buscar la vacante
                _logger.LogInformation("Paso 1: Buscando vacante en MongoDB...");
                var vacante = await _vacantes.Find(v => v.Id == vacanteId).FirstOrDefaultAsync();
                
                if (vacante == null)
                {
                    _logger.LogError($"? Vacante no encontrada con ID: {vacanteId}");
                    throw new InvalidOperationException("Vacante no encontrada");
                }
                
                _logger.LogInformation($"? Vacante encontrada: {vacante.NombrePerfil}");

                // 2. Validar que esté ABIERTA
                _logger.LogInformation($"Paso 2: Validando estado de vacante. Estado actual: {vacante.EstadoVacante}");
                if (vacante.EstadoVacante != "ABIERTA")
                {
                    _logger.LogError($"? La vacante no está abierta. Estado: {vacante.EstadoVacante}");
                    throw new InvalidOperationException($"La vacante no está abierta. Estado actual: {vacante.EstadoVacante}");
                }
                
                _logger.LogInformation("? Vacante está ABIERTA");

                // 3. Obtener colaboradores activos
                _logger.LogInformation("Paso 3: Buscando colaboradores activos...");
                var colaboradoresActivos = await _colaboradores
                    .Find(c => c.Estado == "ACTIVO")
                    .ToListAsync();

                _logger.LogInformation($"Colaboradores activos encontrados: {colaboradoresActivos.Count}");

                if (!colaboradoresActivos.Any())
                {
                    _logger.LogError("? No hay colaboradores activos");
                    throw new InvalidOperationException("No hay colaboradores activos para notificar");
                }

                // 4. Preparar lista de correos
                _logger.LogInformation("Paso 4: Extrayendo correos electrónicos...");
                var destinatarios = colaboradoresActivos
                    .Where(c => !string.IsNullOrEmpty(c.Correo))
                    .Select(c => c.Correo)
                    .ToList();

                _logger.LogInformation($"Correos válidos encontrados: {destinatarios.Count}");
                foreach (var email in destinatarios)
                {
                    _logger.LogInformation($"  - {email}");
                }

                if (!destinatarios.Any())
                {
                    _logger.LogError("? No hay correos válidos");
                    throw new InvalidOperationException("No hay correos válidos de colaboradores");
                }

                // 5. Construir el correo HTML
                _logger.LogInformation("Paso 5: Construyendo cuerpo HTML del correo...");
                var cuerpoHtml = ConstruirCuerpoCorreoVacante(vacante);
                _logger.LogInformation($"? HTML generado (longitud: {cuerpoHtml.Length} caracteres)");

                // 6. Enviar el correo usando SMTP de Gmail
                _logger.LogInformation("Paso 6: Enviando correo...");
                var asunto = $"Nueva Vacante Disponible: {vacante.NombrePerfil}";
                await EnviarCorreoAsync(destinatarios, asunto, cuerpoHtml);

                _logger.LogInformation("=== FIN: Proceso completado exitosamente ===");
                return true;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"? Error de operación: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"? Error inesperado al anunciar vacante: {ex.GetType().Name}");
                _logger.LogError($"Mensaje: {ex.Message}");
                _logger.LogError($"StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"InnerException: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        /// <summary>
        /// Construye el cuerpo HTML del correo para anunciar una vacante
        /// </summary>
        private string ConstruirCuerpoCorreoVacante(Vacante vacante)
        {
            var skillsHtml = vacante.SkillsRequeridos != null && vacante.SkillsRequeridos.Any()
                ? string.Join("", vacante.SkillsRequeridos.Select(s =>
                    $"<li><strong>{s.Nombre}</strong> ({s.Tipo}) - Nivel {s.NivelDeseado}{(s.EsCritico ? " ? Crítico" : "")}</li>"))
                : "<li>No especificados</li>";

            var certificacionesHtml = vacante.CertificacionesRequeridas != null && vacante.CertificacionesRequeridas.Any()
                ? string.Join("", vacante.CertificacionesRequeridas.Select(c => $"<li>{c}</li>"))
                : "<li>No requeridas</li>";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f9f9f9; }}
        .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background-color: white; padding: 30px; border-radius: 0 0 8px 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .section {{ margin-bottom: 20px; }}
        .section-title {{ color: #007bff; font-size: 18px; font-weight: bold; margin-bottom: 10px; }}
        .detail {{ margin-bottom: 10px; }}
        .label {{ font-weight: bold; color: #555; }}
        ul {{ padding-left: 20px; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #777; }}
        .urgencia {{ display: inline-block; padding: 5px 10px; border-radius: 4px; font-weight: bold; }}
        .urgencia-alta {{ background-color: #dc3545; color: white; }}
        .urgencia-media {{ background-color: #ffc107; color: black; }}
        .urgencia-baja {{ background-color: #28a745; color: white; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>?? Nueva Oportunidad de Carrera</h1>
        </div>
        <div class='content'>
            <div class='section'>
                <h2 style='color: #007bff;'>{vacante.NombrePerfil}</h2>
                <p>Nos complace informarte que tenemos una nueva vacante disponible que podría ser de tu interés.</p>
            </div>

            <div class='section'>
                <div class='section-title'>?? Información General</div>
                <div class='detail'><span class='label'>Área:</span> {vacante.Area}</div>
                <div class='detail'><span class='label'>Rol Laboral:</span> {vacante.RolLaboral}</div>
                <div class='detail'><span class='label'>Fecha de Inicio:</span> {vacante.FechaInicio:dd/MM/yyyy}</div>
                <div class='detail'>
                    <span class='label'>Urgencia:</span> 
                    <span class='urgencia urgencia-{vacante.Urgencia.ToLower()}'>{vacante.Urgencia}</span>
                </div>
            </div>

            <div class='section'>
                <div class='section-title'>?? Skills Requeridos</div>
                <ul>{skillsHtml}</ul>
            </div>

            <div class='section'>
                <div class='section-title'>?? Certificaciones Requeridas</div>
                <ul>{certificacionesHtml}</ul>
            </div>

            <div class='section'>
                <p style='background-color: #e7f3ff; padding: 15px; border-left: 4px solid #007bff; margin-top: 20px;'>
                    <strong>¿Te interesa?</strong><br>
                    Si cumples con los requisitos y deseas aplicar a esta vacante, 
                    por favor contacta al departamento de Recursos Humanos o accede a la plataforma 
                    de gestión de talento para más información.
                </p>
            </div>
        </div>
        <div class='footer'>
            <p>Este es un correo automático. Por favor no respondas a este mensaje.</p>
            <p>© {DateTime.Now.Year} TATA - Gestión de Talento Móviles</p>
        </div>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// Envía correo electrónico usando Gmail SMTP con logs detallados
        /// </summary>
        private async Task EnviarCorreoAsync(List<string> destinatarios, string asunto, string cuerpoHtml)
        {
            const string smtpHost = "smtp.gmail.com";
            const int smtpPort = 587;
            const string fromEmail = "mellamonose19@gmail.com";
            const string fromPassword = "mpwcevagvfehlfer";

            _logger.LogInformation("=== Configuración SMTP ===");
            _logger.LogInformation($"Host: {smtpHost}");
            _logger.LogInformation($"Port: {smtpPort}");
            _logger.LogInformation($"From: {fromEmail}");
            _logger.LogInformation($"SSL: True");
            _logger.LogInformation($"Destinatarios (BCC): {destinatarios.Count}");
            _logger.LogInformation($"Asunto: {asunto}");

            try
            {
                using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(fromEmail, fromPassword);
                    smtpClient.Timeout = 30000; // 30 segundos
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    _logger.LogInformation("Cliente SMTP configurado");

                    using (var mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(fromEmail, "TATA - Gestión de Talento");
                        mailMessage.Subject = asunto;
                        mailMessage.Body = cuerpoHtml;
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Priority = MailPriority.Normal;

                        // Agregar destinatarios como BCC para privacidad
                        _logger.LogInformation("Agregando destinatarios...");
                        foreach (var email in destinatarios)
                        {
                            try
                            {
                                mailMessage.Bcc.Add(new MailAddress(email));
                                _logger.LogInformation($"  ? Agregado: {email}");
                            }
                            catch (FormatException ex)
                            {
                                _logger.LogWarning($"  ?? Email inválido ignorado: {email} - {ex.Message}");
                            }
                        }

                        if (mailMessage.Bcc.Count == 0)
                        {
                            _logger.LogError("? No hay destinatarios válidos para enviar");
                            throw new InvalidOperationException("No hay destinatarios válidos");
                        }

                        _logger.LogInformation($"Total destinatarios válidos: {mailMessage.Bcc.Count}");
                        _logger.LogInformation("Enviando correo...");

                        await smtpClient.SendMailAsync(mailMessage);

                        _logger.LogInformation("? ¡CORREO ENVIADO EXITOSAMENTE!");
                        _logger.LogInformation($"? Correo enviado a {mailMessage.Bcc.Count} destinatarios");
                        Console.WriteLine($"? Correo enviado exitosamente a {mailMessage.Bcc.Count} destinatarios");
                    }
                }
            }
            catch (SmtpException ex)
            {
                _logger.LogError("? Error SMTP al enviar correo");
                _logger.LogError($"StatusCode: {ex.StatusCode}");
                _logger.LogError($"Mensaje: {ex.Message}");
                _logger.LogError($"InnerException: {ex.InnerException?.Message}");
                throw new InvalidOperationException($"Error SMTP: {ex.Message}. Verifica las credenciales y la conexión de red.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"? Error inesperado al enviar correo: {ex.GetType().Name}");
                _logger.LogError($"Mensaje: {ex.Message}");
                _logger.LogError($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
