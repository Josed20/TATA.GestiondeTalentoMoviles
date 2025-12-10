using System;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    /// <summary>
    /// DTO para respuesta de carga de certificado
    /// </summary>
    public class CertificadoUploadResponseDto
    {
        public string FileName { get; set; } = null!;
        public string ArchivoPdfUrl { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long FileSizeBytes { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    /// <summary>
    /// DTO para solicitud de descarga de certificado
    /// </summary>
    public class CertificadoDownloadUrlRequestDto
    {
        public string ArchivoPdfUrl { get; set; } = null!;
    }

    /// <summary>
    /// DTO para respuesta de URL de descarga
    /// </summary>
    public class CertificadoDownloadUrlResponseDto
    {
        public string DownloadUrl { get; set; } = null!;
        public bool IsExternalUrl { get; set; }
    }
}
