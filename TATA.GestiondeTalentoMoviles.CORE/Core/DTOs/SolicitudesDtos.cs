using System;
using System.Collections.Generic;

namespace TATA.GestiondeTalentoMoviles.CORE.DTOs
{
    // ====================================
    // DTOs de subdocumentos
    // ====================================

    // Para tipoSolicitudGeneral = "CERTIFICACION"
    public class CertificacionPropuestaCreateDto
    {
        public string Nombre { get; set; } = null!;
        public string Institucion { get; set; } = null!;
        public DateTime? FechaObtencion { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string? ArchivoPdfUrl { get; set; }
    }

    public class CertificacionPropuestaReadDto
    {
        public string Nombre { get; set; } = null!;
        public string Institucion { get; set; } = null!;
        public DateTime? FechaObtencion { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string? ArchivoPdfUrl { get; set; }
    }

    // Para tipoSolicitudGeneral = "ENTREVISTA_DESEMPENO"
    public class DatosEntrevistaPropuestaCreateDto
    {
        public string Motivo { get; set; } = null!;
        public string Periodo { get; set; } = null!; // Ej: "2025"
        public DateTime? FechaSugerida { get; set; }
        // Opcional: puede enviarse o se toma del usuario autenticado
        public string? PropuestoPorUsuarioId { get; set; }
    }

    public class DatosEntrevistaPropuestaReadDto
    {
        public string Motivo { get; set; } = null!;
        public string Periodo { get; set; } = null!;
        public DateTime? FechaSugerida { get; set; }
        public string PropuestoPorUsuarioId { get; set; } = null!;
    }

    // ====================================
    // DTO para CREATE (POST /api/Solicitudes)
    // ====================================
    public class SolicitudCreateDto
    {
        // "CERTIFICACION" / "ENTREVISTA_DESEMPENO"
        public string TipoSolicitudGeneral { get; set; } = null!;

        // Ej: "NUEVA", "RENOVACION", "PERIODICA", etc.
        public string TipoSolicitud { get; set; } = null!;

        public string ColaboradorId { get; set; } = null!;

        // Opcional según el tipo
        public string? CertificacionIdAnterior { get; set; }

        // Solo se usa cuando TipoSolicitudGeneral = "CERTIFICACION"
        public CertificacionPropuestaCreateDto? CertificacionPropuesta { get; set; }

        // Solo se usa cuando TipoSolicitudGeneral = "ENTREVISTA_DESEMPENO"
        public DatosEntrevistaPropuestaCreateDto? DatosEntrevistaPropuesta { get; set; }

        // Solo cuando TipoSolicitudGeneral = "ACTUALIZACION_SKILLS"
        public List<CambioSkillPropuestaCreateDto>? CambiosSkillsPropuestos { get; set; }


        // NUEVO: Opcional - Si no se envía, se puede tomar del JWT o usar uno por defecto
        public string? CreadoPorUsuarioId { get; set; }
    }

    // ====================================
    // DTO para READ (GET /api/Solicitudes, GET /api/Solicitudes/{id})
    // ====================================
    public class SolicitudReadDto
    {
        public string Id { get; set; } = null!;

        public string TipoSolicitudGeneral { get; set; } = null!;
        public string TipoSolicitud { get; set; } = null!;
        public string ColaboradorId { get; set; } = null!;

        public string? CertificacionIdAnterior { get; set; }
        public CertificacionPropuestaReadDto? CertificacionPropuesta { get; set; }
        public DatosEntrevistaPropuestaReadDto? DatosEntrevistaPropuesta { get; set; }

        public List<CambioSkillPropuestaReadDto>? CambiosSkillsPropuestos { get; set; }


        // Workflow
        public string EstadoSolicitud { get; set; } = null!; // PENDIENTE / APROBADA / RECHAZADA / ...

        public string? ObservacionAdmin { get; set; }

        public string CreadoPorUsuarioId { get; set; } = null!;
        public string? RevisadoPorUsuarioId { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaRevision { get; set; }
    }

    // ====================================
    // DTO para UPDATE ESTADO (PUT /api/Solicitudes/{id})
    // ====================================
    public class SolicitudUpdateEstadoDto
    {
        // "APROBADA", "RECHAZADA", "PROGRAMADA", etc.
        public string EstadoSolicitud { get; set; } = null!;

        // Comentario opcional del admin (obligatorio si RECHAZADA)
        public string? ObservacionAdmin { get; set; }
        
        // NUEVO: Opcional - ID del usuario que revisa
        public string? RevisadoPorUsuarioId { get; set; }
    }
    public class CambioSkillPropuestaCreateDto
    {
        public string Nombre { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public int? NivelActual { get; set; }
        public int NivelPropuesto { get; set; }
        public bool? EsCriticoActual { get; set; }
        public bool EsCriticoPropuesto { get; set; }
        public string? Motivo { get; set; }
    }
    public class CambioSkillPropuestaReadDto
    {
        public string Nombre { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public int? NivelActual { get; set; }
        public int NivelPropuesto { get; set; }
        public bool? EsCriticoActual { get; set; }
        public bool EsCriticoPropuesto { get; set; }
        public string? Motivo { get; set; }
    }


}
