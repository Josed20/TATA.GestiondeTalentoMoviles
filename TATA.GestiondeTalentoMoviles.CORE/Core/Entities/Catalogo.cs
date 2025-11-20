using System.Collections.Generic;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Entities
{
    public class NivelSkill
    {
        public int Codigo { get; set; }
        public string Descripcion { get; set; } = null!;
    }

    public class Catalogo
    {
        // Se mapea al campo _id de MongoDB
        public string Id { get; set; } = null!;

        public List<string> Areas { get; set; } = new();
        public List<string> RolesLaborales { get; set; } = new();
        public List<NivelSkill> NivelesSkill { get; set; } = new();
        public List<string> TiposSkill { get; set; } = new();
    }
}