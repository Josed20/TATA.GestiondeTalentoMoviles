using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Entities
{
    public class Alerta
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("tipo")]
        public string Tipo { get; set; }

        [BsonElement("estado")]
        public string Estado { get; set; }

        [BsonElement("colaboradorId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ColaboradorId { get; set; }

        [BsonElement("vacanteId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string VacanteId { get; set; }

        [BsonElement("procesoMatchingId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProcesoMatchingId { get; set; }

        [BsonElement("detalle")]
        public AlertaDetalle Detalle { get; set; }

        [BsonElement("destinatarios")]
        public List<AlertaDestinatario> Destinatarios { get; set; }

        [BsonElement("fechaCreacion")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime FechaCreacion { get; set; }

        [BsonElement("fechaActualizacion")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime FechaActualizacion { get; set; }

        [BsonElement("usuarioResponsable")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UsuarioResponsable { get; set; }
    }

    public class AlertaDetalle
    {
        // Caso PROXIMA_EVALUACION
        [BsonIgnoreIfNull]
        [BsonElement("fechaProximaEvaluacion")]
        public DateTime? FechaProximaEvaluacion { get; set; }

        // Caso BRECHA_SKILLS
        [BsonIgnoreIfNull]
        [BsonElement("skillsFaltantes")]
        public List<SkillFaltante> SkillsFaltantes { get; set; }

        [BsonElement("descripcion")]
        public string Descripcion { get; set; }
    }

    public class SkillFaltante
    {
        [BsonElement("nombre")]
        public string Nombre { get; set; }

        [BsonElement("nivelRequerido")]
        public int NivelRequerido { get; set; }
    }

    public class AlertaDestinatario
    {
        [BsonElement("usuarioId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UsuarioId { get; set; }

        [BsonElement("tipo")]
        public string Tipo { get; set; }
    }
}

