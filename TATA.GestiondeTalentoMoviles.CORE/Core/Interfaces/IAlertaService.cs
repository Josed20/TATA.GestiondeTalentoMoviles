using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface IAlertaService
    {
        Task<AlertaViewDto> CreateAsync(AlertaCreateDto dto);
        Task<IEnumerable<AlertaViewDto>> GetAllAsync();
        Task<AlertaViewDto> GetByIdAsync(string id);
        Task<AlertaViewDto> UpdateAsync(string id, AlertaUpdateDto dto);
        Task DeleteAsync(string id);

        // 🔥 Métodos adicionales
        Task<IEnumerable<AlertaViewDto>> GetAlertasPorColaboradorAsync(string colaboradorId);
        Task<IEnumerable<AlertaViewDto>> GetAlertasPendientesAsync();
    }
}

