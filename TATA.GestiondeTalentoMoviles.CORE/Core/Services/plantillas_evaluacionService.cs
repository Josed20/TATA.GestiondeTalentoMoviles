using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Core.DTOs; // Contiene EvaluacionCreateDto y EvaluacionReadDto
using TATA.GestiondeTalentoMoviles.CORE.Entities; // Contiene la clase Evaluacion (Entidad)
using TATA.GestiondeTalentoMoviles.CORE.Interfaces; // Contiene IEvaluacionService y IEvaluacionRepository

namespace TATA.GestiondeTalentoMoviles.CORE.Services
{
    // Implementa la interfaz de servicio
    public class EvaluacionService : IEvaluacionService
    {
        private readonly IEvaluacionRepository _repository;

        // Inyección de dependencia del repositorio
        public EvaluacionService(IEvaluacionRepository repository)
        {
            _repository = repository;
        }

        // --- 🟢 Método de Creación (POST) ---
        public async Task<EvaluacionReadDto> CreateAsync(EvaluacionCreateDto createDto)
        {
            // Mapear DTO de creación a la Entidad (Evaluacion)
            var evaluacion = MapToEntity(createDto);
            evaluacion.FechaDeCreacion = DateTime.UtcNow; // Asignar la fecha de creación

            var nuevaEvaluacion = await _repository.CreateAsync(evaluacion);

            // Mapear la Entidad resultante al DTO de lectura y devolver
            return MapToReadDto(nuevaEvaluacion);
        }

        // --- 🟢 Método para Obtener Todos (GET) ---
        public async Task<IEnumerable<EvaluacionReadDto>> GetAllAsync()
        {
            var evaluaciones = await _repository.GetAllAsync();

            // Mapear la lista de Entidades a una lista de DTOs de lectura
            return evaluaciones.Select(e => MapToReadDto(e)).ToList();
        }

        // --- 🟢 Método para Obtener por ID (GET) ---
        public async Task<EvaluacionReadDto?> GetByIdAsync(string id)
        {
            var e = await _repository.GetByIdAsync(id);
            if (e == null) return null;

            return MapToReadDto(e);
        }

        // --- 🟠 Método de Actualización (PUT) ---
        public async Task<EvaluacionReadDto?> UpdateAsync(string id, EvaluacionCreateDto updateDto)
        {
            // 1. Verificar si existe
            var existingEvaluacion = await _repository.GetByIdAsync(id);
            if (existingEvaluacion == null) return null;

            // 2. Mapear DTO a Entidad, manteniendo el ID y la fecha de creación original
            var evaluacionToUpdate = MapToEntity(updateDto);
            evaluacionToUpdate.FechaDeCreacion = existingEvaluacion.FechaDeCreacion; // Mantener fecha original
            evaluacionToUpdate.Id = id; // Asegurar que el ID esté en la entidad

            // 3. Llamar al repositorio para actualizar
            var wasUpdated = await _repository.UpdateAsync(id, evaluacionToUpdate);

            // 4. Devolver el objeto actualizado si la operación fue exitosa
            return wasUpdated ? MapToReadDto(evaluacionToUpdate) : null;
        }

        // --- 🔴 Método de Eliminación (DELETE) ---
        public async Task<bool> DeleteAsync(string id)
        {
            // Llama al repositorio para la eliminación física
            return await _repository.DeleteAsync(id);
        }

        // --- Métodos de Mapeo Auxiliares ---

        private Evaluacion MapToEntity(EvaluacionCreateDto dto)
        {
            return new Evaluacion
            {
                Colaborador = dto.Colaborador,
                RolActual = dto.RolActual,
                LiderEvaluador = dto.LiderEvaluador,
                TipoDeEvaluacion = dto.TipoDeEvaluacion,
                FechaDeEvaluacion = dto.FechaDeEvaluacion,
                SkillsEvaluadas = dto.SkillsEvaluadas,
                NivelRecomendado = dto.NivelRecomendado,
                Comentarios = dto.Comentarios,
                UsuarioResponsable = dto.UsuarioResponsable
                // FechaDeCreacion se asigna en CreateAsync, no aquí.
            };
        }

        private EvaluacionReadDto MapToReadDto(Evaluacion evaluacion)
        {
            return new EvaluacionReadDto
            {
                Id = evaluacion.Id!,
                Colaborador = evaluacion.Colaborador,
                RolActual = evaluacion.RolActual,
                LiderEvaluador = evaluacion.LiderEvaluador,
                TipoDeEvaluacion = evaluacion.TipoDeEvaluacion,
                FechaDeEvaluacion = evaluacion.FechaDeEvaluacion,
                SkillsEvaluadas = evaluacion.SkillsEvaluadas,
                NivelRecomendado = evaluacion.NivelRecomendado,
                Comentarios = evaluacion.Comentarios,
                UsuarioResponsable = evaluacion.UsuarioResponsable,
                FechaDeCreacion = evaluacion.FechaDeCreacion
            };
        }
    }
}