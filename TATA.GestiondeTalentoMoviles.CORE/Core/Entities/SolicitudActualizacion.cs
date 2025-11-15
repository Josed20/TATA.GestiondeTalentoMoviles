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
        public string ColaboradorId { get; set; }

        [BsonElement("skillId")]
        public string SkillId { get; set; }

        [BsonElement("nuevoNivel")]
        public string NuevoNivel { get; set; }

        [BsonElement("evidenciaUrl")]
        public string EvidenciaUrl { get; set; }

        [BsonElement("estado")]
        public string Estado { get; set; }

        [BsonElement("comentariosRRHH")]
        public string? ComentariosRRHH { get; set; }

        [BsonElement("fechaSolicitud")]
        public DateTime FechaSolicitud { get; set; }

        [BsonElement("fechaRevision")]
        public DateTime? FechaRevision { get; set; }
    }
}
