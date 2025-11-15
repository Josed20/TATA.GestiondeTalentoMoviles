using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RecomendacionVacante
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string colaboradorId { get; set; } = null!;
    public string VacanteId { get; set; } = null!;
    public string Motivo { get; set; } = null!;
    public int NivelMatch { get; set; } = 0;
    public string NivelConfianza { get; set; } = null!;
}
