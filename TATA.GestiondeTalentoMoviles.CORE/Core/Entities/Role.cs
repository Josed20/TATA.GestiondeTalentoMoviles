using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Entities
{
    public class Role
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;
    }
}
