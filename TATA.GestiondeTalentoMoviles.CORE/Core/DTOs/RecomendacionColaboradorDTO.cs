using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace TATA.GestiondeTalentoMoviles.CORE.DTOs
{
    public class RecomendacionColaboradorDTO
    {
        public string? Id { get; set; } = null!;
        public string ColaboradorId { get; set; } = null!;
        public DateTime FechaGeneracion { get; set; } 
        public List<DetalleRecomendacionDTO> Recomendaciones { get; set; } = null!;
    }
}


