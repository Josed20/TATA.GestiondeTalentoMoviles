namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    public class EvaluacionReadDto
    {
        public string Id { get; set; } = null!;
        public string Colaborador { get; set; } = null!;
        public string RolActual { get; set; } = null!;
        public string LiderEvaluador { get; set; } = null!;
        public string TipoDeEvaluacion { get; set; } = null!;
        public DateTime FechaDeEvaluacion { get; set; }
        public List<string> SkillsEvaluadas { get; set; } = new();
        public string NivelRecomendado { get; set; } = null!;
        public string? Comentarios { get; set; }
        public string UsuarioResponsable { get; set; } = null!;
        public DateTime FechaDeCreacion { get; set; }
    }
}