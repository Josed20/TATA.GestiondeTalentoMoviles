using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;

namespace TATA.GestiondeTalentoMoviles.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificadosController : ControllerBase
    {
        private readonly IB2StorageService _storageService;
        private readonly ILogger<CertificadosController> _logger;
        private const long MAX_FILE_SIZE = 10 * 1024 * 1024; // 10 MB
        private const string ALLOWED_CONTENT_TYPE = "application/pdf";

        public CertificadosController(
            IB2StorageService storageService,
            ILogger<CertificadosController> logger)
        {
            _storageService = storageService;
            _logger = logger;
        }

        /// <summary>
        /// Descarga un archivo desde B2 mediante streaming a traves del servidor
        /// </summary>
        [HttpGet("download-file")]
        public async Task<IActionResult> DownloadFile([FromQuery] string archivoPdfUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(archivoPdfUrl))
                {
                    return BadRequest(new { message = "El parámetro 'archivoPdfUrl' es requerido" });
                }

                _logger.LogInformation("Iniciando descarga de certificado: {Archivo}", archivoPdfUrl);

                var (content, contentType) = await _storageService.DownloadFileAsync(archivoPdfUrl);

                var fileName = Path.GetFileName(archivoPdfUrl) ?? "download.pdf";

                return File(content, contentType ?? "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al descargar certificado: {Message}", ex.Message);
                return StatusCode(500, new { message = "Error al descargar certificado", details = ex.Message });
            }
        }

        /// <summary>
        /// Sube un PDF de certificación a Backblaze B2
        /// </summary>
        /// <param name="file">Archivo PDF a subir</param>
        /// <param name="colaboradorId">ID del colaborador (opcional, para organización)</param>
        /// <param name="nombreCertificacion">Nombre de la certificación (para nombre de archivo amigable)</param>
        /// <returns>Respuesta con referencia del archivo subido</returns>
        [HttpPost("upload")]
        [ProducesResponseType(typeof(CertificadoUploadResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Upload(
            IFormFile? file,
            [FromQuery] string? colaboradorId,
            [FromQuery] string? nombreCertificacion)
        {
            try
            {
                // Validar que el archivo exista
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "El archivo es requerido" });
                }

                // Validar tipo de contenido
                if (file.ContentType != ALLOWED_CONTENT_TYPE)
                {
                    return BadRequest(new { message = $"Solo se permiten archivos PDF. Content-Type recibido: {file.ContentType}" });
                }

                // Validar tamaño
                if (file.Length > MAX_FILE_SIZE)
                {
                    return BadRequest(new { message = $"El archivo es demasiado grande. Máximo: {MAX_FILE_SIZE / 1024 / 1024} MB" });
                }

                // Construir nombre de archivo único
                var fileName = GenerateFileName(colaboradorId, nombreCertificacion);

                _logger.LogInformation($"Iniciando carga: {fileName} (tamaño: {file.Length} bytes)");

                // Subir a B2
                var archivoPdfUrl = await _storageService.UploadCertificateAsync(
                    file.OpenReadStream(),
                    fileName,
                    file.ContentType
                );

                // Construir respuesta
                var response = new CertificadoUploadResponseDto
                {
                    FileName = fileName,
                    ArchivoPdfUrl = archivoPdfUrl,
                    ContentType = file.ContentType,
                    FileSizeBytes = file.Length,
                    UploadedAt = DateTime.UtcNow
                };

                _logger.LogInformation($"Carga exitosa: {fileName}");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al cargar certificado: {ex.Message}", ex);
                return StatusCode(500, new { message = "Error al cargar el archivo", details = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint de diagnóstico para listar archivos recientes en el bucket B2
        /// </summary>
        [HttpGet("debug/list-files")]
        public async Task<IActionResult> ListFilesDebug([FromQuery] int maxCount = 10)
        {
            try
            {
                var files = await _storageService.ListRecentFilesAsync(maxCount);
                return Ok(new { count = files.Count, files = files });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al listar archivos: {ex.Message}", ex);
                return StatusCode(500, new { message = "Error al listar archivos", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene la URL de descarga para un certificado
        /// </summary>
        /// <param name="archivoPdfUrl">Referencia del archivo guardado en B2</param>
        /// <returns>URL de descarga completa</returns>
        [HttpGet("download-url")]
        [ProducesResponseType(typeof(CertificadoDownloadUrlResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDownloadUrl([FromQuery] string archivoPdfUrl)
        {
            try
            {
                // Validar que se proporcione la referencia
                if (string.IsNullOrWhiteSpace(archivoPdfUrl))
                {
                    return BadRequest(new { message = "El parámetro 'archivoPdfUrl' es requerido" });
                }

                _logger.LogInformation($"Generando URL de descarga para: {archivoPdfUrl}");

                // Obtener URL de descarga
                var downloadUrl = await _storageService.GetDownloadUrlAsync(archivoPdfUrl);

                var response = new CertificadoDownloadUrlResponseDto
                {
                    DownloadUrl = downloadUrl,
                    IsExternalUrl = downloadUrl.StartsWith("http://") || downloadUrl.StartsWith("https://")
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al generar URL de descarga: {ex.Message}", ex);
                return StatusCode(500, new { message = "Error al generar URL de descarga", details = ex.Message });
            }
        }

        // ====================================
        // Métodos privados
        // ====================================

        /// <summary>
        /// Genera un nombre de archivo único y organizado
        /// Formato: {colaboradorId}/{yyyy}/{MM}/{Guid}-{slug}.pdf
        /// </summary>
        private static string GenerateFileName(string? colaboradorId, string? nombreCertificacion)
        {
            var now = DateTime.UtcNow;
            var guid = Guid.NewGuid().ToString("N").Substring(0, 8); // 8 caracteres del GUID
            var slug = GenerateSlug(nombreCertificacion ?? "certificado");

            // Si no hay colaboradorId, usar una carpeta genérica
            var colabId = !string.IsNullOrWhiteSpace(colaboradorId) ? colaboradorId : "sin-asignar";

            return $"{colabId}/{now:yyyy}/{now:MM}/{guid}-{slug}.pdf";
        }

        /// <summary>
        /// Convierte un nombre en un slug seguro para usar en nombres de archivo
        /// </summary>
        private static string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "documento";

            // Convertir a minúsculas y eliminar caracteres especiales
            var slug = System.Text.RegularExpressions.Regex.Replace(
                input.ToLowerInvariant(),
                @"[^a-z0-9]+",
                "-"
            ).Trim('-');

            // Limitar a 50 caracteres
            return slug.Length > 50 ? slug.Substring(0, 50) : slug;
        }
    }
}
