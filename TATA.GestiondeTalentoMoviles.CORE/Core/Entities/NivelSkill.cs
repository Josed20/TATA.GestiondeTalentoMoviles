using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TATA.GestiondeTalentoMoviles.CORE.Entities
{
    public class NivelSkill
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("codigo")]
        public int Codigo { get; set; }

        [BsonElement("nombre")]
        public string Nombre { get; set; } = null!;
    }
}
