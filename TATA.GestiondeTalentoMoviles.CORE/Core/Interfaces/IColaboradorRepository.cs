using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TATA.GestiondeTalentoMoviles.CORE.Entities; // corregido namespace de Entities

namespace TATA.GestiondeTalentoMoviles.CORE.Interfaces // corregido namespace de Interfaces
{
    public interface IColaboradorRepository
    {
        Task<IEnumerable<Colaborador>> GetAllAsync();
        Task<Colaborador?> GetByIdAsync(string id);
        Task<Colaborador> CreateAsync(Colaborador colaborador);
    }
}