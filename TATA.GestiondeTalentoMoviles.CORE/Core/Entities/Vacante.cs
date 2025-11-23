using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TATA.GestiondeTalentoMoviles.CORE.Entities
{
    public class Vacante
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nombrePerfil")]
        public string NombrePerfil { get; set; } = null!;

        [BsonElement("area")]
        public string Area { get; set; } = null!;

        [BsonElement("rolLaboral")]
        public string RolLaboral { get; set; } = null!;

        [BsonElement("skillsRequeridos")]
        public List<SkillRequeridoVacante> SkillsRequeridos { get; set; } = new();

        [BsonElement("certificacionesRequeridas")]
        public List<string> CertificacionesRequeridas { get; set; } = new();

        [BsonElement("fechaInicio")]
        public DateTime FechaInicio { get; set; }

        [BsonElement("urgencia")]
        public string Urgencia { get; set; } = "MEDIA";

        [BsonElement("estadoVacante")]
        public string EstadoVacante { get; set; } = "ABIERTA";

        [BsonElement("creadaPorUsuarioId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CreadaPorUsuarioId { get; set; } = null!;

        [BsonElement("fechaCreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [BsonElement("fechaActualizacion")]
        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;

        [BsonElement("usuarioActualizacion")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UsuarioActualizacion { get; set; } = null!;
    }

    public class SkillRequeridoVacante
    {

        [BsonElement("nombre")]
        public string Nombre { get; set; } = null!;

        [BsonElement("tipo")]
        public string Tipo { get; set; } = null!;

        [BsonElement("nivelDeseado")]
        public int NivelDeseado { get; set; }

        [BsonElement("esCritico")]
        public bool EsCritico { get; set; }
    }
}
