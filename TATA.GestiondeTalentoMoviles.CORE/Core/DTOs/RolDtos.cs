namespace TATA.GestiondeTalentoMoviles.CORE.Core.DTOs
{
    public class RolCreateDto
    {
        public string Nombre { get; set; } = null!;
    }

    public class RolReadDto
    {
        public string Id { get; set; } = null!;
        public string Nombre { get; set; } = null!;
    }
}