using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TATA.GestiondeTalentoMoviles.CORE.Entities
{
    /// <summary>
    /// Clase que representa una plantilla para tipos de evaluación en MongoDB
    /// </summary>
    public class PlantillaEvaluacion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // tipoEvaluacion (Ejemplo: ENTREVISTA, TECNICA, DESEMPEÑO)
        [BsonElement("tipoEvaluacion")]
        public string TipoEvaluacion { get; set; } = null!;

        // descripcion de la plantilla
        [BsonElement("descripcion")]
        public string Descripcion { get; set; } = null!;

        // fechaCreacion
        [BsonElement("fechaCreacion")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // fechaActualizacion
        [BsonElement("fechaActualizacion")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;
    }
}
