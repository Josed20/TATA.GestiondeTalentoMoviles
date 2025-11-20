using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TATA.GestiondeTalentoMoviles.CORE.Entities
{
    [BsonIgnoreExtraElements] // Ignora campos extra que pueda tener el documento en Mongo
    public class Colaborador
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nombres")]
        public string Nombres { get; set; } = null!;

        [BsonElement("apellidos")]
        public string Apellidos { get; set; } = null!;

        [BsonElement("correo")]
        public string Correo { get; set; } = null!;

        [BsonElement("area")]
        public string Area { get; set; } = null!;

        // Ej: "Backend Developer", "QA Tester", etc. Viene del catálogo.
        [BsonElement("rolLaboral")]
        public string RolLaboral { get; set; } = null!;

        // "ACTIVO" / "INACTIVO"
        [BsonElement("estado")]
        public string Estado { get; set; } = "ACTIVO";

        [BsonElement("disponibleParaMovilidad")]
        public bool DisponibleParaMovilidad { get; set; }

        // Skills embebidos (no referencias)
        [BsonElement("skills")]
        public List<SkillColaborador> Skills { get; set; } = new();

        // Certificaciones embebidas
        [BsonElement("certificaciones")]
        public List<CertificacionColaborador> Certificaciones { get; set; } = new();

        // Auditoría a nivel colaborador
        [BsonElement("fechaRegistro")]
        public DateTime? FechaRegistro { get; set; }

        [BsonElement("fechaActualizacion")]
        public DateTime? FechaActualizacion { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class SkillColaborador
    {
        [BsonElement("nombre")]
        public string Nombre { get; set; } = null!;   // Ej: ".NET", "Comunicación"

        // "TECNICO" o "BLANDO" (del catálogo)
        [BsonElement("tipo")]
        public string Tipo { get; set; } = null!;

        // 1–4 según catalogos_globales.nivelesSkill.codigo
        [BsonElement("nivel")]
        public int Nivel { get; set; }

        [BsonElement("esCritico")]
        public bool EsCritico { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class CertificacionColaborador
    {
        // Si manejas las certificaciones como documentos aparte, lo puedes tratar como ObjectId.
        [BsonElement("certificacionId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? CertificacionId { get; set; }

        [BsonElement("nombre")]
        public string Nombre { get; set; } = null!;      // "Azure Fundamentals"

        [BsonElement("institucion")]
        public string Institucion { get; set; } = null!; // "Microsoft"

        [BsonElement("fechaObtencion")]
        public DateTime? FechaObtencion { get; set; }

        [BsonElement("fechaVencimiento")]
        public DateTime? FechaVencimiento { get; set; }

        [BsonElement("archivoPdfUrl")]
        public string? ArchivoPdfUrl { get; set; }

        // "VIGENTE", "VENCIDA", etc.
        [BsonElement("estado")]
        public string Estado { get; set; } = "VIGENTE";

        [BsonElement("fechaRegistro")]
        public DateTime? FechaRegistro { get; set; }

        [BsonElement("fechaActualizacion")]
        public DateTime? FechaActualizacion { get; set; }

        // Próxima fecha sugerida para reevaluar esa certificación
        [BsonElement("proximaEvaluacion")]
        public DateTime? ProximaEvaluacion { get; set; }
    }
}
