using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Entities
{
    public class ProcesosMatching
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("vacanteId")]
        public string VacanteId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("ejecutadoPorUsuarioId")]
        public string EjecutadoPorUsuarioId { get; set; }

        [BsonElement("fechaEjecucion")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? FechaEjecucion { get; set; }

        [BsonElement("umbralMatch")]
        public double UmbralMatch { get; set; }

        [BsonElement("candidatos")]
        public List<Candidato> Candidatos { get; set; } = new();

        [BsonElement("brechasDetectadas")]
        public List<BrechaDetectada> BrechasDetectadas { get; set; } = new();

        [BsonElement("resultadoGlobal")]
        public string ResultadoGlobal { get; set; }

        [BsonElement("mensajeSistema")]
        public string MensajeSistema { get; set; }

        [BsonElement("fechaCreacion")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime FechaCreacion { get; set; }
    }

    public class Candidato
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("colaboradorId")]
        public string ColaboradorId { get; set; }

        [BsonElement("nombreColaborador")]
        public string NombreColaborador { get; set; }

        [BsonElement("porcentajeMatch")]
        public double PorcentajeMatch { get; set; }

        [BsonElement("detalleMatch")]
        public List<DetalleMatch> DetalleMatch { get; set; } = new();

        [BsonElement("disponibilidad")]
        public string Disponibilidad { get; set; }
    }

    public class DetalleMatch
    {
        [BsonElement("skill")]
        public string Skill { get; set; }

        [BsonElement("nivelColaborador")]
        public int NivelColaborador { get; set; }

        [BsonElement("nivelRequerido")]
        public int NivelRequerido { get; set; }

        [BsonElement("puntaje")]
        public double Puntaje { get; set; }
    }

    public class BrechaDetectada
    {
        [BsonElement("skill")]
        public string Skill { get; set; }

        [BsonElement("nivelRequerido")]
        public int NivelRequerido { get; set; }

        [BsonElement("promedioActual")]
        public double PromedioActual { get; set; }

        [BsonElement("cantidadColaboradoresConSkill")]
        public int CantidadColaboradoresConSkill { get; set; }
    }
}
