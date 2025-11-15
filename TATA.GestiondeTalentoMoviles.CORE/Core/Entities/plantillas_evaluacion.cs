using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TATA.GestiondeTalentoMoviles.CORE.Entities
{
    /**
     * Clase que representa una Evaluación de Desempeño
     * Mapea a la colección 'evaluaciones'
     */
    public class Evaluacion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Mapea a 'colaborador'
        [BsonElement("colaborador")]
        public string Colaborador { get; set; } = null!;

        // Mapea a 'rol_actual'
        [BsonElement("rol_actual")]
        public string RolActual { get; set; } = null!;

        // Mapea a 'lider_evaluador'
        [BsonElement("lider_evaluador")]
        public string LiderEvaluador { get; set; } = null!;

        // Mapea a 'tipo_de_evaluacion'
        [BsonElement("tipo_de_evaluacion")]
        public string TipoDeEvaluacion { get; set; } = null!;

        // Mapea a 'fecha_de_evaluacion' (Tipo BSON Date)
        [BsonElement("fecha_de_evaluacion")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime FechaDeEvaluacion { get; set; }

        // Mapea a 'skills_evaluadas' (Array de strings)
        [BsonElement("skills_evaluadas")]
        public List<string> SkillsEvaluadas { get; set; } = new();

        // Mapea a 'nivel_recomendado'
        [BsonElement("nivel_recomendado")]
        public string NivelRecomendado { get; set; } = null!;

        // Mapea a 'comentarios' (Opcional, puede ser null)
        [BsonElement("comentarios")]
        public string? Comentarios { get; set; }

        // Mapea a 'usuario_responsable'
        [BsonElement("usuario_responsable")]
        public string UsuarioResponsable { get; set; } = null!;

        // Mapea a 'fecha_de_creacion' (Tipo BSON Date)
        [BsonElement("fecha_de_creacion")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime FechaDeCreacion { get; set; }
    }
}