using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TATA.GestiondeTalentoMoviles.CORE.Entities;
using TATA.GestiondeTalentoMoviles.CORE.Interfaces;

namespace TATA.GestiondeTalentoMoviles.CORE.Services
{
    public class EvaluacionesIIService : IEvaluacionesIIService
    {
        private readonly IEvaluacionesIIRepository _repository;

        public EvaluacionesIIService(IEvaluacionesIIRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<EvaluacionesII>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<EvaluacionesII?> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(EvaluacionesII plantilla)
        {
            plantilla.FechaCreacion = DateTime.Now;
            plantilla.FechaActualizacion = DateTime.Now;

            await _repository.AddAsync(plantilla);
        }

        public async Task UpdateAsync(EvaluacionesII plantilla)
        {
            plantilla.FechaActualizacion = DateTime.Now;
            await _repository.UpdateAsync(plantilla);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
