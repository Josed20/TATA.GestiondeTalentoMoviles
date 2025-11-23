using System.Threading.Tasks;
using System.Text.Json;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface ICatalogoService
    {
        Task<CatalogoReadDto?> GetAsync(string id);
        Task<CatalogoReadDto?> GetFirstAsync();
        Task<CatalogoReadDto> UpdateAsync(string seccion, JsonElement data);
        Task<bool> DeleteIndexAsync(string seccion, int index);
        Task<CatalogoReadDto> CreateSectionAsync(CatalogoCreateSectionDto dto);
        Task<CatalogoReadDto> AddItemToSectionAsync(string seccion, object item);
    }
}