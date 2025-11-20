using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TATA.GestiondeTalentoMoviles.CORE.Entities
{
    /// <summary>
    /// Clase que representa una Evaluación de Desempeño almacenada en MongoDB
    /// </summary>
    public class Evaluacion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // colaboradorId
        [BsonElement("colaboradorId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ColaboradorId { get; set; } = null!;

        // rolActual
        [BsonElement("rolActual")]
        public string RolActual { get; set; } = null!;

        // liderEvaluador
        [BsonElement("liderEvaluador")]
        public string LiderEvaluador { get; set; } = null!;

        // fechaEvaluacion
        [BsonElement("fechaEvaluacion")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime FechaEvaluacion { get; set; }

        // tipoEvaluacion
        [BsonElement("tipoEvaluacion")]
        public string TipoEvaluacion { get; set; } = null!;

        // skillsEvaluados (lista de objetos)
        [BsonElement("skillsEvaluados")]
        public List<SkillEvaluado> SkillsEvaluados { get; set; } = new();

        // comentarios
        [BsonElement("comentarios")]
        public string? Comentarios { get; set; }

        // usuarioResponsable
        [BsonElement("usuarioResponsable")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UsuarioResponsable { get; set; } = null!;

        // fechaCreacion
        [BsonElement("fechaCreacion")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime FechaCreacion { get; set; }

        // fechaActualizacion
        [BsonElement("fechaActualizacion")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime FechaActualizacion { get; set; }
    }

    /// <summary>
    /// Subdocumento: skill evaluado dentro de Evaluación
    /// </summary>
    public class SkillEvaluado
    {
        [BsonElement("nombre")]
        public string Nombre { get; set; } = null!;

        [BsonElement("tipo")]
        public string Tipo { get; set; } = null!;  // TECNICO / BLANDO

        [BsonElement("nivelActual")]
        public int NivelActual { get; set; }

        [BsonElement("nivelRecomendado")]
        public int NivelRecomendado { get; set; }
    }
}
