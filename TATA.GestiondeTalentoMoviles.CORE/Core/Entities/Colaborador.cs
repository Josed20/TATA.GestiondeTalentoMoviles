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

        // Array de ObjectIds que referencian a la colección skills
        [BsonElement("skills")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Skills { get; set; } = new();

        // Código de nivel que referencia a nivelskills.codigo (int)
        [BsonElement("nivelCodigo")]
        public int? NivelCodigo { get; set; }

        // Array de certificaciones embebidas
        [BsonElement("certificaciones")]
        public List<CertificacionColaborador> Certificaciones { get; set; } = new();

        // Objeto disponibilidad
        [BsonElement("disponibilidad")]
        public DisponibilidadColaborador Disponibilidad { get; set; } = new();
    }

    // Certificación embebida en el colaborador
    public class CertificacionColaborador
    {
        [BsonElement("nombre")]
        public string Nombre { get; set; } = null!;

        [BsonElement("imagenUrl")]
        public string? ImagenUrl { get; set; }

        [BsonElement("fechaObtencion")]
        public DateTime? FechaObtencion { get; set; }

        [BsonElement("estado")]
        public string Estado { get; set; } = "vigente";
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
