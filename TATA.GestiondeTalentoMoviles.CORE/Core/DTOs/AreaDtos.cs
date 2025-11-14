namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    public class AreaCreateDto
    {
        public string Nombre { get; set; } = null!;
    }

    public class AreaReadDto
    {
        public string Id { get; set; } = null!;
        public string Nombre { get; set; } = null!;
    }
}