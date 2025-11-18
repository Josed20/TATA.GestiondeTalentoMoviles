namespace TATA.GestiondeTalentoMoviles.CORE.Core.Constants
{
    /// <summary>
    /// Define los roles válidos del sistema
    /// </summary>
    public static class AppRoles
    {
        public const string ADMIN = "ADMIN";
        public const string BUSINESS_MANAGER = "BUSINESS_MANAGER";
        public const string COLABORADOR = "COLABORADOR";

        /// <summary>
        /// Valida si un rol es válido
        /// </summary>
        public static bool IsValidRole(string role)
        {
            return role == ADMIN || role == BUSINESS_MANAGER || role == COLABORADOR;
        }

        /// <summary>
        /// Obtiene todos los roles válidos
        /// </summary>
        public static string[] GetAllRoles()
        {
            return new[] { ADMIN, BUSINESS_MANAGER, COLABORADOR };
        }
    }
}
