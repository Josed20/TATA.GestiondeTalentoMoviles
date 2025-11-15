using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface IColaboradorRepository
    {
        Task<IEnumerable<Colaborador>> GetAllAsync();
        Task<Colaborador?> GetByIdAsync(string id);
        Task<Colaborador> CreateAsync(Colaborador colaborador);
        Task<bool> UpdateAsync(string id, Colaborador colaborador);
        Task<bool> DeleteAsync(string id);
    }
}