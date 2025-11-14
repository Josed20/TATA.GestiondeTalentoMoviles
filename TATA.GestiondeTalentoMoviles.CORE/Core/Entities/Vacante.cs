using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Entities
{
    public class Vacante
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("perfil_name")]
        public string PerfilName { get; set; } = string.Empty;

        [JsonPropertyName("id_area")]
        public string IdArea { get; set; } = string.Empty;

        [JsonPropertyName("start_date")]
        public DateTimeOffset StartDate { get; set; }

        [JsonPropertyName("urgency")]
        public int Urgency { get; set; }

        [JsonPropertyName("state")]
        public int State { get; set; }

        [JsonPropertyName("id_rol")]
        public string IdRol { get; set; } = string.Empty;

        [JsonPropertyName("certifications")]
        public List<string> Certifications { get; set; } = new();

        [JsonPropertyName("skills")]
        public List<string> Skills { get; set; } = new();
    }
}
