using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface ICatalogoService
    {
        Task<CatalogoReadDto?> GetAsync(string id);
        Task<CatalogoReadDto?> GetFirstAsync();
        Task<CatalogoReadDto> UpdateAsync(string id, CatalogoUpdateDto dto);
    }
}