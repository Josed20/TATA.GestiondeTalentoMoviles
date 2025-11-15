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

        [BsonElement("perfil_name")]
        public string PerfilName { get; set; } = null!;

        [BsonElement("id_area")]
        public string IdArea { get; set; } = null!;

        [BsonElement("start_date")]
        public DateTimeOffset StartDate { get; set; }

        [BsonElement("urgency")]
        public int Urgency { get; set; }

        [BsonElement("state")]
        public int State { get; set; }

        [BsonElement("id_rol")]
        public string IdRol { get; set; } = null!;

        [BsonElement("certifications")]
        public List<string> Certifications { get; set; } = new();

        [BsonElement("skills")]
        public List<string> Skills { get; set; } = new();
    }
}
