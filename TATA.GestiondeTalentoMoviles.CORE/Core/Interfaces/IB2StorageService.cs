using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    /// <summary>
    /// Interfaz para servicio de almacenamiento en Backblaze B2
    /// </summary>
    public interface IB2StorageService
    {
        /// <summary>
        /// Sube un certificado (PDF) a Backblaze B2
        /// </summary>
        /// <param name="fileStream">Stream del archivo PDF</param>
        /// <param name="fileName">Nombre del archivo incluyendo ruta (ej: "colaboradorId/yyyy/MM/guid-nombre.pdf")</param>
        /// <param name="contentType">Tipo de contenido (ej: "application/pdf")</param>
        /// <returns>URL o referencia del archivo guardado en B2</returns>
        Task<string> UploadCertificateAsync(Stream fileStream, string fileName, string contentType);

        /// <summary>
        /// Obtiene la URL de descarga para un archivo guardado en B2
        /// </summary>
        /// <param name="archivoPdfUrl">Referencia del archivo (puede ser fileName o URL completa)</param>
        /// <returns>URL de descarga completa</returns>
        Task<string> GetDownloadUrlAsync(string archivoPdfUrl);

        /// <summary>
        /// Elimina un archivo de B2 (opcional para futuros borrados)
        /// </summary>
        /// <param name="fileName">Nombre del archivo a eliminar</param>
        /// <returns>True si se eliminó correctamente</returns>
        Task<bool> DeleteFileAsync(string fileName);

        /// <summary>
        /// Lista los nombres de archivos recientes en el bucket configurado
        /// </summary>
        /// <param name="maxCount">Número máximo de archivos a listar</param>
        Task<IReadOnlyList<string>> ListRecentFilesAsync(int maxCount = 10);

        /// <summary>
        /// Descarga un archivo desde B2 y devuelve su contenido y content-type.
        /// </summary>
        /// <param name="fileName">Nombre interno del archivo en el bucket (ej: "carpeta/2025/12/archivo.pdf")</param>
        Task<(byte[] Content, string ContentType)> DownloadFileAsync(string fileName);
    }
}
