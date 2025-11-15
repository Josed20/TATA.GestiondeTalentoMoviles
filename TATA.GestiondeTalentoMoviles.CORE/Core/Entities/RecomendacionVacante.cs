using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace TATA.GestiondeTalentoMoviles.CORE.Entities
{
    public class RecomendacionVacante
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("colaboradorId")]
        public string ColaboradorId { get; set; } = null!;

        [BsonElement("vacanteId")]
        public string VacanteId { get; set; } = null!;

        [BsonElement("fechaGeneracion")]
        public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;

        [BsonElement("detalle")]
        public DetalleRecomendacionVacante Detalle { get; set; } = null!;
    }

    public class DetalleRecomendacionVacante
    {
        [BsonElement("motivo")]
        public string Motivo { get; set; } = null!;

        [BsonElement("nivelMatch")]
        public int NivelMatch { get; set; } = 0;

        [BsonElement("nivelConfianza")]
        public string NivelConfianza { get; set; } = null!;
    }
}
