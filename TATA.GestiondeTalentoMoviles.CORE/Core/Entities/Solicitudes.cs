using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TATA.GestiondeTalentoMoviles.CORE.Entities
{
    [BsonIgnoreExtraElements]
    public class Solicitud
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // "CERTIFICACION" / "ENTREVISTA_DESEMPENO"
        [BsonElement("tipoSolicitudGeneral")]
        public string TipoSolicitudGeneral { get; set; } = null!;

        // Ej: "NUEVA", "RENOVACION", "PERIODICA", etc.
        [BsonElement("tipoSolicitud")]
        public string TipoSolicitud { get; set; } = null!;

        // Colaborador al que está asociada la solicitud
        [BsonElement("colaboradorId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ColaboradorId { get; set; } = null!;

        // ============================
        // Campos específicos - CERTIFICACION
        // ============================

        // Si fuera renovación, puedes guardar la certificación anterior
        [BsonElement("certificacionIdAnterior")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? CertificacionIdAnterior { get; set; }

        // Datos de la nueva certificación propuesta
        [BsonElement("certificacionPropuesta")]
        public CertificacionPropuestaSolicitud? CertificacionPropuesta { get; set; }

        // ============================
        // Campos específicos - ENTREVISTA_DESEMPENO
        // ============================

        [BsonElement("datosEntrevistaPropuesta")]
        public DatosEntrevistaPropuestaSolicitud? DatosEntrevistaPropuesta { get; set; }


        // Lista de cambios propuestos sobre los skills del colaborador
        [BsonElement("cambiosSkillsPropuestos")]
        public List<CambioSkillPropuestaSolicitud>? CambiosSkillsPropuestos { get; set; }





        // ============================
        // Workflow / estado de la solicitud
        // ============================

        // "PENDIENTE", "EN_REVISION", "APROBADA", "RECHAZADA", "PROGRAMADA", etc.
        [BsonElement("estadoSolicitud")]
        public string EstadoSolicitud { get; set; } = "PENDIENTE";

        // Comentario del administrador cuando aprueba/rechaza
        [BsonElement("observacionAdmin")]
        public string? ObservacionAdmin { get; set; }

        // Usuario que creó la solicitud (colaborador o admin)
        [BsonElement("creadoPorUsuarioId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CreadoPorUsuarioId { get; set; } = null!;

        // Usuario que revisó (admin). Null mientras esté PENDIENTE
        [BsonElement("revisadoPorUsuarioId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? RevisadoPorUsuarioId { get; set; }

        [BsonElement("fechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        [BsonElement("fechaRevision")]
        public DateTime? FechaRevision { get; set; }
    }

    // =========================================
    // Subdocumento: certificacionPropuesta
    // Para tipoSolicitudGeneral = "CERTIFICACION"
    // =========================================
    [BsonIgnoreExtraElements]
    public class CertificacionPropuestaSolicitud
    {
        [BsonElement("nombre")]
        public string Nombre { get; set; } = null!;

        [BsonElement("institucion")]
        public string Institucion { get; set; } = null!;

        [BsonElement("fechaObtencion")]
        public DateTime? FechaObtencion { get; set; }

        [BsonElement("fechaVencimiento")]
        public DateTime? FechaVencimiento { get; set; }

        [BsonElement("archivoPdfUrl")]
        public string? ArchivoPdfUrl { get; set; }
    }

    // =========================================
    // Subdocumento: datosEntrevistaPropuesta
    // Para tipoSolicitudGeneral = "ENTREVISTA_DESEMPENO"
    // =========================================
    [BsonIgnoreExtraElements]
    public class DatosEntrevistaPropuestaSolicitud
    {
        [BsonElement("motivo")]
        public string Motivo { get; set; } = null!;

        [BsonElement("periodo")]
        public string Periodo { get; set; } = null!; // Ej: "2025"

        [BsonElement("propuestoPorUsuarioId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PropuestoPorUsuarioId { get; set; } = null!;

        [BsonElement("fechaSugerida")]
        public DateTime? FechaSugerida { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class CambioSkillPropuestaSolicitud
    {
        [BsonElement("nombre")]
        public string Nombre { get; set; } = null!;   // ".NET", "Liderazgo", etc.

        [BsonElement("tipo")]
        public string Tipo { get; set; } = null!;     // "TECNICO" / "BLANDO"

        [BsonElement("nivelActual")]
        public int? NivelActual { get; set; }         // null si es un skill nuevo

        [BsonElement("nivelPropuesto")]
        public int NivelPropuesto { get; set; }       // 1–4

        [BsonElement("esCriticoActual")]
        public bool? EsCriticoActual { get; set; }    // null si es nuevo

        [BsonElement("esCriticoPropuesto")]
        public bool EsCriticoPropuesto { get; set; }

        [BsonElement("motivo")]
        public string? Motivo { get; set; }           // “Subir de nivel por experiencia…”
    }

}
