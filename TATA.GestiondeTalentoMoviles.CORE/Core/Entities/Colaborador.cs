using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TATA.GestiondeTalentoMoviles.CORE.Entities
{
    public class Colaborador
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string RolActual { get; set; } = null!;
        public string SkillPrimario { get; set; } = null!;
        public string SkillSecundario { get; set; } = null!;
        public int NivelDominio { get; set; }
        public List<string> Certificaciones { get; set; } = new();
        public string Disponibilidad { get; set; } = "Disponible";
        public int? DiasDisponibilidad { get; set; }
    }
}