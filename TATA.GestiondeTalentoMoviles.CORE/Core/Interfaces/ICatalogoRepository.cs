using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface ICatalogoRepository
    {
        Task<Catalogo?> GetAsync(string id);
        Task<Catalogo?> GetFirstAsync();
        Task CreateOrReplaceAsync(Catalogo catalogo);
    }
}