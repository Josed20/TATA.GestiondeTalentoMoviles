using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TATA.GestiondeTalentoMoviles.CORE.Entities
{
    public class Colaborador
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nombres")]
        public string Nombres { get; set; } = null!;

        [BsonElement("apellidos")]
        public string Apellidos { get; set; } = null!;

        [BsonElement("area")]
        public string Area { get; set; } = null!;

        [BsonElement("rolActual")]
        public string RolActual { get; set; } = null!;

        // Array de skills del colaborador
        [BsonElement("skills")]
        public List<ColaboradorSkill> Skills { get; set; } = new();

        // Objeto disponibilidad
        [BsonElement("disponibilidad")]
        public DisponibilidadColaborador Disponibilidad { get; set; } = new();
    }

    // Elemento del array "skills" en Mongo
    public class ColaboradorSkill
    {
        [BsonElement("skillId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SkillId { get; set; } = null!;  // referencia a Skill.Id

        [BsonElement("nivelId")]
        public int NivelId { get; set; }             // referencia a NivelSkill.Id

        [BsonElement("certificaciones")]
        public List<string> Certificaciones { get; set; } = new();
    }

    // Subdocumento "disponibilidad"
    public class DisponibilidadColaborador
    {
        [BsonElement("estado")]
        public string Estado { get; set; } = "Disponible";

        [BsonElement("dias")]
        public int Dias { get; set; }
    }
}
