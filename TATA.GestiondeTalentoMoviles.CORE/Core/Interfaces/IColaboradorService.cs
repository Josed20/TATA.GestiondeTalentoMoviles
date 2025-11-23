using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs;

namespace TATA.GestiondeTalentoMoviles.CORE.Core.Interfaces
{
    public interface IColaboradorService
    {
        // ====================================
        // Métodos CRUD básicos
        // ====================================
        
        /// <summary>
        /// Crea un nuevo colaborador
        /// </summary>
        Task<ColaboradorReadDto> CreateAsync(ColaboradorCreateDto createDto);
        
        /// <summary>
        /// Obtiene todos los colaboradores (ACTIVOS e INACTIVOS)
        /// </summary>
        Task<IEnumerable<ColaboradorReadDto>> GetAllAsync();
        
        /// <summary>
        /// Obtiene un colaborador por su ID
        /// </summary>
        Task<ColaboradorReadDto?> GetByIdAsync(string id);
        
        /// <summary>
        /// Actualiza un colaborador existente
        /// </summary>
        Task<ColaboradorReadDto?> UpdateAsync(string id, ColaboradorUpdateDto updateDto);
        
        /// <summary>
        /// Elimina lógicamente un colaborador (marca como INACTIVO)
        /// </summary>
        Task<bool> DeleteAsync(string id);
    }
}