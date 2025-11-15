using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace TATA.GestiondeTalentoMoviles.CORE.Entities
{
    public class RecomendacionColaborador
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("colaboradorId")]
        public string colaboradorId { get; set; } = null!;

        [BsonElement("recomendaciones")]
        public List<DetalleRecomendacion> Recomendaciones { get; set; } = null!;

        [BsonElement("fechaGeneracion")]
        public DateTime FechaGeneracion { get; set; } 
    }

    public class DetalleRecomendacion
    {
        [BsonElement("recomendadoId")]
        public string RecomendadoId { get; set; } = null!;

        [BsonElement("motivo")]
        public string Motivo { get; set; } = null!;

        [BsonElement("coincidenciaSkills")]
        public int CoincidenciaSkills { get; set; } = 0;

        [BsonElement("proyectosPrevios")]
        public List<string> ProyectosPrevios { get; set; } = null!;

        [BsonElement("nivelConfianza")]
        public string NivelConfianza { get; set; } = null!;
    }
}
