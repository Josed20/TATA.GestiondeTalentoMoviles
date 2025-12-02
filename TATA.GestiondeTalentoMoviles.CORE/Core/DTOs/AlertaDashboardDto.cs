using System;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    /// <summary>
    /// DTO unificado para el dashboard de alertas móvil
    /// </summary>
    public class AlertaDashboardDto
    {
        /// <summary>
        /// ID de la vacante, evaluación, certificación, etc.
        /// </summary>
        public string IdReferencia { get; set; }

        /// <summary>
        /// Título de la alerta (Ej: "Vence Azure Fundamentals")
        /// </summary>
        public string Titulo { get; set; }

        /// <summary>
        /// Mensaje descriptivo (Ej: "Vence en 15 días")
        /// </summary>
        public string Mensaje { get; set; }

        /// <summary>
        /// Fecha en formato string yyyy-MM-dd
        /// </summary>
        public string Fecha { get; set; }

        /// <summary>
        /// Tipo de origen: CERTIFICACION, SKILL_GAP, GENERICA, VACANTE_DISPONIBLE
        /// </summary>
        public string TipoOrigen { get; set; }

        /// <summary>
        /// Indica si la alerta está activa
        /// </summary>
        public bool Activa { get; set; }

        /// <summary>
        /// Color de prioridad: ROJO, AMARILLO, VERDE
        /// </summary>
        public string ColorPrioridad { get; set; }
    }

    /// <summary>
    /// DTO para anunciar una vacante por correo
    /// </summary>
    public class AnunciarVacanteDto
    {
        public string VacanteId { get; set; }
    }
}
