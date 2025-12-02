namespace TATA.GestiondeTalentoMoviles.CORE.Core.Options
{
    /// <summary>
    /// Configuración para la integración con Backblaze B2.
    /// <para>
    /// Es importante que <see cref="DownloadUrlBase"/> contenga la "Friendly URL" del bucket en Backblaze,
    /// por ejemplo: https://f005.backblazeb2.com/file/tcs-certificados
    /// (esto es lo que se usará para construir las URLs públicas: "{DownloadUrlBase}/{fileName}").
    /// </para>
    /// </summary>
    public class BackblazeB2Options
    {
        /// <summary>
        /// El KeyId (keyID) provisto por Backblaze (ej.: 0058c432dec5f210000000001)
        /// </summary>
        public string KeyId { get; set; } = null!;

        /// <summary>
        /// La Application Key (secreta). NO commitear en el repositorio; definirla en variables de entorno.
        /// </summary>
        public string ApplicationKey { get; set; } = null!;

        /// <summary>
        /// Nombre exacto del bucket (ej.: tcs-certificados)
        /// </summary>
        public string BucketName { get; set; } = null!;

        /// <summary>
        /// URL base para descargas (Friendly URL) del bucket en Backblaze.
        /// Ejemplo: https://f005.backblazeb2.com/file/tcs-certificados
        /// </summary>
        public string DownloadUrlBase { get; set; } = null!;
    }
}
