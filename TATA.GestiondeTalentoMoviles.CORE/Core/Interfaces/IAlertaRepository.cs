using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TATA.GestiondeTalentoMoviles.CORE.Core.Entities;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces.Repositories
{
    public interface IAlertaRepository
    {
        Task<Alerta> GetByIdAsync(string id);
        Task<IEnumerable<Alerta>> GetAllAsync();
        Task CreateAsync(Alerta alerta);
        Task UpdateAsync(string id, Alerta alertaIn);
        Task DeleteAsync(string id);
    }
}
