using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces;
using TATA.GestiondeTalentoMoviles.CORE.Core.Options;

namespace TATA.GestiondeTalentoMoviles.CORE.Infrastructure.Storage
{
    /// <summary>
    /// Implementación del servicio de almacenamiento en Backblaze B2
    /// </summary>
    public class BackblazeB2StorageService : IB2StorageService
    {
        private readonly BackblazeB2Options _options;
        private readonly ILogger<BackblazeB2StorageService> _logger;
        private readonly HttpClient _httpClient;

        // URLs de endpoints de B2
        private const string B2_AUTHORIZE_ACCOUNT_URL = "https://api.backblazeb2.com/b2api/v2/b2_authorize_account";
        private string? _authorizationToken;
        private string? _apiUrl;
        private string? _downloadUrl;
        private string? _accountId;

        // Upload specifics
        private string? _uploadUrl;
        private string? _uploadAuthToken;
        private string? _bucketId;

        public BackblazeB2StorageService(
            IOptions<BackblazeB2Options> options,
            ILogger<BackblazeB2StorageService> logger,
            HttpClient httpClient)
        {
            _options = options.Value;
            _logger = logger;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Sube un certificado (PDF) a Backblaze B2
        /// </summary>
        public async Task<string> UploadCertificateAsync(Stream fileStream, string fileName, string contentType)
        {
            try
            {
                _logger.LogInformation("Iniciando carga de archivo a B2: {FileName}", fileName);

                // Paso 1: Autenticarse con B2
                await AuthenticateAsync();

                // Paso 2: Resolver bucket
                var (bucketId, bucketName) = await ResolveBucketAsync();

                // Paso 3: Obtener URL de carga para el bucket
                await GetUploadUrlAsync(bucketId);

                // Paso 4: Subir archivo usando uploadUrl y uploadAuthToken
                var uploadedFileName = await UploadFileAsync(fileStream, fileName, contentType);

                _logger.LogInformation("Archivo cargado exitosamente a B2: {FileName} (bucket: {BucketName}, id: {BucketId})", uploadedFileName, bucketName, bucketId);

                // Devolver la referencia del archivo que se usar? en la BD
                return uploadedFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir archivo a B2: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtiene la URL de descarga para un archivo
        /// </summary>
        public async Task<string> GetDownloadUrlAsync(string archivoPdfUrl)
        {
            try
            {
                // Si ya es una URL completa, devolverla tal cual
                if (archivoPdfUrl.StartsWith("http://") || archivoPdfUrl.StartsWith("https://"))
                {
                    return archivoPdfUrl;
                }

                // Si es solo el fileName, construir la URL de descarga usando DownloadUrlBase tal cual
                return $"{_options.DownloadUrlBase}/{archivoPdfUrl}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener URL de descarga: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Elimina un archivo de B2
        /// </summary>
        public async Task<bool> DeleteFileAsync(string fileName)
        {
            try
            {
                _logger.LogInformation("Intentando eliminar archivo de B2: {FileName}", fileName);

                // Implementación futura: usar b2_delete_file_version
                _logger.LogWarning("Eliminación de archivos aún no implementada para: {FileName}", fileName);

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar archivo de B2: {Message}", ex.Message);
                return false;
            }
        }

        public async Task<IReadOnlyList<string>> ListRecentFilesAsync(int maxCount = 10)
        {
            try
            {
                await AuthenticateAsync();
                var (bucketId, bucketName) = await ResolveBucketAsync();

                var url = $"{_apiUrl}/b2api/v2/b2_list_file_names";
                var payload = new { bucketId = bucketId, maxFileCount = maxCount };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, url);

                // Use TryAddWithoutValidation to avoid HttpHeader parsing of Backblaze token
                request.Headers.Remove("Authorization");
                request.Headers.TryAddWithoutValidation("Authorization", _authorizationToken);

                request.Content = content;

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var result = new List<string>();
                if (root.TryGetProperty("files", out var filesElement) && filesElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var f in filesElement.EnumerateArray())
                    {
                        if (f.TryGetProperty("fileName", out var fn))
                        {
                            result.Add(fn.GetString()!);
                        }
                    }
                }

                _logger.LogInformation("Listado de archivos obtenido (bucket: {BucketName}, id: {BucketId}, count: {Count})", bucketName, bucketId, result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listando archivos en B2: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Descarga un archivo desde B2 y devuelve su contenido y content-type.
        /// </summary>
        public async Task<(byte[] Content, string ContentType)> DownloadFileAsync(string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new ArgumentException("fileName es requerido", nameof(fileName));

                // Autenticarse y resolver bucket
                await AuthenticateAsync();
                var (bucketId, bucketName) = await ResolveBucketAsync();

                if (string.IsNullOrWhiteSpace(_downloadUrl))
                    throw new InvalidOperationException("_downloadUrl no está inicializado");

                // Construir URL de descarga usando b2_download_file_by_name
                var url = $"{_downloadUrl}/file/{bucketName}/{fileName}";

                var request = new HttpRequestMessage(HttpMethod.Get, url);

                // Use TryAddWithoutValidation to evitar parsing del token de Backblaze
                request.Headers.Remove("Authorization");
                request.Headers.TryAddWithoutValidation("Authorization", _authorizationToken);

                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                using var responseStream = await response.Content.ReadAsStreamAsync();
                using var ms = new MemoryStream();
                await responseStream.CopyToAsync(ms);
                var bytes = ms.ToArray();

                var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/pdf";

                _logger.LogInformation("Descarga de B2 OK: {FileName} ({Length} bytes)", fileName, bytes.Length);

                return (bytes, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error descargando archivo de B2: {Message}", ex.Message);
                throw;
            }
        }

        // ====================================
        // Métodos privados
        // ====================================

        /// <summary>
        /// Se autentica con B2 usando las credenciales configuradas
        /// </summary>
        private async Task AuthenticateAsync()
        {
            try
            {
                // Si ya estamos autenticados y tenemos apiUrl y accountId, no reautenticamos
                if (!string.IsNullOrWhiteSpace(_authorizationToken) && !string.IsNullOrWhiteSpace(_apiUrl) && !string.IsNullOrWhiteSpace(_accountId))
                    return;

                var keyString = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{_options.KeyId}:{_options.ApplicationKey}")
                );

                var request = new HttpRequestMessage(HttpMethod.Get, B2_AUTHORIZE_ACCOUNT_URL);
                request.Headers.Add("Authorization", $"Basic {keyString}");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;

                if (root.TryGetProperty("authorizationToken", out var authToken))
                {
                    _authorizationToken = authToken.GetString();
                }

                if (root.TryGetProperty("apiUrl", out var apiUrl))
                {
                    _apiUrl = apiUrl.GetString();
                }

                if (root.TryGetProperty("downloadUrl", out var downloadUrl))
                {
                    _downloadUrl = downloadUrl.GetString();
                }

                if (root.TryGetProperty("accountId", out var accountId))
                {
                    _accountId = accountId.GetString();
                }

                _logger.LogInformation("Autenticación con B2 exitosa. apiUrl: {ApiUrl}", _apiUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en autenticación con B2: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Resuelve el bucket configurado y devuelve su bucketId
        /// </summary>
        private async Task<(string bucketId, string bucketName)> ResolveBucketAsync()
        {
            try
            {
                // Si ya lo resolvimos, devolver
                if (!string.IsNullOrWhiteSpace(_bucketId))
                {
                    return (_bucketId, _options.BucketName);
                }

                await AuthenticateAsync();

                var url = $"{_apiUrl}/b2api/v2/b2_list_buckets";
                var payload = new { accountId = _accountId };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, url);

                // Use TryAddWithoutValidation to avoid HttpHeader parsing of Backblaze token
                request.Headers.Remove("Authorization");
                request.Headers.TryAddWithoutValidation("Authorization", _authorizationToken);

                request.Content = content;

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("buckets", out var bucketsElement) && bucketsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var b in bucketsElement.EnumerateArray())
                    {
                        var name = b.GetProperty("bucketName").GetString();
                        var id = b.GetProperty("bucketId").GetString();
                        if (name == _options.BucketName)
                        {
                            _bucketId = id;
                            _logger.LogInformation("Usando bucket B2 {BucketName} (id: {BucketId})", name, id);
                            return (id!, name!);
                        }
                    }
                }

                _logger.LogError("El bucket '{BucketName}' no fue encontrado en la cuenta B2.", _options.BucketName);
                throw new InvalidOperationException($"Bucket '{_options.BucketName}' no encontrado en Backblaze B2");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolviendo bucket B2: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtiene una URL de carga para el bucket
        /// </summary>
        private async Task GetUploadUrlAsync(string bucketId)
        {
            try
            {
                // Si ya tenemos uploadUrl y token, no pedir otra
                if (!string.IsNullOrWhiteSpace(_uploadUrl) && !string.IsNullOrWhiteSpace(_uploadAuthToken))
                    return;

                var url = $"{_apiUrl}/b2api/v2/b2_get_upload_url";
                var payload = new { bucketId = bucketId };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, url);

                // Use TryAddWithoutValidation to avoid HttpHeader parsing of Backblaze token
                request.Headers.Remove("Authorization");
                request.Headers.TryAddWithoutValidation("Authorization", _authorizationToken);

                request.Content = content;

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("uploadUrl", out var uploadUrl))
                {
                    _uploadUrl = uploadUrl.GetString();
                }

                if (root.TryGetProperty("authorizationToken", out var authToken))
                {
                    _uploadAuthToken = authToken.GetString();
                }

                _logger.LogInformation("Upload URL obtenida para bucketId {BucketId}", bucketId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener URL de carga: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Sube el archivo al bucket de B2
        /// </summary>
        private async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_uploadUrl) || string.IsNullOrWhiteSpace(_uploadAuthToken))
                {
                    throw new InvalidOperationException("Upload URL o token no inicializados");
                }

                // Leer el contenido del archivo
                using var ms = new MemoryStream();
                await fileStream.CopyToAsync(ms);
                var fileBytes = ms.ToArray();

                var request = new HttpRequestMessage(HttpMethod.Post, _uploadUrl!);
                var byteContent = new ByteArrayContent(fileBytes);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue(contentType ?? "application/octet-stream");
                request.Content = byteContent;

                // Headers requeridos por B2
                // Use TryAddWithoutValidation to avoid parsing issues with token
                request.Headers.Remove("Authorization");
                request.Headers.TryAddWithoutValidation("Authorization", _uploadAuthToken);
                // El nombre de archivo debe ser enviado en X-Bz-File-Name y no debe estar codificado en base64
                request.Headers.TryAddWithoutValidation("X-Bz-File-Name", Uri.EscapeDataString(fileName));
                request.Headers.TryAddWithoutValidation("X-Bz-Content-Sha1", "do_not_verify");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                // Opcional: parsear respuesta para obtener fileId o fileName devuelto

                _logger.LogInformation("Archivo {FileName} procesado: {Length} bytes", fileName, fileBytes.Length);

                // Devolver el nombre del archivo como referencia
                return fileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir archivo: {Message}", ex.Message);
                throw;
            }
        }
    }
}
