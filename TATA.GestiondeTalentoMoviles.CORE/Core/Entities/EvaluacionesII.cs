using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TATA.GestiondeTalentoMoviles.CORE.Entities
{
    public class EvaluacionesII
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("tipoEvaluacion")]
        public string TipoEvaluacion { get; set; } = null!;

        [BsonElement("descripcion")]
        public string Descripcion { get; set; } = null!;

        [BsonElement("fechaCreacion")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [BsonElement("fechaActualizacion")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;
    }
}
