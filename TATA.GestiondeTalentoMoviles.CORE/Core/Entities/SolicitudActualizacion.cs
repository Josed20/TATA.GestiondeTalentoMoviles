using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TATA.GestiondeTalentoMoviles.CORE.Entities
{
    public class SolicitudActualizacion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("colaboradorId")]
        public string ColaboradorId { get; set; } = null!;

        [BsonElement("skillId")]
        public string SkillId { get; set; } = null!;

        [BsonElement("nuevoNivel")]
        public string NuevoNivel { get; set; } = null!;

        [BsonElement("evidenciaUrl")]
        public string EvidenciaUrl { get; set; } = null!;

        [BsonElement("estado")]
        public string Estado { get; set; } = "Pendiente";

        [BsonElement("comentariosRRHH")]
        public string? ComentariosRRHH { get; set; } = null!;

        [BsonElement("fechaSolicitud")]
        public DateTime FechaSolicitud { get; set; } 

        [BsonElement("fechaRevision")]
        public DateTime? FechaRevision { get; set; }
    }
}
