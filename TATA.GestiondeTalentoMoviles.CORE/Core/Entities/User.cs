using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Entities
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("rolSistema")]
        public string RolSistema { get; set; } // Ajustado a string singular

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("colaboradorId")]
        public string ColaboradorId { get; set; }

        [BsonElement("intentosFallidos")]
        public int IntentosFallidos { get; set; }

        [BsonElement("bloqueadoHasta")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? BloqueadoHasta { get; set; } // Permite valores nulos

        [BsonElement("ultimoAcceso")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UltimoAcceso { get; set; }

        [BsonElement("fechaCreacion")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime FechaCreacion { get; set; }
    }
}