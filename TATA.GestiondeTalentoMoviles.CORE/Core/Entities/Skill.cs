using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TATA.GestiondeTalentoMoviles.CORE.Entities
{
    public class Skill
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nombre")]
        public string Nombre { get; set; } = null!;

        [BsonElement("tipo")]
        public string Tipo { get; set; } = null!;
    }
}

